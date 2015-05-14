using UnityEngine;
using System.Collections;

public class PlayerAttacked : MonoBehaviour
{
    public BoltEntity entity;
    ITFSPlayerState state;

    void Awake()
    {
        /*for (int i = 0; i <= 14; i++)
            Physics.IgnoreLayerCollision(i, 15);*/
    }
    void Update()
    {
        if (state == null && entity != null)
            if (entity.isAttached)
            {
                state = entity.GetState<ITFSPlayerState>();
            }
    }

    /*float delay = 0;
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerFightSystem>() == null)
            return;
        BoltEntity enemy = other.GetComponent<PlayerFightSystem>().entity;
        if (state == null && BoltNetwork.isServer && enemy != null)
            return;
        else if (enemy == entity || state.attacking == false)
            return;

        Debug.Log(other.name);
        return;

        if (delay < Time.time)
        {
            delay = Time.time + 0.3f;

            using (var evnt = playSound.Raise(Bolt.GlobalTargets.AllClients, Bolt.ReliabilityModes.Unreliable))
            {
                evnt.entity = entity;
                evnt.soundID = 16;
            }
            enemy.GetState<ITFSPlayerState>().health -= 20;

            Debug.Log("Attacked player");
        }
    }*/
}
