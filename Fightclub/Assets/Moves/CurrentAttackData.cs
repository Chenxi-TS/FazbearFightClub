using UnityEngine;

public class CurrentAttackData
{
    int frameAttackStarted;
    GameObject hitbox;
    Projectile projectile;
    BehaviorTree.Tree attackOwner;
    MoveData moveData;
    int direction;
    public CurrentAttackData(int frameStarted, MoveData moveData, GameObject hitbox, Projectile projectile, BehaviorTree.Tree attackOwner, int direction)
    {
        frameAttackStarted = frameStarted;
        this.moveData = moveData;
        this.hitbox = hitbox;
        this.projectile = projectile;
        this.attackOwner = attackOwner;
        this.direction = direction;
    }
    public int GetStartingFrame { get { return frameAttackStarted;} }
    public MoveData GetMoveData { get { return moveData; } }
    public GameObject GetHitbox { get { return hitbox; } }
    public Projectile Projectile { get { return projectile; } }
    public int Direction { get { return direction; } }
    public void NotifyOwnerAttackConnected()
    {
        attackOwner.HitConnected(moveData);
    }
}
