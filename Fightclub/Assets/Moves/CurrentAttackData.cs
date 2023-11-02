using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentAttackData
{
    int frameAttackStarted;
    GameObject hitbox;
    public CurrentAttackData(int frameStarted, MoveData moveData, GameObject hitbox)
    {
        frameAttackStarted = frameStarted;
        this.moveData = moveData;
        this.hitbox = hitbox;
    }
    public int GetStartingFrame { get { return frameAttackStarted;} }
    MoveData moveData;
    public MoveData GetMoveData { get { return moveData; } }
    public GameObject GetHitbox { get { return hitbox; } }
}
