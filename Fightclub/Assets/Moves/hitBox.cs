using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class hitBox : MonoBehaviour
{
    BehaviorTree.Node moveUser;
    CurrentAttackData attackData;
    public void setUser(BehaviorTree.Node user)
    {
        moveUser = user;
        Debug.Log("USER IS " + user);
    }
    public void setAttackData(CurrentAttackData attackData)
    {
        this.attackData = attackData;
    }
    private void OnTriggerEnter(Collider other)
    {
        Observer hurtSubject = other.GetComponent<MoveListHolder>().masterTree;
        attackData = new CurrentAttackData(GameManager.Instance.GetCurrentFrame, attackData.GetMoveData, attackData.GetHitbox, attackData.Projectile);
        hurtSubject.OnNotify(attackData);
    }

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }
}
