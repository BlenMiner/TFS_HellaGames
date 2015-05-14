using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RacksController : Bolt.EntityBehaviour<IWeaponRacks> 
{
    bool startChecking = false;

    public GameObject[] slots;
    GameObject[] weapons = new GameObject[10];

    void Start()
    {
        /*if(BoltNetwork.isClient)
        {
            this.enabled = false;
        }*/
    }
    bool me = false;
    public override void Attached()
    {
        if (BoltNetwork.isServer)
        {
            entity.TakeControl();

            me = true;
            startChecking = true;
            StartCalled();
        }
        else
        {
            startChecking = true;
        }
    }
    void StartCalled()
    {
        fillRack fR = new fillRack();
        fR.entity = entity;
        fR.Send();
    }
    void Update()
    {
        if(startChecking)
        {
            for(int i = 0; i < 10; i ++)
            {
                if(state.ItemsInside[i].itemID > 0)
                {
                    if(weapons[i] == null)
                    {
                        weapons[i] = GameObject.Instantiate(ItemsDatabase.getItem(state.ItemsInside[i].itemID).ItemObjectWorld) as GameObject;
                        weapons[i].transform.parent = slots[i].transform;
                        weapons[i].transform.localPosition = new Vector3(0, 0, 0);
                    }
                }
                else
                {
                    if (weapons[i] != null)
                        Destroy(weapons[i]);
                }
            }
        }
    }
}
