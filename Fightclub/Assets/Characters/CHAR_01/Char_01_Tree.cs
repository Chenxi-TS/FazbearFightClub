using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace BehaviorTree
{
    public class Char_01_Tree : Tree, Observer
    {
        Node root;
        public Char_01_Tree(int playerSlotNumber) : base(playerSlotNumber)
        {
            rb = GameManager.Instance.characters[0].GetComponent<Rigidbody>();
            transform = rb.transform;
            animator = transform.GetComponent<Animator>();
            //base sets up a new inputManager for this tree specifically and adds movement and attack buttons
            List<MoveData> moveList = GameManager.Instance.characters[0].GetComponent<MoveListHolder>().moveList;
            //Light: Paw Punch
            MoveData PAW_PUNCH = moveList[0];
            AttackTask pawPunch = new AttackTask(PAW_PUNCH, this, transform);
            //Medium: Hand-break
            MoveData HAND_BREAK = moveList[1];
            AttackTask handBreak = new AttackTask(HAND_BREAK, this, transform);
            //Heavy: Elbow Wave
            /*MoveData ELBOW_WAVE = moveList[2];
            AttackTask elbowWave = new AttackTask(ELBOW_WAVE, this, transform);

            //Crouching: Light: Mic Punch (Low)
            MoveData MIC_PUNCH = moveList[3];
            AttackTask micPunch = new AttackTask (MIC_PUNCH, this, transform);
            //Crouching: Medium: Speak Into the Mic (Low)
            MoveData SPEAK_MIC = moveList[4];
            AttackTask speakMic = new AttackTask(SPEAK_MIC, this, transform);
            //Crouching: Heavy: Bear Sweep (Low)
            MoveData BEAR_SWEEP = moveList[5];
            AttackTask bearSweep = new AttackTask(BEAR_SWEEP, this, transform);*/
            
            //set up character tree below//
            root = new CheckAttackStateDecor(new List<Node>
            {
                new Selector(new List<Node> { //0
                    new Selector(new List<Node> //1
                    { 
                        //new CheckHealthDecorator(deathTask, healthref),
                        //new CheckAttackStateDecor(burstTask, burstMove),
                        //new takeDamageTask(healthref)
                    }, "deathBurstTakeDamageSelector"),
                    new Selector(new List<Node> //2
                    {
                        new Selector(new List<Node> //3
                        {
                            new CheckGroundStateDecorator(new List<Node>
                            {
                                new Selector(/*list of air OK moves*/)
                            }, GroundState.AIRBORNE),
                            new Selector(new List<Node>
                            {
                                //Standings
                                new CheckQueueDecorator(new List<Node>{pawPunch}, this, PAW_PUNCH.inputNotations,0),
                                new CheckQueueDecorator(new List<Node>{handBreak}, this, HAND_BREAK.inputNotations, 0),
                                /*new CheckQueueDecorator(new List<Node>{elbowWave}, this, ELBOW_WAVE.inputNotations, 0),
                                //Crouchings
                                new CheckQueueDecorator(new List<Node>{micPunch}, this, MIC_PUNCH.inputNotations, 0),
                                new CheckQueueDecorator(new List<Node>{speakMic}, this, SPEAK_MIC.inputNotations, 0),
                                new CheckQueueDecorator(new List<Node>{bearSweep}, this, BEAR_SWEEP.inputNotations, 0),*/
                                //Aerial
                            }, "groundAttackSelector")
                        }, "air/groundSelector"),
                        new Selector(new List<Node> //4
                        {
                            new CheckQueueDecorator(new List<Node>(){new MoveTask(rb, 1, 3, "move right") }, this, "6",0), //right
                            new CheckQueueDecorator(new List<Node>(){new MoveTask(rb, -1, 3, "move left") }, this, "4",0)//left
                            //jump
                            //crouch
                        }, "movementSelector")
                    }, "attackMoveSelector")
                }, "baseSelector")
            });
        }
        void Observer.OnNotify(string eventKey)
        {
            readCommands(eventKey);
        }
        public override NodeState Evaluate()
        {
            return this.root.Evaluate();
        }
    }
}
