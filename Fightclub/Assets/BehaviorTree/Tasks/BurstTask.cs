using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class BurstTask : Node
    {
        public BurstTask() : base() { }
        public override NodeState Evaluate()
        {
            return NodeState.FAILURE;
        }
    }
}
