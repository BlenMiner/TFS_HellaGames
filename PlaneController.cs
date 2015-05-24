using UnityEngine;
using System.Collections;

public class PlaneController : Bolt.EntityBehaviour<IPlane> 
{
    public override void Attached()
    {
        state.transform.SetTransforms(this.transform);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 80);

        float d = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(state.targetpos.x, 0, state.targetpos.z));
        if (d < 1 && state.dropedAirDrops == false)
        {
            state.dropedAirDrops = true;
            BoltEntity airDrop = BoltNetwork.Instantiate(BoltPrefabs._01_SupplyBoxTier4, null, transform.position, Quaternion.identity);
            airDrop.TakeControl();
        }
        else if (d > 5000)
        {
            BoltNetwork.Destroy(entity);
        }
    }
}
