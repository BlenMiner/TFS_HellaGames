using UnityEngine;
using System.Collections;

public class AirdropController : Bolt.EntityBehaviour<IAirDrop>
{
    public override void Attached()
    {
        if (BoltNetwork.isClient)
            rigidbody.isKinematic = true;

        state.tranform.SetTransforms(this.transform);
        state.SetAnimator(this.GetComponent<Animator>());

        Physics.IgnoreLayerCollision(23, 9);
        Physics.IgnoreLayerCollision(23, 15);
        Physics.IgnoreLayerCollision(23, 17);
    }

    void Update()
    {
        if (state.grounded == true)
        {
            this.enabled = false;
        }
        if (BoltNetwork.isClient)
            return;

        Vector3 fwd = Vector3.down;
        if (Physics.Raycast(transform.position, fwd, 1.5f))
        {
            state.grounded = true;
        }
    }
}
