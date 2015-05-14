using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public BoltEntity entity;
    public PlayerWeaponController pwc;

    public int drawSound = 7;
    public int drawSound2 = -1;
	
	public void Draw ()
    {
        if (BoltNetwork.isClient && pwc.entity != null) return;
        if (drawSound >= 0)
        {
            playSound evnt = playSound.Create(Bolt.GlobalTargets.Everyone);
            {
                if (drawSound >= 7 && drawSound <= 9)
                    drawSound = Random.Range(7, 9);

                evnt.entity = entity;
                int id = drawSound;
                evnt.soundID = id;
            }
            evnt.Send();
        }

        if(drawSound2 >= 0)
        {
            playSound evnt = playSound.Create(Bolt.GlobalTargets.Everyone); 
            {
                evnt.entity = entity;
                int id = drawSound2;
                evnt.soundID = id;
            }
            evnt.Send();
        }
    }
    public void Swing()
    {
        if (BoltNetwork.isClient && pwc.entity != null) return;

        playSound evnt = playSound.Create(Bolt.GlobalTargets.Everyone); 
        {
            evnt.entity = entity;
            int id = Random.Range(10, 15);
            evnt.soundID = id;
        }
        evnt.Send();
    }
    public void CanAttack()
    {
        pwc.CanAttack();
    }
    public void Block()
    {
        if (BoltNetwork.isClient && pwc.entity != null) return;
        playSound evnt = playSound.Create(Bolt.GlobalTargets.Everyone); 
        {
            evnt.entity = entity;
            int id = Random.Range(5, 6);
            evnt.soundID = id;
        }
        evnt.Send();
    }
    public void BlockHit()
    {
        if (BoltNetwork.isClient && pwc.entity != null) return;
        playSound evnt = playSound.Create(Bolt.GlobalTargets.Everyone); 
        {
            evnt.entity = entity;
            int id = Random.Range(2, 4);
            evnt.soundID = id;
        }
        evnt.Send();
    }
    public void Stab()
    {
        if (BoltNetwork.isClient) return;

    }

    public void Squirt()
    {
        if (entity.isOwner)
        {
            BleedBehavior.BloodAmount += 0.05f;
        }
    }
    public void ChargeWeapon(float t)
    {
        pwc.ChargeWeapon(t);
    }

    public void EndFirstComoboPart()
    {
        pwc.SwitchCombo();
    }
    public void EndAttack()
    {
        pwc.EndAttack();
    }

}
