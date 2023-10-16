using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class CheckGroundStateDecorator : Node
    {
        GroundState desiredState;
        public CheckGroundStateDecorator(GroundState desiredState, Node child) : base(child)
        {
            this.desiredState = desiredState;
        }
        public override NodeState Evaluate()
        {
            if (!checkDataStatus("GroundState", GroundState.GROUNDED))
                return NodeState.FAILURE;
            //Debug.Log("GroundState: " + (GroundState)findData("GroundState"));
            if((GroundState)findData("GroundState") == desiredState)
            {
                return childrenNodes[0].Evaluate();
            }
            return NodeState.FAILURE;
        }
    }
}
