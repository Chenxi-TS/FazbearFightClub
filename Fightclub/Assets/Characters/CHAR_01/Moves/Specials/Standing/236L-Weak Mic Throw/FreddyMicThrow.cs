using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreddyMicThrow : Projectile
{
    public override void spawnProjectile(int direction, int frameStarted)
    {
        base.spawnProjectile(direction, frameStarted);
        rb.useGravity = false;
        rb.velocity = new Vector3(speed * direction, 0, 0);
    }
    protected override void updateProjectile()
    {
        base.updateProjectile();
        Debug.Log(rb.velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        active = false;
        gameObject.SetActive(false);
        Debug.Log("FREDDY PROJECTILE HIT");
        //Notify Hit
    }
}
