using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ini;
using Bolt;

[BoltGlobalBehaviour(BoltNetworkModes.Host, "Main Demo")]
public class TutorialServerCallbacks : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string map)
    {
        IniFile ini = new IniFile(Application.dataPath + "/settings.ini");

        if (ini.IniReadValue("Server", "initiateServer") == "false")
        {
            TutorialPlayerObjectRegistry.CreateServerPlayer();
            TutorialPlayerObjectRegistry.serverPlayer.Spawn();
        }
        SpawnChests();
        //TutorialPlayerObjectRegistry.SpawnUnistorm();
        TutorialPlayerObjectRegistry.SpawnDonkey();
    }
    void SpawnChests()
    {
        GameObject holder = GameObject.FindGameObjectWithTag("EntityHolder");
        for (int i = 0; i < 100; i++)
        {
            BoltEntity character = BoltNetwork.Instantiate(BoltPrefabs.Chest1, RandomDirectionPosition(), Quaternion.identity);
            character.TakeControl();

            if (holder != null)
                character.gameObject.transform.parent = holder.transform;

            List<ItemSpawnInfo> items = new List<ItemSpawnInfo>();
            {
                items.Add(new ItemSpawnInfo(Item.ItemTypeE.Material, 0, 10, 2));
                items.Add(new ItemSpawnInfo(Item.ItemTypeE.Building, 0, 2, 20));
            }

            character.GetComponent<ChestContainer>().ItemTypes = items;
            character.GetComponent<ChestContainer>().buildingModels = ItemsDatabase.instance.building;

            i++;
        }
    }

    public override void Connected(BoltConnection connection)
    {
       /* if(ServerManager.matchStarted)
        {
            BoltNetwork.Refuse(connection.RemoteEndPoint);
        }*/
    }

    float time;
    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        if (!ServerManager.matchStarted || Time.time - time <= 21)
        {
            TutorialPlayerObjectRegistry.CreateClientPlayer(connection);
            TutorialPlayerObjectRegistry.GetTutorialPlayer(connection).Spawn();
        }
        else
        {
            BoltEntity be = BoltNetwork.Instantiate(BoltPrefabs.TFSSpectator);
            be.AssignControl(connection);
        }
    }

    List<BoltEntity> planes = new List<BoltEntity>();
    List<BoltEntity> airDrops = new List<BoltEntity>();

    void Awake()
    {
        time = Time.time;
        nextAirdropTimeLeft = Mathf.FloorToInt(Time.time) + Random.Range((60 * 1), (60 * 2));
    }
    void Update()
    {
        AirDropSpawner();
        spw();
    }

    void spw()
    {
        if (planes.Count > 0)
        {
            for (int i = 0; i < planes.Count; i++)
            {
                if (planes[i] != null)
                {
                    IPlane state = planes[i].GetState<IPlane>();
                    planes[i].transform.Translate(Vector3.forward * Time.deltaTime * 80);

                    float d = Vector3.Distance(new Vector3(planes[i].transform.position.x, 0, planes[i].transform.position.z), new Vector3(state.targetpos.x, 0, state.targetpos.z));
                    if (d < 1 && state.dropedAirDrops == false)
                    {
                        state.dropedAirDrops = true;
                        BoltEntity airDrop = BoltNetwork.Instantiate(BoltPrefabs._01_SupplyBoxTier4, null, planes[i].transform.position, Quaternion.identity);
                        airDrop.TakeControl();
                        airDrops.Add(airDrop);
                    }
                    else if (d > 5000)
                    {
                        BoltEntity p = planes[i];
                        planes.Remove(planes[i]);

                        BoltNetwork.Destroy(p);

                        i--;
                        continue;
                    }
                }

            }
        }
    }

    int nextAirdropTimeLeft;
    void AirDropSpawner()
    {
        if(ServerManager.matchStarted == true)
        {
            if(Time.time > nextAirdropTimeLeft)
            {
                CallAirDropRandom();

                nextAirdropTimeLeft = Mathf.FloorToInt(Time.time) + Random.Range((60 * 1), (60 * 2));
            }
        }
    }
    void CallAirDropRandomPlayerPosition()
    {
        int rI = Random.Range(0, TutorialPlayerObjectRegistry.allPlayers.Count);
        CallAirDrop(TutorialPlayerObjectRegistry.allPlayers[rI].character.transform.position);
    }
    void CallAirDropRandom()
    {
        CallAirDrop(RandomDirection());
    }

    void OnGUI()
    {
        if (BoltNetwork.isServer)
        {
            if (GUILayout.Button("Call airdrop"))
            {
                CallAirDropRandomPlayerPosition();
            }
            if (GUILayout.Button("Call airdrop Random"))
            {
                CallAirDropRandom();
            }
        }
    }

    void CallAirDrop(Vector3 pos)
    {
        try
        {
            BoltEntity plane = BoltNetwork.Instantiate(BoltPrefabs.Plane);
            plane.TakeControl();

            planes.Add(plane);

            IPlane state = plane.GetState<IPlane>();

            Vector3 spawnPoint = RandomDirection();
            Vector3 randomDirection = spawnPoint.normalized;

            state.transform.SetTransforms(plane.transform);
            plane.transform.position = new Vector3(spawnPoint.x * 2, spawnPoint.y, spawnPoint.z * 2);

            state.targetpos = pos;

            plane.transform.LookAt(new Vector3(pos.x, plane.transform.position.y, pos.z));
        }
        catch { }
    }
    Vector3 RandomDirection()
    {
        Vector3 pos = new Vector3(Random.Range(-500, 500), 260, Random.Range(-500, 500));

        Vector3 fwd = Vector3.down;
        RaycastHit hit;
        if (Physics.Raycast(new Ray(pos, fwd), out hit, 10000f))
        {
            if (!hit.transform.name.Contains("Far"))
            {
                //if (!ServerInventoryCallbacks.IsInBlockedZone(hit.point))
                    return pos;
            }
        }

        return RandomDirection();
    }
    Vector3 RandomDirectionPosition()
    {
        Vector3 pos = new Vector3(Random.Range(-500, 500), 260, Random.Range(-500, 500));

        Vector3 fwd = Vector3.down;
        RaycastHit hit;
        if (Physics.Raycast(new Ray(pos, fwd), out hit, 50000f))
        {
            //if (!hit.transform.name.Contains("Far"))
            {
                //if (!ServerInventoryCallbacks.IsInBlockedZone(hit.point))
                    return hit.point;
            }
        }

        return new Vector3();
        //return RandomDirectionPosition();
    }

}
