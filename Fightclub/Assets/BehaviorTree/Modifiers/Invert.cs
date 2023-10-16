using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Invert : Node
    {
        public Invert(Node child) : base(child) { }
        public override NodeState Evaluate()
        {
            NodeState state = childrenNodes[0].Evaluate();
            switch (state)
            {
                case NodeState.RUNNING:
                    return NodeState.FAILURE;
                case NodeState.SUCCESS:
                    return NodeState.FAILURE;
                case NodeState.FAILURE:
                    return NodeState.SUCCESS;
            }
            return NodeState.SUCCESS;
        }
    }
}