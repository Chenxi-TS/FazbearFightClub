using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class CrouchTask : Node
    {
        Tree masterTree;
        AnimationClip animation;
        bool crouch;
        public CrouchTask(Tree masterTree, AnimationClip animation, bool crouch)
        {
            this.masterTree = masterTree;
            this.animation = animation;
            this.crouch = crouch;
        }
        public override NodeState Evaluate()
        {
            if(root == null)
                root = findRoot();
            Debug.Log("CROUCH REACHED " + crouch);
            if (!checkDataStatus("GroundState", GroundState.CROUCHING))
                return NodeState.FAILURE;

            GroundState state = (GroundState)findData("GroundState");
            if (state == GroundState.CROUCHING && !crouch)
            {
                removeData("GroundState");
                root.addData("GroundState", GroundState.GROUNDED);
                masterTree.playAnimation(animation);
            }
            else if(state == GroundState.GROUNDED && crouch)
            {
                removeData("GroundState");
                root.addData("GroundState", GroundState.CROUCHING);
                masterTree.playAnimation(animation);
            }
            return NodeState.SUCCESS;
        }
    }
}
