using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bolt;

public class ServerInventoryCallbacks : Bolt.GlobalEventListener
{
    BuildingObjects BUILDINGMODELS;

    public List<Item> ItemList = new List<Item>();
    public List<ItemCraft> Crafting = new List<ItemCraft>();

    public static List<Transform> foundations = new List<Transform>();
    public static List<Transform> chests = new List<Transform>();
    public static List<Transform> walls = new List<Transform>();
    public static List<Transform> roofs = new List<Transform>();

    public LayerMask lm;

    public bool runCallbacks = false;

    public WeaponObjects weapons;

    void Awake()
    {
        lmR = lmRs;
        BUILDINGMODELS = ItemsDatabase.instance.building;
        DontDestroyOnLoad(gameObject);

        ItemList = ItemsDatabase.instance.ItemList;
        Crafting = ItemsDatabase.instance.Crafting;
    }
    void Update()
    {
        if (runCallbacks) return;

        if (BoltNetwork.isServer)
        {
            runCallbacks = true;
        }
        else
        {
            runCallbacks = false;
        }
    }

    //Settings
    public override void OnEvent(EntityAttachedServer e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;
        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();

        AddItem(state, ItemsDatabase.getItem("Crossbow"), 1);
        AddItem(state, ItemsDatabase.getItem("ClothTent"), 1);
        AddItem(state, ItemsDatabase.getItem("ClothHut"), 1);

        AddItem(state, ItemsDatabase.getItem("chest"), 1);

        AddItem(state, ItemsDatabase.getItem("campfire"), 5);
        AddItem(state, ItemsDatabase.getItem("DesertHelmet"), 1);
        AddItem(state, ItemsDatabase.getItem("DesertBody"), 1);
        AddItem(state, ItemsDatabase.getItem("DesertShoes"), 1);

        AddItem(state, ItemsDatabase.getItem("leggings"), 1);
        AddItem(state, ItemsDatabase.getItem("pants"), 1);

        AddItem(state, ItemsDatabase.getItem("Torch"), 1);
        AddItem(state, ItemsDatabase.getItem("Machete"), 1);
        AddItem(state, ItemsDatabase.getItem("nails"), 100);
        AddItem(state, ItemsDatabase.getItem("wood"), 100);
    }
    public override void OnEvent(InventoryOpen e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();

        state.insideChest = false;
        state.showInventory = !state.showInventory;

        if(state.showInventory == false)
        {
            OnInventoryClose(state, e.entity);
        }
    }

    void OnInventoryClose(ITFSPlayerState state, BoltEntity e)
    {
        if(state.draggingItem.ItemID > 0 && state.draggingItem.ItemNum > 0)
        {
            DropItem dI = DropItem.Create(Bolt.GlobalTargets.OnlyServer);
            dI.entity = e;
            dI.Send();
        }
    }

