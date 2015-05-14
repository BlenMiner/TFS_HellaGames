using UnityEngine;
using System.Collections;

public class DropedItemController : Bolt.EntityBehaviour<IDropedItem> 
{
    public override void Attached()
    {
        if (BoltNetwork.isClient)
            rigidbody.isKinematic = true;
        state.transform.SetTransforms(this.transform);

        //GameObject go = ItemsDatabase.getItem(state.itemID).ItemObjectWorld;

        //if(go == null)
        //{
            /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = transform;
            cube.transform.localPosition = new Vector3(0, 0, 0);
            cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);*/
       // }
       // else
        //{
         //   GameObject g = Instantiate(go) as GameObject;
         //   g.transform.parent = transform;
         //   g.transform.localPosition = new Vector3(0, 0, 0);
        //}
    }
}
