using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace BehaviorTree
{
    public class UpdateGroundStateTask : Node
    {
        Rigidbody rb;
        Transform transform;
        public UpdateGroundStateTask(Rigidbody rb, Transform transform) : base() 
        {
            this.rb = rb;
            this.transform = transform;
        }

        //Updates "GroundState" data
        //-> return NodeState.FAILURE if there is nothing to update or finished updating this frame
        public override NodeState Evaluate()
        {
            //Debug.Log("Updating Ground State");
            if (!checkDataStatus("GroundState", GroundState.GROUNDED))
                return NodeState.FAILURE;

            CurrentJumpData currentJumpData;
            GroundState currentGroundState = (GroundState)findData("GroundState");

            switch(currentGroundState)
            {
                case GroundState.GROUNDED:
                    if (findData("StartUpJump") == null)
                    {
                        return NodeState.FAILURE;
                    }
                    removeData("GroundState");
                    root.addData("GroundState", GroundState.START_UP);
                    break;
                case GroundState.START_UP:
                    currentJumpData = (CurrentJumpData)findData("StartUpJump"); 
                    if (GameManager.Instance.GetCurrentFrame - currentJumpData.startFrame > currentJumpData.startUp)
                    {
                        rb.velocity = new Vector3(currentJumpData.direction * .5f, 1, 0) * currentJumpData.force;
                        removeData("GroundState");
                        root.addData("GroundState", GroundState.START_UP);
                    }
                    if (GameManager.Instance.GetCurrentFrame - currentJumpData.startFrame > currentJumpData.startUp + 2)
                    {
                        removeData("GroundState");
                        root.addData("GroundState", GroundState.AIRBORNE);
                    }
                    break;
                case GroundState.AIRBORNE:
                    if (findData("RecoveryJump") == null)
                    {
                        //Debug.Log("Ground layer is number: " + LayerMask.GetMask("Ground")); //8
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.05f, 8))
                        {
                            Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.down) * hit.distance, Color.green);

                            currentJumpData = (CurrentJumpData)findData("StartUpJump");
                            removeData("StartUpJump");
                            currentJumpData.startFrame = GameManager.Instance.GetCurrentFrame;
                            addData("RecoveryJump", currentJumpData);
                            removeData("GroundState");
                            root.addData("GroundState", GroundState.RECOVERY);
                        }
                        else
                            Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.down) * 1.05f, Color.red);
                    }
                    break;
                case GroundState.RECOVERY:
                        currentJumpData = (CurrentJumpData)findData("RecoveryJump");
                        //Debug.Log(GameManager.Instance.GetCurrentFrame - currentJumpData.startFrame);
                        if (GameManager.Instance.GetCurrentFrame - currentJumpData.startFrame > currentJumpData.recovery)
                        {
                            removeData("RecoveryJump");
                            removeData("GroundState");
                            root.addData("GroundState", GroundState.GROUNDED);
                        }
                    break;
            }
            return NodeState.FAILURE;
        }
    }
}
