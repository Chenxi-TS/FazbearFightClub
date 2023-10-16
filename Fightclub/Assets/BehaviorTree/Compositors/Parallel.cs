using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace BehaviorTree
{
    public class Parallel : Node
    {
        public Parallel (List<Node> childrenNodes) : base (childrenNodes) { }

        public override NodeState Evaluate()
        {
            if (childrenNodes.Count < 0)
                return NodeState.FAILURE;
            bool anyChildrenSuccess = false;
            foreach (Node node in childrenNodes) 
            {
                NodeState state = node.Evaluate();
                switch(state)
                {
                    case NodeState.RUNNING:
                        anyChildrenSuccess = true;
                        continue;
                    case NodeState.SUCCESS:
                        anyChildrenSuccess = true;
                        continue;
                    case NodeState.FAILURE:
                        continue;
                }
            }
            if (!anyChildrenSuccess)
            {
                return NodeState.FAILURE;
            }
            else
                return NodeState.SUCCESS;
        }
    }
}
