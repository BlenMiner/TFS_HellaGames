using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInventoryShow : Bolt.GlobalEventListener
{
    #region variables
    public StateImages stateImages;

    public Texture2D InventoryBackground;
    public Texture2D InventoryStorage;
    public Texture2D InventorySlot;
    public Texture2D InventorySlotOccupied;

	public Texture2D InventoryHotbarSlot;
	public Texture2D InventoryMouseHover;
	public Texture2D InventoryHotbarSelected;

    public BuildingObjects building;
    public PlayerWeaponController wepController;
    
    #region private
    Vector2 GUIoldPosition = new Vector2(50, 50);
    Vector2 oldOpacity = new Vector2(0, 0);
    Vector3 lastPositionChest;
    GUIState old_guiState = GUIState.None;
    GUIState guiState = GUIState.None;
    Vector2 mouseOldPosition;
    Vector2 mouseOldPosition2;
    Vector2 lastKnownPosition;
    Texture2D image;
    bool showInventory = false;
    bool draggingItem = false;
    ItemInfo selecteditem;
    #endregion

    int[,] Inventory = new int[8, 4];
    int[,] InventoryNum = new int[8, 4];

    int[] armorSlots = new int[4];
    int[] armorSlotsNum = new int[4];

    int[] InventoryHotbarContent = new int[8];
    int[] InventoryHotbarNum = new int[8];

    public List<Item> ItemList = new List<Item>();
    public List<ItemCraft> Crafting = new List<ItemCraft>();

    public LayerMask lm;
    GameObject camera;

	ItemsDatabase ItemsDatabase;
    #endregion
    
    bool insideChest = false;
    bool insideAirDrop = false;
    bool isbuilding = false;
    Item selectedslot;
    Bolt.NetworkId chestID;
    public WeaponObjects weapons;
    void Awake()
    {
        /*if(BoltNetwork.isServer)
        {
            AddItem(getItem("S2"), 1);
            AddItem(getItem("Crossbow"), 1);
            AddItem(getItem("Door"), 5);
            AddItem(getItem("Wall1"), 5);
            AddItem(getItem("Foundation"), 5);
            AddItem(getItem("Roof"), 5);
            AddItem(getItem("WoodDoor"), 5);
            AddItem(getItem("Window"), 5);
            AddItem(getItem("Fence"), 5);
            AddItem(getItem("StairWay"), 5);

            AddItem(getItem("syringe"), 250);
            AddItem(getItem("redplant"), 250);
            AddItem(getItem("blueplant"), 250);
        }*/
        Inventory = new int[8, 4];
        InventoryNum = new int[8, 4];

        InventoryHotbarContent = new int[8];
        InventoryHotbarNum = new int[8];

        armorSlots = new int[4];
        armorSlotsNum = new int[4];

        ItemList = ItemsDatabase.instance.ItemList;
        Crafting = ItemsDatabase.instance.Crafting;

        camera = GetComponent<CameraObj>().Camera;

        ResetInventory();
    }
    
    [System.NonSerialized]
    public BoltEntity me;
    ITFSPlayerState state;

    public override void ControlOfEntityGained(BoltEntity arg)
    {
        if (!arg.name.Contains("TFS"))
            return;

        me = arg;
        state = me.GetState<ITFSPlayerState>();

        state.AddCallback("inventory[]", UpdateInventory);
        state.AddCallback("draggingItem", UpdateDraggedItem);
		state.AddCallback("armorSlots[]", UpdateArmor);
		state.AddCallback("hotbar[]", UpdateHotbar);
        state.AddCallback("building", UpdateBuilding);
        state.AddCallback("showInventory", OpenInventory);
        state.AddCallback("selectedItemID", UpdateSelectedItem);

        EnityAttached(arg);
    }
    public override void ControlOfEntityLost(BoltEntity arg)
    {
        Debug.Log("Removing callbacks.");

        state.RemoveCallback("inventory[]", UpdateInventory);
        state.RemoveCallback("draggingItem", UpdateDraggedItem);
        state.RemoveCallback("armorSlots[]", UpdateArmor);
        state.RemoveCallback("hotbar[]", UpdateHotbar);
        state.RemoveCallback("building", UpdateBuilding);
        state.RemoveCallback("showInventory", OpenInventory);

        me = null;
        state = null;
    }
    public void ResetInventory()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                Inventory[x, y] = 0;
                InventoryNum[x, y] = 0;
            }
        }

        for (int x = 0; x < 4; x++)
        {
            armorSlots[x] = 0;
            armorSlotsNum[x] = 0;
        }
        for (int x = 0; x < 8; x++)
        {
            InventoryHotbarContent[x] = 0;
            InventoryHotbarNum[x] = 0;
        }
    }

    bool once = false;
    bool updatedChest = false;
    bool updatedAirdrop = false;
    ITFSChestState container;
    IAirDrop airDrop;

    public BlurEffect blur;
    void Update()
    {
        if (me == null || state == null) 
        {
            if(BoltNetwork.isClient)
            {
                this.enabled = false;
            }
			return;
		}

        if(!showInventory)
        {
            blur.enabled = false;
        }

        if (insideChest)
        {
            if (updatedChest == false)
            {
                container = getChest(chestID);
                if (container != null)
                    SetChestItems(container);
                updatedChest = true;
            }
            else
            {
                if (container != null)
                    SetChestItems(container);
            }
        }
        else
        {
            updatedChest = false;
        }

        if (insideAirDrop)
        {
            if (updatedAirdrop == false)
            {
                airDrop = getAirdop(chestID);
                if (container != null)
                    SetAirDropItems(airDrop);
                updatedAirdrop = true;
            }
            else
            {
                if (airDrop != null)
                    SetAirDropItems(airDrop);
            }
        }
        else
        {
            updatedAirdrop = false;
        }

        if(insideAirDrop == false && insideChest == false)
        {
            ClearChestItems();
        }

        if (isbuilding == true)
        {
            Building();
        }
        else
        {
            if (previewG != null)
            {
                Destroy(previewG);
                previewG = null;
            }
        }

        SelectItem();

		if (selecteditem == null)
						draggingItem = false;
				else
						draggingItem = true;

        if (Input.GetKeyDown(KeyCode.I))
            OpenInventory(me);

        if (Input.GetKeyDown(KeyCode.Escape))
            OpenExitMenu(me);

        if (Input.GetKeyDown(KeyCode.E))
            OpenContainer(me);

        if (guiState != old_guiState)
        {
            old_guiState = guiState;
            OnGUIChanged();
        }
        if (isbuilding == false)
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 7f, lm))
            {
                if (hit.transform.tag == "Chest")
                {
                    if (hit.transform.position != lastPositionChest)
                    {
                        guiState = GUIState.None;
                        lastPositionChest = hit.transform.position;

                    }
                    else
                        guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "Arrow")
                {
                    guiState = GUIState.Chest;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        showInventory = false;
                        if (Network.isServer)
                        {
                            //Server_PickupArrow(hit.transform.gameObject.GetComponent<ArrowBehavior>().id, Network.player);
                        }
                        else
                        {
                            //networkView.RPC("Server_PickupArrow", RPCMode.Server, hit.transform.gameObject.GetComponent<ArrowBehavior>().id, Network.player);
                        }
                    }
                }
                else if (hit.transform.tag == "1" && hit.transform.childCount > 0)
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "2" && hit.transform.childCount > 0)
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "3" && hit.transform.childCount > 0)
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "4" && hit.transform.childCount > 0)
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "5" && hit.transform.childCount > 0)
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "6" && hit.transform.childCount > 0)
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "7" && hit.transform.childCount > 0)
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "8" && hit.transform.childCount > 0)
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "9" && hit.transform.childCount > 0)
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "10" && hit.transform.childCount > 0)
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "AirDrop")
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "Campfire")
                {
                    guiState = GUIState.Chest;
                }
                else if (hit.transform.tag == "Drop")
                {
                    guiState = GUIState.Chest;
                    IDropedItem dic = hit.transform.gameObject.GetComponent<BoltEntity>().GetState<IDropedItem>();
                    if (AddItem(getItem(dic.itemID), dic.itemNum))
                    {
                        BoltNetwork.Destroy(hit.transform.gameObject);
                    }
                }
                else if (hit.transform.tag == "Animal" && ItemsDatabase.getItem(state.selectedItemID) != null && ItemsDatabase.getItem(state.selectedItemID).ItemName.Equals("Machete"))
                {
                    IAnimals animal = hit.transform.GetComponent<BoltEntity>().GetState<IAnimals>();
                    if(animal.health <= 0)
                    {
                        guiState = GUIState.Skin;
                    }
                }
                else
                {
                    guiState = GUIState.None;
                }
            }
            else
            {
                guiState = GUIState.None;
            }
        }
    }
    void SelectItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetSelectedItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetSelectedItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetSelectedItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetSelectedItem(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetSelectedItem(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SetSelectedItem(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SetSelectedItem(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SetSelectedItem(7);
        }
    }
    void OnGUIChanged()
    {
        if (me == null || state == null) return;
        /*GUIoldPosition = new Vector2((Screen.width / 2) - 50, (Screen.height / 2) - 50);
        oldOpacity = new Vector2(0, 0);
        oldTrancparency = new Vector2(0, 225);*/

        if (guiState == GUIState.Chest)
        {
            image = stateImages.Chest;
        }
        else if (guiState == GUIState.Door)
        {
            image = stateImages.Door;
        }
        else if (guiState == GUIState.Skin)
        {
            image = stateImages.Skin;
        }
        else
        {

        }
    }

    #region Building System!
    //Building

    public Color red;
    public Color green;
    private GameObject previewG;
    public BoxCollider boxCollider;
    public MeshCollider meshCollider;
    public BuildingSystem bs;
    int currentBuildingItemid = 0;
    void Building()
    {
        if (selectedslot == null || selectedslot.ItemObjectWorld == null || selectedslot.ItemID != currentBuildingItemid || selectedslot.ItemID == null)
        {
            Destroy(previewG);
            previewG = null;
        }

        if (previewG == null /*|| selectedslot.ItemObjectWorld != null*/)
        {
            currentBuildingItemid = state.selectedItemID;
            if (selectedslot.ItemObjectWorld != null)
            {
                previewG = Instantiate(selectedslot.ItemObjectWorld, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

                foreach (Collider e in previewG.GetComponentsInParent<Collider>())
                {
                    e.enabled = false;
                }
                foreach (Collider e in previewG.GetComponentsInChildren<Collider>())
                {
                    e.enabled = false;
                }
                foreach (MeshCollider e in previewG.GetComponentsInParent<MeshCollider>())
                {
                    e.enabled = false;
                }
                foreach (BoxCollider e in previewG.GetComponentsInParent<BoxCollider>())
                {
                    e.enabled = false;
                }
                foreach (MeshCollider e in previewG.GetComponents<MeshCollider>())
                {
                    e.enabled = false;
                }
                foreach (BoxCollider e in previewG.GetComponents<BoxCollider>())
                {
                    e.enabled = false;
                }
                foreach (Collider e in previewG.GetComponents<Collider>())
                {
                    e.enabled = false;
                }

                try
                {
                    previewG.GetComponent<Foundation>().addTolist = false;
                }
                catch { }
                try
                {
                    previewG.GetComponent<Wall>().addTolist = false;
                }
                catch { }

                SetMaterialColor(previewG, red);
                previewG.tag = "Untagged";
                if (previewG.GetComponent<NavMeshObstacle>() != null)
                    previewG.GetComponent<NavMeshObstacle>().enabled = false;
            }
        }
        else
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                previewG.transform.Rotate(0, -30, 0, Space.World);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                previewG.transform.Rotate(0, 30, 0, Space.World);
            }

            if (selectedslot.ItemName == "container")
                MoveChest(selectedslot);
            else if (selectedslot.ItemName == "Foundation")
            {
                MoveFoundation(selectedslot);
            }
            else if (selectedslot.ItemName == "Wall1" || selectedslot.ItemName == "Door" || selectedslot.ItemName == "Window" || selectedslot.ItemName == "Fence"
                || selectedslot.ItemName == "StairWay" || selectedslot.ItemName == "Stick")
            {
                MoveWall(selectedslot);
            }
            else if (selectedslot.ItemName == "Roof")
            {
                MoveRoof(selectedslot);
            }
            else if (selectedslot.ItemName == "WoodDoor")
            {
                MoveDoor(selectedslot);
            }
            else if (selectedslot.ItemName == "Trap1")
            {
                MoveChest(selectedslot);
            }
            else
            {
                MoveChest(selectedslot);
            }
        }
    }

    void SetMaterialColor(GameObject go, Color c)
    {
        foreach (MeshRenderer mr in go.GetComponents<MeshRenderer>())
        {
            mr.material = ItemsDatabase.instance.building.buildingMaterial;
            mr.material.color = c;
            mr.castShadows = false;
            mr.receiveShadows = false;
        }
        foreach (MeshRenderer mr in go.GetComponentsInChildren<MeshRenderer>())
        {
            mr.material = ItemsDatabase.instance.building.buildingMaterial;
            mr.material.color = c;
            mr.castShadows = false;
            mr.receiveShadows = false;
        }
        foreach (SkinnedMeshRenderer mr in go.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            mr.material = ItemsDatabase.instance.building.buildingMaterial;
            mr.material.color = c;
            mr.castShadows = false;
            mr.receiveShadows = false;
        }
    }

    public void MoveChest(Item itemid)
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 7f, bs.Chests))
        {
            previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.9f);

            if (ServerInventoryCallbacks.CanPlaceChest(hit.point))
            {
                SetMaterialColor(previewG, green);
                if(Input.GetMouseButtonDown(0))
                {
                    PlaceChest(previewG.transform.rotation);
                }
            }
            else
            {
                SetMaterialColor(previewG, red);
            }
        }
        else
        {
            previewG.transform.position = new Vector3(0, 0, 0);
        }
    }
    public void MoveFoundation(Item itemid)
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 7f, bs.Foundations))
        {
            if (hit.transform.tag == "FoundationPlace")
            {
                if (ServerInventoryCallbacks.CanPlaceFoundation(hit.transform.GetComponent<FoundationPlace>().GetPosition(), true))
                {
                    SetMaterialColor(previewG, green);
                    previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.transform.GetComponent<FoundationPlace>().GetPosition(), 0.9f);
                    previewG.transform.rotation = Quaternion.identity;
                    if (Input.GetMouseButtonDown(0))
                    {
                        PlaceFoundation(hit.transform.GetComponent<FoundationPlace>().GetPosition(), hit.transform.GetComponent<FoundationPlace>().GetFoundationP().rotation);
                    }
                }
                else
                {
                    SetMaterialColor(previewG, red);

                    previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.9f);
                    previewG.transform.rotation = Quaternion.identity;
                }
            }
            else if (!ServerInventoryCallbacks.CanPlaceFoundation(hit.point, false))
            {
                SetMaterialColor(previewG, red);

                previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.9f);
                previewG.transform.rotation = Quaternion.identity;
            }
            else if (ServerInventoryCallbacks.CanPlaceFoundation(hit.point, true))
            {
                previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.9f);
                previewG.transform.rotation = Quaternion.identity;

                SetMaterialColor(previewG, green);
                if (Input.GetMouseButtonDown(0))
                {
                    PlaceFoundation(hit.point, Quaternion.identity);
                }
            }
        }
        else
        {
            previewG.transform.position = new Vector3();
        }
    }
    public void MoveWall(Item itemid)
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 7f, bs.Walls))
        {
            if (hit.transform.tag == "Foundation" || hit.transform.tag == "RoofFoundation")
            {
                Collider collider = hit.transform.GetComponent<Foundation>().wallLimits;

                if (itemid.ItemName == "StairWay")
                    previewG.transform.position = hit.transform.position + new Vector3(0, 1, 0);
                else
                    previewG.transform.position = hit.point;

                if (/*collider.bounds.Intersects(previewG.collider.bounds) && */ServerInventoryCallbacks.CanPlaceWall(previewG.transform.position))
                {
                    if (previewG != null)
                        SetMaterialColor(previewG, green);
                    if (Input.GetMouseButtonDown(0))
                    {
                        PlaceWall(previewG.transform.rotation);
                    }
                }
                else
                    SetMaterialColor(previewG, red);
            }
            else if (hit.transform.tag == "WallPlace")
            {
                if (itemid.ItemName != "StairWay")
                {
                    previewG.transform.position = hit.transform.position;
                    previewG.transform.rotation = hit.transform.rotation;

                    SetMaterialColor(previewG, green);
                    if (Input.GetMouseButtonDown(0))
                    {
                        PlaceWall(previewG.transform.rotation);
                    }
                }
                else
                {
                    SetMaterialColor(previewG, red);
                    previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.5f);
                }
            }
            else
            {
                SetMaterialColor(previewG, red);
                previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.5f);
            }
        }
        else
        {
            previewG.transform.position = new Vector3();
            //Destroy(previewG);
        }
    }
    public void MoveRoof(Item itemid)
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 7f, bs.Roofs))
        {
            if (hit.transform.tag == "RoofPlace")
            {
                previewG.transform.position = hit.transform.position;

                if (ServerInventoryCallbacks.CanPlaceRoof(hit.transform.position))
                {
                    SetMaterialColor(previewG, green);
                    if (Input.GetMouseButtonDown(0))
                    {
                        PlaceRoof();
                    }
                }
                else
                    SetMaterialColor(previewG, red);
            }
            else if (hit.transform.tag == "RoofPlaceN")
            {
                previewG.transform.position = hit.transform.position;

                if (ServerInventoryCallbacks.CanPlaceRoof(hit.transform.position))
                {
                    SetMaterialColor(previewG, green);
                    if (Input.GetMouseButtonDown(0))
                    {
                        PlaceRoof();
                    }
                }
                else
                    SetMaterialColor(previewG, red);
            }
            else
            {
                SetMaterialColor(previewG, red);
                previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.5f);
            }
        }
        else
        {
            previewG.transform.position = new Vector3();
            //Destroy(previewG);
        }
    }
    public void MoveDoor(Item itemid)
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 7f, bs.Door))
        {
            if (hit.transform.tag == "DoorPlace")
            {
                previewG.transform.position = hit.transform.position;

                if (ServerInventoryCallbacks.CanPlaceDoor(hit.transform.position))
                {
                    SetMaterialColor(previewG, green);
                    if (Input.GetMouseButtonDown(0))
                    {
                        PlaceDoor(hit.transform.rotation);
                    }
                }
                else
                    SetMaterialColor(previewG, red);
            }
            else
            {
                SetMaterialColor(previewG, red);
                previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.5f);
            }
        }
        else
        {
            previewG.transform.position = new Vector3();
            //Destroy(previewG);
        }
    }

    //End Builing
    #endregion

    public Rect menuOffset;
    public float menuOffsetBorder;
    public float invStoragespace;
    public float invStoragespacey;
    public Rect iposition2 = new Rect(-360f, -9.89f, 30, 30);
    public float border = -4.69f;
    public float itemNumOffset;
    void OnGUI()
    {
        if (me == null || state == null) return;

        //ShowStatusBar();

        Event e = Event.current;
        //CraftingMenu(e);
        //////////////////////////////////
        {
            Rect position = new Rect((Screen.width / 2) - (35 * 3) / 2, (Screen.height / 2) - (35 * 3) / 2, (721 / 15), (1024 / 15));

            if (guiState == GUIState.None)
            {
                oldOpacity = Vector2.Lerp(oldOpacity, new Vector2(0, 0), 0.1f);
                GUI.color = new Color32(255, 255, 255, (byte)oldOpacity.y);

                GUIoldPosition = Vector2.Lerp(GUIoldPosition, new Vector2(position.x, position.y - 10), 0.1f);
            }
            else
            {
                oldOpacity = Vector2.Lerp(oldOpacity, new Vector2(0, 255), 0.1f);
                GUI.color = new Color32(255, 255, 255, (byte)oldOpacity.y);

                GUIoldPosition = Vector2.Lerp(GUIoldPosition, new Vector2(position.x, position.y), 0.1f);
            }

            Rect pos = new Rect(GUIoldPosition.x, GUIoldPosition.y, 35 * 2, 35 * 2);
            if (image != null)
                GUI.DrawTexture(pos, image);
        }
        /////////////////////////////
        GUI.color = Color.white;
        float scale = 1.2f;
        ShowHotbar(scale, e);

        if (showInventory)
        {
            blur.enabled = true;
            float percentage = 2.61f;
            Vector2 InventoryScale = new Vector2(InventoryBackground.width / percentage, InventoryBackground.height / percentage);
            Vector2 InventoryStorageScale = new Vector2(InventoryStorage.width / percentage, InventoryStorage.height / percentage);

            int borderPixelsX = 696;
            int borderPixelsY = 224;

            Rect iposition = new Rect(Screen.width - (InventoryScale.x / 2) - borderPixelsX, Screen.height - (InventoryScale.y / 2) - borderPixelsY, InventoryScale.x, InventoryScale.y);
            Rect ipositionstorage = new Rect(Screen.width - (InventoryStorageScale.x / 2) - borderPixelsX, Screen.height - (InventoryStorageScale.y / 2) - borderPixelsY, InventoryStorageScale.x, InventoryStorageScale.y);

            GUI.DrawTexture(new Rect(ipositionstorage.x - iposition2.x, ipositionstorage.y - iposition2.y, ipositionstorage.width - iposition2.width, ipositionstorage.height - iposition2.height), InventoryStorage);
            GUI.DrawTexture(iposition, InventoryBackground);

            float invBeginsx = menuOffset.x;
            float invBeginsz = menuOffset.y;

            float slotHeight = menuOffset.width;
            float slotWidth = menuOffset.height;

            ArmorSlots(iposition, e);

            int slotID = 0;
            bool delete = true;
            for (int z = 0; z < 4; z++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Rect slotPos = new Rect();

                    if (x >= 4)
                    {
                        slotPos = new Rect((iposition.x + invBeginsx) + (x * slotWidth) + (menuOffsetBorder * x) + invStoragespace, (iposition.y + invBeginsz) + (z * slotHeight) + (invStoragespacey * z), slotWidth, slotHeight);
                        
                    }
                    else
                    {
                        slotPos = new Rect((iposition.x + invBeginsx) + (x * slotWidth) + (menuOffsetBorder * x), (iposition.y + invBeginsz) + (z * slotHeight) + (invStoragespacey * z), slotWidth, slotHeight);
                    }

                    Rect position = slotPos;
                    {
                        position.width += border;
                        position.height += border;

                        position.x -= border / 2;
                        position.y -= border / 2;
                    }

                    if (position.Contains(e.mousePosition))
                    {                        
                        GUI.DrawTexture(slotPos, InventoryMouseHover);
                        //If clicks while holding item
                        if (e.type == EventType.mouseDown && draggingItem && e.button == 0)
                        {
                            if (x >= 4 && state.insideChest || state.insideAirDrop)
                            {
                                mouseOldPosition = new Vector2(position.x, position.y);
                                mouseOldPosition2 = new Vector2(position.x, position.y);

                                MoveItem(slotID);

                                //MOVE ITEM CHEST
                            }
                            else if (x < 4)
                            {
                                mouseOldPosition = new Vector2(position.x, position.y);
                                mouseOldPosition2 = new Vector2(position.x, position.y);

                                MoveItem(slotID);
                            }

                            break;
                        }

                        if (e.type == EventType.mouseDown && e.button == 1)
                        {
                            if (x >= 4 && state.insideChest || state.insideAirDrop)
                            {
                                mouseOldPosition = new Vector2(position.x, position.y);
                                mouseOldPosition2 = new Vector2(position.x, position.y);

                                SplitItem(slotID);
                            }
                            else if (x < 4)
                            {
                                mouseOldPosition = new Vector2(position.x, position.y);
                                mouseOldPosition2 = new Vector2(position.x, position.y);

                                SplitItem(slotID);
                                break;
                            }
                        }

                        //Pick item
                        if (Inventory[x, z] > 0)
                        {
                            if (e.type == EventType.mouseDown && e.button == 0)
                            {
                                if (!draggingItem)
                                {
                                    if (x >= 4 && state.insideChest || state.insideAirDrop)
                                    {
                                        mouseOldPosition = new Vector2(position.x, position.y);
                                        mouseOldPosition2 = new Vector2(position.x, position.y);

                                        DragItem(slotID);
                                    }
                                    else if (x < 4)
                                    {
                                        mouseOldPosition = new Vector2(position.x, position.y);
                                        mouseOldPosition2 = new Vector2(position.x, position.y);

                                        DragItem(slotID);
                                    }
                                }
                                else
                                {
                                    //Move item from SlotX/Z to this one
                                }
                            }
                        }
                    }
                    /*else
                        GUI.DrawTexture(position, InventorySlotTex);*/

                    /*if (Inventory[x, z] > 0)
                    {
                        if (getItem(Inventory[x, z]) != null)
                        {
                            GUI.DrawTexture(position, getItem(Inventory[x, z]).ItemIcon);

                            if (getItem(Inventory[x, z]).ItemMaxStack > 1)
                                GUI.Label(position, InventoryNum[x, z] + "");
                        }
                    }*/

                    if (x >= 4)
                    {
                        GUI.DrawTexture(slotPos, InventoryHotbarSlot);
                    }

                    if (getItem(Inventory[x, z]).ItemID > 0 && getItem(Inventory[x, z]).ItemIcon != null)
                    {
                        GUI.DrawTexture(position, getItem(Inventory[x, z]).ItemIcon);

                        if (getItem(Inventory[x, z]).ItemMaxStack > 1)
                        {
                            GUI.color = Color.gray;
                            GUI.Label(new Rect(position.x + (itemNumOffset), position.y + (itemNumOffset), position.width, position.height), InventoryNum[x, z] + "", FontNumStyle);
                            GUI.color = Color.white;
                        }
                    }

                    if (Inventory[x, z] > 0 && position.Contains(e.mousePosition))
                    {
                        if (getItem(Inventory[x, z]) != null)
                        {
                            if (getItem(Inventory[x, z]).ItemName != null)
                            {
                                iMouseOver = getItem(Inventory[x, z]);
                                //ToolTip(getItem(Inventory[x, z]).ItemName, getItem(Inventory[x, z]).ItemName);
                                delete = false;
                            }
                        }
                    }
                    else
                    {
                        if (delete)
                            iMouseOver = null;
                    }

                    if (x == 0 && z == 0 && selecteditem != null && selecteditem.item.ItemID >= 1)
                    {
                        mouseOldPosition = e.mousePosition/*Vector2.Lerp(mouseOldPosition, e.mousePosition, 0.2f)*/;

                        GUI.DrawTexture(new Rect(mouseOldPosition.x, mouseOldPosition.y, position.width, position.height), selecteditem.item.ItemIcon);

                        if (selecteditem.item.ItemMaxStack > 1)
                        {
                            GUI.color = Color.gray;
                            GUI.Label(new Rect(mouseOldPosition.x + itemNumOffset, mouseOldPosition.y + itemNumOffset, slotWidth, slotHeight), selecteditem.itemNum + "", FontNumStyle);
                            GUI.color = Color.white;
                        }

                        lastKnownPosition = mouseOldPosition;
                    }

                    slotID++;
                }
            }
            ShowToolTip();
        }

        ItemDescription(e);
    }

	public GUIStyle FontStyle;
	public GUIStyle FontNumStyle;

	public void ShowHotbar(float scale, Event e)
	{
		int spacingX = 10;
		int spacingY = 10;

		float slotWidth = 48;

		float hotbarWidth = (slotWidth * 8) + (spacingX * 8);
		float hotbarHeight = (slotWidth + spacingY);
		
		for (int x = 0; x < NetworkManager.invWidth; x++)
		{
			Rect position = new Rect((Screen.width - hotbarWidth) + (x * slotWidth), (Screen.height - hotbarHeight) - spacingY, slotWidth, slotWidth);

			int borderX = 100;
			position.x -= borderX;

			if( x > 0)
			{
				position.x += spacingX * x;
			}

			Rect slotP = position;
			{
				float border = ((150 * position.width) / 306) * 2;

				slotP.width += border;
				slotP.height += border;

				slotP.x -= border / 2;
				slotP.y -= border / 2;
			}

            GUI.DrawTexture(slotP, InventoryHotbarSlot);

			GUI.color = Color.black;
			GUI.Label(position, (x + 1).ToString(), FontStyle);
			GUI.color = Color.white;

            if (getItem(InventoryHotbarContent[x]).ItemID >= 1)
			{
				GUI.DrawTexture(position, getItem(InventoryHotbarContent[x]).ItemIcon);
				if (getItem(InventoryHotbarContent[x]).ItemMaxStack > 1)
				{
					GUI.color = Color.gray;
					GUI.Label(new Rect(position.x + (slotWidth - (InventoryHotbarNum[x].ToString().Length * 8)), position.y + (slotWidth - 20), position.width, position.height) , InventoryHotbarNum[x] + "", FontNumStyle);
					GUI.color = Color.white;
				}
			}
			
			if (position.Contains(e.mousePosition))
			{
                /*if(selectedslot == x)
                    GUI.DrawTexture(slotP, InventoryMouseHover);*/

				GUI.DrawTexture(slotP, InventoryMouseHover);
				
				//If clicks while holding item
				if (e.type == EventType.mouseDown && draggingItem && e.button == 0 && showInventory)
				{					
					mouseOldPosition = new Vector2(position.x, position.y);
					mouseOldPosition2 = new Vector2(position.x, position.y);
					
					MoveHotbarItem(x);
					
					break;
				}
				
				if (e.type == EventType.mouseDown && e.button == 1 && draggingItem == false && showInventory)
				{
					mouseOldPosition = new Vector2(position.x, position.y);
					mouseOldPosition2 = new Vector2(position.x, position.y);
					
					SplitItemHotbar(x);
					
					break;
				}
				else if (e.type == EventType.mouseDown && e.button == 1 && draggingItem == true && showInventory)
				{
					mouseOldPosition = new Vector2(position.x, position.y);
                    mouseOldPosition2 = new Vector2(position.x, position.y);

                    SplitItemHotbar(x);

                    break;
                }

                //Pick item
                if (getItem(InventoryHotbarContent[x]).ItemID != 0)
                {
                    if (e.type == EventType.mouseDown && e.button == 0 && showInventory)
                    {
                        if (!draggingItem)
                        {
                            if (getItem(InventoryHotbarContent[x]).ItemID != 0)
                            {
                                mouseOldPosition = new Vector2(position.x, position.y);
                                mouseOldPosition2 = new Vector2(position.x, position.y);

                                //slotidd = slotID;

                                DragItemHotbar(x);
                            }
                        }
                        else
                        {
                            //Move item from SlotX/Z to this one
                        }
                    }
                }

				if (getItem(InventoryHotbarContent[x]) != null && position.Contains(e.mousePosition))
                {
					if (getItem(InventoryHotbarContent[x]).ItemName != null)
                    {
						ToolTip(getItem(InventoryHotbarContent[x]).ItemName, getItem(InventoryHotbarContent[x]).ItemName);
                    }
                }
            }
        }
    }
	public void ArmorSlots(Rect iposition, Event e)
	{
		//Slot width/height
		float slotHeight = ToScaleWidth(227, iposition, 2880);
		float slotWidth = ToScaleHeight(227, iposition, 1594);

		Item helmet = getItem(armorSlots[0]);
		Item chest = getItem(armorSlots[1]);
		Item leggings = getItem(armorSlots[2]);
		Item boots = getItem(armorSlots[3]);
		
		//Helmet slot
		{
			float slot1Beginsx = ToScaleWidth(85, iposition, 2880);
			float slot1Beginsy = ToScaleHeight(333, iposition, 1594);
			
			Rect slot1 = new Rect(iposition.x + slot1Beginsx, iposition.y + slot1Beginsy, slotWidth, slotHeight);
			GUI.DrawTexture(slot1, InventorySlot);
			
			checkIfClick(slot1, e, 0);
            if (helmet.ItemID > 0)
			{
				GUI.DrawTexture(slot1,helmet.ItemIcon);
				if (helmet.ItemMaxStack > 1)
					GUI.Label(slot1, helmet + "");
			}
		}
		//Body armor slot
		{
			float slot2Beginsx = ToScaleWidth(-50, iposition, 2880);
			float slot2Beginsy = ToScaleHeight(687, iposition, 1594);
			
			Rect slot2 = new Rect(iposition.x + slot2Beginsx, iposition.y + slot2Beginsy, slotWidth, slotHeight);
			GUI.DrawTexture(slot2, InventorySlot);
			
			checkIfClick(slot2, e, 1);
            if (chest.ItemID > 0)
			{
				GUI.DrawTexture(slot2, chest.ItemIcon);
			}
		}
		//Leggings slot
		{
			float slot3Beginsx = ToScaleWidth(-50, iposition, 2880);
			float slot3Beginsy = ToScaleHeight(951, iposition, 1594);
			
			Rect slot3 = new Rect(iposition.x + slot3Beginsx, iposition.y + slot3Beginsy, slotWidth, slotHeight);
			GUI.DrawTexture(slot3, InventorySlot);
			checkIfClick(slot3, e, 2);

            if (leggings.ItemID > 0)
			{
				GUI.DrawTexture(slot3, leggings.ItemIcon);
			}
		}
		//Shoes slot
		{
			float slot4Beginsx = ToScaleWidth(85, iposition, 2880);
			float slot4Beginsy = ToScaleHeight(1231, iposition, 1594);
			
			Rect slot4 = new Rect(iposition.x + slot4Beginsx, iposition.y + slot4Beginsy, slotWidth, slotHeight);
			GUI.DrawTexture(slot4, InventorySlot);
			checkIfClick(slot4, e, 3);

            if (boots.ItemID > 0)
			{
				GUI.DrawTexture(slot4, boots.ItemIcon);
			}
		}
	}

	public void checkIfClick(Rect slot1, Event e, int slotid)
	{
		if (slot1.Contains(e.mousePosition))
		{
			//GUI.DrawTexture(position, InventorySlotTex);

			Item helmet = getItem(armorSlots[0]);
			Item chest = getItem(armorSlots[1]);
			Item leggings = getItem(armorSlots[2]);
			Item boots = getItem(armorSlots[3]);
			
			//If clicks while holding item
            if (e.type == EventType.mouseDown && e.button == 0)
            {
                /*bool showNum = false;
                if (selecteditem.item.ItemMaxStack > 1)
                    showNum = true;*/
                //itemsLeftToAnimate.Add(new ItemAnimation(lastKnownPosition, new Vector2(slot1.x, slot1.y), mouseOldPosition2, selecteditem.item.ItemIcon,  selecteditem.itemNum, showNum));

                mouseOldPosition = new Vector2(slot1.x, slot1.y);
                mouseOldPosition2 = new Vector2(slot1.x, slot1.y);

                if (state.draggingItem.ItemID > 0)
                {
                    MoveArmor(slotid);
                }
                else
                {
                    DragArmor(slotid);
                }
                //MoveItemArmor(slotid); // change 0
                return;
            }
		}
	}

    Item iMouseOver = null;
    public void ItemDescription(Event e)
    {
        if(iMouseOver != null)
        {
            GUI.color = Color.black;
            GUILayout.BeginArea(new Rect(e.mousePosition.x + 20, e.mousePosition.y, 200, 100), "", "box");
            {
                GUI.color = Color.yellow;
                GUILayout.Label(iMouseOver.ItemName);
                GUI.color = Color.white;

                GUILayout.Space(5);
                GUILayout.Label(iMouseOver.ItemDescription);
            }
            GUILayout.EndArea();
        }
    }

    #region Updates
    public void UpdateDraggedItem()
    {
        ITFSPlayerState actorState = (ITFSPlayerState)state;
        selecteditem = new ItemInfo(getItem(actorState.draggingItem.ItemID), actorState.draggingItem.ItemNum);
    }
    public void UpdateBuilding()
    {
        ITFSPlayerState actorState = (ITFSPlayerState)state;
        isbuilding = actorState.building;
    }
    public void OpenInventory()
    {
        ITFSPlayerState actorState = (ITFSPlayerState)state;
        showInventory = actorState.showInventory;
        insideChest = actorState.insideChest;
        insideAirDrop = actorState.insideAirDrop;
        chestID = actorState.chestID;

        OnInventoryOpened();
    }
    public void UpdateSelectedItem()
    {
        ITFSPlayerState actorState = (ITFSPlayerState)state;

        selectedslot = getItem(actorState.selectedItemID);

        wepController.DiselectAll();
        wepController.SetSelectedWeapon(getItem(actorState.selectedItemID));
    }
	public void UpdateArmor (Bolt.IState state, string path, Bolt.ArrayIndices indices)
	{
		int index = indices [0];
		ITFSPlayerState actorState = (ITFSPlayerState)state;

		armorSlots [index] = actorState.armorSlots [index].ItemID;
        armorSlotsNum[index] = actorState.armorSlots[index].ItemNum;
	}
	public void UpdateHotbar (Bolt.IState state, string path, Bolt.ArrayIndices indices)
	{
		int index = indices [0];
		ITFSPlayerState actorState = (ITFSPlayerState)state;
		
        InventoryHotbarContent[index] = actorState.hotbar[index].ItemID;
        InventoryHotbarNum[index] = actorState.hotbar[index].ItemNum;
	}
    public void UpdateInventory (Bolt.IState state, string path, Bolt.ArrayIndices indices)
    {
        int index = indices[0];
        ITFSPlayerState actorState = (ITFSPlayerState)state;

        int slotID = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if(index == slotID)
                {
					Inventory[x,y] = actorState.inventory[index].ItemID;
                    InventoryNum[x, y] = actorState.inventory[index].ItemNum;
                }
                slotID++;
            }
        }
    }

    void OnInventoryOpened()
    {
        if(insideChest)
        {
            ITFSChestState container = getChest(chestID);
            SetChestItems(container);
        }
        else
        {
            ClearChestItems();
        }
    }
    public ITFSChestState getChest(Bolt.NetworkId id)
    {
        GameObject[] chests = GameObject.FindGameObjectsWithTag("Chest");
        for (int i = 0; i < chests.Length; i++)
        {
            if (chests[i].GetComponent<BoltEntity>() == null)
            {
                Destroy(chests[i]);
            }
            else
            {
                Bolt.NetworkId chest = chests[i].GetComponent<BoltEntity>().networkId;
                if (chest == id)
                {
                    return chests[i].GetComponent<BoltEntity>().GetState<ITFSChestState>();
                }
            }
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
    
    public void SetChestItems(ITFSChestState chest)
    {
        int slotid = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 4; x < 8; x++)
            {
                Inventory[x, y] = chest.inventory[slotid].ItemID/*items[x - 4, y]*/;
                InventoryNum[x, y] = chest.inventory[slotid].ItemNum/*itemsNum[x - 4, y]*/;
                slotid++;
            }
        }
    }
    public void SetAirDropItems(IAirDrop airdop)
    {
        int slotid = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 4; x < 8; x++)
            {
                {
                    Inventory[x, y] = airdop.inventory[slotid].ItemID;
                    InventoryNum[x, y] = airdop.inventory[slotid].ItemNum;
                }
                slotid++;
            }
        }
    }
    public void ClearChestItems()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 4; x < 8; x++)
            {
                Inventory[x, y] = 0;
                InventoryNum[x, y] = 0;
            }
        }
    }
    #endregion
    #region saveSpace
    string tName, tDesc;
    bool showTooTip = false;
    void ToolTip(string dname, string desc)
    {
        showTooTip = true;
        tName = dname;
        tDesc = desc;
        showTooTip = false;
    }
    void ShowToolTip()
    {
        if (tName == null) return;
        if (tName.Length > 2 && showTooTip)
        {
            Event e = Event.current;
            GUI.Label(new Rect(e.mousePosition.x + 5, e.mousePosition.y + 5, 100, 20), tName, "box");
        }
    }

    public float ToScaleWidth(float originalPixels, Rect currentPixels, int orignialWidth)
    {
        return (originalPixels * currentPixels.width) / orignialWidth;
    }
    public float ToScaleHeight(float originalPixels, Rect currentPixels, int orignialHeight)
    {
        return (originalPixels * currentPixels.height) / orignialHeight;
    }

    #endregion

    public int GetItemNum(Item i)
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
    //Settings
    void EnityAttached(BoltEntity entity)
    {
        EntityAttachedServer EAS = EntityAttachedServer.Create(Bolt.GlobalTargets.OnlyServer);
        {
            EAS.entity = entity;
        }
        EAS.Send();
    }
    void OpenInventory(BoltEntity entity)
    {
        InventoryOpen evnt = InventoryOpen.Create(Bolt.GlobalTargets.OnlyServer);
        evnt.entity = entity;
        evnt.showInventory = showInventory;
        evnt.Send();

        /*InventoryOpen evnt = new InventoryOpen();
        
       // {
            evnt.entity = entity;
            evnt.showInventory = showInventory;
        //}

        evnt.Send();*/
    }
    void OpenExitMenu(BoltEntity entity)
    {
        ExitMenu evnt = ExitMenu.Create(Bolt.GlobalTargets.OnlyServer);
        evnt.entity = entity;
        evnt.Send();
    }
    void OpenContainer(BoltEntity entity)
    {
        OpenDoorI();
        InventoryOpenContainer evnt = InventoryOpenContainer.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.entity = entity;
        }
        evnt.Send();
    }

    //Move item
    void MoveItem(int slotID)
    {
        InventoryMoveItem evnt = InventoryMoveItem.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.slotID = slotID;
            evnt.pln = me;
        }
        evnt.Send();
    }
	void MoveItemArmor(int slotID)
	{
        InventoryMoveItemArmor evnt = InventoryMoveItemArmor.Create(Bolt.GlobalTargets.OnlyServer);
		{
			evnt.slotID = slotID;
			evnt.enity = me;
		}
        evnt.Send();
	}
	void MoveHotbarItem(int slotID)
	{
        InventoryMoveItemHotBar evnt = InventoryMoveItemHotBar.Create(Bolt.GlobalTargets.OnlyServer);
		{
			evnt.slotID = slotID;
			evnt.enity = me;
		}
        evnt.Send();
	}

    //Drag item
    void DragItem(int slotID)
    {
        InventoryDragItem evnt = InventoryDragItem.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.slotID = slotID;
            evnt.pln = me;
        }
        evnt.Send();
    }
	void DragItemHotbar(int slotID)
	{
        InventoryDragItemHotbar evnt = InventoryDragItemHotbar.Create(Bolt.GlobalTargets.OnlyServer);
		{
			evnt.slotID = slotID;
			evnt.enity = me;
		}
        evnt.Send();
    }

    //Split item
    void SplitItem(int slotID)
    {
        InventorySplitItem evnt = InventorySplitItem.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.slotID = slotID;
            evnt.pln = me;
        }
        evnt.Send();
    }
	void SplitItemHotbar(int slotID)
	{
        InventorySplitItemHotbar evnt = InventorySplitItemHotbar.Create(Bolt.GlobalTargets.OnlyServer);
		{
			evnt.slotID = slotID;
			evnt.enity = me;
		}
        evnt.Send();
    }

    //Move armor item
    void MoveArmor(int slotID)
    {
        if (slotID > 4) return;
        InventoryMoveArmor evnt = InventoryMoveArmor.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.slotID = slotID;
            evnt.entity = me;
        }
        evnt.Send();
    }
    void DragArmor(int slotID)
    {
        if (slotID > 4) return;
        InventoryDragArmor evnt = InventoryDragArmor.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.slotID = slotID;
            evnt.entity = me;
        }
        evnt.Send();
    }

    void SetSelectedItem(int slotID)
    {
        InventorySetSelectedItem evnt = InventorySetSelectedItem.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.slotID = slotID;
            evnt.entity = me;
        }
        evnt.Send();
    }
    void PlaceFoundation(Vector3 pos, Quaternion rot)
    {
        BuildFoundation evnt = BuildFoundation.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.entity = me;
        }
        evnt.Send();
    }
    void PlaceChest(Quaternion rot)
    {
        BuildChest evnt = BuildChest.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.entity = me;
            evnt.rot = rot;
        }
        evnt.Send();
    }
    void PlaceWall(Quaternion rot)
    {
        BuildWall evnt = BuildWall.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.entity = me;
            evnt.rotation = rot;
        }
        evnt.Send();
    }
    void PlaceRoof()
    {
        BuildRoof evnt = BuildRoof.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.entity = me;
        }
        evnt.Send();
    }
    void PlaceDoor(Quaternion rot)
    {
        BuildDoor evnt = BuildDoor.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.entity = me;
            evnt.rotation = rot;
        }
        evnt.Send();
    }
    void OpenDoorI()
    {
        OpenDoor evnt = OpenDoor.Create(Bolt.GlobalTargets.OnlyServer);
        {
            evnt.entity = me;
        }
        evnt.Send();
    }


    #region invManage
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
    public void SetItemNum(int slotid, int num)
    {
        state.inventory[slotid].ItemNum = num;
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

    public bool AddItem(Item e, int count)
    {
        int slotID = 0;
        for (int z = 0; z < 4; z++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (isChestSlot(slotID))
                    continue;
                if (state.inventory[slotID].ItemID == 0 || state.inventory[slotID].ItemID == e.ItemID)
                {
                    if (state.inventory[slotID].ItemNum != getItem(name).ItemMaxStack)
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
                            AddItem(e, rest);
                        }

                        return true;
                    }
                }
                slotID++;
            }
        }

        return false;
    }

    #endregion
    public enum GUIState
    {
        None,
        Chest,
        Door,
        Skin
    }
}

[System.Serializable]
public class BuildingObjects
{
    public GameObject ChestModel;
    public GameObject Door;
    public GameObject Wall1;
    public GameObject Foundation;
    public GameObject Roof;
    public GameObject WoodDoor;
    public GameObject Window;
    public GameObject Fence;
    public GameObject StairWay;
    public GameObject Stick;
    public GameObject campFire;

    public GameObject Trap1;
    public GameObject ClothTent;
    public GameObject ClothHut;


    public Material buildingMaterial;
}
[System.Serializable]
public class WeaponObjects
{
    public GameObject[] Weapons;
}
[System.Serializable]
public class BuildingSystem
{
    public LayerMask Foundations;
    public LayerMask Walls;
    public LayerMask Roofs;
    public LayerMask Chests;
    public LayerMask Door;
    public LayerMask OpenD;
}
