using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCommand : Command
{
    public override void Execute()
    {
        OnNotify("8");
    }
    public override void Execute(bool negativeEdge)
    {
        if (negativeEdge)
            OnNotify("8e");
        else
            OnNotify("8d");
    }
}
