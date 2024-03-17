using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace BehaviorTree
{
    public class CheckQueueDecorator : Node
    {
        Tree masterTree;
        string inputNotations;
        int bufferFrames = 0;

        public CheckQueueDecorator(List<Node> childrenNodes, Tree masterTree, string inputNotations, int bufferFrames) : base(childrenNodes)
        {
            this.masterTree = masterTree;
            this.inputNotations = inputNotations;
            this.bufferFrames = bufferFrames;
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
            int requiredInputsFound = checkRequiredInputs(actionsWithinBuffer, inputNotationsArray, 0, getFirstFrame(actionsWithinBuffer));
            if (requiredInputsFound == inputNotationsArray.Length)
                return childrenNodes[0].Evaluate();
            else
            {
                return NodeState.FAILURE;
            }
        }
        int checkRequiredInputs(Dictionary<int, List<string>> actionsWithinBuffer, string[] inputNotationsArray, int inputPointer, int lastFoundFrame)
        {
            while (inputPointer < inputNotationsArray.Length)
            {
                bool foundMatch = false;

                // Loop over actionsWithinBuffer for every inputPointer increment
                foreach (KeyValuePair<int, List<string>> action in actionsWithinBuffer)
                {
                    foreach (string s in action.Value)
                    {
                        if (inputNotationsArray[inputPointer] == s && action.Key >= lastFoundFrame)
                        {
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

            return inputPointer;
        }
        //just gets first frame? i forgot why i did it like this exactly, best to just leave it be i forgor whats happening x_x this is what i get for not commenting
        int getFirstFrame(Dictionary<int, List<string>> actionDictionary)
        {
            foreach (int frame in actionDictionary.Keys)
                return frame;
            return -1;
        }
    }
}
