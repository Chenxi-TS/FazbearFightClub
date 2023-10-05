using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class CheckGroundStateDecorator : Node
    {
        GroundState trueGroundState;
        public CheckGroundStateDecorator(List<Node> listOfChildren, GroundState desiredGroundState): base(listOfChildren) 
        {
            trueGroundState = desiredGroundState;
        }
        public override NodeState Evaluate()
        {
            if (root == null)
                root = findRoot();
            if (findData("Grounded") == null)
            {
                Debug.LogWarning("Grounded data not found");
                root.addData("Grounded", GroundState.GROUNDED);
                return NodeState.FAILURE;
            }
            if ((GroundState)findData("Grounded") != trueGroundState)
                return NodeState.FAILURE;
            else
            {
                foreach(Node child in childrenNodes)
                {
                    return child.Evaluate();
                }
            }
            Debug.Log("Requested GroundState is true but decorator has no children");
            return NodeState.FAILURE;
        }
    }
}
