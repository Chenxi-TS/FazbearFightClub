using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviorTree
{
    public class AttackTask : Node
    {
        MoveData moveData;
        MoveData prereqMove = null;
        Tree masterTree;
        GameObject hitbox;
        Projectile projectile;
        Rigidbody rb;
        public AttackTask (MoveData moveData, Tree masterTree, Rigidbody rb, Transform transform)
        {
            this.moveData = moveData;
            this.masterTree = masterTree;
            this.rb = rb;
            hitbox = MonoBehaviour.Instantiate(moveData.hitbox);
            hitbox.transform.SetParent(transform);
            hitbox.transform.localPosition = Vector3.zero;
            hitbox.SetActive(false);
        }
        //If this move is a Rekka (follow up move/multi hit move),
        //specify the prerequisite move
        public AttackTask(MoveData moveData, Tree masterTree, Rigidbody rb, Transform transform, MoveData prereqMove)
        {
            this.moveData = moveData;
            this.masterTree = masterTree;
            this.rb = rb;
            hitbox = MonoBehaviour.Instantiate(moveData.hitbox);
            hitbox.transform.SetParent(transform);
            hitbox.transform.localPosition = Vector3.zero;
            hitbox.SetActive(false);
            this.prereqMove = prereqMove;
        }

        public override NodeState Evaluate()
        {
            if (root == null)
                root = findRoot();
            //Checking status of "AttackState"
            //-> warns if "AttackState" is missing
            //-> warns if "AttackState" is not type of AttackState
            if (findData("AttackState") == null)
            {
                Debug.LogError("AttackState data is not in tree");
                return NodeState.FAILURE;
            }
            if (findData("AttackState") is not AttackState)
            {
                Debug.LogError("AttackState data in tree is not of type AttackState");
                return NodeState.FAILURE;
            }

            //Checks if this move can be performed without interuption
            //-> FAILURE if we are being hit
            //-> FAILURE if current attack is not the prerequisite move
            //-> perform attack if no move is being performed currently
            AttackState currentAttackState = (AttackState)findData("AttackState");
            if (currentAttackState >= AttackState.HIT_STUN)
            {
                Debug.Log("Recovering from hit");
                return NodeState.FAILURE;
            }
            if (currentAttackState == AttackState.NONE && prereqMove == null)
                performAttack();

            //Checks if this move can be interupt current move
            //-> checks if this move has a prereqMove
            //-> normals cancelable in order of H>M>L
            //-> commands cancel all normals
            //-> specials cancel all normals and specials
            //-> specials can cancel specified specials
            CurrentAttackData currentAttackData = (CurrentAttackData)findData("CurrentAttack");
            if (currentAttackData == null)
                return NodeState.FAILURE;
            if (prereqMove != null)
                if (currentAttackData.GetMoveData != prereqMove)
                    return NodeState.FAILURE;
            MoveData.MoveType currentAttackType = currentAttackData.GetMoveData.type;
            //Debug.Log("CURRENT TYPE " + currentAttackType);
            if (currentAttackState == AttackState.RECOVERY)
            {
                switch (currentAttackType)
                {
                    case < MoveData.MoveType.COMMAND:
                        if (moveData.type > currentAttackType)
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
                        else if (currentAttackData.GetMoveData.specialCancelables.Count > 0)
                        {
                            foreach(MoveData specials in currentAttackData.GetMoveData.specialCancelables)
                            {
                                if (specials.type != MoveData.MoveType.SPECIAL)
                                    Debug.LogError(currentAttackData.GetMoveData.moveName + " has " + specials.moveName + " in specialCancelables and is not a special");
                                if (specials.moveName == moveData.moveName)
                                    performAttack();
                            }
                        }
                        break;
                }
            }
            return NodeState.FAILURE;
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
            rb.velocity = new Vector3 (0, rb.velocity.y, 0);
            if (moveData.projectile != null)
            {
                GameObject projectileClone = MonoBehaviour.Instantiate(moveData.projectile);
                projectileClone.SetActive(false);
                projectile = projectileClone.GetComponent<Projectile>();
                projectile.setSpawn(GameManager.Instance.characters[masterTree.getcharacterID].GetComponent<MoveListHolder>().projectileFirePoints[projectile.firePointNum]);
            }
            root.addData("CurrentAttack", new CurrentAttackData(GameManager.Instance.GetCurrentFrame, moveData, hitbox, projectile));
            removeData("AttackState");
            root.addData("AttackState", AttackState.START_UP);
            masterTree.playAnimation(moveData.moveAnimation);
            return NodeState.SUCCESS;
        }
    }
}
