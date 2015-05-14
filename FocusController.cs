using UnityEngine;
using System.Collections;

public class FocusController : MonoBehaviour {

    public GameObject go;
	
	void Update ()
    {
	    RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 10000f))
        {
            go.transform.position = hit.point;
        }
	}
}
