using UnityEngine;
using System.Collections;

public class SoundManagerCrossbow : MonoBehaviour 
{
    public BoltEntity entity;
    public PlayerWeaponController pwc;

    public SkinnedMeshRenderer arrow;
    bool reloaded = false;

    public void Draw()
    {
        if (reloaded == false)
            arrow.enabled = false;
        else
            arrow.enabled = true;

        if (BoltNetwork.isClient && pwc.entity != null) return;
        playSound evnt = playSound.Create(Bolt.GlobalTargets.Everyone);
        {
            evnt.entity = entity;
            int id = Random.Range(7, 9);
            evnt.soundID = id;
        }
        evnt.Send();
    }
    public void Attack()
    {
        if (reloaded == false)
            arrow.enabled = false;
        else
            arrow.enabled = true;

        if (BoltNetwork.isClient && pwc.entity != null) return;

        pwc.FireArrow();
        playSound evnt = playSound.Create(Bolt.GlobalTargets.Everyone); 
        {
            evnt.entity = entity;
            int id = Random.Range(208, 211);
            evnt.soundID = id;
        }
        evnt.Send();
    }
    public void Reload()
    {
        arrow.enabled = true;
    }

    public void SetReloaded(string b)
    {
        if(b == "false")
        {
            arrow.enabled = false;
            reloaded = false;
        }
        else
        {
            arrow.enabled = true;
            reloaded = true;
        }
    }
    public void EndAttack()
    {
        pwc.EndAttack();
    }
}
