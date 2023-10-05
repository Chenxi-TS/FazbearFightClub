using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Selector : Node
    {
        string selectorName;
        public Selector() { }
        public Selector(List<Node> childrenNodes, string name) : base(childrenNodes) 
        { 
            selectorName = name;
        }
        public override NodeState Evaluate()
        {
            if (childrenNodes.Count <= 0)
                return NodeState.FAILURE;
            bool anyChildFailure = false;
            foreach(Node child in childrenNodes)
            {
                NodeState childState = child.Evaluate();
                switch(childState)
                {
                    case NodeState.RUNNING:
                        return NodeState.RUNNING;
                    case NodeState.SUCCESS:
                        return NodeState.SUCCESS;
                    case NodeState.FAILURE:
                        anyChildFailure = true;
                        continue;
                }
            }
            if (anyChildFailure)
                return NodeState.FAILURE;
            return NodeState.SUCCESS;
        }
    }
}
