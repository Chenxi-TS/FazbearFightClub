using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using UnityEngine;

namespace BehaviorTree
{
    public class UpdateAttackStateTask : Node
    {
        public UpdateAttackStateTask() : base(){ }

        public override NodeState Evaluate()
        {
            //Debug.Log("Updating Attack State");
            if (!checkDataStatus("AttackState", AttackState.NONE))
                return NodeState.FAILURE;
            //If no attack is currently being performed, there is nothing to evaluate
            if ((AttackState)findData("AttackState") == AttackState.NONE)
                return NodeState.SUCCESS;

            //Gets the "CurrentAttack" being performed and calculates its frames using the starting frame and current frame of the game round
            //-> AttackState.START_UP > AttackState.ACTIVE > AttackState.RECOVERY > AttackState.NONE
            //-> activates hitbox switching to AttackState.ACTIVE state
            //-> deactivates hitbox switching to AttackState.RECOVERY state
            CurrentAttackData curAttack = (CurrentAttackData)findData("CurrentAttack");
            MoveData curAttackData = curAttack.GetMoveData;
            int frameAttackStarted = curAttack.GetStartingFrame;
            switch ((AttackState)findData("AttackState"))
            {
                //switch to active state
                case AttackState.START_UP:
                    if (GameManager.Instance.GetCurrentFrame - frameAttackStarted > curAttackData.startUpFrames)
                    {
                        removeData("AttackState");
                        root.addData("AttackState", AttackState.ACTIVE);
                        curAttack.GetHitbox.SetActive(true);
                    }
                    break;
                //switch to recovery state
                case AttackState.ACTIVE:
                    if (GameManager.Instance.GetCurrentFrame - frameAttackStarted > curAttackData.startUpFrames + curAttackData.activeFrames)
                    {
                        removeData("AttackState"); 
                        root.addData("AttackState", AttackState.RECOVERY);
                        curAttack.GetHitbox.SetActive(false);
                    }
                    break;
                //switch to no attack state (remove CurrentAttackData)
                case AttackState.RECOVERY:
                    if (GameManager.Instance.GetCurrentFrame - frameAttackStarted > curAttackData.startUpFrames + curAttackData.activeFrames + curAttackData.recoveryFrames)
                    {
                        removeData("AttackState");
                        root.addData("AttackState", AttackState.NONE);
                        removeData("CurrentAttack");
                        return NodeState.SUCCESS;
                    }
                    break;
            }

            /*Debug.Log("CURRENT ATTACK: " + curAttackData.moveName + "\n" + 
                "CURRENT FRAME: " + (GameManager.Instance.GetCurrentFrame - frameAttackStarted).ToString() + 
            "(" + (AttackState)findData("AttackState") + ")");*/

            return NodeState.RUNNING;
        }
    }
}
