using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TutorialPlayerObjectRegistry
{
    static List<TutorialPlayerObject> players = new List<TutorialPlayerObject>();

    static TutorialPlayerObject CreatePlayer(BoltConnection connection)
    {
        TutorialPlayerObject p;

        p = new TutorialPlayerObject();

        p.connection = connection;
        if (p.connection != null)
        {
            p.connection.UserData = p;
        }

        players.Add(p);

        return p;
    }

    public static List<TutorialPlayerObject> allPlayers
    {
        get { return players; }
    }
    public static TutorialPlayerObject serverPlayer
    {
        get { return players.First(x => x.isServer); }
    }

    public static TutorialPlayerObject CreateServerPlayer()
    {
        return CreatePlayer(null);
    }
    public static void SpawnChest()
    {
        GameObject holder = GameObject.FindGameObjectWithTag("EntityHolder");
        int i = 0;
        foreach(GameObject chests in GameObject.FindGameObjectsWithTag("Chest"))
        {
            BoltEntity character = BoltNetwork.Instantiate(BoltPrefabs.Chest1);
            character.TakeControl();

            character.gameObject.transform.parent = holder.transform;

            character.transform.position = chests.transform.position;
            character.transform.rotation = chests.transform.rotation;

            character.GetComponent<ChestContainer>().ItemTypes = chests.GetComponent<ChestContainer>().ItemTypes;
            character.GetComponent<ChestContainer>().buildingModels = chests.GetComponent<ChestContainer>().buildingModels;

            GameObject.Destroy(chests);
            i++;
        }
    }
    public static void SpawnUnistorm()
    {
        BoltEntity character = BoltNetwork.Instantiate(BoltPrefabs.UniStormPrefab_C);
        character.TakeControl();
    }

    public static void SpawnDonkey()
    {
        foreach(GameObject spawner in GameObject.FindGameObjectsWithTag("DonkeySpawn"))
        {
            BoltEntity character = BoltNetwork.Instantiate(BoltPrefabs.Donkey_2012_);
            character.TakeControl();

            character.transform.position = spawner.transform.position;
            character.GetComponent<NavMeshAgent>().enabled = true;
        }
    }

    public static TutorialPlayerObject CreateClientPlayer(BoltConnection connection)
    {
        return CreatePlayer(connection);
    }

    public static TutorialPlayerObject GetTutorialPlayer(BoltConnection connection)
    {
        if (connection == null)
        {
            return serverPlayer;
        }

        return (TutorialPlayerObject)connection.UserData;
    }
}
