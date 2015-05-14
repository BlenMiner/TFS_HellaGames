using UnityEngine;
using System.Collections;

public class boxtest : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }
}
