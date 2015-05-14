using UnityEngine;
using System.Collections;

public class PlaneController : Bolt.EntityBehaviour<IPlane> 
{
    public override void Attached()
    {
        state.transform.SetTransforms(this.transform);
    }
}
