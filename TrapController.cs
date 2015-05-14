using UnityEngine;
using System.Collections;

public class TrapController : Bolt.EntityBehaviour<ITrap> {

    public override void Attached()
    {
 	     state.SetAnimator(GetComponent<Animator>());
    }
	void OnTriggerEnter (Collider other)
    {
        if (state.activated == false)
        {
            if (other.tag == "Animal")
            {
                IAnimals s = other.GetComponent<BoltEntity>().GetState<IAnimals>();
                s.health -= 100;

                state.activated = true;
            }
            else
            {
                ITFSPlayerState s = other.GetComponent<BoltEntity>().GetState<ITFSPlayerState>();
                s.health -= 100;

                GotHit evnt = new GotHit();
                {
                    evnt.blood = true;
                }
                evnt.Send();

                state.activated = true;
            }
        }
	}
}
