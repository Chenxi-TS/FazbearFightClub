using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviorTree
{
    public enum GroundState
    {
        GROUNDED,
        START_UP,
        AIRBORNE,
        CROUCHING,
        RECOVERY
    }
    public enum AttackState
    {
        NONE,
        START_UP,
        ACTIVE,
        RECOVERY,
        HIT_STUN, //move these to a enum later
        HIT_STUN_RECOVERY,
        KNOCK_DOWN,
        HARD_KNOCK_DOWN,
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

        protected Rigidbody rb;
        protected Transform transform;
        protected Animator animator;

        protected Dictionary<KeyCode, Command> characterMoveList = new Dictionary<KeyCode, Command>();
        protected Dictionary<int, List<string>> queuedActions = new Dictionary<int, List<string>>();

        public Tree(int playerSlotNumber)
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
        public virtual List<string> getQueuedActions(int numberOfFramesBack, int currentFrame)
        {
            List<string> allQueuedActions = new List<string>();
            if (currentFrame - numberOfFramesBack < 0)
            {
                return null;
            }
            if (numberOfFramesBack == 0)
            {
                if (queuedActions.ContainsKey(currentFrame))
                {
                    foreach (string s in queuedActions[currentFrame])
                        allQueuedActions.Add(s);

                    return allQueuedActions;
                }
            }
            for (int i = currentFrame - numberOfFramesBack; i <= currentFrame; i++)
            {
                if (queuedActions.ContainsKey(i))
                {
                    foreach(string s in queuedActions[i])
                        allQueuedActions.Add(s);
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
                Debug.LogException(e);
                return;
            }
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
                return "3";
            return currentKey;
        }
    }
}