    public override void OnEvent(ExitMenu e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();

        state.exitMenu = !state.exitMenu;

    }
    public override void OnEvent(ToggleCraftingMenu e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();

        state.crafting = !state.crafting;

    }
    public override void OnEvent(CraftItem e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();

        int[,] Inventory, InventoryNum;

        Inventory = new int[8, 4];
        InventoryNum = new int[8, 4];

        int slotID = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                Inventory[x, y] = state.inventory[slotID].ItemID;
                InventoryNum[x, y] = state.inventory[slotID].ItemNum;
                slotID++;
            }
        }

        ItemCraft craft = getItemC(e.itemID);
        foreach (ItemInfo i in craft.ItemsNeeded)
        {
            if (GetItemNum(getItem(i.item.ItemID), Inventory, InventoryNum) >= i.itemNum)
            {
            }
            else
            {
                return;
            }
        }

        foreach (ItemInfo i in craft.ItemsNeeded)
        {
            RemoveItem(i.item.ItemID, i.itemNum, state);
        }
        AddItem(state, craft.Item.ItemID, craft.itemCount);
        
    }

    public ItemCraft getItemC(int id)
    {
        foreach(ItemCraft c in Crafting)
        {
            if(c.Item.ItemID == id)
            {
                return c;
            }
        }
        return null;
    }
    public void RemoveItem(int id, int num, ITFSPlayerState state)
    {
        int removedItems = 0;
        int i = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if(state.inventory[i].ItemID == id)
                {
                    if( state.inventory[i].ItemNum >= num)
                    {
                        state.inventory[i].ItemNum -= num;
                        if (state.inventory[i].ItemNum == 0)
                            state.inventory[i].ItemID = 0;

                        removedItems = num;
                    }
                    else
                    {
                        removedItems = state.inventory[i].ItemNum;
                        state.inventory[i].ItemNum = 0;
                        state.inventory[i].ItemID = 0;

                        RemoveItem(id, num - removedItems, state);
                    }
                }
                i++;
            }
        }
    }

    public override void OnEvent(DropItem e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();

        BoltEntity drop = BoltNetwork.Instantiate(BoltPrefabs.DropedItem, null, e.entity.transform.position + new Vector3(0,1,0), Quaternion.identity);
        drop.TakeControl();

        IDropedItem dropS = drop.GetState<IDropedItem>();
        dropS.itemID = state.draggingItem.ItemID;
        dropS.itemNum = state.draggingItem.ItemNum;

        state.draggingItem.ItemID = 0;
        state.draggingItem.ItemNum = 0;
    }

    public override void OnEvent(InventoryOpenContainer e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;
        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();

        if (state.showInventory == true)
        {
            state.showInventory = false;
            state.insideChest = false;
            state.insideAirDrop = false;

            if (state.showInventory == false)
            {
                OnInventoryClose(state, e.entity);
            }

            return;
        }
        
        Vector3 cameraPos = state.transform.Position + new Vector3(0, 1.662258f, 0);

        RaycastHit hit;
        if (Physics.Raycast(cameraPos, state.cameraForward, out hit, 7f, lm))
        {
            if (hit.transform.tag == "Chest")
            {
                Bolt.NetworkId id = hit.transform.gameObject.GetComponent<BoltEntity>().networkId;

                state.showInventory = true;
                state.insideChest = true;
                state.chestID = id;

                hit.transform.gameObject.GetComponent<ChestContainer>().state.open = chestOpen(id);
            }
            else if (hit.transform.tag == "AirDrop")
            {
                Bolt.NetworkId id = hit.transform.gameObject.GetComponent<BoltEntity>().networkId;

                state.showInventory = true;
                state.insideAirDrop = true;
                state.chestID = id;
            }
            else if (hit.transform.tag == "Campfire")
            {
                Bolt.NetworkId id = hit.transform.gameObject.GetComponent<BoltEntity>().networkId;

                state.showInventory = true;
                state.insideAirDrop = true;
                state.chestID = id;
            }
            else if (hit.transform.tag == "1")
            {
                if(hit.transform.childCount > 0)
                {
                    IWeaponRacks wepaonr = hit.transform.parent.gameObject.GetComponent<BoltEntity>().GetState<IWeaponRacks>();
                    int i = int.Parse(hit.transform.tag) - 2;
                    int id = wepaonr.ItemsInside[i].itemID;
                    if (AddItem(state, id, 1))
                        wepaonr.ItemsInside[i].itemID = 0;
                }
            }
            else if (hit.transform.tag == "2")
            {
                if (hit.transform.childCount > 0)
                {
                    IWeaponRacks wepaonr = hit.transform.parent.gameObject.GetComponent<BoltEntity>().GetState<IWeaponRacks>();
                    int i = int.Parse(hit.transform.tag) - 2;
                    int id = wepaonr.ItemsInside[i].itemID;
                    if (AddItem(state, id, 1))
                    {
                        wepaonr.ItemsInside[i].itemID = 0;
                    }
                }
            }
            else if (hit.transform.tag == "3")
            {
                if (hit.transform.childCount > 0)
                {
                    IWeaponRacks wepaonr = hit.transform.parent.gameObject.GetComponent<BoltEntity>().GetState<IWeaponRacks>();
                    int i = int.Parse(hit.transform.tag) - 2;
                    int id = wepaonr.ItemsInside[i].itemID;
                    if (AddItem(state, id, 1))
                        wepaonr.ItemsInside[i].itemID = 0;
                }
            }
            else if (hit.transform.tag == "4")
            {
                if (hit.transform.childCount > 0)
                {
                    IWeaponRacks wepaonr = hit.transform.parent.gameObject.GetComponent<BoltEntity>().GetState<IWeaponRacks>();
                    int i = int.Parse(hit.transform.tag) - 2;
                    int id = wepaonr.ItemsInside[i].itemID;
                    if (AddItem(state, id, 1))
                        wepaonr.ItemsInside[i].itemID = 0;
                }
            }
            else if (hit.transform.tag == "5")
            {
                if (hit.transform.childCount > 0)
                {
                    IWeaponRacks wepaonr = hit.transform.parent.gameObject.GetComponent<BoltEntity>().GetState<IWeaponRacks>();
                    int i = int.Parse(hit.transform.tag) - 2;
                    int id = wepaonr.ItemsInside[i].itemID;
                    if (AddItem(state, id, 1))
                        wepaonr.ItemsInside[i].itemID = 0;
                }
            }
            else if (hit.transform.tag == "6")
            {
                if (hit.transform.childCount > 0)
                {
                    IWeaponRacks wepaonr = hit.transform.parent.gameObject.GetComponent<BoltEntity>().GetState<IWeaponRacks>();
                    int i = int.Parse(hit.transform.tag) - 2;
                    int id = wepaonr.ItemsInside[i].itemID;
                    if (AddItem(state, id, 1))
                        wepaonr.ItemsInside[i].itemID = 0;
                }
            }
            else if (hit.transform.tag == "8")
            {
                if (hit.transform.childCount > 0)
                {
                    IWeaponRacks wepaonr = hit.transform.parent.gameObject.GetComponent<BoltEntity>().GetState<IWeaponRacks>();
                    int i = int.Parse(hit.transform.tag) - 2;
                    int id = wepaonr.ItemsInside[i].itemID;
                    if (AddItem(state, id, 1))
                        wepaonr.ItemsInside[i].itemID = 0;
                }
            }
            else if (hit.transform.tag == "9")
            {
                if (hit.transform.childCount > 0)
                {
                    IWeaponRacks wepaonr = hit.transform.parent.gameObject.GetComponent<BoltEntity>().GetState<IWeaponRacks>();
                    int i = int.Parse(hit.transform.tag) - 2;
                    int id = wepaonr.ItemsInside[i].itemID;
                    if (AddItem(state, id, 1))
                        wepaonr.ItemsInside[i].itemID = 0;
                }
            }
            else if (hit.transform.tag == "10")
            {
                if (hit.transform.childCount > 0)
                {
                    IWeaponRacks wepaonr = hit.transform.parent.gameObject.GetComponent<BoltEntity>().GetState<IWeaponRacks>();
                    int i = int.Parse(hit.transform.tag) - 2;
                    int id = wepaonr.ItemsInside[i].itemID;
                    if (AddItem(state, id, 1))
                        wepaonr.ItemsInside[i].itemID = 0;
                }
            }
            else if (hit.transform.tag == "Drop")
            {
                if (hit.transform.childCount > 0)
                {
                    IDropedItem dropit = hit.transform.gameObject.GetComponent<BoltEntity>().GetState<IDropedItem>();
                    
                    if(AddItem(state, dropit.itemID, dropit.itemNum))
                    {
                        BoltNetwork.Destroy(hit.transform.gameObject.GetComponent<BoltEntity>());
                    }
                }
            }
            else if (hit.transform.tag == "Animal" && 
                ItemsDatabase.getItem(state.selectedItemID) != null && 
                ItemsDatabase.getItem(state.selectedItemID).ItemName.Equals("Machete") && state.Skinning == false)
            {
                IAnimals animal = hit.transform.GetComponent<BoltEntity>().GetState<IAnimals>();
                if (animal.health <= 0)
                {
                    
                    state.Skinning = true;

                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = e.entity;
                        pS.soundID = 216;
                    }
                    pS.Send();

                    StartCoroutine(KillDonkey(state, hit.transform.GetComponent<BoltEntity>()));
                }
            }
        }

        if (state.showInventory == false)
        {
            OnInventoryClose(state, e.entity);
        }
    }
    IEnumerator KillDonkey(ITFSPlayerState state, BoltEntity donkeyu)
    {
        yield return new WaitForSeconds(2f);
        AddItem(state, ItemsDatabase.getItem("donkeyMeat"), 6);
        AddItem(state, ItemsDatabase.getItem("leather"), 3);

        BoltNetwork.Destroy(donkeyu);
    }
    public override void OnEvent(InventorySetSelectedItem e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;
        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();

        state.selectedItemID = 0;
        state.building = false;
        //state.fighting = false;

        e.entity.gameObject.GetComponent<PlayerWeaponController>().DiselectAll();

        state.selectedItemID = state.hotbar[e.slotID].ItemID;

        if( getItem(state.hotbar[e.slotID].ItemID).ItemType == Item.ItemTypeE.Building ||
            getItem(state.hotbar[e.slotID].ItemID).ItemType == Item.ItemTypeE.Trap)
        {
            state.building = true;
        }
        else
        {
            state.building = false;
        }

        if (getItem(state.hotbar[e.slotID].ItemID).ItemType == Item.ItemTypeE.Weapon)
        {
            e.entity.gameObject.GetComponent<PlayerWeaponController>().SetSelectedWeapon(getItem(state.hotbar[e.slotID].ItemID));
            //state.fighting = true;
        }
        else
        {
            //state.fighting = false;
        }

        if (state.showInventory == false)
        {
            OnInventoryClose(state, e.entity);
        }
    }
    
    //Building
    BuildingSystem bs;
    public override void OnEvent(BuildFoundation e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();
        Vector3 cameraPos = state.transform.Position + new Vector3(0, 1.662258f, 0);

        if (state.showInventory == true || state.exitMenu == true)
            return;

        if (state.building == false || getItem(state.selectedItemID).ItemType != Item.ItemTypeE.Building && getItem(state.selectedItemID).ItemType != Item.ItemTypeE.Trap)
        {
            return;
        }

        bs = e.entity.gameObject.GetComponent<PlayerInventoryShow>().bs;
        
        RaycastHit hit;
        if (Physics.Raycast(cameraPos, state.cameraForward, out hit, 7f, bs.Foundations))
        {
            if (hit.transform.tag == "FoundationPlace")
            {
                if (CanPlaceFoundation(hit.transform.GetComponent<FoundationPlace>().GetPosition(), true))
                {
                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = e.entity;
                        pS.soundID = 205;
                    }
                    pS.Send();

                    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Foundation, hit.transform.GetComponent<FoundationPlace>().GetPosition(), hit.transform.GetComponent<FoundationPlace>().GetFoundationP().rotation);
                    entity.TakeControl();

                    Destroy(hit.transform.gameObject);

                    StartCoroutine(setIdle(entity));

                    try
                    {
                        entity.GetComponent<NavMeshObstacle>().enabled = true;
                    }
                    catch
                    {

                    }
                    foundations.Add(entity.gameObject.transform);
                }
                else
                {
                }
            }
            else
            {
                if (CanPlaceFoundation(hit.point, false))
                {
                    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Foundation, hit.point, Quaternion.identity);
                    entity.TakeControl();
                    entity.GetComponent<NavMeshObstacle>().enabled = true;
                    foundations.Add(entity.gameObject.transform);
                    StartCoroutine(setIdle(entity));

                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = e.entity;
                        pS.soundID = 205;
                    }
                    pS.Send();
                }
            }
        }
    }
    public override void OnEvent(BuildWall e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();
        Vector3 cameraPos = state.transform.Position + new Vector3(0, 1.662258f, 0);

        if (state.showInventory == true || state.exitMenu == true)
            return;

        if (state.building == false || getItem(state.selectedItemID).ItemType != Item.ItemTypeE.Building && getItem(state.selectedItemID).ItemType != Item.ItemTypeE.Trap)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(cameraPos, state.cameraForward, out hit, 7f, bs.Walls))
        {
            if (hit.transform.tag == "Foundation" || hit.transform.tag == "RoofFoundation")
            {
                Collider collider = hit.transform.GetComponent<Foundation>().wallLimits;

                GameObject previewG = GameObject.Instantiate(getItem(state.selectedItemID).ItemObjectWorld) as GameObject;
                previewG.transform.position = hit.point;
                previewG.transform.rotation = e.rotation;
                previewG.transform.tag = "Untagged";

                if (/*collider.bounds.Intersects(previewG.collider.bounds) && */CanPlaceWall(hit.point))
                {
                    BoltEntity entity = BoltNetwork.Instantiate(getItem(state.selectedItemID).ItemObjectWorld.GetComponent<BoltEntity>().prefabId, hit.point, e.rotation);
                    entity.TakeControl();

                    StartCoroutine(setIdle(entity));
                    walls.Add(entity.transform);

                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = e.entity;
                        pS.soundID = 206;
                    }
                    pS.Send();
                   /* if (getItem(state.selectedItemID).ItemName == "Wall1")
                    {
                        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Wall1, hit.point, e.rotation);
                        entity.TakeControl();

                        StartCoroutine(setIdle(entity));
                        walls.Add(entity.transform);

                        playSound pS = new playSound(); 
                        {
                            evnt.entity = e.entity;
                            evnt.soundID = 206;
                        }
                    }
                    else if (getItem(state.selectedItemID).ItemName == "Window")
                    {
                        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Window, hit.point, e.rotation);
                        entity.TakeControl();

                        StartCoroutine(setIdle(entity));
                        walls.Add(entity.transform);

                        playSound pS = new playSound(); 
                        {
                            evnt.entity = e.entity;
                            evnt.soundID = 206;
                        }
                    }
                    else if (getItem(state.selectedItemID).ItemName == "Fence")
                    {
                        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Fence_Section, hit.point, e.rotation);
                        entity.TakeControl();

                        StartCoroutine(setIdle(entity));
                        walls.Add(entity.transform);

                        playSound pS = new playSound(); 
                        {
                            evnt.entity = e.entity;
                            evnt.soundID = 206;
                        }
                    }
                    else if (getItem(state.selectedItemID).ItemName == "campfire")
                    {
                        BoltEntity entity = BoltNetwork.Instantiate(getItem(state.selectedItemID).ItemObjectWorld.GetComponent<BoltEntity>().prefabId, hit.point, e.rotation);
                        entity.TakeControl();

                        StartCoroutine(setIdle(entity));
                        walls.Add(entity.transform);

                        playSound pS = new playSound(); 
                        {
                            evnt.entity = e.entity;
                            evnt.soundID = 206;
                        }
                    }
                    else if (getItem(state.selectedItemID).ItemName == "Stick")
                    {
                        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.stick, hit.point, e.rotation);
                        entity.TakeControl();

                        StartCoroutine(setIdle(entity));
                        walls.Add(entity.transform);

                        playSound pS = new playSound(); 
                        {
                            evnt.entity = e.entity;
                            evnt.soundID = 206;
                        }
                    }
                    else if (getItem(state.selectedItemID).ItemName == "Door")
                    {
                        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Door, hit.point, e.rotation);
                        entity.TakeControl();

                        StartCoroutine(setIdle(entity));
                        walls.Add(entity.transform);

                        playSound pS = new playSound(); 
                        {
                            evnt.entity = e.entity;
                            evnt.soundID = 206;
                        }
                    }
                    else if (getItem(state.selectedItemID).ItemName == "StairWay")
                    {
                        Vector3 offset =  new Vector3();
                        if (hit.transform.name == "Foundation")
                            offset = new Vector3(0, 1, 0);

                        BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Stairway, hit.transform.position + offset, e.rotation);
                        entity.TakeControl();

                        StartCoroutine(setIdle(entity));
                        walls.Add(entity.transform);

                        playSound pS = new playSound(); 
                        {
                            evnt.entity = e.entity;
                            evnt.soundID = 206;
                        }
                    }*/
                }

                Destroy(previewG);
            }
            else if (hit.transform.tag == "WallPlace" && CanPlaceWall(hit.transform.position))
            {
                if (getItem(state.selectedItemID).ItemName == "Wall1")
                {
                    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Wall1, hit.transform.position, hit.transform.rotation);
                    entity.TakeControl();

                    StartCoroutine(setIdle(entity));
                    walls.Add(entity.transform);

                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = e.entity;
                        pS.soundID = 206;
                    }
                    pS.Send();
                }
                else if (getItem(state.selectedItemID).ItemName == "Window")
                {
                    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Window, hit.transform.position, hit.transform.rotation);
                    entity.TakeControl();

                    StartCoroutine(setIdle(entity));
                    walls.Add(entity.transform);

                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = e.entity;
                        pS.soundID = 206;
                    }
                    pS.Send();
                }
                else if (getItem(state.selectedItemID).ItemName == "Fence")
                {
                    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Fence_Section, hit.transform.position, hit.transform.rotation);
                    entity.TakeControl();

                    StartCoroutine(setIdle(entity));
                    walls.Add(entity.transform);

                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = e.entity;
                        pS.soundID = 206;
                    }
                    pS.Send();
                }
                else if (getItem(state.selectedItemID).ItemName == "Door")
                {
                    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.Door, hit.transform.position, hit.transform.rotation);
                    entity.TakeControl();

                    StartCoroutine(setIdle(entity));
                    walls.Add(entity.transform);

                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = e.entity;
                        pS.soundID = 206;
                    }
                    pS.Send();
                }
            }
        }
    }
    public override void OnEvent(BuildChest e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        bs = e.entity.gameObject.GetComponent<PlayerInventoryShow>().bs;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();
        Vector3 cameraPos = state.transform.Position + new Vector3(0, 1.662258f, 0);

        if (state.showInventory == true || state.exitMenu == true)
            return;

        RaycastHit hit;
        if (Physics.Raycast(cameraPos, state.cameraForward, out hit, 7f, bs.Chests))
        {
            if (CanPlaceChest(hit.point))
            {
                BoltEntity entity = null;

                entity = BoltNetwork.Instantiate(getItem(state.selectedItemID).ItemObjectWorld.GetComponent<BoltEntity>().prefabId, hit.point, e.rot);

                if (entity.GetComponent<ChestContainer>() != null)
                {
                    entity.GetComponent<ChestContainer>().ItemTypes = new List<ItemSpawnInfo>();
                }

                StartCoroutine(setIdle(entity));
                entity.TakeControl();

                playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                {
                    pS.entity = e.entity;
                    pS.soundID = 205;
                }
                pS.Send();
            }
            else
            {
            }
        }
    }
    public override void OnEvent(BuildRoof e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();
        Vector3 cameraPos = state.transform.Position + new Vector3(0, 1.662258f, 0);

        if (state.showInventory == true || state.exitMenu == true)
            return;

        if (state.building == false || getItem(state.selectedItemID).ItemType != Item.ItemTypeE.Building && getItem(state.selectedItemID).ItemType != Item.ItemTypeE.Trap)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(cameraPos, state.cameraForward, out hit, 7f, bs.Roofs))
        {
            if (hit.transform.tag == "RoofPlace")
            {
                if (CanPlaceRoof(hit.transform.position))
                {
                    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.MetalRoof, hit.transform.position, Quaternion.identity);
                    entity.TakeControl();

                    Destroy(hit.transform.gameObject);

                    StartCoroutine(setIdle(entity));
                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = e.entity;
                        pS.soundID = 206;
                    }
                    pS.Send();
                }
            }
            else if (hit.transform.tag == "RoofPlaceN")
            {
                if (CanPlaceRoof(hit.transform.position))
                {
                    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.MetalRoof, hit.transform.position, Quaternion.identity);
                    entity.TakeControl();

                    Destroy(hit.transform.gameObject);

                    StartCoroutine(setIdle(entity));
                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = e.entity;
                        pS.soundID = 206;
                    }
                    pS.Send();
                }
            }
        }
    }
    public override void OnEvent(BuildDoor e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();
        Vector3 cameraPos = state.transform.Position + new Vector3(0, 1.662258f, 0);

        if (state.showInventory == true || state.exitMenu == true)
            return;

        if (state.building == false || getItem(state.selectedItemID).ItemType != Item.ItemTypeE.Building && getItem(state.selectedItemID).ItemType != Item.ItemTypeE.Trap)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(cameraPos, state.cameraForward, out hit, 7f, bs.Door))
        {
            if (hit.transform.tag == "DoorPlace")
            {
                if (CanPlaceDoor(hit.transform.position))
                {
                    BoltEntity entity = BoltNetwork.Instantiate(BoltPrefabs.WoodDoor, hit.transform.position, e.rotation);
                    entity.TakeControl();

                    StartCoroutine(setIdle(entity));
                    playSound pS = playSound.Create(Bolt.GlobalTargets.Everyone);
                    {
                        pS.entity = e.entity;
                        pS.soundID = 206;
                    }
                    pS.Send();
                }
            }
        }
    }
    public override void OnEvent(OpenDoor e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();
        Vector3 cameraPos = state.transform.Position + new Vector3(0, 1.662258f, 0);

        if (state.showInventory == true || state.exitMenu == true)
            return;
        
        bs = e.entity.gameObject.GetComponent<PlayerInventoryShow>().bs;

        RaycastHit hit;
        if (Physics.Raycast(cameraPos, state.cameraForward, out hit, 7f, bs.OpenD))
        {
            if (hit.transform.tag == "Door")
            {
                IDoor d = hit.transform.gameObject.GetComponent<BoltEntity>().GetState<IDoor>();
                d.open = !d.open;
            }
        }
    }

    IEnumerator setIdle(BoltEntity obj)
    {
        //yield return new WaitForSeconds(1f);
        /*foreach (TutorialPlayerObject pl in TutorialPlayerObjectRegistry.allPlayers)
        {
            if (pl.connection != null)
                obj.Idle(pl.connection, true);
        }*/

        yield return 0;
    }

    //Move item
    public override void OnEvent(InventoryMoveItem e)
    {
        if (!runCallbacks && e.pln.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.pln.GetState<ITFSPlayerState>();

        if (state.inventoryHeight <= 0)  return;

        Item itemToPickup = getItem(state.inventory[e.slotID].ItemID); int itemToPickupNum = state.inventory[e.slotID].ItemNum;

        if (isChestSlot(e.slotID))
        {
            ITFSChestState chest = null;
            IAirDrop airdop = null;

            if (state.insideChest)
                chest = getChest(state.chestID).state;
            else
            {
                airdop = getAirdop(state.chestID);
            }

            if (state.insideChest == false && state.insideAirDrop == false)
                return;

            int slotID = toChestSlotId(e.slotID);

            if (chest != null)
            {
                itemToPickup = getItem(chest.inventory[slotID].ItemID);
                itemToPickupNum = chest.inventory[slotID].ItemNum;

                if (itemToPickup.ItemID == state.draggingItem.ItemID)
                {
                    if (itemToPickupNum + state.draggingItem.ItemNum <= itemToPickup.ItemMaxStack)
                    {
                        chest.inventory[slotID].ItemID = state.draggingItem.ItemID;
                        chest.inventory[slotID].ItemNum = itemToPickupNum + state.draggingItem.ItemNum;
                        SetDraggingItem(state, 0, 0);

                        return;
                    }
                    else
                    {
                        int rest = 0;

                        rest = (itemToPickupNum + state.draggingItem.ItemNum) - itemToPickup.ItemMaxStack;
                        itemToPickupNum = itemToPickup.ItemMaxStack;

                        chest.inventory[slotID].ItemID = state.draggingItem.ItemID;
                        chest.inventory[slotID].ItemNum = itemToPickup.ItemMaxStack;

                        if (rest > 0)
                        {
                            state.draggingItem.ItemID = itemToPickup.ItemID;
                            state.draggingItem.ItemNum = rest;
                        }
                    }
                }
                else
                {
                    chest.inventory[slotID].ItemID = state.draggingItem.ItemID;
                    chest.inventory[slotID].ItemNum = state.draggingItem.ItemNum;
                    SetDraggingItem(state, itemToPickup.ItemID, itemToPickupNum);
                }
            }
            else
            {
                itemToPickup = getItem(airdop.inventory[slotID].ItemID);
                itemToPickupNum = airdop.inventory[slotID].ItemNum;

                if (itemToPickup.ItemID == state.draggingItem.ItemID)
                {
                    if (itemToPickupNum + state.draggingItem.ItemNum <= itemToPickup.ItemMaxStack)
                    {
                        airdop.inventory[slotID].ItemID = state.draggingItem.ItemID;
                        airdop.inventory[slotID].ItemNum = itemToPickupNum + state.draggingItem.ItemNum;
                        SetDraggingItem(state, 0, 0);

                        return;
                    }
                    else
                    {
                        int rest = 0;

                        rest = (itemToPickupNum + state.draggingItem.ItemNum) - itemToPickup.ItemMaxStack;
                        itemToPickupNum = itemToPickup.ItemMaxStack;

                        airdop.inventory[slotID].ItemID = state.draggingItem.ItemID;
                        airdop.inventory[slotID].ItemNum = itemToPickup.ItemMaxStack;

                        if (rest > 0)
                        {
                            state.draggingItem.ItemID = itemToPickup.ItemID;
                            state.draggingItem.ItemNum = rest;
                        }
                    }
                }
                else
                {
                    airdop.inventory[slotID].ItemID = state.draggingItem.ItemID;
                    airdop.inventory[slotID].ItemNum = state.draggingItem.ItemNum;
                    SetDraggingItem(state, itemToPickup.ItemID, itemToPickupNum);
                }
            }

            
            return;
        }

        if (itemToPickup.ItemID == state.draggingItem.ItemID)
        {
            if (itemToPickupNum + state.draggingItem.ItemNum <= itemToPickup.ItemMaxStack)
            {
                SetInventoryItem(state, e.slotID, state.draggingItem.ItemID, itemToPickupNum + state.draggingItem.ItemNum);
                SetDraggingItem(state, 0, 0);

                return;
            }
            else
            {
                int rest = 0;

                rest = (itemToPickupNum + state.draggingItem.ItemNum) - itemToPickup.ItemMaxStack;
                itemToPickupNum = itemToPickup.ItemMaxStack;

                SetInventoryItem(state, e.slotID, state.draggingItem.ItemID, itemToPickup.ItemMaxStack);

                if (rest > 0)
                {
                    state.draggingItem.ItemID = itemToPickup.ItemID;
                    state.draggingItem.ItemNum = rest;
                }
            }
        }
        else
        {
            SetInventoryItem(state, e.slotID, state.draggingItem.ItemID, state.draggingItem.ItemNum);
            SetDraggingItem(state, itemToPickup.ItemID, itemToPickupNum);
        }
    }
    public override void OnEvent(InventoryMoveItemArmor e)
    {
        if (!runCallbacks && e.enity.GetState<ITFSPlayerState>().dead == false) return;
        ITFSPlayerState state = e.enity.GetState<ITFSPlayerState>();

        Item itemToPickup = getItem(state.armorSlots[e.slotID].ItemID); int itemToPickupNum = state.armorSlots[e.slotID].ItemNum;

        if (state.inventoryHeight <= 0) return;
        if (state.draggingItem.ItemID < 0)
            return;

        if (itemToPickup == null)
        {
            itemToPickup = new Item();
            itemToPickup.ItemType = Item.ItemTypeE.None;
        }
        int slot = e.slotID;

        if (slot == 0)
        {
            if (getItem(state.draggingItem.ItemID).ItemType != Item.ItemTypeE.HeadArmor)
            {
                return;
            }
        }
        if (slot == 1)
        {
            if (getItem(state.draggingItem.ItemID).ItemType != Item.ItemTypeE.BodyArmor)
            {
                return;
            }
        }
        if (slot == 2)
        {
            if (getItem(state.draggingItem.ItemID).ItemType != Item.ItemTypeE.LegsArmor)
            {
                return;
            }
        }
        if (slot == 3)
        {
            if (getItem(state.draggingItem.ItemID).ItemType != Item.ItemTypeE.ShooesArmor)
            {
                return;
            }
        }

        if (itemToPickup.ItemID == getItem(state.draggingItem.ItemID).ItemID)
        {
            if (itemToPickupNum + state.draggingItem.ItemNum <= itemToPickup.ItemMaxStack)
            {
                SetArmorItem(state, e.slotID, state.draggingItem.ItemID, itemToPickupNum + state.draggingItem.ItemNum);
                SetDraggingItem(state, 0, 0);
                return;
            }
            else
            {
                int rest = 0;

                rest = (itemToPickupNum + state.draggingItem.ItemNum) - itemToPickup.ItemMaxStack;
                itemToPickupNum = itemToPickup.ItemMaxStack;

                SetArmorItem(state, e.slotID, state.draggingItem.ItemID, itemToPickup.ItemMaxStack);

                if (rest > 0)
                {
                    SetDraggingItem(state, itemToPickup.ItemID, rest);
                }
            }
        }
        else
        {
            SetArmorItem(state, e.slotID, state.draggingItem.ItemID, state.draggingItem.ItemNum);
            SetDraggingItem(state, itemToPickup.ItemID, itemToPickupNum);
        }
    }
    public override void OnEvent(InventoryMoveItemHotBar e)
    {
        if (!runCallbacks && e.enity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.enity.GetState<ITFSPlayerState>();
        int slotID = e.slotID;

        if (state.inventoryHeight <= 0) return;
        Item itemToPickup = getItem(state.hotbar[e.slotID].ItemID); int itemToPickupNum = state.hotbar[e.slotID].ItemNum;
        state.selectedItemID = 0;
        if (itemToPickup == null)
        {
            itemToPickup = new Item();
        }

        if (itemToPickup.ItemID == state.draggingItem.ItemID)
        {
            if (itemToPickupNum + state.draggingItem.ItemNum <= itemToPickup.ItemMaxStack)
            {
                SetHotbarItem(state, e.slotID, state.draggingItem.ItemID, itemToPickupNum + state.draggingItem.ItemNum);
                SetDraggingItem(state, 0, 0);

                return;
            }
            else
            {
                int rest = 0;

                rest = (itemToPickupNum + state.draggingItem.ItemNum) - itemToPickup.ItemMaxStack;
                itemToPickupNum = itemToPickup.ItemMaxStack;

                SetHotbarItem(state, e.slotID, state.draggingItem.ItemID, itemToPickup.ItemMaxStack);

                if (rest > 0)
                {
                    //using (var mod = state)
                    {
                        state.draggingItem.ItemID = itemToPickup.ItemID;
                        state.draggingItem.ItemNum = rest;
                    }
                }
            }
        }
        else
        {
            SetHotbarItem(state, e.slotID, state.draggingItem.ItemID, state.draggingItem.ItemNum);
            SetDraggingItem(state, itemToPickup.ItemID, itemToPickupNum);
        }
    }

    //Drag item
    public override void OnEvent(InventoryDragItem e)
    {

        if (!runCallbacks && e.pln.GetState<ITFSPlayerState>().dead == false) return;
        ITFSPlayerState state = e.pln.GetState<ITFSPlayerState>();

        Item slotItem = getItem(state.inventory[e.slotID].ItemID); int slotNum = state.inventory[e.slotID].ItemNum;
        Item dragI = getItem(state.draggingItem.ItemID); int dragN = state.draggingItem.ItemNum;

        if (isChestSlot(e.slotID))
        {
            ITFSChestState chest = null;
            IAirDrop airdop = null;

            if (state.insideChest)
                chest = getChest(state.chestID).state;
            else
            {
                airdop = getAirdop(state.chestID);
            }

            if (chest != null)
            {
                if (state.insideChest == false)
                    return;

                int slotID = toChestSlotId(e.slotID);

                slotItem = getItem(chest.inventory[slotID].ItemID);
                slotNum = chest.inventory[slotID].ItemNum;

                if (chest.inventory[slotID].ItemID < 0)
                    return;

                state.draggingItem.ItemID = slotItem.ItemID;
                state.draggingItem.ItemNum = slotNum;

                if (dragN == 0)
                {
                    chest.inventory[slotID].ItemID = 0;
                    chest.inventory[slotID].ItemNum = dragN;
                }
                chest.inventory[slotID].ItemNum = dragN;

                return;
            }
            else
            {
                if (state.insideAirDrop == false)
                    return;

                int slotID = toChestSlotId(e.slotID);

                slotItem = getItem(airdop.inventory[slotID].ItemID);
                slotNum = airdop.inventory[slotID].ItemNum;

                if (airdop.inventory[slotID].ItemID < 0)
                    return;

                state.draggingItem.ItemID = slotItem.ItemID;
                state.draggingItem.ItemNum = slotNum;

                if (dragN == 0)
                {
                    airdop.inventory[slotID].ItemID = 0;
                    airdop.inventory[slotID].ItemNum = dragN;
                }
                airdop.inventory[slotID].ItemNum = dragN;

                return;
            }
        }



        if (state.inventory[e.slotID].ItemID < 0)
            return;

        state.draggingItem.ItemID = slotItem.ItemID;
        state.draggingItem.ItemNum = slotNum;

        if (dragN == 0)
            SetInventoryItem(state, e.slotID, 0, dragN);
        SetInventoryItem(state, e.slotID, dragN);
    }
    public override void OnEvent(InventoryDragItemHotbar e)
    {
        if (!runCallbacks && e.enity.GetState<ITFSPlayerState>().dead == false) return;
        ITFSPlayerState state = e.enity.GetState<ITFSPlayerState>();

        state.selectedItemID = 0;

        if (state.draggingItem.ItemID >= 1) return;

        Item slotItem = getItem(state.hotbar[e.slotID].ItemID); int slotNum = state.hotbar[e.slotID].ItemNum;
        Item dragI = getItem(state.draggingItem.ItemID); int dragN = state.draggingItem.ItemNum;

        if (state.hotbar[e.slotID].ItemID < 0)
            return;

        using (var mod = state)
        {
            mod.draggingItem.ItemID = slotItem.ItemID;
            mod.draggingItem.ItemNum = slotNum;
        }

        using (var mod = state)
        {
            if (dragN == 0)
                SetHotbarItem(state, e.slotID, 0, dragN);
            SetHotbarItem(state, e.slotID, dragN);
        }

        /*if (pln == Network.player)
            Client_DragItem(slotid, slotItem.ItemID, slotNum);
        else
            networkView.RPC("Client_DragItem", pln, slotid, slotItem.ItemID, slotNum);*/
    }

    //Split item
    public override void OnEvent(InventorySplitItemHotbar e)
    {
        if (!runCallbacks && e.enity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.enity.GetState<ITFSPlayerState>();

        Item slotItem = getItem(state.hotbar[e.slotID].ItemID); int slotNum = state.hotbar[e.slotID].ItemNum;
        state.selectedItemID = 0;
        if (slotItem.ItemMaxStack == 1)
            return;

        if (slotNum > 1 && state.draggingItem.ItemID < 1)
        {
            int slotnumberhalf = slotNum / 2;
            int slotRest = slotNum - slotnumberhalf;

            state.draggingItem.ItemID = slotItem.ItemID;
            state.draggingItem.ItemNum = slotRest;

            SetHotbarItem(state, e.slotID, slotItem.ItemID, slotnumberhalf);
            return;
        }

        if (state.draggingItem.ItemID > 0 && slotItem.ItemID == state.draggingItem.ItemID && state.draggingItem.ItemNum > 0)
        {
            SetHotbarItem(state, e.slotID, state.hotbar[e.slotID].ItemNum + 1);
            if (state.draggingItem.ItemNum - 1 < 1)
                state.draggingItem.ItemID = 0;
            state.draggingItem.ItemNum = state.draggingItem.ItemNum - 1;

        }
        else if (state.draggingItem.ItemID > 0 && slotItem.ItemID < 1 && state.draggingItem.ItemNum > 0)
        {
            SetHotbarItem(state, e.slotID, state.draggingItem.ItemID, 1);
            if (state.draggingItem.ItemNum - 1 < 1)
                SetDraggingItem(state, 0, 0);
            else
                SetDraggingItem(state, state.draggingItem.ItemID, state.draggingItem.ItemNum - 1);
        }
    }
    public override void OnEvent(InventorySplitItem e)
    {
        if (!runCallbacks && e.pln.GetState<ITFSPlayerState>().dead == false) return;
        ITFSPlayerState state = e.pln.GetState<ITFSPlayerState>();

        Item slotItem = getItem(state.inventory[e.slotID].ItemID); int slotNum = state.inventory[e.slotID].ItemNum;

        if (isChestSlot(e.slotID))
        {
            ITFSChestState chest = null;
            IAirDrop airdop = null;

            if (state.insideChest)
                chest = getChest(state.chestID).state;
            else
            {
                airdop = getAirdop(state.chestID);
            }

            if (chest != null)
            {
                if (state.insideChest == false)
                    return;

                int slotID = toChestSlotId(e.slotID);

                slotItem = getItem(chest.inventory[slotID].ItemID);
                slotNum = chest.inventory[slotID].ItemNum;

                if (slotNum > 1 && state.draggingItem.ItemID < 1)
                {
                    int slotnumberhalf = slotNum / 2;
                    int slotRest = slotNum - slotnumberhalf;

                    state.draggingItem.ItemID = slotItem.ItemID;
                    state.draggingItem.ItemNum = slotRest;

                    chest.inventory[slotID].ItemID = slotItem.ItemID;
                    chest.inventory[slotID].ItemNum = slotnumberhalf;
                    return;
                }

                if (state.draggingItem.ItemID > 0 && slotItem.ItemID == state.draggingItem.ItemID && state.draggingItem.ItemNum > 0)
                {
                    chest.inventory[slotID].ItemNum = chest.inventory[slotID].ItemNum + 1;

                    if (state.draggingItem.ItemNum - 1 < 1)
                        state.draggingItem.ItemID = 0;
                    state.draggingItem.ItemNum = state.draggingItem.ItemNum - 1;
                }
                else if (state.draggingItem.ItemID > 0 && slotItem.ItemID < 1 && state.draggingItem.ItemNum > 0)
                {
                    chest.inventory[slotID].ItemID = state.draggingItem.ItemID;
                    chest.inventory[slotID].ItemNum = 1;

                    if (state.draggingItem.ItemNum - 1 < 1)
                        SetDraggingItem(state, 0, 0);
                    else
                        SetDraggingItem(state, state.draggingItem.ItemID, state.draggingItem.ItemNum - 1);
                }
                return;
            }
            else
            {
                if (state.insideAirDrop == false)
                    return;

                int slotID = toChestSlotId(e.slotID);

                slotItem = getItem(airdop.inventory[slotID].ItemID);
                slotNum = airdop.inventory[slotID].ItemNum;

                if (slotNum > 1 && state.draggingItem.ItemID < 1)
                {
                    int slotnumberhalf = slotNum / 2;
                    int slotRest = slotNum - slotnumberhalf;

                    state.draggingItem.ItemID = slotItem.ItemID;
                    state.draggingItem.ItemNum = slotRest;

                    airdop.inventory[slotID].ItemID = slotItem.ItemID;
                    airdop.inventory[slotID].ItemNum = slotnumberhalf;
                    return;
                }

                if (state.draggingItem.ItemID > 0 && slotItem.ItemID == state.draggingItem.ItemID && state.draggingItem.ItemNum > 0)
                {
                    airdop.inventory[slotID].ItemNum = airdop.inventory[slotID].ItemNum + 1;

                    if (state.draggingItem.ItemNum - 1 < 1)
                        state.draggingItem.ItemID = 0;
                    state.draggingItem.ItemNum = state.draggingItem.ItemNum - 1;
                }
                else if (state.draggingItem.ItemID > 0 && slotItem.ItemID < 1 && state.draggingItem.ItemNum > 0)
                {
                    airdop.inventory[slotID].ItemID = state.draggingItem.ItemID;
                    airdop.inventory[slotID].ItemNum = 1;

                    if (state.draggingItem.ItemNum - 1 < 1)
                        SetDraggingItem(state, 0, 0);
                    else
                        SetDraggingItem(state, state.draggingItem.ItemID, state.draggingItem.ItemNum - 1);
                }
                return;
            }
        }

        if (slotItem.ItemMaxStack == 1)
            return;

        if (slotNum > 1 && state.draggingItem.ItemID < 1)
        {
            int slotnumberhalf = slotNum / 2;
            int slotRest = slotNum - slotnumberhalf;

            state.draggingItem.ItemID = slotItem.ItemID;
            state.draggingItem.ItemNum = slotRest;

            SetInventoryItem(state, e.slotID, slotItem.ItemID, slotnumberhalf);
            return;
        }

        if (state.draggingItem.ItemID > 0 && slotItem.ItemID == state.draggingItem.ItemID && state.draggingItem.ItemNum > 0)
        {
            SetInventoryItem(state, e.slotID, state.inventory[e.slotID].ItemNum + 1);
            if (state.draggingItem.ItemNum - 1 < 1)
                state.draggingItem.ItemID = 0;
            state.draggingItem.ItemNum = state.draggingItem.ItemNum - 1;
        }
        else if (state.draggingItem.ItemID > 0 && slotItem.ItemID < 1 && state.draggingItem.ItemNum > 0)
        {
            SetInventoryItem(state, e.slotID, state.draggingItem.ItemID, 1);
            if (state.draggingItem.ItemNum - 1 < 1)
                SetDraggingItem(state, 0, 0);
            else
                SetDraggingItem(state, state.draggingItem.ItemID, state.draggingItem.ItemNum - 1);
        }
    }

    //Move/Drag Armor
    public override void OnEvent(InventoryMoveArmor e)
    {
        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;

        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();

        if (state.inventoryHeight <= 0) return;

        Item itemToPickup = getItem(state.armorSlots[e.slotID].ItemID); int itemToPickupNum = 1;

        Item dragging = getItem(state.draggingItem.ItemID);

        if (dragging == null ||
            e.slotID == 0 && dragging.ItemType == Item.ItemTypeE.HeadArmor ||
            e.slotID == 1 && dragging.ItemType == Item.ItemTypeE.BodyArmor ||
            e.slotID == 2 && dragging.ItemType == Item.ItemTypeE.LegsArmor ||
            e.slotID == 3 && dragging.ItemType == Item.ItemTypeE.ShooesArmor)
        {
        }
        else
        {
            return;
        }

        if (itemToPickup.ItemID == state.draggingItem.ItemID)
        {
            if (itemToPickupNum + state.draggingItem.ItemNum <= itemToPickup.ItemMaxStack)
            {
                SetArmorItem(state, e.slotID, state.draggingItem.ItemID, itemToPickupNum + state.draggingItem.ItemNum);
                SetDraggingItem(state, 0, 0);

                return;
            }
            else
            {
                int rest = 0;

                rest = (itemToPickupNum + state.draggingItem.ItemNum) - itemToPickup.ItemMaxStack;
                itemToPickupNum = itemToPickup.ItemMaxStack;

                SetArmorItem(state, e.slotID, state.draggingItem.ItemID, itemToPickup.ItemMaxStack);

                if (rest > 0)
                {
                    state.draggingItem.ItemID = itemToPickup.ItemID;
                    state.draggingItem.ItemNum = rest;
                }
            }
        }
        else
        {
            SetArmorItem(state, e.slotID, state.draggingItem.ItemID, state.draggingItem.ItemNum);
            SetDraggingItem(state, itemToPickup.ItemID, itemToPickupNum);
        }
    }
    public override void OnEvent(InventoryDragArmor e)
    {

        if (!runCallbacks && e.entity.GetState<ITFSPlayerState>().dead == false) return;
        ITFSPlayerState state = e.entity.GetState<ITFSPlayerState>();

        Item slotItem = getItem(state.armorSlots[e.slotID].ItemID); int slotNum = 1;
        Item dragI = getItem(state.draggingItem.ItemID); int dragN = state.draggingItem.ItemNum;

        if (state.draggingItem.ItemID <= 0)
        {
            state.draggingItem.ItemID = slotItem.ItemID;
            state.draggingItem.ItemNum = slotNum;

            SetArmorItem(state, e.slotID, 0, 0);
        }
    }

    //FillRacks
    public override void OnEvent(fillRack e)
    {
        int[] weapons = new int[10];

        List<Item> allWeapons = new List<Item>();

        foreach (Item i in ItemsDatabase.instance.ItemList)
        {
            if (i.ItemObjectWorld != null && i.ItemType == Item.ItemTypeE.Weapon)
            {
                allWeapons.Add(i);
            }
        }

        //how many weapons to spawn ( 1 - 10 , ten being the max!!)
        int weapondc = Random.Range(1, 10);
        IWeaponRacks state = e.entity.GetState<IWeaponRacks>();

        for(int i = 0; i < weapondc; i++)
        {
            state.ItemsInside[i].itemID = allWeapons[Random.Range(0, allWeapons.Count)].ItemID;
        }
    }

    void SetHotbarItem(ITFSPlayerState state, int slot, int id, int num)
    {
        if (id < 0)
            id = 0;
        state.hotbar[slot].ItemID = id;
        state.hotbar[slot].ItemNum = num;
    }
    void SetHotbarItem(ITFSPlayerState state, int slot, int num)
    {
        state.hotbar[slot].ItemNum = num;
    }
    void SetInventoryItem(ITFSPlayerState state, int slot, int id, int num)
    {
        if (id < 0)
            id = 0;
        state.inventory[slot].ItemID = id;
        state.inventory[slot].ItemNum = num;
    }

    void SetArmorItem(ITFSPlayerState state, int slot, int num)
    {
        state.armorSlots[slot].ItemNum = num;
    }

    void SetInventoryItem(ITFSPlayerState state, int slot, int num)
    {
        state.inventory[slot].ItemNum = num;
    }
    void SetArmorItem(ITFSPlayerState state, int slot, int id, int num)
    {
        if (id < 0)
            id = 0;
        state.armorSlots[slot].ItemID = id;
        state.armorSlots[slot].ItemNum = 1;
    }
    void SetDraggingItem(ITFSPlayerState state, int id, int num)
    {
        if (id < 0)
            id = 0;
        state.draggingItem.ItemID = id;
        state.draggingItem.ItemNum = num;
    }

    public static bool CanPlaceWall(Vector3 pos)
    {
        if (IsInBlockedZone(pos))
            return false;
        foreach (Collider c in Physics.OverlapSphere(pos, 2.1f))
        {
            if (c.tag == "Wall" && Vector3.Distance(c.transform.position, pos) < 2.1f)
            {
                return false;
            }
        }
        return true;
    }
    public static bool CheckIfDestroy(Vector3 pos)
    {
        if (IsInBlockedZone(pos))
            return false;

        foreach (Collider c in Physics.OverlapSphere(pos, 3f))
        {
            if (c.tag == "Foundation" && Vector3.Distance(c.transform.position, pos) < 3)
            {
                return false;
            }
        }
        return true;
    }
    public static bool CanPlaceRoof(Vector3 pos)
    {
        if (IsInBlockedZone(pos))
            return false;
        GameObject[] roofs = GameObject.FindGameObjectsWithTag("Roof");

        ServerInventoryCallbacks.roofs.Clear();
        foreach (GameObject go in roofs)
        {
            ServerInventoryCallbacks.roofs.Add(go.transform);
        }

        foreach (Collider c in Physics.OverlapSphere(pos, 2f))
        {
            if (c.tag == "Roof" && !WallExists(pos) && Vector3.Distance(c.transform.position, pos) < 2f)
            {
                return false;
            }
        }

        return true;
    }
    public static bool WallExists(Vector3 roofPos)
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        foreach (Collider c in Physics.OverlapSphere(roofPos, 6f))
        {
            if (c.tag == "Wall" && Vector3.Distance(c.transform.position, roofPos) < 6f)
            {
                return false;
            }
        }

        return false;
    }
    public static bool CanPlaceChest(Vector3 pos)
    {
        if (IsInBlockedZone(pos))
            return false;

        foreach(Collider c in Physics.OverlapSphere(pos, 1.5f))
        {
            if (c.tag == "Chest" || c.tag == "Trap" || c.tag == "Campfire" || c.tag == "Tent")
            {
                if (Vector3.Distance(c.transform.position, pos) < 3f)
                    return false;
            }
        }

        return true;
    }
    public static bool CanPlaceDoor(Vector3 pos)
    {
        if (IsInBlockedZone(pos))
            return false;

        foreach (Collider c in Physics.OverlapSphere(pos, 2.1f))
        {
            if (c.tag == "Door" && Vector3.Distance(c.transform.position, pos) < 2.1f)
            {
                return false;
            }
        }

        return true;
    }

    public LayerMask lmRs;
    public static LayerMask lmR;

    bool IsColliding(Collider[] c, GameObject previewG)
    {
        foreach (Collider co in c)
        {
            if (co.bounds.Intersects(previewG.collider.bounds))
                return true;
        }
        return false;
    }
    public static bool CanPlaceFoundation(Vector3 pos, bool doCheck)
    {
        if (IsInBlockedZone(pos))
            return false;

        if (doCheck)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos + new Vector3(0, 2, 0), Vector3.down, out hit, 3f, lmR))
            {

            }
            else
            {
                return false;
            }
        }

        if (foundations.Count > 0)
        {
            for (int i = 0; i < foundations.Count; i++)
            {
                if (foundations[i].position != null)
                {
                    try
                    {
                        if (!FoundationIsFromHere(pos, foundations[i].position))
                        {
                            if (Vector3.Distance(pos, foundations[i].position) < 7f)
                            {
                                return false;
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
        return true;
    }
    public static bool FoundationIsFromHere(Vector3 pos, Vector3 foundationp)
    {
        Vector3 posR = pos - foundationp;
        if (posR.x % 4 == 0 && posR.z % 4 == 0)
        {
            return true;
        }
        return false;
    }
    public static bool IsInBlockedZone(Vector3 pos)
    {
        foreach (GameObject blocked in GameObject.FindGameObjectsWithTag("BlockedArea"))
        {
            BoxCollider c = blocked.GetComponent<BoxCollider>();
            if (c.collider.bounds.Contains(new Vector3(pos.x, c.transform.position.y + 10f, pos.z)))
                return true;
        }
        return false;
    }

    public Item getItem(int id)
    {
        foreach (Item i in ItemList)
        {
            if (i.ItemID == id)
            {
                return i;
            }
        }

        return new Item();
    }
    public Item getItem(string name)
    {
        foreach (Item i in ItemList)
        {
            if (i.ItemName == name)
            {
                return i;
            }
        }

        return new Item();
    }
    public bool isChestSlot(int slot)
    {
        int slotID = 0;
        for (int z = 0; z < 4; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (slotID == slot)
                {
                    if (x >= 4)
                        return true;
                }
                slotID++;
            }
        }
        return false;
    }
    ChestContainer getChest(Bolt.NetworkId id)
    {
        foreach(GameObject c in GameObject.FindGameObjectsWithTag("Chest"))
        {
            if (c.GetComponent<BoltEntity>().networkId == id)
                return c.GetComponent<ChestContainer>();
        }
        return null;
    }
    public IAirDrop getAirdop(Bolt.NetworkId id)
    {
        GameObject[] airdrops = GameObject.FindGameObjectsWithTag("AirDrop");
        for (int i = 0; i < airdrops.Length; i++)
        {
            if (airdrops[i].GetComponent<BoltEntity>() == null)
            {
                Destroy(airdrops[i]);
            }
            else
            {
                Bolt.NetworkId airdrop = airdrops[i].GetComponent<BoltEntity>().networkId;
                if (airdrop == id)
                {
                    return airdrops[i].GetComponent<BoltEntity>().GetState<IAirDrop>();
                }
            }
        }
        GameObject[] campfires = GameObject.FindGameObjectsWithTag("Campfire");
        for (int i = 0; i < campfires.Length; i++)
        {
            if (campfires[i].GetComponent<BoltEntity>() == null)
            {
                Destroy(campfires[i]);
            }
            else
            {
                Bolt.NetworkId airdrop = campfires[i].GetComponent<BoltEntity>().networkId;
                if (airdrop == id)
                {
                    return campfires[i].GetComponent<BoltEntity>().GetState<IAirDrop>();
                }
            }
        }
        return null;
    }

    int toChestSlotId(int id)
    {
        int slotID = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if(slotID == id)
                {
                    if( x >= 4)
                    {
                        if (y == 0)
                            return id - 4;
                        else if (y == 1)
                        {
                            return id - 8;
                        }
                        else if (y == 2)
                        {
                            return id - 12;
                        }
                        else
                        {
                            return id - 16;
                        }
                    }
                }
                slotID++;
            }
        }
        return 0;
    }

    bool chestOpen(Bolt.NetworkId id)
    {
        List<TutorialPlayerObject> players = TutorialPlayerObjectRegistry.allPlayers;
        foreach(TutorialPlayerObject pl in players)
        {
            if (pl.character.GetComponent<BoltEntity>().networkId == id && pl.character.GetState<ITFSPlayerState>().insideChest == true)
                return true;
        }
        return false;
    }
    public int GetItemNum(Item i, int[,] Inventory, int[,] InventoryNum)
    {
        int totalnum = 0;
        for (int z = 0; z < 4; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                Item item = getItem(Inventory[x, z]);
                if (item != null && item.ItemID == i.ItemID)
                {
                    totalnum += InventoryNum[x, z];
                }
            }
        }

        return totalnum;
    }
    public bool AddItem(ITFSPlayerState state, Item e, int count)
    {
        int slotID = 0;
        for (int z = 0; z < 4; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (isChestSlot(slotID))
                {
                    slotID++;
                    continue;
                }

                if (state.inventory[slotID].ItemID == 0 || state.inventory[slotID].ItemID == e.ItemID)
                {
                    if (state.inventory[slotID].ItemNum < e.ItemMaxStack)
                    {
                        int rest = 0;

                        state.inventory[slotID].ItemID = e.ItemID;
                        //Inventory[x, z] = e.ItemID;

                        if (state.inventory[slotID].ItemNum + count > e.ItemMaxStack)
                        {
                            rest = (state.inventory[slotID].ItemNum + count) - e.ItemMaxStack;
                            state.inventory[slotID].ItemNum = e.ItemMaxStack;
                        }
                        else
                        {
                            state.inventory[slotID].ItemNum += count;
                        }

                        if (rest > 0)
                        {
                            AddItem(state, e, rest);
                        }

                        return true;
                    }
                }
                slotID++;
            }
        }

        return false;
    }
    public bool AddItem(ITFSPlayerState state, int id, int count)
    {
        Item e = getItem(id);
        int slotID = 0;
        for (int z = 0; z < 4; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (isChestSlot(slotID))
                {
                    slotID++;
                    continue;
                }

                if (state.inventory[slotID].ItemID == 0 || state.inventory[slotID].ItemID == e.ItemID)
                {
                    if (state.inventory[slotID].ItemNum < e.ItemMaxStack)
                    {
                        int rest = 0;

                        state.inventory[slotID].ItemID = e.ItemID;
                        //Inventory[x, z] = e.ItemID;

                        if (state.inventory[slotID].ItemNum + count > e.ItemMaxStack)
                        {
                            rest = (state.inventory[slotID].ItemNum + count) - e.ItemMaxStack;
                            state.inventory[slotID].ItemNum = e.ItemMaxStack;
                        }
                        else
                        {
                            state.inventory[slotID].ItemNum += count;
                        }

                        if (rest > 0)
                        {
                            AddItem(state, e, rest);
                        }

                        return true;
                    }
                }
                slotID++;
            }
        }

        return false;
    }
}

public class ray
{
    public Vector3 pos,dir;
    public ray ( Vector3 POS, Vector3 DIR)
    {
        pos = POS;
        dir = DIR;
    }
}