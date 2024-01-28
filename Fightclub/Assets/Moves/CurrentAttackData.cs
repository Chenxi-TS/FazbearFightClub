using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentAttackData
{
    int frameAttackStarted;
    GameObject hitbox;
    Projectile projectile;
    BehaviorTree.Tree attackOwner;
    MoveData moveData;
    public CurrentAttackData(int frameStarted, MoveData moveData, GameObject hitbox, Projectile projectile, BehaviorTree.Tree attackOwner)
    {
        frameAttackStarted = frameStarted;
        this.moveData = moveData;
        this.hitbox = hitbox;
        this.projectile = projectile;
        this.attackOwner = attackOwner;
    }
    public int GetStartingFrame { get { return frameAttackStarted;} }
    public MoveData GetMoveData { get { return moveData; } }
    public GameObject GetHitbox { get { return hitbox; } }
    public Projectile Projectile { get { return projectile; } }
    public void NotifyOwnerAttackConnected()
    {
        attackOwner.HitConnected(moveData);
    }
}
