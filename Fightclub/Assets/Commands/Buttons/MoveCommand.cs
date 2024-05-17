using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand :Command
{
    int movementNotation;
    public MoveCommand(int movementNotation)
    {
        this.movementNotation = movementNotation;
    }
    public override void Execute()
    { 
        OnNotify(movementNotation.ToString());
    }
    public override void Execute(bool negativeEdge)
    {
        if(negativeEdge)
            OnNotify(movementNotation.ToString() + "e");
        else
            OnNotify(movementNotation.ToString() + "d");
    }
}
