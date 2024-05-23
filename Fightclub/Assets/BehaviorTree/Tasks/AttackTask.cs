
using UnityEngine;

namespace BehaviorTree
{
    public class AttackTask : Node
    {   
        Tree masterTree;
        Rigidbody rb;

        MoveData moveData;
        MoveData prereqMove = null;

        GameObject hitbox;
        GameObject projectileClone;
        Projectile projectile;

        int playerSlot = 0;
        public AttackTask (MoveData moveData, Tree masterTree, Rigidbody rb, Transform transform, int playerSlot)
        {
            this.moveData = moveData;
            this.masterTree = masterTree;
            this.rb = rb;
            this.playerSlot = playerSlot;
            //move hitbox spawn
            hitbox = MonoBehaviour.Instantiate(moveData.hitbox);
            hitbox.transform.SetParent(transform);
            hitbox.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            hitbox.transform.localPosition = Vector3.zero;
            hitbox.SetActive(false);
            hitbox.GetComponentInChildren<hitBox>().setUser(masterTree);
            //projectile spawn
            if (moveData.projectile != null)
            {
                projectileClone = MonoBehaviour.Instantiate(moveData.projectile);
                projectileClone.transform.SetParent(transform);
                projectileClone.SetActive(false);
                hitbox.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                projectileClone.GetComponent<hitBox>().setUser(masterTree);
                projectileClone.transform.SetParent(null);
            }

            //change hitbox local transform base on where the player is facing
            updateHitboxLocalScale(hitbox.transform);
        }
        //If this move is a Rekka (follow up move/multi hit move),
        //specify the prerequisite move
        public AttackTask(MoveData moveData, Tree masterTree, Rigidbody rb, Transform transform, MoveData prereqMove, int playerSlot)
        {
            this.moveData = moveData;
            this.masterTree = masterTree;
            this.rb = rb;
            this.playerSlot = playerSlot;
            hitbox = MonoBehaviour.Instantiate(moveData.hitbox);
            hitbox.transform.SetParent(transform);
            hitbox.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            hitbox.transform.localPosition = Vector3.zero;
            hitbox.SetActive(false);
            hitbox.GetComponentInChildren<hitBox>().setUser(masterTree);
            this.prereqMove = prereqMove;
            //change hitbox local transform base on where the player is facing
            updateHitboxLocalScale(hitbox.transform);
        }

        public override NodeState Evaluate()
        {
            if (root == null)
            {
                root = findRoot();
                //Debug.Log("ATTACK ROOT " + root);
            }
            //Checking status of "AttackState"
            //-> warns if "AttackState" is missing
            //-> warns if "AttackState" is not type of AttackState
            if (findData("AttackState") == null)
            {
                //Debug.Log("AttackState data is not in tree");
                return NodeState.FAILURE;
            }
            if (findData("AttackState") is not AttackState)
            {
                //Debug.Log("AttackState data in tree is not of type AttackState");
                return NodeState.FAILURE;
            }

            //Checks if this move can be performed without interuption
            //-> FAILURE if we are being hit
            //-> FAILURE if current attack is not the prerequisite move
            //-> perform attack now if no move is being performed currently
            AttackState currentAttackState = (AttackState)findData("AttackState");
            if (currentAttackState >= AttackState.HIT_STUN_RECOVERY)
            {
                //Debug.Log("Recovering from hit");
                return NodeState.FAILURE;
            }
            if (currentAttackState == AttackState.NONE && prereqMove == null)
                performAttack((GroundState)findData("GroundState"));

            //If move is currently being perform, check if this move can interupt current move
            //-> checks if this move has a prereqMove
            //-> normals cancelable order of Heavy>Medium>Light
            //-> commands cancel all normals
            //-> specials cancel all commands and normals
            //-> specials can cancel specified specials
            CurrentAttackData currentAttackData = (CurrentAttackData)findData("CurrentAttack");
            if (currentAttackData == null)
                return NodeState.FAILURE;
            if (prereqMove != null)
                if (currentAttackData.GetMoveData != prereqMove)
                    return NodeState.FAILURE;
                else
                {
                    if(currentAttackState == AttackState.RECOVERY)
                        performAttack((GroundState)findData("GroundState"));
                }
            MoveType currentAttackType = currentAttackData.GetMoveData.type;
            //Debug.Log("CURRENT TYPE " + currentAttackType);
            if (currentAttackState == AttackState.RECOVERY)
            {
                switch (currentAttackType)
                {
                    case < MoveType.COMMAND:
                        if (moveData.type > currentAttackType)
                        {
                            //Debug.Log("CANCEL? " + moveData.type + " vs. " + currentAttackType);
                            performAttack((GroundState)findData("GroundState"));
                        }
                        break;
                    case MoveType.COMMAND:
                        if (moveData.type >= MoveType.SPECIAL)
                            performAttack((GroundState)findData("GroundState"));
                        break;
                    case MoveType.SPECIAL: 
                        if (moveData.type >= MoveType.SUPER)
                        {
                            performAttack((GroundState)findData("GroundState"));
                        }
                        else if (currentAttackData.GetMoveData.specialCancelables.Count > 0)
                        {
                            foreach(MoveData specials in currentAttackData.GetMoveData.specialCancelables)
                            {
                                if (specials.type != MoveType.SPECIAL)
                                    //Debug.LogError(currentAttackData.GetMoveData.moveName + " has " + specials.moveName + " in specialCancelables and is not a special");
                                if (specials.moveName == moveData.moveName)
                                    performAttack((GroundState)findData("GroundState"));
                            }
                        }
                        break;
                }
            }
            return NodeState.FAILURE;
        }

