using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviorTree
{
    public class JumpTask : Node
    {
        Tree masterTree;
        AnimationClip animation;

        int direction;
        float jumpForce;
        public JumpTask(Tree masterTree, int direction, float jumpForce) 
        { 
            this.masterTree = masterTree;

            this.direction = direction;
            this.jumpForce = jumpForce;
        }
        public override NodeState Evaluate()
        {
            Debug.Log("JUMP TASK REACHED");
            root = findRoot();

            if (findData("StartUpJump") == null)
            {
                root.addData("StartUpJump", new CurrentJumpData(GameManager.Instance.GetCurrentFrame, direction, 8, 8, jumpForce));
                masterTree.playAnimation(animation);
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
    public struct CurrentJumpData
    {
        public int startFrame;
        public int direction;
        public int startUp;
        public int recovery;
        public float force;
        public CurrentJumpData(int startFrame, int direction, int startUp, int recovery, float force)
        {
            this.startFrame = startFrame;
            this.direction=direction;
            this.startUp=startUp;
            this.recovery=recovery;
            this.force=force;
        }
    }
}
