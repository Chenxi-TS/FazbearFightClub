using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class CheckHitQueue : Node
    {
        Tree masterTree;
        int bufferFrames;
        public CheckHitQueue(List<Node> childrenNodes, Tree masterTree) : base(childrenNodes)
        {
            this.masterTree = masterTree;
            bufferFrames = 0;
        }
        public CheckHitQueue(List<Node> childrenNodes ,Tree masterTree, int bufferFrames) : base(childrenNodes) 
        { 
            this.masterTree = masterTree;
            this.bufferFrames = bufferFrames;
        }
        public override NodeState Evaluate()
        {
            if(findData("AttackState") != null)
            {
                if((AttackState)findData("AttackState") >= AttackState.HIT_STUN_RECOVERY)
                {
                    return childrenNodes[0].Evaluate();
                }    
            }
            Dictionary<int, List<string>> queued = masterTree.getQueuedActions(bufferFrames, GameManager.Instance.GetCurrentFrame);
            foreach(KeyValuePair<int, List<string>> pair in queued)
            {
                foreach(string action in pair.Value)
                {
                    if(action == "GOT HIT")
                    {
                        return childrenNodes[0].Evaluate();
                    }
                }
            }
            return NodeState.FAILURE;
        }
    }
}
