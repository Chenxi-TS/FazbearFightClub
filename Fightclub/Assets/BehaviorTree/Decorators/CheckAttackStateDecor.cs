using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using UnityEngine;

namespace BehaviorTree
{
    public class CheckAttackStateDecor : Node
    {
        Transform transformParent;
        public CheckAttackStateDecor(List<Node> childrenNodes) : base(childrenNodes){ }

        public override NodeState Evaluate()
        {
            //if AttackState data not in tree, add it to tree
            if (findData("AttackState") == null)
            {
                Debug.LogWarning("AttackState data not found");
                addData("AttackState", AttackState.NONE);
                return NodeState.FAILURE;
            }
            //if AttackState data value is not of type AttackState in tree
            if (findData("AttackState") is not AttackState)
            {
                Debug.LogError("AttackState data in tree is not of type AttackState");
                return NodeState.FAILURE;
            }
            if ((AttackState)findData("AttackState") == AttackState.NONE)
                return childrenNodes[0].Evaluate();

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
                        addData("AttackState", AttackState.ACTIVE);
                        curAttack.GetHitbox.SetActive(true);
                    }
                    break;
                //switch to recovery state
                case AttackState.ACTIVE:
                    if (GameManager.Instance.GetCurrentFrame - frameAttackStarted > curAttackData.startUpFrames + curAttackData.activeFrames)
                    {
                        removeData("AttackState");
                        addData("AttackState", AttackState.RECOVERY);
                        curAttack.GetHitbox.SetActive(false);
                    }
                    break;
                //switch to no attack state (remove CurrentAttackData)
                case AttackState.RECOVERY:
                    if (GameManager.Instance.GetCurrentFrame - frameAttackStarted > curAttackData.startUpFrames + curAttackData.activeFrames + curAttackData.recoveryFrames)
                    {
                        removeData("AttackState");
                        addData("AttackState", AttackState.NONE);
                        removeData("CurrentAttack");
                        return NodeState.SUCCESS;
                    }
                    break;
            }
            Debug.Log("CURRENT ATTACK: " + curAttackData.moveName + "\n" + 
                "CURRENT FRAME: " + (GameManager.Instance.GetCurrentFrame - frameAttackStarted).ToString() + 
            "(" + (AttackState)findData("AttackState") + ")");
            return NodeState.RUNNING;
        }
    }
}
