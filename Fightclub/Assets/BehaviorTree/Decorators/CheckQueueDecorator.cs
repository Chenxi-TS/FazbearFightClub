using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            List<string> actionsWithinBuffer = masterTree.getQueuedActions(bufferFrames, GameManager.Instance.GetCurrentFrame);
            if (actionsWithinBuffer == null)
                return NodeState.FAILURE;
            int inputPointer = 0;
            for(int actionPointer = 0; actionPointer < actionsWithinBuffer.Count && inputPointer < inputNotations.Length; actionPointer++)
            {
                if (actionsWithinBuffer[actionPointer] == inputNotations[inputPointer].ToString())
                    inputPointer++;
            }
            if (inputPointer == inputNotations.Length)
                return childrenNodes[0].Evaluate();
            else
                return NodeState.FAILURE;
        }
    }
}
