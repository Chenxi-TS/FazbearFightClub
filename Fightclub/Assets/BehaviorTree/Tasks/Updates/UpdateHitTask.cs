using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class UpdateHitTask : Node
    {
        public UpdateHitTask() : base() { }
        public override NodeState Evaluate()
        {
            return NodeState.FAILURE;
        }
    }
}
