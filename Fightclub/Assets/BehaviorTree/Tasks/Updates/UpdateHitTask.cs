using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class UpdateHitTask : Node
    {
        Tree masterTree;
        List<AnimationClip> recoveryAnimations;
        public UpdateHitTask(Tree masterTree, List<AnimationClip> recoveryAnimations) : base()
        { 
            this.masterTree = masterTree;
            this.recoveryAnimations = recoveryAnimations;
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
                case AttackState.HIT_STUN_CROUCH:
                case AttackState.HIT_STUN_RECOVERY:
                    Debug.Log("HIT STUN: Stuned:" + (GameManager.Instance.GetCurrentFrame - startingFrame + 1) + ", Current: " + GameManager.Instance.GetCurrentFrame + " Start:" + startingFrame);
                    MoveData enemyMove = enemyAttackData.GetMoveData;
                    if (GameManager.Instance.GetCurrentFrame - startingFrame  + 1 >= (enemyMove.activeFrames + enemyMove.recoveryFrames + enemyMove.hitAdvantage))
                    {
                        Debug.Log("HIT STUN Advantage: +" +  enemyMove.hitAdvantage + 
                            "\n RECOVERED ON FRAME:" + GameManager.Instance.GetCurrentFrame +
                            ", STUNNED FOR: " + (enemyMove.activeFrames + enemyMove.recoveryFrames + enemyMove.hitAdvantage));
                        Debug.Log("RECOVERED FROM HIT STUN STATE");

                        if((AttackState)root.findData("AttackState") == AttackState.HIT_STUN_CROUCH)
                            masterTree.playAnimation(recoveryAnimations[3]);
                        else
                        {
                            switch(enemyAttackData.GetMoveData.hitType)
                            {
                                case HitType.MID:
                                    masterTree.playAnimation(recoveryAnimations[0]);
                                    break;
                                case HitType.OVERHEAD:
                                    masterTree.playAnimation(recoveryAnimations[1]);
                                    break;
                                case HitType.LOW:
                                    masterTree.playAnimation(recoveryAnimations[2]);
                                    break;
                            }
                        }
                        removeData("AttackState");
                        root.addData("AttackState", AttackState.NONE);
                        return NodeState.FAILURE;
                    }
                    return NodeState.SUCCESS;
                case AttackState.KNOCK_DOWN: 
                    break;
                case AttackState.HARD_KNOCK_DOWN:
                    break;
                case AttackState.GRABBED:
                    int grabRecoveryFrames = enemyAttackData.GetMoveData.grabRecovery;
                    if(GameManager.Instance.GetCurrentFrame - startingFrame + 1 >= grabRecoveryFrames)
                    {
                        Debug.Log("GRAB STUNNED: " + grabRecoveryFrames
                            + "\n RECOVERED ON FRAME: " + GameManager.Instance.GetCurrentFrame);
                        removeData("AttackState");
                        root.addData("AttackState", AttackState.NONE);
                        //masterTree.playAnimation();
                        return NodeState.FAILURE;
                    }
                    break;
                default:
                    break;
            }


            return NodeState.FAILURE;
        }
    }
}
