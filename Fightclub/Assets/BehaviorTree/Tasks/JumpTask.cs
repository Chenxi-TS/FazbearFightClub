using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviorTree
{
    public class JumpTask : Node
    {
        Tree masterTree;
        AnimationClip jumpUp;

        int direction;
        float jumpForce;
        public JumpTask(Tree masterTree, int direction, float jumpForce, AnimationClip jumpUp) 
        { 
            this.masterTree = masterTree;

            this.direction = direction;
            this.jumpForce = jumpForce;

            this.jumpUp = jumpUp;
        }
        public override NodeState Evaluate()
        {
            //Debug.Log("JUMP TASK REACHED");
            root = findRoot();
            //Adds StartUpJump to the root of the tree
            //, UpdatesGroundStateTask reads it the next 
            //frame and enters jump startup state.
            //Takes a minimum of 3 frames including startUp frames.

            //If startUp is less than 3 frames then start up will be 3 frames,
            //otherwise start up will be startUp number of frames
            //(--changes into at least 1 frame directly chaning groundstate into startup here--)
            if (findData("StartUpJump") == null)
            {
                Debug.Log(GameManager.Instance.GetCurrentFrame + "CHECKING JUMP FRAMES");
                root.addData("StartUpJump", new CurrentJumpData(GameManager.Instance.GetCurrentFrame, direction, 1, 3, jumpForce));
                masterTree.playAnimation(jumpUp);
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
