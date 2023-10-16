using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    //Move task is responsible for moving the characters
    //left and right.
    public class MoveTask : Node
    {
        int direction;
        Rigidbody rb;
        string moveName; //just for debug purposes
        float speed;

        public MoveTask(Rigidbody rb, int direction, float speed, string name) 
        {
            this.rb = rb;
            this.direction = direction;
            this.speed = speed;
            moveName = name;
        }

        public override NodeState Evaluate()
        {
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
            if((GroundState)findData("GroundState") == GroundState.GROUNDED)
                rb.velocity = new Vector3((float)findData("Speed") * direction, rb.velocity.y,0);

            return NodeState.SUCCESS;
        }
    }
}
