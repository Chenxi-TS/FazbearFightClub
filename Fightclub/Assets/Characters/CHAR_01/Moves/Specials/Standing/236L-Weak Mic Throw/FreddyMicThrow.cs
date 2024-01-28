using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreddyMicThrow : Projectile
{
    public override void spawnProjectile(int direction, int frameStarted, BehaviorTree.Tree moveUser)
    {
        base.spawnProjectile(direction, frameStarted, moveUser);
        rb.useGravity = false;
        rb.velocity = new Vector3(speed * direction, 0, 0);
    }
    protected override void updateProjectile()
    {
        base.updateProjectile();
        if(GetComponent<Renderer>().isVisible == false)
        {
            active = false;
            gameObject.SetActive(false);
            Debug.Log("FREDDY PROJECTILE OFF SCREEN");
        }
        Debug.Log(rb.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        BehaviorTree.Tree hurtSubject = other.GetComponent<MoveListHolder>().masterTree;
        if (hurtSubject == moveUser)
        {
            return;
        }
        active = false;
        gameObject.SetActive(false);
        Debug.Log("FREDDY PROJECTILE HIT");
        //Notify Hit
    }
}
