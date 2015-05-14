using UnityEngine;
using System.Collections;

public class Celling : MonoBehaviour {

    public bool addTolist = true;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (addTolist)
            NetworkManager.ceillings.Add(gameObject);
    }
}
