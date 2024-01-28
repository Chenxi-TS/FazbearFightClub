using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace BehaviorTree
{
    public enum GroundState
    {
        START_UP,
        AIRBORNE,
        RECOVERY,
        GROUNDED,
        CROUCHING,
    }
    public enum MovementState
    {
        NONE,
        WALKING,
        UNCROUCHING
    }
    public enum AttackState
    {
        //Attacking
        NONE,
        START_UP,
        ACTIVE,
        GRABBING,
        RECOVERY, 
        //(Anything below recovery means getting hit)damn the enemy got hands
        HIT_STUN_RECOVERY,
        KNOCK_DOWN,
        HARD_KNOCK_DOWN,
        GRABBED,
    }
    public enum DefenseState
    {
        NONE,
        HIGH_BLOCK,
        MID_BLOCK,
        LOW_BLOCK,
    }

    public class Tree : Node, Observer
    {
        public List<MoveData> allMoves;
        protected List<AnimationClip> movementAnimations;
        protected List<AnimationClip> damageAnimations;
        protected int characterID;
        public int getcharacterID { get { return characterID; } }

        protected Rigidbody rb;
        protected Transform transform;
        public Transform getTransform { get { return transform; } }
        protected Animator animator;

        protected Dictionary<KeyCode, Command> characterMoveList = new Dictionary<KeyCode, Command>();
        protected Dictionary<int, List<string>> queuedActions = new Dictionary<int, List<string>>();

        public Tree(int playerSlotNumber, GameObject characterBody)
        {
            characterMoveList.Clear();
            //movement
            JumpCommand jump = new JumpCommand();
            CrouchCommand crouch = new CrouchCommand();
            MoveCommand moveRight = new MoveCommand(6);
            MoveCommand moveLeft = new MoveCommand(4);
            //attacks
            AttackCommand lightAttack = new AttackCommand(AttackCommand.AttackButtons.LIGHT);
            AttackCommand MediumAttack = new AttackCommand(AttackCommand.AttackButtons.MEDIUM);
            AttackCommand HeavyAttack = new AttackCommand(AttackCommand.AttackButtons.HEAVY);

            if (playerSlotNumber == 1)
            {
                Debug.Log("player 1 tree created");
                #region PLAYER 1
                characterMoveList.Add(KeyCode.W, jump);
                characterMoveList.Add(KeyCode.A, moveLeft);
                characterMoveList.Add(KeyCode.S, crouch);
                characterMoveList.Add(KeyCode.D, moveRight);

                characterMoveList.Add(KeyCode.C, lightAttack);
                characterMoveList.Add(KeyCode.V, MediumAttack);
                characterMoveList.Add(KeyCode.B, HeavyAttack);
                #endregion
            }
            else
            {
                #region PLAYER 2
                characterMoveList.Add(KeyCode.UpArrow, jump);
                characterMoveList.Add(KeyCode.LeftArrow, moveLeft);
                characterMoveList.Add(KeyCode.DownArrow, crouch);
                characterMoveList.Add(KeyCode.RightArrow, moveRight);

                characterMoveList.Add(KeyCode.J, lightAttack);
                characterMoveList.Add(KeyCode.K, MediumAttack);
                characterMoveList.Add(KeyCode.L, HeavyAttack);
                #endregion
            }
            InputHandler inputManager = new InputHandler(this, characterMoveList);
            GameManager.Instance.SetPlayerHandler(playerSlotNumber, inputManager);
        }
        public virtual void Evaluate() { }
        public virtual Dictionary<int,List<string>> getQueuedActions(int numberOfFramesBack, int currentFrame)
        {
            Dictionary<int, List<string>> allQueuedActions = new Dictionary<int, List<string>>();
            if (currentFrame - numberOfFramesBack < 0)
            {
                return null;
            }
            for (int i = currentFrame - numberOfFramesBack; i <= currentFrame; i++)
            {
                if (queuedActions.ContainsKey(i))
                {
                    foreach (string s in queuedActions[i])
                    {
                        string[] stringArray = s.Split(",");
                        foreach (string sa in stringArray)
                        {
                            if (allQueuedActions.ContainsKey(i))
                                allQueuedActions[i].Add(sa);
                            else
                            {
                                allQueuedActions.Add(i, new List<string>()); 
                                allQueuedActions[i].Add(sa);
                            }
                        }
                    }
                }
            }
            return allQueuedActions;
        }
        public virtual void playAnimation(AnimationClip animation)
        {
            if (animation == null)
                return;
            if (animator == null)
                return;
            try
            {
                animator.Play(animation.name);
            }
            catch (Exception e)
            {
                Debug.Log("NOT PLAYING ANIMATION");
                Debug.LogException(e);
                return;
            }
        }
        public virtual void playAnimation(AnimationClip animation, bool force)
        {
            if (animation == null)
                return;
            if (animator == null)
                return;
            try
            {
                animator.Play(animation.name, 0, 0);
            }
            catch (Exception e)
            {
                Debug.Log("NOT PLAYING ANIMATION");
                Debug.LogException(e);
                return;
            }
        }
        public void resetAnimation()
        {
            animator.Rebind();
        }
        protected void readCommands(string eventKey)
        {
            int curFrame = GameManager.Instance.GetCurrentFrame;
            if (queuedActions.ContainsKey(curFrame))
            {
                foreach (string action in queuedActions[curFrame])
                {
                    if (action == eventKey)
                        break;
                    string possibleDiagonal = checkForDiagonals(eventKey, action);
                    if(possibleDiagonal != eventKey)
                    {
                        //Debug.Log(curFrame + " (REMOVED AND CHANGED INTO ⤵)");
                        queuedActions[curFrame].Remove(action);
                        eventKey = possibleDiagonal;
                        break;
                    }
                }
                queuedActions[curFrame].Add(eventKey);
            }
            else
            {
                queuedActions.Add(curFrame, new List<string>());
                queuedActions[curFrame].Add(eventKey);
            }
            //Debug.Log("queued action " + eventKey + " on frame " + curFrame);
        }
        protected void gotHit(CurrentAttackData attackData)
        {
            if (root == null)
                root = findRoot();
            int curFrame = GameManager.Instance.GetCurrentFrame;
            if (queuedActions.ContainsKey(curFrame))
            {
                queuedActions[curFrame].Add("GOT HIT");
            }
            else
            {
                List<string> gotHitAction = new List<string>();
                gotHitAction.Add("GOT HIT");
                queuedActions.Add(curFrame, gotHitAction);
            }
            if ((GroundState)root.findData("GroundState") < GroundState.GROUNDED)
            {
                if ((AttackState)root.findData("AttackState") <= AttackState.RECOVERY)
                    playAnimation(damageAnimations[3]);
                else
                    playAnimation(damageAnimations[4]);
            }
            else if ((GroundState)root.findData("GroundState") == GroundState.CROUCHING)
            {
                playAnimation(damageAnimations[5]);
            }
            else
            {
                switch (attackData.GetMoveData.hitType)
                {
                    case HitType.MID:
                        playAnimation(damageAnimations[0], true);
                        break;
                    case HitType.OVERHEAD:
                        playAnimation(damageAnimations[1], true);
                        break;
                    case HitType.LOW:
                        playAnimation(damageAnimations[2], true);
                        break;
                    case HitType.GRAB:
                        playAnimation(damageAnimations[3], true);
                        break;
                }
            }
            root.removeData("EnemyAttackData");
            root.addData("EnemyAttackData", attackData);
            root.removeData("AttackState");
            switch (attackData.GetMoveData.powerType)
            {
                case PowerType.NORMAL:
                    root.addData("AttackState", AttackState.HIT_STUN_RECOVERY);
                    break;
                case PowerType.KNOCK_DOWN:
                    root.addData("AttackState", AttackState.KNOCK_DOWN);
                    break;
                case PowerType.HARD_KNOCK_DOWN:
                    root.addData("AttackState", AttackState.HARD_KNOCK_DOWN);
                    break;
                case PowerType.GRAB:
                    root.addData("AttackState", AttackState.GRABBED);
                    attackData.NotifyOwnerAttackConnected();
                    break;
            }
            Debug.Log("GOT HIT " + transform.name + " " + curFrame);
            rb.velocity = Vector3.zero;
            GameManager.Instance.hitStop(.13f);
        }
        public virtual void HitConnected(MoveData connectedMove) { }
        string checkForDiagonals(string currentKey, string exisitingKey)
        {
            bool up = false;
            bool left = false;
            bool down = false;
            bool right = false;
            switch (currentKey)
            {
                case "8":
                    up = true;
                    break;
                case "4":
                    left = true;
                    break;
                case "2":
                    down = true;
                    break;
                case "6":
                    right = true;
                    break;
            }
            switch (exisitingKey)
            {
                case "8":
                    up = true;
                    break;
                case "4":
                    left = true;
                    break;
                case "2":
                    down = true;
                    break;
                case "6":
                    right = true;
                    break;
            }
            if (up && left)
                return "7";
            else if (up && right)
                return "9";
            else if (down && left)
                return "1";
            else if (down & right)
            {
                Debug.Log("RETURNS 3");
                return "3";
            }
            return currentKey;
        }
    }
}
