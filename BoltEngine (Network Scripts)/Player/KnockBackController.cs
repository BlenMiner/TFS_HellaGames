using UnityEngine;
using System.Collections;

public class KnockBackController : Bolt.EntityEventListener
{
    public PlayerWeaponController pwc;
    CharacterController cc;
    void Awake()
    {
        cc = gameObject.GetComponent<CharacterController>();
    }
    
    public override void OnEvent(KnockBack e)
    {
        cc.Move((e.direction.normalized));
    }
    public override void OnEvent(GotHit e)
    {
        if (entity.isOwner)
        {
            if (e.blood)
                BleedBehavior.BloodAmount += 0.5f;

            pwc.GetAnimator().SetTrigger("Hit");
        }
    }
}
