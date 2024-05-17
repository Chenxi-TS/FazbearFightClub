using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    //Move task is responsible for moving the characters
    //left and right.
    public class MoveTask : Node
    {
        Tree masterTree;
        int direction;
        Rigidbody rb;
        string moveName; //just for debug purposes
        float speed;
        int slot;
        AnimationClip forward;
        AnimationClip backward;
        string name;

        public MoveTask(Rigidbody rb, int direction, float speed, string name, Tree masterTree, int slot, AnimationClip forward, AnimationClip backward) 
        {
            this.rb = rb;
            this.direction = direction;
            this.speed = speed;
            moveName = name;
            this.masterTree = masterTree;
            this.slot = slot;
            this.forward = forward;
            this.backward = backward;
            this.name = name;
        }

        public override NodeState Evaluate()
        {
            if (root == null)
                root = findRoot();
            Debug.Log("move task reached " + moveName);
            if (findData("Speed") == null)
            {
                Debug.LogWarning("Speed data not found");
                parent.addData("Speed", speed);
                return NodeState.FAILURE;
            }
            if(findData("Speed") is not float)
            {
                Debug.LogError("Speed data in tree is not of type float");
                return NodeState.FAILURE;
            }
            if(rb == null)
            {
                Debug.LogError("Rigibody is null in " + moveName);
                return NodeState.FAILURE;
            }
            if ((AttackState)findData("AttackState") > AttackState.NONE)
                return NodeState.FAILURE;
            if ((GroundState)findData("GroundState") == GroundState.GROUNDED)
            {
                if(findData("MovementState") != null)
                    removeData("MovementState");
                root.addData("MovementState", MovementState.WALKING);
                rb.velocity = new Vector3((float)findData("Speed") * direction, rb.velocity.y, 0);
                Debug.Log(name);
                playAnimation();
            }

            return NodeState.SUCCESS;
        }

        void playAnimation()
        {
            if(slot == 1)
            {
                switch(GameManager.Instance.getPlayer1FacingRight)
                {
                    case true:
                        if (direction < 0)
                            masterTree.playAnimation(backward);
                        else
                            masterTree.playAnimation(forward);
                        break;
                    case false:
                        if(direction > 0)
                            masterTree.playAnimation(backward);
                        else
                            masterTree.playAnimation(forward);
                        break;
                }
            }
            else if(slot == 2)
            {
                switch (GameManager.Instance.getPlayer1FacingRight)
                {
                    case true:
                        if (direction > 0)
                            masterTree.playAnimation(backward);
                        else
                            masterTree.playAnimation(forward);
                        break;
                    case false:
                        if (direction < 0)
                            masterTree.playAnimation(backward);
                        else
                            masterTree.playAnimation(forward);
                        break;
                }
            }
        }
    }
}
