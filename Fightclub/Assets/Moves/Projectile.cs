using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Connect;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int firePointNum;

    [SerializeField]
    protected int activeFrames;
    [SerializeField]
    protected float speed;

    protected Transform firepoint;
    protected bool active;
    protected int direction;
    protected int frameStarted;
    protected Rigidbody rb;

    private void Update()
    {
        updateProjectile();
    }
    public void setSpawn(Transform firepoint)
    {
        this.firepoint = firepoint;
    }
    public virtual void spawnProjectile(int direction, int frameStarted)
    {
        rb = GetComponent<Rigidbody>();
        transform.position = firepoint.position;
        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z);
        this.frameStarted = frameStarted;

        transform.gameObject.SetActive(true);
        Debug.Log(gameObject, this);
        active = true;
    }
    protected virtual void updateProjectile() 
    {
        if (!active)
            return;
        if(GameManager.Instance.GetCurrentFrame - frameStarted >= activeFrames)
        {
            Debug.Log("PROJECTILE ACTIVE OUT");
            active = false;
            gameObject.SetActive(false);
        }
    }
}
