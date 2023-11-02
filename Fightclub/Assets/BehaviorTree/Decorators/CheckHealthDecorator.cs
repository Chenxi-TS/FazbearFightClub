using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class CheckHealthDecorator : Node
    {
        float health;
        public CheckHealthDecorator(List<Node> childrenNodes, ref float health): base(childrenNodes) 
        {
            this.health = health;
            this.health -= 10;
            Debug.Log(this.health);
        }
        public override NodeState Evaluate()
        {
            if (health <= 0 || childrenNodes.Count <= 0)
                return NodeState.FAILURE;
            foreach(Node child in childrenNodes)
            {
                return child.Evaluate();
            }
            return NodeState.FAILURE;
        }
    }
}