using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Char_01_Tree : Tree, Observer
    {
        //base sets up a new inputManager for this tree specifically and adds movement and attack buttons
        public Char_01_Tree(int playerSlotNumber, GameObject characterBody) : base(playerSlotNumber, characterBody)
        {
            Debug.Log("CHAR 1 TREE MADE");
            rb = characterBody.GetComponent<Rigidbody>();
            transform = rb.transform;
            animator = transform.GetChild(0).GetComponent<Animator>();

            MoveListHolder moveListHolder = characterBody.GetComponent<MoveListHolder>();
            moveListHolder.masterTree = this;

            movementAnimations = moveListHolder.movementAnimations;
            damageAnimations = moveListHolder.damageAnimations;
            recoveryAnimations = moveListHolder.recoveryAnimations;

            Debug.Log(this + " TREE");

            #region ATTACKS
            List<MoveData> moveList = moveListHolder.moveList;
            //Light: Paw Punch
            MoveData PAW_PUNCH = moveList[0];
            AttackTask pawPunch = new AttackTask(PAW_PUNCH, this, rb, transform, playerSlotNumber);
            //Medium: Hand-break
            MoveData HAND_BREAK = moveList[1];
            AttackTask handBreak = new AttackTask(HAND_BREAK, this, rb, transform, playerSlotNumber);
            //Heavy: Elbow Wave
            MoveData ELBOW_WAVE = moveList[2];
            AttackTask elbowWave = new AttackTask(ELBOW_WAVE, this, rb, transform, playerSlotNumber);
            MoveData ELBOW_WAVE_2 = moveList[2].specialCancelables[0];
            AttackTask elbowWave2 = new AttackTask(ELBOW_WAVE_2, this, rb, transform, ELBOW_WAVE, playerSlotNumber);

            //Crouching: Light: Mic Punch (Low)
            MoveData MIC_PUNCH = moveList[3];
            AttackTask micPunch = new AttackTask(MIC_PUNCH, this, rb, transform, playerSlotNumber);
            //Crouching: Medium: Speak Into the Mic (Low)
            MoveData SPEAK_MIC = moveList[4];
            AttackTask speakMic = new AttackTask(SPEAK_MIC, this, rb, transform, playerSlotNumber);
            //Crouching: Heavy: Bear Sweep (Low)
            MoveData BEAR_SWEEP = moveList[5];
            AttackTask bearSweep = new AttackTask(BEAR_SWEEP, this, rb, transform, playerSlotNumber);

            //Aerial: Light: Midair Punch (Overhead)
            MoveData MIDAIR_PUNCH = moveList[6];
            AttackTask midairPunch = new AttackTask(MIDAIR_PUNCH, this, rb, transform, playerSlotNumber);
            //Aerial: Medium: Downward Mic (Overhead)
            MoveData DOWNWARD_MIC = moveList[7];
            AttackTask downwardMic = new AttackTask(DOWNWARD_MIC, this, rb, transform, playerSlotNumber);
            //Aerial: Heavy: Flat Footed Kick (Overhead)
            MoveData FLAT_FOOTED_KICK = moveList[8];
            AttackTask flatFootedKick = new AttackTask(FLAT_FOOTED_KICK, this, rb, transform, playerSlotNumber);

            //Command: Frame Bash
            MoveData FRAME_BASH = moveList[9];
            AttackTask frameBash = new AttackTask(FRAME_BASH, this, rb, transform, playerSlotNumber);
            //Command: Grab: Power Outage
            MoveData GRAB = moveList[10];
            AttackTask grab = new AttackTask(GRAB, this, rb, transform, playerSlotNumber);

            //Special: Weak Mic Throw
            MoveData WEAK_MIC_THROW = moveList[11];
            AttackTask weakMicThrow = new AttackTask(WEAK_MIC_THROW,  this, rb, transform, playerSlotNumber);
            
            //Crouching
            MoveData CROUCHING = moveList[12];
            CrouchTask crouching = new CrouchTask(this, CROUCHING.moveAnimation, true);
            MoveData UNCROUCHING = moveList[13];
            CrouchTask uncroucing = new CrouchTask(this, UNCROUCHING.moveAnimation, false);
            #endregion
            //character tree below//
            root =
            new Selector(new List<Node> //First
            {
                new BurstTask(),
                //Updates
                new UpdateGroundStateTask(this, rb, transform, movementAnimations[3], playerSlotNumber),
                new CheckHitQueue(new List<Node>{new UpdateHitTask(this, recoveryAnimations)}, this),
                new UpdateAttackStateTask(this),
                new UpdateDefenseStateTask(this, playerSlotNumber),
                //Movements & Attacks
                new Selector(new List<Node>
                {
                    // Attacks
                    new Selector(new List<Node>
                    {
                        //Standing Attacks
                        new CheckGroundStateDecorator(GroundState.GROUNDED,
                            new Selector(new List<Node>
                            {
                                new CheckQueueDecorator(new List<Node>{ frameBash}, this, FRAME_BASH.inputNotations, 1),
                                new CheckQueueDecorator(new List<Node>{ grab}, this, GRAB.inputNotations, 1),
                                new CheckQueueDecorator(new List<Node>{ weakMicThrow}, this, WEAK_MIC_THROW.inputNotations, 8),

                                new CheckQueueDecorator(new List<Node>{ pawPunch}, this, PAW_PUNCH.inputNotations, 0),
                                new CheckQueueDecorator(new List<Node>{ handBreak}, this, HAND_BREAK.inputNotations, 0),
                                new CheckQueueDecorator(new List<Node>{ elbowWave}, this, ELBOW_WAVE.inputNotations,0),
                                elbowWave2
                            })
                        ),
                        //Crouching Attacks
                        new CheckGroundStateDecorator(GroundState.CROUCHING,
                            new Selector(new List<Node>
                            {
                                new CheckQueueDecorator(new List<Node>{ micPunch}, this, MIC_PUNCH.inputNotations, 0),
                                new CheckQueueDecorator(new List<Node>{ speakMic}, this, SPEAK_MIC.inputNotations, 0),
                                new CheckQueueDecorator(new List<Node>{ bearSweep}, this, BEAR_SWEEP.inputNotations, 0)
                            })
                        ),
                        //Aerial Attacks
                        new CheckGroundStateDecorator(GroundState.AIRBORNE,
                            new Selector(new List<Node>
                            {
                                new CheckQueueDecorator(new List<Node>{ midairPunch}, this, MIDAIR_PUNCH.inputNotations, 0),
                                new CheckQueueDecorator(new List<Node>{ downwardMic}, this, DOWNWARD_MIC.inputNotations, 0),
                                new CheckQueueDecorator(new List<Node>{ flatFootedKick}, this, FLAT_FOOTED_KICK.inputNotations, 0)
                            })
                        ),
                    }),
                    //Crouch
                    new Selector(new List<Node> {
                        new CheckGroundStateDecorator(GroundState.GROUNDED,
                            new Selector(new List<Node> {
                                new CheckQueueDecorator(new List<Node>{ crouching}, this, "2", 0),
                                new CheckQueueDecorator(new List<Node>{ crouching}, this, "1", 0),
                                new CheckQueueDecorator(new List<Node>{ crouching}, this, "3", 0)
                            })),
                        
                        //Uncrouch
                        new CheckGroundStateDecorator(GroundState.CROUCHING,
                            new CheckQueueDecorator(new List<Node>{ uncroucing}, this, "!2,!1,!3,!2e,!1e,!3e", true, 0))
                    }),
                    new CheckGroundStateDecorator(GroundState.GROUNDED,
                        new Selector(new List<Node>{ //Movements
                            //Jump
                            new Selector(new List<Node>
                            {
                                new CheckQueueDecorator(new List<Node>{ new JumpTask(this, 1, 18, movementAnimations[2])}, this, "9", 0, false), //Jump right
                                new CheckQueueDecorator(new List<Node>{ new JumpTask(this, -1, 18, movementAnimations[2]) }, this, "7", 0, false), //Jump left
                                new CheckQueueDecorator(new List<Node>{ new JumpTask(this, 0, 18, movementAnimations[2]) }, this, "8", 0, false), //Jump up
                                
                            }),
                            //Left & Right
                            new Selector (new List<Node>
                            {
                                new CheckQueueDecorator(new List<Node>{ new MoveTask(rb, -1, 5, "Move Left", this, playerSlotNumber, movementAnimations[0], movementAnimations[1])}, this, "4", 0, false),
                                new CheckQueueDecorator(new List<Node>{ new MoveTask(rb, 1, 5, "Move Right",this, playerSlotNumber, movementAnimations[0], movementAnimations[1])}, this, "6", 0, false),
                                new IdleTask(this, rb, movementAnimations[6]),
                            })
                        })
                    )
                })
            });
            root.addData("DefenseState", DefenseState.NONE);
        }
        void Observer.OnNotify(object value)
        {
            if(value is string)
                readCommands((string)value);
            if (value is CurrentAttackData)
                GotHit((CurrentAttackData)value);
        }
        public override void Evaluate()
        {
            NodeState state = root.Evaluate();
        }

        public override void HitConnected(MoveData connectedMove)
        {
            if(connectedMove.powerType == PowerType.GRAB)
            {
                root.removeData("AttackState");
                root.addData("AttackState", AttackState.GRABBING);
                playAnimation(movementAnimations[7]);
            }
        }
    }
}
