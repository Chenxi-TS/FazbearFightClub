using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchCommand : Command
{
    public override void Execute()
    {
        OnNotify("2");
    }
    public override void Execute(bool negativeEdge)
    {
        if (negativeEdge)
            OnNotify("2e");
        else
            OnNotify("2d");
    }
}