        //Start Move Execution
        NodeState performAttack(GroundState groundState)
        {
            //Debug.Log("MOVE PERFORMED " + moveData.name + " " + GameManager.Instance.GetCurrentFrame);
            removeData("CurrentAttack");

            //Check for GameManager and moveData
            if (GameManager.Instance == null)
                Debug.Log("GameManagerNULL");
            else if (moveData == null)
                Debug.Log("moveDataNULL");

            //Stop momentum if grounded
            if (groundState != GroundState.AIRBORNE)
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            else if (groundState == GroundState.AIRBORNE)
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y/10, 0);


            //If this move includes a projectile
            //->set up projectile fire point position
            if (moveData.projectile != null)
            {
                if (projectileClone.activeSelf == true)
                    return NodeState.FAILURE;
                projectile = projectileClone.GetComponent<Projectile>();
                projectile.setSpawn(rb.transform);
            }
            //Create CurrentAttackData object
            //->set CurrentAttack data for the tree
            //->give CurrentAttack data for hitbox
            int attackDirection = 1;
            if(masterTree.getPlayerSlotNum == 1)
            {
                if(!GameManager.Instance.getPlayer1FacingRight)
                    attackDirection = -1;
            }
            else if(masterTree.getPlayerSlotNum == 2)
            {
                if(GameManager.Instance.getPlayer1FacingRight)
                    attackDirection = -1;
            }
            CurrentAttackData curAttack = new CurrentAttackData(GameManager.Instance.GetCurrentFrame, moveData, hitbox, projectile, masterTree, attackDirection);
            root.addData("CurrentAttack", curAttack);
            hitbox.GetComponentInChildren<hitBox>().setAttackData((CurrentAttackData)findData("CurrentAttack"));

            //Also give projectile's hitbox CurrentAttack data if move includes a projectile
            if (moveData.projectile != null)
                projectile.GetComponent<hitBox>().setAttackData((CurrentAttackData)findData("CurrentAttack"));

            removeData("AttackState");
            root.addData("AttackState", AttackState.START_UP);
            masterTree.playAnimation(moveData.moveAnimation);
            return NodeState.SUCCESS;
        }

        void updateHitboxLocalScale(Transform hitbox)
        {
            if (GameManager.Instance.getPlayer1FacingRight)
            {
                //Debug.Log("RIGHT face");
                if (playerSlot == 1)
                    hitbox.localScale = new Vector3(1 * 2.5F, hitbox.localScale.y, hitbox.localScale.z);
                else
                    hitbox.localScale = new Vector3(-1 * 2.5F, hitbox.localScale.y, hitbox.localScale.z);
            }
            else if (!GameManager.Instance.getPlayer1FacingRight)
            {
                //Debug.Log("LEFT face");
                if (playerSlot == 1)
                    hitbox.localScale = new Vector3(-1 * 2.5F, hitbox.localScale.y, hitbox.localScale.z);
                else
                    hitbox.localScale = new Vector3(1 * 2.5F, hitbox.localScale.y, hitbox.localScale.z);
            }
        }
    }
}
