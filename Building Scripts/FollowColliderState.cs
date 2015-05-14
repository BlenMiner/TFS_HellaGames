using UnityEngine;
using System.Collections;

public class FollowColliderState : MonoBehaviour
{
    public Collider follow;
    public Collider[] colliders;

    bool active = false;

    void Start()
    {
        active = follow;
        foreach (Collider c in colliders)
        {
            if (c != null)
                c.enabled = follow.enabled;
        }
    }

    BoltEntity root;
    void Update()
    {
        if (root == null)
            root = transform.root.GetComponent<BoltEntity>();

        if (root.isAttached)
            this.enabled = false;

        if (follow == null)
            return;
        if (active != follow.enabled)
        {
            foreach (Collider c in colliders)
            {
                if (c != null)
                    c.enabled = follow.enabled;
            }
            active = follow.enabled;
        }
    }
}
