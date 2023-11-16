using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentAttackData
{
    int frameAttackStarted;
    GameObject hitbox;
    Projectile projectile;
    public CurrentAttackData(int frameStarted, MoveData moveData, GameObject hitbox, Projectile projectile)
    {
        frameAttackStarted = frameStarted;
        this.moveData = moveData;
        this.hitbox = hitbox;
        this.projectile = projectile;
    }
    public int GetStartingFrame { get { return frameAttackStarted;} }
    MoveData moveData;
    public MoveData GetMoveData { get { return moveData; } }
    public GameObject GetHitbox { get { return hitbox; } }
    public Projectile Projectile { get { return projectile; } }
}
