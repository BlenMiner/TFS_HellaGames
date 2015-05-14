using UnityEngine;
using System.Collections;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "Main Demo")]
public class EntitySpawnerS : Bolt.EntityEventListener
{
    public override void OnEvent(SpawnPrefabBolt e)
    {
        Debug.Log(e.prefab.ToString());
        //GameObject go = GameObject.Instantiate(e.prefab.ToString(), e.position, Quaternion.identity) as GameObject;
    }
}
