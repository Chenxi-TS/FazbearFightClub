using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviorTree
{
    public class AttackTask : Node
    {
        MoveData moveData;
        Tree masterTree;
        GameObject hitbox;
        public AttackTask (MoveData moveData, Tree masterTree, Transform transform)
        {
            this.moveData = moveData;
            this.masterTree = masterTree;
            hitbox = MonoBehaviour.Instantiate(moveData.hitbox);
            hitbox.transform.SetParent(transform);
            hitbox.transform.localPosition = Vector3.zero;
            hitbox.SetActive(false);
        }

        public override NodeState Evaluate()
        {
            if (root == null)
                root = findRoot();
            //if AttackState is not found, CheckAttackStateDecor probably not in tree
            if (findData("AttackState") == null)
            {
                Debug.LogError("AttackState data is not in tree");
                return NodeState.FAILURE;
            }
            //if AttackState data value is not of type AttackState in tree
            if (findData("AttackState") is not AttackState)
            {
                Debug.LogError("AttackState data in tree is not of type AttackState");
                return NodeState.FAILURE;
            }

            AttackState currentAttackState;
            currentAttackState = (AttackState)findData("AttackState");
            //if in hit recovery, return
            if (currentAttackState >= AttackState.HIT_STUN)
            {
                Debug.Log("Recovering from hit");
                return NodeState.FAILURE;
            }
            //if no move is currently being formed, perform attack
            if (currentAttackState == AttackState.NONE)
                performAttack();
            //check if current move is cancelable by this attack
            CurrentAttackData currentAttackData = (CurrentAttackData)findData("CurrentAttack");
            MoveData.MoveType currentAttackType = currentAttackData.GetMoveData.type;
            if (currentAttackState == AttackState.RECOVERY)
            {
                switch (currentAttackType)
                {
                    case < MoveData.MoveType.COMMAND:
                        if (currentAttackType < moveData.type)
                            performAttack();
                        break;
                    case MoveData.MoveType.COMMAND:
                        if (moveData.type >= MoveData.MoveType.SPECIAL)
                            performAttack();
                        break;
                    case MoveData.MoveType.SPECIAL:
                        if (moveData.type >= MoveData.MoveType.SUPER)
                        {
                            performAttack();
                        }
                        else if (currentAttackData.GetMoveData.specialCancelable)
                        {
                            if (currentAttackData.GetMoveData.nameOfSpecialThatCancelsThisMove == moveData.moveName)
                                performAttack();
                        }
                        break;
                }
            }
            return NodeState.RUNNING;
        }

        NodeState performAttack()
        {
            Debug.Log("AttackTask performed: " + moveData.moveName);
            //play animation, spawn hitboxes, add currentAttack
            removeData("CurrentAttack");

            if (GameManager.Instance == null)
                Debug.Log("GameManagerNULL");
            else if (moveData == null)
                Debug.Log("moveDataNULL");

            root.addData("CurrentAttack", new CurrentAttackData(GameManager.Instance.GetCurrentFrame, moveData, hitbox));
            removeData("AttackState");
            root.addData("AttackState", AttackState.START_UP);
            masterTree.playAnimation(moveData.moveAnimation);
            return NodeState.SUCCESS;
        }
    }
}
