using UnityEngine;
using System.Collections;

public class Wall : Bolt.EntityBehaviour<IBuildings>
{
    public bool addTolist = true;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (addTolist)
            ServerInventoryCallbacks.walls.Add(gameObject.transform);
    }

    bool canWork = false;
    public override void Attached()
    {
        canWork = true;
    }
}
