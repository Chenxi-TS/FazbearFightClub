using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviorTree
{
    public class UpdateDefenseStateTask : Node
    {
        Tree masterTree;
        int playerSlotNum;
        public UpdateDefenseStateTask(Tree masterTree, int playerSlot) : base()
        {
            this.masterTree = masterTree;
            playerSlotNum = playerSlot;
        }
        public override NodeState Evaluate()
        {
            if (root == null)
                root = findRoot();
            if (!checkDataStatus("DefenseState", DefenseState.NONE))
                return NodeState.FAILURE;

            Dictionary<int, List<string>> actionsWithinBuffer = masterTree.getQueuedActions(0, GameManager.Instance.GetCurrentFrame);
            if (actionsWithinBuffer.Count < 1)
            {
                Debug.Log("block update count < 1");
                root.removeData("DefenseState");
                return NodeState.FAILURE;
            }
            string currentAction = actionsWithinBuffer.First().Value[0].ToString();
            Debug.Log(currentAction + " block check");
            if (playerSlotNum == 1)
            {
                checkIfblocking(GameManager.Instance.getPlayer1FacingRight, currentAction);
            }
            else if(playerSlotNum == 2)
            {
                checkIfblocking(!GameManager.Instance.getPlayer1FacingRight, currentAction);
            }
            return NodeState.FAILURE;
        }
        void checkIfblocking(bool leftIsBlock, string currentAction)
        {
            if (leftIsBlock)
            {
                if (currentAction == "4")
                {
                    removeData("DefenseState");
                    root.addData("DefenseState", DefenseState.HIGH_BLOCK);
                }
                else if (currentAction == "1")
                {
                    removeData("DefenseState");
                    root.addData("DefenseState", DefenseState.LOW_BLOCK);
                }
            }
            else
            {
                if (currentAction == "6")
                {
                    removeData("DefenseState");
                    root.addData("DefenseState", DefenseState.HIGH_BLOCK);
                }
                else if (currentAction == "3")
                {
                    removeData("DefenseState");
                    root.addData("DefenseState", DefenseState.LOW_BLOCK);
                }
            }
        }
    }
}
