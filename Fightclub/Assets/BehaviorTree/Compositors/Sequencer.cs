using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Sequencer : Node
    {
        public Sequencer() { }
        public Sequencer(List<Node> childrenNodes): base(childrenNodes) { }

        public override NodeState Evaluate()
        {
            bool anychildRunning = false;
            foreach(Node child in childrenNodes)
            {
                NodeState childState = child.Evaluate();
                switch (childState)
                {
                    case NodeState.RUNNING:
                        anychildRunning = true;
                        continue;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.FAILURE:
                        return NodeState.FAILURE;
                }
            }
            if (anychildRunning)
                return NodeState.RUNNING;
            return NodeState.SUCCESS;
        }
    }
}
