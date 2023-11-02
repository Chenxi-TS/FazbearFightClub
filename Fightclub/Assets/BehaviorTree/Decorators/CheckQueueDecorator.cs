using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace BehaviorTree
{
    public class CheckQueueDecorator : Node
    {
        Tree masterTree;
        char[] inputNotations;
        int bufferFrames = 0;
        public CheckQueueDecorator(List<Node> childrenNodes, Tree masterTree, string inputNotations, int bufferFrames) : base(childrenNodes) 
        { 
            this.masterTree = masterTree;
            this.inputNotations = inputNotations.ToCharArray();
            this.bufferFrames = bufferFrames;
        }
        public override NodeState Evaluate()
        {
            //Gets all inputs performed by player starting from the numbers of bufferframes back to current frame
            //check for required inputNotations within that time
            List<string> actionsWithinBuffer = masterTree.getQueuedActions(bufferFrames, GameManager.Instance.GetCurrentFrame);
            if (actionsWithinBuffer == null)
                return NodeState.FAILURE;
            int inputPointer = 0;
            for(int actionPointer = 0; actionPointer < actionsWithinBuffer.Count && inputPointer < inputNotations.Length; actionPointer++)
            {
                if (actionsWithinBuffer[actionPointer] == inputNotations[inputPointer].ToString())
                {
                    inputPointer++;
                }
                else 
                {
                    if (translateNotations((actionsWithinBuffer[actionPointer]), inputNotations[inputPointer].ToString()))
                        inputPointer++;
                }
            }
            if (inputPointer == inputNotations.Length)
                return childrenNodes[0].Evaluate();
            else
                return NodeState.FAILURE;
        }
        bool translateNotations(string actionNotation, string inputNotation)
        {
            switch (inputNotation) 
            {
                //up
                case "8":
                    if (actionNotation == "7" || actionNotation == "9")
                    {
                        //Debug.Log(actionNotation + " CASE 8");
                        return true;
                    }
                    break;
                //left
                case "4":
                    if (actionNotation == "7" || actionNotation == "1")
                        return true;
                    break;
                //down
                case "2":
                    if (actionNotation == "1" || actionNotation == "3")
                        return true;
                    break;
                //right
                case "6":
                    if (actionNotation == "9" || actionNotation == "3")
                        return true;
                    break;
            }
            return false;
        }
    }
}
