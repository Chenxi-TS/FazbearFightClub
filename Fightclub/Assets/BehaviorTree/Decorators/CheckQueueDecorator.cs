using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;

namespace BehaviorTree
{
    public class CheckQueueDecorator : Node
    {
        Tree masterTree;
        string inputNotations;
        int bufferFrames = 0;
        bool translateMove = true;
        bool nonInputs = false;

        public CheckQueueDecorator(List<Node> childrenNodes, Tree masterTree, string inputNotations, int bufferFrames) : base(childrenNodes)
        {
            this.masterTree = masterTree;
            this.inputNotations = inputNotations;
            this.bufferFrames = bufferFrames;
        }
        public CheckQueueDecorator(List<Node> childrenNodes, Tree masterTree, string inputNotations, int bufferFrames, bool translateMove) : base(childrenNodes)
        {
            this.masterTree = masterTree;
            this.inputNotations = inputNotations;
            this.bufferFrames = bufferFrames;
            this.translateMove = translateMove;
        }
        public CheckQueueDecorator(List<Node> childrenNodes, Tree masterTree, string inputNotations, bool nonInput, int bufferFrames): base(childrenNodes)
        {
            this.masterTree = masterTree;
            this.inputNotations = inputNotations;
            this.bufferFrames = bufferFrames;
            this.nonInputs = nonInput;
        }
        public override NodeState Evaluate()
        {
            //Debug.Log("CHECK QUEUE REACHED " + aname);
            //Gets all inputs performed by player starting from the numbers of bufferframes back to current frame
            //check for required inputNotations within that time
            string[] inputNotationsArray = inputNotations.Split(",");
            if(bufferFrames < inputNotationsArray.Length - 1)
                bufferFrames = inputNotationsArray.Length - 1;
            Dictionary<int, List<string>> actionsWithinBuffer = masterTree.getQueuedActions(bufferFrames, GameManager.Instance.GetCurrentFrame);
            
            if (actionsWithinBuffer == null && inputNotations != "")
            {
                Debug.Log("CHECK ACTION NULL " + childrenNodes[0]);
                return NodeState.FAILURE;
            }
            bool requiredInputsFound = false;
            if(nonInputs)
                requiredInputsFound = checkForNonInputs(actionsWithinBuffer, inputNotationsArray);
            else
                requiredInputsFound = checkRequiredInputs(actionsWithinBuffer, inputNotationsArray, getFirstFrame(actionsWithinBuffer));
            if (requiredInputsFound)
                return childrenNodes[0].Evaluate();
            else
            {
                return NodeState.FAILURE;
            }
        }
        bool checkRequiredInputs(Dictionary<int, List<string>> actionsWithinBuffer, string[] inputNotationsArray, int lastFoundFrame)
        {
            int inputPointer = 0;
            while (inputPointer < inputNotationsArray.Length)
            {
                bool foundMatch = false;

                // Loop over actionsWithinBuffer for every inputPointer increment
                foreach (KeyValuePair<int, List<string>> action in actionsWithinBuffer)
                {
                    foreach (string s in action.Value)
                    {
                        //translate inputnotationarray here
                        if (inputNotationsArray[inputPointer] == translateForSideSwap(s) && action.Key >= lastFoundFrame)
                        {
                            Debug.Log("PASSED " + s);
                            inputPointer++;
                            //so it stays in sequence of inputs
                            lastFoundFrame = action.Key;
                            foundMatch = true;
                            break;
                        }
                    }
                    if (foundMatch) break;
                }
                if (!foundMatch)
                {
                    break;
                }
            }
            if (inputPointer == inputNotationsArray.Length)
                return true;
            return false;
        }
        bool checkForNonInputs(Dictionary<int, List<string>> actionsWithinBuffer, string[] inputNotationsArray)
        {
            int pointer = 0;
            foreach(KeyValuePair<int, List<string>> action in actionsWithinBuffer)
            {
                foreach(string s in action.Value)
                {
                    string nonInput = inputNotationsArray[pointer].ToCharArray()[1].ToString();
                    Debug.Log("NON INPUT READING " + s + " vs. " + nonInput);
                    if (s == nonInput)
                    {
                        return false;
                    }
                }
                pointer++;
            }
            Debug.Log("NON INPUT, UNCROUCH " + pointer);
            return true;
        }
        //gets the frame when the first action within the buffer was pressed
        int getFirstFrame(Dictionary<int, List<string>> actionDictionary)
        {
            foreach (int frame in actionDictionary.Keys)
                return frame;
            return -1;
        }

        string translateForSideSwap(string requiredKey)
        {
            if (!translateMove)
                return requiredKey;
            if (masterTree.getPlayerSlotNum == 1)
            {
                if (!GameManager.Instance.getPlayer1FacingRight)
                    return sideSwapDictionary(requiredKey);
            }
            else if (masterTree.getPlayerSlotNum == 2)
            {
                if (GameManager.Instance.getPlayer1FacingRight)
                    return sideSwapDictionary(requiredKey);
            }
            return requiredKey;
        }
        string sideSwapDictionary(string requiredKey)
        {
            Debug.Log(requiredKey + "TRANSLATE");
            switch(requiredKey)
            {
                case "1":
                    return "3";
                case "3":
                    return "1";
                case "4":
                    return "6";
                case "6":
                    return "4";
                case "7":
                    return "9";
                case "9":
                    return "7";
            }
            return requiredKey;
        }
    }
}
