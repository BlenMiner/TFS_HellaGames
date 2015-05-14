using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public void Awake()
    {
        DisablesCol();
    }
    public BoltEntity entity;

    void DisablesCol()
    {
        foreach (Collider c in entity.gameObject.GetComponents<Collider>())
        {
            if (this.collider != c)
            {
                Physics.IgnoreCollision(this.collider, c);
            }
        }
        foreach (Collider c in entity.gameObject.GetComponentsInParent<Collider>())
        {
            if (this.collider != c)
                Physics.IgnoreCollision(this.collider, c);
        }
        foreach (Collider c in entity.gameObject.GetComponentsInChildren<Collider>())
        {
            if (this.collider != c)
                Physics.IgnoreCollision(this.collider, c);
        }
    }
}
