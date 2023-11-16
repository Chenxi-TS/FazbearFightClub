using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class UpdateHitTask : Node
    {
        public UpdateHitTask() : base()
        { 
        }
        public override NodeState Evaluate()
        {
            Debug.Log("GOT HIT TASK");
            //Debug.Log("UPDATE ATTACK REACHED");
            //If "AttackState" data does not exist in tree or if AttackState.NONE, there is nothing to update
            if (!checkDataStatus("AttackState", AttackState.NONE))
                return NodeState.FAILURE;
            if ((AttackState)findData("AttackState") <= AttackState.RECOVERY)
                return NodeState.FAILURE;

            CurrentAttackData enemyAttackData = (CurrentAttackData)findData("EnemyAttackData");
            int startingFrame = enemyAttackData.GetStartingFrame;
            //check if defending here

            switch((AttackState)findData("AttackState"))
            {
                case AttackState.HIT_STUN_RECOVERY:
                    Debug.Log("HIT STUN " + (GameManager.Instance.GetCurrentFrame - startingFrame) + ", " + GameManager.Instance.GetCurrentFrame + " " + startingFrame);
                    MoveData enemyMove = enemyAttackData.GetMoveData;
                    if (GameManager.Instance.GetCurrentFrame - startingFrame >= (enemyMove.activeFrames + enemyMove.recoveryFrames + enemyMove.hitAdvantage - 1))
                    {
                        Debug.Log("HIT STUN +" +  enemyMove.hitAdvantage + "\n STUNNED FOR: "  +(enemyMove.activeFrames + enemyMove.recoveryFrames + enemyMove.hitAdvantage) +
                            "\n FRAME:" + GameManager.Instance.GetCurrentFrame);
                        removeData("AttackState");
                        root.addData("AttackState", AttackState.NONE);
                        Debug.Log("RECOVER FROM HIT " + (AttackState)findData("AttackState"));
                        return NodeState.FAILURE;
                    }
                    //return NodeState.RUNNING;
                    return NodeState.SUCCESS;
                case AttackState.KNOCK_DOWN: 
                    break;
                case AttackState.HARD_KNOCK_DOWN:
                    break;
                default:
                    break;
            }


            return NodeState.FAILURE;
        }
    }
}
