using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class HitUpdateTask : Node
    {
        public HitUpdateTask() : base() { }
        public override NodeState Evaluate()
        {
            return NodeState.FAILURE;
        }
    }
}
