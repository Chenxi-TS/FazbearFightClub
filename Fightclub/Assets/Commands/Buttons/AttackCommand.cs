using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command
{
    public enum AttackButtons
    {
        LIGHT,
        MEDIUM,
        HEAVY,
    }
    AttackButtons assignedButton;
    public AttackCommand(AttackButtons assignedAttackButton)
    {
        assignedButton = assignedAttackButton;
    }
    public override void Execute()
    {
        switch(assignedButton)
        {
            case AttackButtons.LIGHT:
                OnNotify("L");
                break;
            case AttackButtons.MEDIUM:
                OnNotify("M");
                break;
            case AttackButtons.HEAVY:
                OnNotify("H");
                break;
        }
    }
    public override void Execute(bool negativeEdge)
    {
        if(negativeEdge)
        {
            switch (assignedButton)
            {
                case AttackButtons.LIGHT:
                    OnNotify("Le");
                    break;
                case AttackButtons.MEDIUM:
                    OnNotify("Me");
                    break;
                case AttackButtons.HEAVY:
                    OnNotify("He");
                    break;
            }
        }
        else
        {
            switch (assignedButton)
            {
                case AttackButtons.LIGHT:
                    OnNotify("Ld");
                    break;
                case AttackButtons.MEDIUM:
                    OnNotify("Md");
                    break;
                case AttackButtons.HEAVY:
                    OnNotify("Hd");
                    break;
            }
        }
    }
}
