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
    override public void Execute()
    { 
        OnNotify(movementNotation.ToString());
    }
}
