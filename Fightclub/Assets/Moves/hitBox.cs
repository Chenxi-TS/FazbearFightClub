using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class hitBox : MonoBehaviour
{
    BehaviorTree.Tree moveUser;
    CurrentAttackData attackData;
    public void setUser(BehaviorTree.Tree user)
    {
        moveUser = user;
        //Debug.Log("USER IS " + user);
    }
    public void setAttackData(CurrentAttackData attackData)
    {
        this.attackData = attackData;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("got a hit on " + other.transform.name);
        MoveListHolder otherHolder;
        other.TryGetComponent<MoveListHolder>(out otherHolder);
        Observer hurtSubject;
        if (otherHolder != null)
        {
            hurtSubject = otherHolder.masterTree;
            if (hurtSubject == moveUser)
            {
                return;
            }
            Debug.Log(hurtSubject + "HURT");
            attackData = new CurrentAttackData(GameManager.Instance.GetCurrentFrame, attackData.GetMoveData, attackData.GetHitbox, attackData.Projectile, moveUser, attackData.Direction);
            hurtSubject.OnNotify(attackData);
        }
    }

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }
}
