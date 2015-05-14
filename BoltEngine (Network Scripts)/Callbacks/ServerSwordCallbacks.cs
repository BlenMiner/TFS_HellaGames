using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerSwordCallbacks : Bolt.GlobalEventListener
{
    ItemsDatabase ItemsDatabase;
    public List<Item> ItemList = new List<Item>();
    public bool runCallbacks = false;
    public bool loadedWeapons = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (BoltNetwork.isServer)
        {
            runCallbacks = true;
        }
        else
        {
            runCallbacks = false;
        }
    }

    //Item selected
    public override void OnEvent(InventorySetSelectedItem e)
    {
        if (!runCallbacks) return;
        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();

        if (loadedWeapons == false)
        {
            ItemList = ItemsDatabase.instance.ItemList;
            loadedWeapons = true;
        }
    }

    //Player attack
    public override void OnEvent(PlayerSwingSword e)
    {
        if (!runCallbacks) return;
        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();


    }
}
