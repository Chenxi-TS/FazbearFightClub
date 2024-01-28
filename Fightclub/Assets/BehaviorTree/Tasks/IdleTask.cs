using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class IdleTask : Node
    {
        Tree masterTree;
        Rigidbody rb;
        AnimationClip idle;
        public IdleTask(Tree masterTree, Rigidbody rb, AnimationClip idle)
        {
            this.masterTree = masterTree;
            this.rb = rb;
            this.idle = idle;
        }
        public override NodeState Evaluate()
        {
            if (root == null)
                root = findRoot();
            if (rb == null)
            {
                return NodeState.FAILURE;
            }
            if (findData("MovementState") == null)
            {
                //Debug.LogError("MovementState data is not in tree");
                root.addData("MovementState", MovementState.NONE);
                return NodeState.FAILURE;
            }
            if (findData("MovementState") is not MovementState)
            {
                //Debug.LogError("MovementState data in tree is not of type MovementState");
                return NodeState.FAILURE;
            }

            AttackState attackState = (AttackState)findData("AttackState");
            MovementState moveState = (MovementState)findData("MovementState");
            
            Debug.Log("aidle " + attackState);
            if (attackState != AttackState.NONE)
                return NodeState.FAILURE;
            if (moveState != MovementState.WALKING)
                return NodeState.FAILURE;
            rb.velocity = Vector3.zero;
            masterTree.playAnimation(idle);
            removeData("MovementState");
            root.addData("MovementState", MovementState.NONE);
            return NodeState.FAILURE;
        }
    }
}
