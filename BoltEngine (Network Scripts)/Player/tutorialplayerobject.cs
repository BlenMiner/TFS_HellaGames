using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialPlayerObject
{
    public BoltEntity character;
    public BoltConnection connection;

    public bool isServer
    {
        get { return connection == null; }
    }
    public bool isClient
    {
        get { return connection != null; }
    }

    public void Spawn()
    {
        if (!character)
        {
            character = BoltNetwork.Instantiate(BoltPrefabs.TFSPlayer);

            if (isServer)
            {
                
                character.TakeControl();
            }
            else
            {
                character.AssignControl(connection);
            }
        }

        // teleport entity to a random spawn position
        character.transform.position = RandomPosition();
    }
    public void ResetInventory()
    {
        ITFSPlayerState state = character.GetState<ITFSPlayerState>();

        state.showInventory = false;
        state.draggingItem.ItemID = 0;

        /*using (var mod = state)
        {
            for (int x = 0; x < 32; x++)
            {
                mod.inventory[x].ItemNum = 0;
                mod.inventory[x].ItemID = 0;
            }

			for (int x = 0; x < 4; x++)
			{
                mod.armorSlots[x].ItemID = 0;
			}
			for (int x = 0; x < 8; x++)
			{
                mod.hotbar[x].ItemID = 0;
                mod.hotbar[x].ItemNum = 0;
			}
        }*/
    }



    Vector3 RandomPosition()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
        List<GameObject> clearspawnPoints = new List<GameObject>();

        foreach(GameObject go in spawnPoints)
        {
            if(!go.name.Contains("Occupied"))
                clearspawnPoints.Add(go);
        }
        int index = Random.Range(0, clearspawnPoints.Count - 1);
        clearspawnPoints[index].name = clearspawnPoints[index].name + " Occupied";
        return clearspawnPoints[index].transform.position + new Vector3(0, 2, 0);
    }
}
