using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "New Move")]
public class MoveData : ScriptableObject
{
    public int startUpFrames;
    public int activeFrames;
    public int recoveryFrames;

    public bool antiAir;
    public bool overhead;
    public bool low;
}
