using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchCommand : Command
{
    public override void Execute()
    {
        OnNotify("2");
    }
}
