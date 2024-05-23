using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using UnityEngine;

namespace BehaviorTree
{
    public class UpdateAttackStateTask : Node
    {
        Tree masterTree;
        public UpdateAttackStateTask(Tree masterTree) : base()
        { 
            this.masterTree = masterTree;
        }

        //Updates AttackState if character is performing a move
        //-> returns NodeState.FAILURE when there is nothing to update or finished for the frame
        public override NodeState Evaluate()
        {
            //Debug.Log("UPDATE ATTACK REACHED");
            //If "AttackState" data does not exist in tree or if AttackState.NONE, there is nothing to update
            if (!checkDataStatus("AttackState", AttackState.NONE))
                return NodeState.FAILURE;
            AttackState state = (AttackState)findData("AttackState");
            if (state == AttackState.NONE || state > AttackState.RECOVERY)
                return NodeState.FAILURE;

            //Gets the "CurrentAttack" being performed and calculates its frames using the starting frame and current frame of the game round
            //-> AttackState.START_UP > AttackState.ACTIVE > AttackState.RECOVERY > AttackState.NONE
            //-> activates hitbox switching to AttackState.ACTIVE state
            //-> deactivates hitbox switching to AttackState.RECOVERY state
            CurrentAttackData curAttack = (CurrentAttackData)findData("CurrentAttack");
            MoveData curAttackData = curAttack.GetMoveData;
            int frameAttackStarted = curAttack.GetStartingFrame;

            /*Debug.Log("CURRENT ATTACK: " + curAttackData.moveName + "\n" + 
                "CURRENT FRAME: " + (GameManager.Instance.GetCurrentFrame) + ", Frame Started:" + frameAttackStarted + 
            "(" + (AttackState)findData("AttackState") + ")");*/

            switch ((AttackState)findData("AttackState"))
            {
                //switch to active state
                case AttackState.START_UP:
                    if (GameManager.Instance.GetCurrentFrame - frameAttackStarted + 1 >= curAttackData.startUpFrames)
                    {
                        removeData("AttackState");
                        root.addData("AttackState", AttackState.ACTIVE);
                        curAttack.GetHitbox.SetActive(true);
                    }
                    break;
                //switch to recovery state
                case AttackState.ACTIVE:
                    if (GameManager.Instance.GetCurrentFrame - frameAttackStarted + 1 >= (curAttackData.startUpFrames + curAttackData.activeFrames))
                    {
                        removeData("AttackState"); 
                        root.addData("AttackState", AttackState.RECOVERY);;
                        if (curAttack.Projectile != null)
                        {
                            curAttack.Projectile.spawnProjectile(curAttack.Direction, GameManager.Instance.GetCurrentFrame, masterTree);
                        }
                        curAttack.GetHitbox.SetActive(false);
                    }
                    break;
                //switch to no attack state (remove CurrentAttackData)
                case AttackState.RECOVERY:
                    if (GameManager.Instance.GetCurrentFrame - frameAttackStarted + 1 >= (curAttackData.startUpFrames + curAttackData.activeFrames + curAttackData.recoveryFrames))
                    {
                        removeData("AttackState");
                        root.addData("AttackState", AttackState.NONE);
                        removeData("CurrentAttack");
                        return NodeState.FAILURE;
                    }
                    break;
                //switch to no attack state (remove CurrentAttackData)
                case AttackState.GRABBING:
                    curAttack.GetHitbox.SetActive(false);
                    if (GameManager.Instance.GetCurrentFrame - frameAttackStarted + 1 >= (curAttackData.startUpFrames + curAttackData.activeFrames + curAttackData.grabRecovery))
                    {
                        removeData("AttackState");
                        root.addData("AttackState", AttackState.NONE);
                        removeData("CurrentAttack");
                        return NodeState.FAILURE;
                    }
                    break;
            }
            return NodeState.FAILURE;
        }
    }
}
