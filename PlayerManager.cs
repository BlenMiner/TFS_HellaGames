using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    /*#region
    bool myPlayer = false;
    public bool showInv = false;

    public Texture2D InventoryMainTray;
    public Texture2D InventoryHotbar;

    public Texture2D BlockChestImage;
    public Texture2D InventorySlot;

    public Texture2D Crosshair;

    public GameObject camera;
    public MouseLook[] mouse;

    public Item[,] InventoryContent = new Item[NetworkManager.invWidth, NetworkManager.invHeight];
    public int[,] InventoryNum = new int[NetworkManager.invWidth, NetworkManager.invHeight];

    public Item[,] InventoryHotbarContent = new Item[NetworkManager.invWidth, 1];
    public int[,] InventoryHotbarNum = new int[NetworkManager.invWidth, 1];

    public Item[,] InventoryArmorContent = new Item[1, 4];
    public int[,] InventoryArmorNum = new int[1, 4];

    public StateImages stateImages;

    bool draggingItem = false;
    int slotidd = -1;
    Item selecteditem = null;
    int selecteditemnum = 0;

    int invID;
    bool insideChest = false;
    bool someoneelseInsideChest = false;

    GUIState old_guiState = GUIState.None;
    GUIState guiState = GUIState.None;

    Vector2 GUIoldPosition = new Vector2(50,50);
    Vector2 oldOpacity = new Vector2(0, 0);
    Vector3 lastPositionChest;

    public LayerMask lm;
    #endregion
    #region
    public GameObject[] Weapons;
    public GameObject[] WeaponsWorld;
    private float Health = 100;
    private float Hunger = 100;
    private float Stamina = 100;
    public Texture2D StatusBarBackground;
    public Texture2D HealthBar;
    public Texture2D HungerBar;
    public Texture2D StaminaBar;
    public Texture2D HealthBarIcon;
    public Texture2D HungerBarIcon;
    public Texture2D StaminaBarIcon;

    public CraftingImages craftingImages;

    private GameObject previewBuilding;

    CharacterControls controller;
    public GameObject worldView;
    Animator anim;

    public GameObject Butterflies;
    public GameObject Rain;
    public GameObject Snow;
    public GameObject WindyLeaves;
    public GameObject MistCloud; 
    public GameObject SnowObject;

    [System.NonSerialized]
    public bool walking;
    [System.NonSerialized]
    public bool running;
    [System.NonSerialized]
    public bool slashing;
    [System.NonSerialized]
    public bool jumping;
    [System.NonSerialized]
    public bool defending;
    #endregion

    void Awake()
    {
        if(networkView.isMine)
            RegisterLists();

        anim = worldView.GetComponent<Animator>();
        if(Network.isServer)
            MPPlayer.GetPlayer(networkView.owner).PlayerTransform = this.transform;

        controller = GetComponent<CharacterControls>();

        DontDestroyOnLoad(gameObject);
        myPlayer = networkView.isMine;

        if (networkView.isMine)
        {
            Physics.IgnoreLayerCollision(8, 12);
            Physics.IgnoreLayerCollision(8, 13);
            Physics.IgnoreLayerCollision(8, 14);
            Physics.IgnoreLayerCollision(8, 16);

            if (Network.isServer)
                Server_SetPlayerName(Network.player);
            else
                networkView.RPC("Server_SetPlayerName", RPCMode.Server, Network.player);
            gameObject.layer = 8;

            for (int x = 0; x < NetworkManager.invWidth; x++)
            {
                InventoryHotbarContent[x, 0] = new Item();
                InventoryHotbarNum[x, 0] = 0;
            }
            for (int x = 0; x < 4; x++)
            {
                InventoryArmorContent[0, x] = new Item();
                InventoryArmorNum[0, x] = 0;
            }
        }

        if (Network.isServer)
        {
            posOld = MPPlayer.GetPlayer(networkView.owner).PlayerTransform.transform.position;
            rotOld = MPPlayer.GetPlayer(networkView.owner).PlayerTransform.transform.rotation;
        }
    }

    public bool building = false;
    public Item selectedBuilding = null;
    void Update()
    {
        bool inv = true;
        UpdateMovement();
        if (Network.isServer)
        {
            HungerConsume();
            AntiSpeedHack();
            ServerClearMemory();
        }

        if (!networkView.isMine)
            return;

        if (building)
        {
            Building(selectedBuilding);
        }
        else
        {
            if (previewG != null)
            {
                Destroy(previewG);
                previewG = null;
            }
        }

        if (Input.GetButtonDown("cMenu"))
            showCrafting = !showCrafting;

        if(guiState != old_guiState)
        {
            old_guiState = guiState;
            OnGUIChanged();
        }
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
                        inv = false;
                        if (Network.isServer)
                        {
                            Server_PickupArrow(hit.transform.gameObject.GetComponent<ArrowBehavior>().id, Network.player);
                        }
                        else
                        {
                            networkView.RPC("Server_PickupArrow", RPCMode.Server, hit.transform.gameObject.GetComponent<ArrowBehavior>().id, Network.player);
                        }
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

        if (Input.GetKeyDown(KeyCode.E) && networkView.isMine)
        {
            someoneelseInsideChest = false;

            UpdateInventory(); // CHANGE THIS IMIDIATLY
            if (inv == false)
                showInv = false;
            else
                showInv = !showInv;
            itemsLeftToAnimate.Clear();

            if (showInv)
            {
                RaycastHit hit;
                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 7f, lm))
                {
                    if (hit.transform.tag == "Chest")
                    {
                        if (Network.isServer)
                        {
                            GetChestContent(hit.transform.GetComponent<ChestContainer>().id, Network.player);
                        }
                        else
                            networkView.RPC("GetChestContent", RPCMode.Server, hit.transform.GetComponent<ChestContainer>().id, Network.player);
                        insideChest = true;
                        invID = hit.transform.GetComponent<ChestContainer>().id;
                    }
                    else
                    {
                        for (int x = 4; x < NetworkManager.invWidth; x++)
                        {
                            for (int z = 0; z < NetworkManager.invHeight; z++)
                            {
                                InventoryContent[x, z] = new Item();
                                InventoryNum[x, z] = 0;
                            }
                        }
                        insideChest = false;
                    }
                }
                else
                {
                    for (int x = 4; x < NetworkManager.invWidth; x++)
                    {
                        for (int z = 0; z < NetworkManager.invHeight; z++)
                        {
                            InventoryContent[x, z] = new Item();
                            InventoryNum[x, z] = 0;
                        }
                    }
                    insideChest = false;
                }
            }
            else
            {
                for (int x = 4; x < NetworkManager.invWidth; x++)
                {
                    for (int z = 0; z < NetworkManager.invHeight; z++)
                    {
                        InventoryContent[x, z] = new Item();
                        InventoryNum[x, z] = 0;
                    }
                }
                insideChest = false;

                if (Network.isServer)
                    Server_CloseInvChest(invID, Network.player);
                else
                    networkView.RPC("Server_CloseInvChest", RPCMode.Server, invID, Network.player);
            }
        }

        if (showInv)
        {
            DisableMouse();
        }
        else
        {
            EnableMouse();
        }
        
        if (networkView.isMine)
            SelectItem();
    }

    void DisableMouse()
    {
        foreach(MouseLook m in mouse)
        {
            m.enabled = false;
        }
    }
    void EnableMouse()
    {
        foreach (MouseLook m in mouse)
        {
            m.enabled = true;
        }
    }
    public bool CanUseMouse()
    {
        if (showInv)
            return false;

        foreach (MouseLook m in mouse)
        {
            if (m.enabled == false)
                return false;
            else
                return true;
        }

        return true;
    }

    [RPC]
    public void Server_PickupArrow(int id, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);
        ArrowBehavior arrow = GetArrow(id);
        if (arrow == null) return;

        Network.Destroy(arrow.gameObject);
        pl.AddItem("Arrow", 1);
    }
    ArrowBehavior GetArrow(int id)
    {
        foreach(ArrowBehavior a in ArrowBehavior.arrows)
        {
            if (a.id == id)
                return a;
        }
        return null;
    }

    private Item selectedItem;

    #region building system

    private GameObject previewG;
    private BoxCollider boxCollider;
    
    public Color red = new Color(255, 0, 0, 50);
    public Color green = new Color(0, 255, 0, 50);
    public BuildingSystem bs;

    public void Building(Item i)
    {
        if (i == null) return;
        if (i.ItemObjectWorld == null) return;

        if (selectedItem == null)
        {
            if (previewG != null)
            {
                Destroy(previewG);
                previewG = null;
            }
        }
        else if (selectedItem.ItemID != i.ItemID)
        {
            if (previewG != null)
            {
                Destroy(previewG);
                previewG = null;
            }
        }

        if (previewG == null)
        {
            selectedItem = i;
            previewG = Instantiate(i.ItemObjectWorld, new Vector3(0,0,0), Quaternion.identity) as GameObject;

            boxCollider = previewG.GetComponent<BoxCollider>();
            boxCollider.enabled = false;

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

            Destroy(previewG.GetComponent<NetworkView>());
            previewG.renderer.material = NetworkManager.instance.buildingMaterial; 
            previewG.renderer.material.color = red;
            previewG.renderer.castShadows = false;
            previewG.renderer.receiveShadows = false;
        }
        else
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                previewG.transform.Rotate(0, -10, 0, Space.World);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                previewG.transform.Rotate(0, 10, 0, Space.World);
            }

            if (i.ItemName == "container")
                MoveChest();
            else if (i.ItemName == "Foundation")
            {
                MoveFoundation(i);
            }
            else if (i.ItemName == "Wall1" || i.ItemName == "Door")
            {
                MoveWall(i);
            }
            else
            {
                MoveChest();
            }
        }
    }

    public void MoveChest()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 7f, lm))
        {
            previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.9f);
            previewG.transform.rotation.SetLookRotation(transform.position, Vector3.up);

            Bounds bounds = boxCollider.bounds;


            Ray ra = new Ray(boxCollider.transform.position, Vector3.down);
            float distance = 1f;

            if (bounds.IntersectRay(ra, out distance))
            {
                previewG.renderer.material.color = red;
            }
            else
            {
                previewG.renderer.material.color = green;
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
                if (CanPlaceFoundation(hit.transform.GetComponent<FoundationPlace>().GetPosition()))
                {
                    //previewG.transform.parent = hit.transform.GetComponent<FoundationPlace>().GetFoundationP();
                    previewG.renderer.material.color = green;
                    previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.transform.GetComponent<FoundationPlace>().GetPosition(), 0.9f);
                    previewG.transform.rotation = Quaternion.identity;
                    if (Input.GetMouseButtonDown(0) && CanUseMouse())
                    {
                        if (Network.isServer)
                            Server_PlaceStructure(itemid.ItemID, hit.transform.GetComponent<FoundationPlace>().GetPosition(), hit.transform.GetComponent<FoundationPlace>().GetFoundationP().rotation);
                        else
                            networkView.RPC("Server_PlaceStructure", RPCMode.Server, itemid.ItemID, hit.transform.GetComponent<FoundationPlace>().GetPosition(), Quaternion.identity);
                    }
                }
                else
                {
                    previewG.renderer.material.color = red;

                    previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.9f);
                    previewG.transform.rotation = Quaternion.identity;
                }
            }
            else if (!CanPlaceFoundation(hit.point))
            {
                previewG.renderer.material.color = red;

                previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.9f);
                previewG.transform.rotation = Quaternion.identity;
            }
            else
            {
                previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.9f);
                previewG.transform.rotation = Quaternion.identity;

                previewG.renderer.material.color = green;
                if (Input.GetMouseButtonDown(0) && CanUseMouse())
                {
                    if (Network.isServer)
                        Server_PlaceStructure(itemid.ItemID, hit.point, Quaternion.identity);
                    else
                        networkView.RPC("Server_PlaceStructure", RPCMode.Server, itemid.ItemID, hit.point, Quaternion.identity);
                }
            }
        }
        else
        {
            Destroy(previewG);
        }
    }
    public void MoveWall(Item itemid)
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 7f, bs.Walls))
        {
            if (hit.transform.tag == "Foundation")
            {
                Collider collider = hit.transform.GetComponent<Foundation>().wallLimits;

                previewG.transform.position = hit.point;

                if (collider.bounds.Intersects(previewG.collider.bounds) && CanPlaceWall(hit.point))
                {
                    previewG.renderer.material.color = green;

                    if (Input.GetMouseButtonDown(0) && CanUseMouse())
                    {
                        if (Network.isServer)
                            Server_PlaceStructure(itemid.ItemID, hit.point, previewG.transform.rotation);
                        else
                            networkView.RPC("Server_PlaceStructure", RPCMode.Server, itemid.ItemID, hit.point, previewG.transform.rotation);
                    }
                }
                else
                    previewG.renderer.material.color = red;
            }
            else
            {
                previewG.renderer.material.color = red;

                previewG.transform.position = Vector3.Lerp(previewG.transform.position, hit.point, 0.5f);
            }
        }
        else
        {
            previewG.transform.position = new Vector3();
            //Destroy(previewG);
        }
    }

    bool CanPlaceWall(Vector3 pos)
    {
        if (IsInBlockedZone(pos))
            return false;
        if (NetworkManager.walls.Count > 0)
        {
            for (int i = 0; i < NetworkManager.walls.Count; i++)
            {
                if (Vector3.Distance(pos, NetworkManager.walls[i].transform.position) < 2f)
                {
                    return false;
                }
            }
        }
        return true;
    }
    bool IsColliding(Collider[] c)
    {
        foreach(Collider co in c)
        {
            if (co.bounds.Intersects(previewG.collider.bounds))
                return true;
        }
        return false;
    }
    bool CanPlaceFoundation(Vector3 pos)
    {
        if (IsInBlockedZone(pos))
            return false;
        if (NetworkManager.foundations.Count > 0)
        {
            for (int i = 0; i < NetworkManager.foundations.Count; i++)
            {
                if (!FoundationIsFromHere(pos, NetworkManager.foundations[i].transform.position))
                {
                    if (Vector3.Distance(pos, NetworkManager.foundations[i].transform.position) < 7f)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    bool FoundationIsFromHere(Vector3 pos, Vector3 foundationp)
    {
        Vector3 posR = pos - foundationp;
        if (posR.x % 4 == 0 && posR.z % 4 == 0)
        {
            return true;
        }
        return false;
    }

    bool IsInBlockedZone(Vector3 pos)
    {
        foreach(Collider c in NetworkManager.blockedAreas)
        {
            if (c.collider.bounds.Contains(pos))
                return true;
        }
        return false;
    }

    [RPC]
    public void Server_PlaceStructure(int itemID, Vector3 pos, Quaternion rot)
    {
        GameObject f = Network.Instantiate(Item.getItem(itemID).ItemObjectWorld, pos, rot, 0) as GameObject;
    }

    [RPC]
    public void Server_SetPlayerName(NetworkPlayer pl)
    {
        MPPlayer pln = MPPlayer.GetPlayer(pl);
        playerName.text = pln.PlayerName;

        networkView.RPC("Client_SetPlayerName", RPCMode.AllBuffered, pln.PlayerName);
    }
    [RPC]
    public void Client_SetPlayerName(string name)
    {
        playerName.text = name;
    }
    #endregion
    #region Sineps
    /*void CharMov()
    {
        bool moving = CharMoved();

        if (controller.isGrounded)
        {
            jumping = false;
            if (moving)
            {
                if (controller.isSpritning)
                {
                    running = true;
                    walking = false;
                }
                else
                {
                    running = false;
                    walking = true;
                }
            }
            else
            {
                running = false;
                walking = false;
            }
        }
        else
        {
            jumping = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            slashing = true;
        }
        else
        {
            slashing = false;
        }

        if (Input.GetMouseButton(1))
        {
            defending = true;
        }
        else
        {
            defending = false;
        }
    }
    Vector3 lastPos;
    bool CharMoved()
    {
        Vector3 displacement = transform.position - lastPos;
        lastPos = transform.position;
   
        if(displacement.magnitude > 0.001)  // return true if char moved 1mm
        {
            return true;
        } else {
            return false;
        }
    }*/
	/*
    float timetowait = 1f;
    void AntiSpeedHack()
    {
        if (Network.isServer && networkView.owner != Network.player)
        {
            RaycastHit hit;
            if (Physics.Raycast(MPPlayer.GetPlayer(networkView.owner).PlayerTransform.position, Vector3.down, out hit))
            {
                if (Vector3.Distance(hit.point, MPPlayer.GetPlayer(networkView.owner).PlayerTransform.position) < 2.2f)
                {
                    if (Vector3.Distance(pos, posOld) > NetworkManager.walkSpeedRestriction && !MPPlayer.GetPlayer(networkView.owner).PlayerSprinting)
                    {
                        Teleport(networkView.owner, posOld);
                    }
                    else if (Vector3.Distance(pos, posOld) > NetworkManager.runSpeedRestriction && MPPlayer.GetPlayer(networkView.owner).PlayerSprinting)
                    {
                        Teleport(networkView.owner, posOld);
                    }
                }
                else
                {
                    if (Vector3.Distance(pos, posOld) > NetworkManager.airSpeedRestriction)
                    {
                        Teleport(networkView.owner, posOld);
                    }
                }
            }

            if (Time.time > timetowait)
            {
                posOld = MPPlayer.GetPlayer(networkView.owner).PlayerTransform.transform.position;
                rotOld = MPPlayer.GetPlayer(networkView.owner).PlayerTransform.transform.rotation;
                timetowait = Time.time + 1f;
            }
        }
    }

    void SelectItem()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Network.isServer)
                Server_SelectItem(0, Network.player);
            else
                networkView.RPC("Server_SelectItem", RPCMode.Server, 0, Network.player);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Network.isServer)
                Server_SelectItem(1, Network.player);
            else
                networkView.RPC("Server_SelectItem", RPCMode.Server, 1, Network.player);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (Network.isServer)
                Server_SelectItem(2, Network.player);
            else
                networkView.RPC("Server_SelectItem", RPCMode.Server, 2, Network.player);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (Network.isServer)
                Server_SelectItem(3, Network.player);
            else
                networkView.RPC("Server_SelectItem", RPCMode.Server, 3, Network.player);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (Network.isServer)
                Server_SelectItem(4, Network.player);
            else
                networkView.RPC("Server_SelectItem", RPCMode.Server, 4, Network.player);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (Network.isServer)
                Server_SelectItem(5, Network.player);
            else
                networkView.RPC("Server_SelectItem", RPCMode.Server, 5, Network.player);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (Network.isServer)
                Server_SelectItem(6, Network.player);
            else
                networkView.RPC("Server_SelectItem", RPCMode.Server, 6, Network.player);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (Network.isServer)
                Server_SelectItem(7, Network.player);
            else
                networkView.RPC("Server_SelectItem", RPCMode.Server, 7, Network.player);
        }
    }
    void OnGUIChanged()
    {
        /*GUIoldPosition = new Vector2((Screen.width / 2) - 50, (Screen.height / 2) - 50);
        oldOpacity = new Vector2(0, 0);
        oldTrancparency = new Vector2(0, 225);*/
	/*
        if (guiState == GUIState.Chest)
        {
            image = stateImages.Chest;
        }
        else if (guiState == GUIState.Door)
        {
            image = stateImages.Door;
        }
        else
        {
            
        }
    }

    float sinep = 0;
    void ServerClearMemory()
    {
        if (Time.time > sinep)
        {
            System.GC.Collect();
            sinep = Time.time + 60f;
        }
    }
    #endregion
    #region Item Selection
    /// <Being weapons selection>
    /// ///////////////////////////

    [RPC]
    public void Server_SelectItem(int slot, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);
        Item itemToUse = pl.getHotBarItem(slot).item;

        networkView.RPC("Client_DisableAllItems", RPCMode.All, pln);

        if (itemToUse == null)
            itemToUse = new Item();

        if (itemToUse.ItemType == Item.ItemTypeE.Weapon)
        {
            bool activate = true;

            if (pl.SelectedItem.item.ItemID == itemToUse.ItemID)
            {
                activate = !pl.SelectedItem.active;
                pl.SelectedItem.active = activate;
            }

            pl.SelectedItem = new SelectedItem(itemToUse, activate, slot);

            networkView.RPC("Client_SelectWeapon", RPCMode.AllBuffered, itemToUse.ItemID, slot, activate, pln);

            return;
        }
        else if(itemToUse.ItemType == Item.ItemTypeE.Building)
        {
            bool activated = true;

            if(pl.SelectedItem.item.ItemID == itemToUse.ItemID)
            {
                activated = false;
                pl.SelectedItem.active = activated;
            }
            pl.SelectedItem = new SelectedItem(itemToUse, activated, slot);

            if (Network.player == pln)
                Client_SelectBuilding(itemToUse.ItemID, activated);
            else
                networkView.RPC("Client_SelectBuilding", pln, itemToUse.ItemID, activated);
        }
    }
    [RPC]
    public void Client_SelectWeapon(int itemID, int slot, bool active, NetworkPlayer pln)
    {
        if (Network.player == pln)
        {
            Item itemToUse = Item.getItem(itemID);
            SelectedSlot = slot;

            GameObject weapon = findGameObject(itemToUse.ItemName);
            if (weapon != null)
                weapon.SetActive(active);
        }
        else
        {
            Item itemToUse = Item.getItem(itemID);
            SelectedSlot = slot;

            GameObject weaponWorld = findGameObject2(itemToUse.ItemName);
            if (weaponWorld != null)
                weaponWorld.SetActive(active);
        }
    }

    [RPC]
    public void Client_SelectBuilding(int id, bool active)
    {
        Debug.Log("Sinep");
        building = active;
        selectedBuilding = Item.getItem(id);
    }
    [RPC]
    public void Client_ConsumeItem(int slot, int ammount, int foodRestore)
    {

    }
    [RPC]
    public void Client_DisableAllItems(NetworkPlayer pl)
    {
        if (previewG != null)
        {
            Destroy(previewG);
            previewG = null;
        }

        building = false;
        selectedBuilding = null;
        if (Network.player == pl)
        {
            foreach (GameObject go in Weapons)
            {
                go.SetActive(false);
            }

            //Weapons[2].SetActive(true);
        }
        else
        {
            foreach (GameObject go in WeaponsWorld)
            {
                go.SetActive(false);
            }
        }
    }

    public GameObject findGameObject(string name)
    {
        foreach(GameObject go in Weapons)
        {
            if (go.name == name)
                return go;
        }
        return null;
    }
    public GameObject findGameObject2(string name)
    {
        foreach (GameObject go in WeaponsWorld)
        {
            if (go.name == name)
                return go;
        }
        return null;
    }
    int SelectedSlot = -1;
    /// ////////////////////////////////
    /// <End>
    #endregion
    #region Chest Content
    [RPC]
    public void GetChestContent(int id, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);
        ChestContainer chestInvv = NetworkManager.Chests[id];
        string chestInv = NetworkManager.Chests[id].ChestInvToString();

        if(Vector3.Distance(chestInvv.transform.position, pl.PlayerTransform.position)> 10)
        {
            if (pln == Network.player)
            {
                Client_OutofChest();
            }
            else
            {
                networkView.RPC("Client_OutofChest", pln);
            }
            return;
        }

        //chestInv.playerInsideChest = new NetworkPlayer();
        if (chestInvv.chestOpen)
        {
            if (pln == Network.player)
            {
                Client_OutofChest();
            }
            else
            {
                networkView.RPC("Client_OutofChest", pln);
            }
            return;
        }

        chestInvv.chestOpen = true;
        chestInvv.playerInsideChest = pln;
    
        if (pln == Network.player)
        {
            Client_UpdateChestContent(chestInv);
        }
        else
        {
            networkView.RPC("Client_UpdateChestContent", pln, chestInv);
        }
    }
    [RPC]
    public void Client_UpdateChestContent(string content)
    {
        StringToChest(content);
    }
    [RPC]
    public void Client_OutofChest()
    {
        for (int x = 4; x < NetworkManager.invWidth; x++)
        {
            for (int z = 0; z < NetworkManager.invHeight; z++)
            {
                InventoryContent[x, z] = new Item();
                InventoryNum[x, z] = 0;
            }
        }
        insideChest = false;
        someoneelseInsideChest = true;
    }
    [RPC]
    public void Server_CloseInvChest(int id, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);
        ChestContainer chestInv = NetworkManager.Chests[id];

        if(chestInv.playerInsideChest == pln)
        {
            chestInv.playerInsideChest = new NetworkPlayer();
            chestInv.chestOpen = false;
        }
    }
    #endregion
    #region Crafting Menu           
    
    void DisplayCraftinMenu()
    {

    }

    #endregion
    #region GUI
    Vector2 mouseOldPosition;
    Vector2 mouseOldPosition2;
    Vector2 lastKnownPosition;
    Texture2D image;

    public List<ItemAnimation> itemsLeftToAnimate = new List<ItemAnimation>();
    public TextMesh playerName;

    void OnGUI()
    {
        GUI.depth = 0;
        if (!networkView.isMine) return;
        DisplayCraftinMenu();
        GUILayout.Space(100);
        if(GUILayout.Button("Disconnect"))
        {
            Network.Disconnect();
            Destroy(gameObject);
            Application.LoadLevel(0);
        }

        if(showInv)
        {
            if (GUILayout.Button("Toggle Depth of field scatter"))
            {
                camera.GetComponent<DepthOfFieldScatter>().enabled = !camera.GetComponent<DepthOfFieldScatter>().enabled;
            }
            if (GUILayout.Button("Toggle Occlusion Culling"))
            {
                camera.GetComponent<ImprovisedOcclusionCulling>().enabled = !camera.GetComponent<ImprovisedOcclusionCulling>().enabled;
            }
            if (GUILayout.Button("Toggle Color Correction"))
            {
                camera.GetComponent<ColorCorrectionCurves>().enabled = !camera.GetComponent<ColorCorrectionCurves>().enabled;
            }
            if (GUILayout.Button("Toggle Sun Shafts"))
            {
                camera.GetComponent<SunShafts>().enabled = !camera.GetComponent<SunShafts>().enabled;
            }
            if (GUILayout.Button("Toggle SSAOEffect"))
            {
                camera.GetComponent<SSAOEffect>().enabled = !camera.GetComponent<SSAOEffect>().enabled;
            }
            if (GUILayout.Button("Toggle Bloom and Dirty lens"))
            {
                camera.GetComponent<SENaturalBloomAndDirtyLens>().enabled = !camera.GetComponent<SENaturalBloomAndDirtyLens>().enabled;
            }
        }

        ShowStatusBar();

        Event e = Event.current;
        CraftingMenu(e);
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
            GUI.DrawTexture(pos, image);
        }
        /////////////////////////////
        GUI.color = Color.white;
        float scale = 1.2f;
        ShowHotbar(scale, e);

        if (showInv)
        {
            Rect iposition = new Rect(((Screen.width / 2) - ((InventoryMainTray.width / scale) / 2)),
                                      (Screen.height / 2) - ((InventoryMainTray.height / scale) / 2),
                                      InventoryMainTray.width / scale,
                                      InventoryMainTray.height / scale);

            iposition.x -= ToScaleWidth(230, iposition, 2880);

            GUI.DrawTexture(iposition, InventoryMainTray);

            float space = ToScaleWidth(162, iposition, 2880);

            float invBeginsx = ToScaleWidth(666, iposition, 2880);
            float invBeginsz = ToScaleHeight(481, iposition, 1594);

            float slotHeight = ToScaleWidth(227, iposition, 2880);
            float slotWidth = ToScaleHeight(227, iposition, 1594);

            ArmorSlots(iposition, e);

            int slotID = 0;
            for (int x = 0; x < NetworkManager.invWidth; x++)
            {
                for (int z = 0; z < NetworkManager.invHeight; z++)
                {
                    Rect position = new Rect();

                    if (x >= 4)
                    {
                        position = new Rect((iposition.x + invBeginsx) + (x * slotWidth) + space, (iposition.y + invBeginsz) + (z * slotHeight), slotWidth, slotHeight);
                        if (x == 4 && z == 0 && !insideChest)
                        {
                            GUI.DrawTexture(new Rect(position.x, position.y, slotWidth * 4, slotWidth * 4), BlockChestImage);
                            if (someoneelseInsideChest)
                            {
                                GUI.Label(new Rect(position.x + 20, position.y + 20, (slotWidth * 4) - 20, (slotWidth * 4) - 20), "Chest is occupied!");
                            }
                        }
                    }
                    else
                    {
                        position = new Rect((iposition.x + invBeginsx) + (x * slotWidth), (iposition.y + invBeginsz) + (z * slotHeight), slotWidth, slotHeight);
                    }

                    if (position.Contains(e.mousePosition))
                    {
                        //GUI.DrawTexture(position, InventorySlotTex);

                        //If clicks while holding item
                        if (e.type == EventType.mouseDown && draggingItem && e.button == 0)
                        {
                            if (x >= 4 && insideChest)
                            {
                                draggingItem = false;

                                bool showNum = false;
                                if (selecteditem.ItemMaxStack > 1)
                                    showNum = true;
                                itemsLeftToAnimate.Add(new ItemAnimation(lastKnownPosition, new Vector2(position.x, position.y), mouseOldPosition2, selecteditem.ItemIcon, selecteditemnum, showNum));

                                mouseOldPosition = new Vector2(position.x, position.y);
                                mouseOldPosition2 = new Vector2(position.x, position.y);

                                int slot = slotID - 16;
                                MoveItemChest(invID, slot);

                                //MOVE ITEM CHEST
                            }
                            else if (x < 4)
                            {
                                draggingItem = false;

                                bool showNum = false;
                                if (selecteditem.ItemMaxStack > 1)
                                    showNum = true;
                                itemsLeftToAnimate.Add(new ItemAnimation(lastKnownPosition, new Vector2(position.x, position.y), mouseOldPosition2, selecteditem.ItemIcon, selecteditemnum, showNum));

                                mouseOldPosition = new Vector2(position.x, position.y);
                                mouseOldPosition2 = new Vector2(position.x, position.y);

                                MoveItem(slotID);
                            }

                            break;
                        }

                        if (e.type == EventType.mouseDown && e.button == 1 && draggingItem == false)
                        {
                            if (x >= 4 && insideChest)
                            {
                                mouseOldPosition = new Vector2(position.x, position.y);
                                mouseOldPosition2 = new Vector2(position.x, position.y);

                                int slot = slotID - 16;
                                SplitItemChest(invID, slot);
                            }
                            else if (x < 4)
                            {
                                mouseOldPosition = new Vector2(position.x, position.y);
                                mouseOldPosition2 = new Vector2(position.x, position.y);

                                SplitItem(slotID);
                                break;
                            }
                        }
                        else if (e.type == EventType.mouseDown && e.button == 1 && draggingItem == true)
                        {
                            /*mouseOldPosition = new Vector2(position.x, position.y);
                            mouseOldPosition2 = new Vector2(position.x, position.y);

                            SplitItemWhileDragging(x, false, true);*/

                           /* break;
                        }

                        //Pick item
                        if (InventoryContent[x, z].ItemID != -1)
                        {
                            if (e.type == EventType.mouseDown && e.button == 0)
                            {
                                if (!draggingItem)
                                {

                                    if (InventoryContent[x, z].ItemID != -1)
                                    {
                                        if (x >= 4 && insideChest)
                                        {
                                            mouseOldPosition = new Vector2(position.x, position.y);
                                            mouseOldPosition2 = new Vector2(position.x, position.y);

                                            int slot = slotID - 16;
                                            DragItemChest(invID, slot);
                                        }
                                        else if (x < 4)
                                        {
                                            mouseOldPosition = new Vector2(position.x, position.y);
                                            mouseOldPosition2 = new Vector2(position.x, position.y);

                                            slotidd = slotID;
                                            DragItem(slotidd);
                                        }
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
	/*
                    if (InventoryContent[x, z] == null)
                        InventoryContent[x, z] = new Item();
                    if (InventoryContent[x, z].ItemID != -1 && CanShowItem(position))
                    {
                        GUI.DrawTexture(position, InventoryContent[x, z].ItemIcon);
                        if (InventoryContent[x, z].ItemMaxStack > 1)
                            GUI.Label(position, InventoryNum[x, z] + "");
                    }
                    GUI.depth = 1;
                    if (InventoryContent[x, z] != null && position.Contains(e.mousePosition))
                    {
                        if (InventoryContent[x, z].ItemName != null)
                        {
                            ToolTip(InventoryContent[x, z].ItemName, InventoryContent[x, z].ItemName);
                        }
                    }

                    slotID++;
                }
            }

            if (draggingItem)
            {
                mouseOldPosition = Vector2.Lerp(mouseOldPosition, e.mousePosition, 0.2f);
                mouseOldPosition2 = Vector2.Lerp(mouseOldPosition2, e.mousePosition + new Vector2(slotWidth - 10, slotWidth - 10), 0.1f);

                GUI.DrawTexture(new Rect(mouseOldPosition.x, mouseOldPosition.y, slotWidth, slotHeight), selecteditem.ItemIcon);

                if (selecteditem.ItemMaxStack > 1)
                    GUI.Label(new Rect(mouseOldPosition2.x, mouseOldPosition2.y, slotWidth, slotHeight), selecteditemnum + "");

                lastKnownPosition = mouseOldPosition;
            }

            ItemAnimation(slotWidth);
        }
    }
    void ToolTip(string name, string desc)
    {
        GUI.depth = 2;
        Event e = Event.current;
        GUI.Label(new Rect(e.mousePosition.x + 5, e.mousePosition.y + 5, 100, 20), name, "box");
    }

    float h = 100;
    float hu = 100;
    float s = 100;
    public void ShowStatusBar()
    {
        h = Vector2.Lerp(new Vector2(0, h), new Vector2(0, Health), 0.1f).y;
        hu = Vector2.Lerp(new Vector2(0, hu), new Vector2(0, Hunger), 0.1f).y;
        s = Vector2.Lerp(new Vector2(0, s), new Vector2(0, Stamina), 0.05f).y;

        float scale = 3.8f;
        {
            ////////////////////////////////////////////////////////////////////////////////////Health
            Rect bar = new Rect(Screen.width - ((StatusBarBackground.width / scale) * 3),
                                Screen.height - (StatusBarBackground.height / scale),
                                (StatusBarBackground.width / scale),
                                (StatusBarBackground.height / scale));

            float space = ToScaleWidth(80, bar, 821) * 2;
            bar.x -= space * 2;
            bar.y -= space;

            GUI.DrawTexture(bar, StatusBarBackground);

            GUI.DrawTexture(new Rect(bar.x + ToScaleWidth(24, bar, 821) * 3, bar.y + ToScaleHeight(34, bar, 819), HealthBar.width / scale, PercentToHeight(HealthBar.height / scale, h)), HealthBar);
            GUI.DrawTexture(new Rect(bar.x + ToScaleWidth(24, bar, 821) * 3, bar.y + ToScaleHeight(557, bar, 819) + ToScaleHeight(34, bar, 819) * 4, HealthBarIcon.width / scale, HealthBarIcon.height / scale), HealthBarIcon);
        }
        {
            ////////////////////////////////////////////////////////////////////////////////////Stamina
            Rect bar = new Rect(Screen.width - ((StatusBarBackground.width / scale) * 2),
                                Screen.height - (StatusBarBackground.height / scale),
                                (StatusBarBackground.width / scale),
                                (StatusBarBackground.height / scale));

            float space = ToScaleWidth(80, bar, 821) * 2;
            bar.x -= space * 1;
            bar.y -= space;

            GUI.DrawTexture(bar, StatusBarBackground);

            GUI.DrawTexture(new Rect(bar.x + ToScaleWidth(24, bar, 821) * 3, bar.y + ToScaleHeight(34, bar, 819), HealthBar.width / scale, PercentToHeight(HealthBar.height / scale, s)), StaminaBar);
            GUI.DrawTexture(new Rect(bar.x + ToScaleWidth(24, bar, 821) * 3, bar.y + ToScaleHeight(557, bar, 819) + ToScaleHeight(34, bar, 819) * 4, HealthBarIcon.width / scale, HealthBarIcon.height / scale), StaminaBarIcon);

        }
        {
            ////////////////////////////////////////////////////////////////////////////////////Hunger
            Rect bar = new Rect(Screen.width - ((StatusBarBackground.width / scale) * 1),
                                Screen.height - (StatusBarBackground.height / scale),
                                (StatusBarBackground.width / scale),
                                (StatusBarBackground.height / scale));

            float space = ToScaleWidth(80, bar, 821) * 2;
            //bar.x -= space * 3;
            bar.y -= space;

            GUI.DrawTexture(bar, StatusBarBackground);

            GUI.DrawTexture(new Rect(bar.x + ToScaleWidth(24, bar, 821) * 3, bar.y + ToScaleHeight(34, bar, 819), HealthBar.width / scale, PercentToHeight(HealthBar.height / scale, hu)), HungerBar);
            GUI.DrawTexture(new Rect(bar.x + ToScaleWidth(24, bar, 821) * 3, bar.y + ToScaleHeight(557, bar, 819) + ToScaleHeight(34, bar, 819) * 4, HealthBarIcon.width / scale, HealthBarIcon.height / scale), HungerBarIcon);

        }
    }
    public float PercentToHeight(float fullHeight, float percent)
    {
        if (percent <= 0)
            return 0;
        return ((percent * 100) / fullHeight) * 1.45f;
    }

    public void ShowHotbar(float scale, Event e)
    {
        GUI.depth = 0;
        float scales = scale * 1.7f;
        Rect positioni = new Rect((Screen.width / 2) - ((InventoryHotbar.width / scales) / 2),
                                  (Screen.height) - ((InventoryHotbar.height / scales)),
                                  (InventoryHotbar.width / scales),
                                  (InventoryHotbar.height / scales));

        GUI.DrawTexture(positioni, InventoryHotbar);

        float invBeginsx = ToScaleWidth(95, positioni, 2000);
        float invBeginsz = ToScaleHeight(141, positioni, 431);

        float slotWidth = ToScaleWidth(230, positioni, 2000);

        for (int x = 0; x < NetworkManager.invWidth; x++)
        {
            Rect position = new Rect((positioni.x + invBeginsx) + (x * slotWidth), (positioni.y + invBeginsz), slotWidth, slotWidth);

            if (InventoryHotbarContent[x, 0].ItemID != -1 && CanShowItem(position))
            {
                GUI.DrawTexture(position, InventoryHotbarContent[x, 0].ItemIcon);
                if (InventoryHotbarContent[x, 0].ItemMaxStack > 1)
                    GUI.Label(position, InventoryHotbarNum[x, 0] + "");
            }

            if (position.Contains(e.mousePosition))
            {
                //GUI.DrawTexture(position, InventorySlotTex);

                //If clicks while holding item
                if (e.type == EventType.mouseDown && draggingItem && e.button == 0 && showInv)
                {
                    draggingItem = false;

                    bool showNum = false;
                    if (selecteditem.ItemMaxStack > 1)
                        showNum = true;
                    itemsLeftToAnimate.Add(new ItemAnimation(lastKnownPosition, new Vector2(position.x, position.y), mouseOldPosition2, selecteditem.ItemIcon, selecteditemnum, showNum));

                    mouseOldPosition = new Vector2(position.x, position.y);
                    mouseOldPosition2 = new Vector2(position.x, position.y);

                    MoveHotbarItem(x);

                    break;
                }

                if (e.type == EventType.mouseDown && e.button == 1 && draggingItem == false && showInv)
                {
                    mouseOldPosition = new Vector2(position.x, position.y);
                    mouseOldPosition2 = new Vector2(position.x, position.y);

                    SplitHotbarItem(x);

                    break;
                }
                else if (e.type == EventType.mouseDown && e.button == 1 && draggingItem == true && showInv)
                {
                    /*mouseOldPosition = new Vector2(position.x, position.y);
                    mouseOldPosition2 = new Vector2(position.x, position.y);

                    SplitItemWhileDragging(x, true, false);*/
	/*
                    break;
                }

                //Pick item
                if (InventoryHotbarContent[x, 0].ItemID != -1)
                {
                    if (e.type == EventType.mouseDown && e.button == 0 && showInv)
                    {
                        if (!draggingItem)
                        {
                            if (InventoryHotbarContent[x, 0].ItemID != -1)
                            {
                                mouseOldPosition = new Vector2(position.x, position.y);
                                mouseOldPosition2 = new Vector2(position.x, position.y);

                                //slotidd = slotID;

                                DragHotbarItem(x);
                            }
                        }
                        else
                        {
                            //Move item from SlotX/Z to this one
                        }
                    }
                }
                GUI.depth = 1;
                if (InventoryHotbarContent[x, 0] != null && position.Contains(e.mousePosition))
                {
                    if (InventoryHotbarContent[x, 0].ItemName != null)
                    {
                        ToolTip(InventoryHotbarContent[x, 0].ItemName, InventoryHotbarContent[x, 0].ItemName);
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

        //Helmet slot
        {
            float slot1Beginsx = ToScaleWidth(85, iposition, 2880);
            float slot1Beginsy = ToScaleHeight(333, iposition, 1594);

            Rect slot1 = new Rect(iposition.x + slot1Beginsx, iposition.y + slot1Beginsy, slotWidth, slotHeight);
            GUI.DrawTexture(slot1, InventorySlot);

            checkIfClick(slot1, e, 0);
            if (InventoryArmorContent[0, 0].ItemID != -1 && CanShowItem(slot1))
            {
                GUI.DrawTexture(slot1, InventoryArmorContent[0, 0].ItemIcon);
                if (InventoryArmorContent[0, 0].ItemMaxStack > 1)
                    GUI.Label(slot1, InventoryArmorNum[0, 0] + "");
            }
        }
        //Body armor slot
        {
            float slot2Beginsx = ToScaleWidth(-50, iposition, 2880);
            float slot2Beginsy = ToScaleHeight(687, iposition, 1594);

            Rect slot2 = new Rect(iposition.x + slot2Beginsx, iposition.y + slot2Beginsy, slotWidth, slotHeight);
            GUI.DrawTexture(slot2, InventorySlot);

            checkIfClick(slot2, e, 1);
            if (InventoryArmorContent[0, 1].ItemID != -1 && CanShowItem(slot2))
            {
                GUI.DrawTexture(slot2, InventoryArmorContent[0, 1].ItemIcon);
                if (InventoryArmorContent[0, 1].ItemMaxStack > 1)
                    GUI.Label(slot2, InventoryArmorNum[0, 1] + "");
            }
        }
        //Leggings slot
        {
            float slot3Beginsx = ToScaleWidth(-50, iposition, 2880);
            float slot3Beginsy = ToScaleHeight(951, iposition, 1594);

            Rect slot3 = new Rect(iposition.x + slot3Beginsx, iposition.y + slot3Beginsy, slotWidth, slotHeight);
            GUI.DrawTexture(slot3, InventorySlot);
            checkIfClick(slot3, e, 2);

            if (InventoryArmorContent[0, 2].ItemID != -1 && CanShowItem(slot3))
            {
                GUI.DrawTexture(slot3, InventoryArmorContent[0, 2].ItemIcon);
                if (InventoryArmorContent[0, 2].ItemMaxStack > 1)
                    GUI.Label(slot3, InventoryArmorNum[0, 2] + "");
            }
        }
        //Shoes slot
        {
            float slot4Beginsx = ToScaleWidth(85, iposition, 2880);
            float slot4Beginsy = ToScaleHeight(1231, iposition, 1594);

            Rect slot4 = new Rect(iposition.x + slot4Beginsx, iposition.y + slot4Beginsy, slotWidth, slotHeight);
            GUI.DrawTexture(slot4, InventorySlot);
            checkIfClick(slot4, e, 3);

            if (InventoryArmorContent[0, 3].ItemID != -1 && CanShowItem(slot4))
            {
                GUI.DrawTexture(slot4, InventoryArmorContent[0, 3].ItemIcon);
                if (InventoryArmorContent[0, 3].ItemMaxStack > 1)
                    GUI.Label(slot4, InventoryArmorNum[0, 3] + "");
            }
        }

    }

    public void checkIfClick(Rect slot1, Event e, int slotid)
    {
        if (slot1.Contains(e.mousePosition))
        {
            //GUI.DrawTexture(position, InventorySlotTex);

            //If clicks while holding item
            if (e.type == EventType.mouseDown && draggingItem && e.button == 0)
            {
                draggingItem = false;

                bool showNum = false;
                if (selecteditem.ItemMaxStack > 1)
                    showNum = true;
                itemsLeftToAnimate.Add(new ItemAnimation(lastKnownPosition, new Vector2(slot1.x, slot1.y), mouseOldPosition2, selecteditem.ItemIcon, selecteditemnum, showNum));

                mouseOldPosition = new Vector2(slot1.x, slot1.y);
                mouseOldPosition2 = new Vector2(slot1.x, slot1.y);

                MoveItemArmor(slotid); // change 0
                return;
            }

            //Pick item
            if (InventoryArmorContent[0, slotid].ItemID != -1)//Change 0
            {
                if (e.type == EventType.mouseDown && e.button == 0 && showInv)
                {
                    if (!draggingItem)
                    {
                        if (InventoryArmorContent[0, slotid].ItemID != -1)
                        {
                            mouseOldPosition = new Vector2(slot1.x, slot1.y);
                            mouseOldPosition2 = new Vector2(slot1.x, slot1.y);

                            //slotidd = slotID;

                            DragItemArmor(slotid); // change 0
                        }
                    }
                    else
                    {
                        //Move item from SlotX/Z to this one
                    }
                }
            }
        }
    }

    public void ItemAnimation(float width)
    {
        ItemAnimation itemToDelete = null;

        foreach(ItemAnimation item in itemsLeftToAnimate)
        {
            item.initialPos = Vector2.Lerp(item.initialPos, item.pos, 0.2f);
            item.initialPosNum = Vector2.Lerp(item.initialPosNum, item.pos, 0.2f);

            GUI.DrawTexture(new Rect(item.initialPos.x, item.initialPos.y, width, width), item.text);

            if (item.showNum == true)
                GUI.Label(new Rect(item.initialPosNum.x, item.initialPosNum.y, width, width), item.num + "");

            if (Vector2.Distance(item.initialPosNum, item.pos) < 1f)
                itemToDelete = item;
        }

        if (itemToDelete != null)
        {
            itemsLeftToAnimate.Remove(itemToDelete);
            itemToDelete = null;
        }
    }
    public bool CanShowItem(Rect pos)
    {
        foreach(ItemAnimation item in itemsLeftToAnimate)
        {
            if(item.pos.x == pos.x && item.pos.y == pos.y)
            {
                return false;
            }
        }
        return true;
    }

    public string InventoryToString(Item[,] inv, int[,] invNum)
    {
        string invS = "";

        for (int x = 0; x < NetworkManager.invWidth; x++)
        {
            for (int z = 0; z < NetworkManager.invHeight; z++)
            {
                int id = -2;
                int num = -2;
                try
                {
                    num = invNum[x, z];
                }
                catch
                {
                    num = 0;
                }
                try
                {
                    id = inv[x, z].ItemID;
                }
                catch
                {
                    id = -1;
                }

                invS += id + ":" + num + ";";
            }
        }
        return invS;
    }
    public void StringToInventory(string invS)
    {
        char[] splitchar = { ';' };
        string[] invSArgs = null;

        invSArgs = invS.Split(splitchar);

        int slotID = 0;
        for (int x = 0; x < (NetworkManager.invWidth / 2); x++)
        {
            for (int z = 0; z < NetworkManager.invHeight; z++)
            {
                char[] splitchar2 = { ':' };
                string[] itemDetails = null;
                itemDetails = invSArgs[slotID].Split(splitchar2);

                int ItemID = int.Parse(itemDetails[0]);
                int ItemStackNum = int.Parse(itemDetails[1]);

                if (ItemID == -1)
                {
                    InventoryContent[x, z] = new Item();
                    InventoryNum[x, z] = 0;
                }
                else
                {
                    InventoryContent[x, z] = Item.getItem(ItemID);
                    InventoryNum[x, z] = ItemStackNum;
                }
                slotID++;
            }
        }
    }
    public void StringToChest(string invS)
    {
        char[] splitchar = { ';' };
        string[] invSArgs = null;

        invSArgs = invS.Split(splitchar);

        int slotID = 0;
        for (int x = 4; x < NetworkManager.invWidth; x++)
        {
            for (int z = 0; z < NetworkManager.invHeight; z++)
            {
                char[] splitchar2 = { ':' };
                string[] itemDetails = null;
                itemDetails = invSArgs[slotID].Split(splitchar2);

                int ItemID = -1;
                int ItemStackNum = 0;

                ItemID = int.Parse(itemDetails[0]);
                ItemStackNum = int.Parse(itemDetails[1]);

                if (ItemID == -1)
                {
                    InventoryContent[x, z] = new Item();
                    InventoryNum[x, z] = 0;
                }
                else
                {
                    InventoryContent[x, z] = Item.getItem(ItemID);
                    InventoryNum[x, z] = ItemStackNum;
                }
                slotID++;
            }
        }
    }

    public string InventoryToString(Item[,] inv, int[,] invNum, bool sinep)
    {
        string invS = "";

        for (int x = 0; x < NetworkManager.invWidth; x++)
        {
                int id = -2;
                int num = -2;
                try
                {
                    num = invNum[x, 0];
                }
                catch
                {
                    num = 0;
                }
                try
                {
                    id = inv[x, 0].ItemID;
                }
                catch
                {
                    id = -1;
                }

                invS += id + ":" + num + ";";
        }
        return invS;
    }
    public void StringToInventory(string invS, bool sinep)
    {
        char[] splitchar = { ';' };
        string[] invSArgs = null;

        invSArgs = invS.Split(splitchar);

        for (int x = 0; x < NetworkManager.invWidth; x++)
        {
                char[] splitchar2 = { ':' };
                string[] itemDetails = null;
                itemDetails = invSArgs[x].Split(splitchar2);

                int ItemID = int.Parse(itemDetails[0]);
                int ItemStackNum = int.Parse(itemDetails[1]);

                if (ItemID == -1)
                {
                    InventoryHotbarContent[x, 0] = new Item();
                    InventoryHotbarNum[x, 0] = 0;
                }
                else
                {
                    InventoryHotbarContent[x, 0] = Item.getItem(ItemID);
                    InventoryHotbarNum[x, 0] = ItemStackNum;
                }
        }
    }

    public string ArmorToString(Item[,] inv, int[,] invNum)
    {
        string invS = "";

        for (int x = 0; x < 4; x++)
        {
            int id = -2;
            int num = -2;
            try
            {
                num = invNum[0, x];
            }
            catch
            {
                num = 0;
            }
            try
            {
                id = inv[0, x].ItemID;
            }
            catch
            {
                id = -1;
            }

            invS += id + ":" + num + ";";
        }
        return invS;
    }
    public void StringToArmor(string invS)
    {
        char[] splitchar = { ';' };
        string[] invSArgs = null;

        invSArgs = invS.Split(splitchar);

        for (int x = 0; x < 4; x++)
        {
            char[] splitchar2 = { ':' };
            string[] itemDetails = null;
            itemDetails = invSArgs[x].Split(splitchar2);

            int ItemID = int.Parse(itemDetails[0]);
            int ItemStackNum = int.Parse(itemDetails[1]);

            if (ItemID == -1)
            {
                InventoryArmorContent[0, x] = new Item();
                InventoryArmorNum[0, x] = 0;
            }
            else
            {
                InventoryArmorContent[0, x] = Item.getItem(ItemID);
                InventoryArmorNum[0, x] = ItemStackNum;
            }
        }
    }

    #endregion
    #region Crafting GUI & RPC's
    public bool showCrafting = false;
    Vector2 cSize;
    Vector2 cWidth;
    int selectedCat = -1;
    bool showItemsAbleToCraft = false;
    ItemCraft selectedCrafting;

    public List<ItemCraft> CraftingTools = new List<ItemCraft>();
    public List<ItemCraft> CraftingMisc = new List<ItemCraft>();
    public List<ItemCraft> CraftingWep = new List<ItemCraft>();
    public List<ItemCraft> CraftingStruc = new List<ItemCraft>();
    public void RegisterLists()
    {
        foreach(ItemCraft i in NetworkManager.Crafting)
        {
            if (i.ItemType == ItemCraft.ItemT.Tools)
                CraftingTools.Add(i);
            if (i.ItemType == ItemCraft.ItemT.Miscellaneous)
                CraftingMisc.Add(i);
            if (i.ItemType == ItemCraft.ItemT.Weapons)
                CraftingWep.Add(i);
            if (i.ItemType == ItemCraft.ItemT.Structures)
                CraftingStruc.Add(i);
        }
    }
    public void CraftingMenu(Event e)
    {
        float spacing = Screen.width / 30 + 15;
        GUI.BeginGroup(new Rect(cSize.x, 20, cWidth.x, cWidth.y));
        {
            GUI.DrawTexture(new Rect(0, 0, spacing, spacing * 8), craftingImages.WoodenTab);
            for (int i = 0; i < 7; i++)
            {
                Rect r = new Rect(spacing / 10, spacing + ((spacing / 1.2f) * i), spacing / 1.2f, spacing / 1.2f);

                if (showItemsAbleToCraft && selectedCat == i)
                {
                    GUI.DrawTexture(r, craftingImages.tabs_Click[i]);
                    Rect backGround = new Rect(spacing * 1.2f, spacing * 5.5f, spacing * 4.4f, spacing * 2.4f);
                    //Spliters=
                    {
                        float he = spacing / 3;

                        GUI.DrawTexture(new Rect(spacing, spacing / 2.5f, backGround.width + he, he), craftingImages.itemSplitBottom);

                        GUI.DrawTexture(new Rect(spacing, (spacing * 5.5f) - he / 1.5f, backGround.width + he, he), craftingImages.itemSplitTop);
                        GUI.DrawTexture(new Rect(spacing, (spacing * 5.5f) - he / 1.5f, backGround.width + he, he), craftingImages.itemSplitBottom);
                    }
                    //Background
                    {
                        //Crafting
                        GUI.DrawTexture(backGround, craftingImages.wodeen_Craft);
                        {
                            int id = 0;
                            float itemH = spacing;

                            if (GUI.Button(new Rect((backGround.x + spacing / 3) + (itemH * 1) - (spacing / 6), backGround.y + (itemH / 3f), itemH * 2f, itemH / 1.1f), craftingImages.craftButton, ""))
                            {
                                //Craft ITEM
                                if (Network.isServer)
                                    Server_CraftItem(selectedCrafting.Item.ItemName, Network.player);
                                else
                                    networkView.RPC("Server_CraftItem", RPCMode.Server, selectedCrafting.Item.ItemName, Network.player);

                                UpdateInventory();
                            }

                            for (int x = 0; x < 6; x++)
                            {
                                Rect rect = new Rect();
                                if(x == 0)
                                {
                                    rect = new Rect((backGround.x + spacing / 3) + (itemH * x) - (spacing / 6), backGround.y + ((spacing * 1.05f) + (spacing / 3) - (spacing / 6)) - itemH, itemH, itemH);
                                    GUI.DrawTexture(rect, craftingImages.itemSlot);
                                }
                                else if (x == 5)
                                {
                                    rect = new Rect((backGround.x + spacing / 3) + (itemH * 3) - (spacing / 6), backGround.y + ((spacing * 1.05f) + (spacing / 3) - (spacing / 6)) - itemH, itemH, itemH);
                                    GUI.DrawTexture(rect, craftingImages.itemSlot);
                                }
                                else
                                {
                                    rect = new Rect((backGround.x + spacing / 3) + (itemH * (x - 1)) - (spacing / 6), backGround.y + ((spacing * 1.05f) + (spacing / 3) - (spacing / 6)), itemH, itemH);
                                    GUI.DrawTexture(rect, craftingImages.itemSlot);
                                }

                                if (selectedCrafting != null)
                                {
                                    for (int info = 0; info < selectedCrafting.ItemsNeeded.Length; info++)
                                    {
                                        if (id < selectedCrafting.ItemsNeeded.Length)
                                        {
                                            GUI.DrawTexture(rect, selectedCrafting.ItemsNeeded[id].item.ItemIcon);
                                        }
                                    }
                                }

                                //Item info
                                GUI.DrawTexture(new Rect(rect.x + ((itemH / 2)), rect.y + (itemH / 2.5f) + ((itemH / 2) / 2), rect.width - (itemH / 2), rect.height / 3f), craftingImages.itemInfo);

                                if (selectedCrafting != null)
                                {
                                    for (int info = 0; info < selectedCrafting.ItemsNeeded.Length; info++)
                                    {
                                        if (id < selectedCrafting.ItemsNeeded.Length)
                                        {
                                            int itemCount = GetItemNum(selectedCrafting.ItemsNeeded[id].item);

                                            if (itemCount >= selectedCrafting.ItemsNeeded[id].itemNum)
                                            {
                                                GUI.color = Color.green;
                                            }
                                            else
                                            {
                                                GUI.color = Color.red;
                                            }

                                            GUI.Label(new Rect(rect.x + ((itemH / 2) / 2) * 2.5f, rect.y + (itemH / 2.5f) + ((itemH / 2) / 2), rect.width - (itemH / 2), rect.height / 3f), itemCount + "/" + selectedCrafting.ItemsNeeded[id].itemNum);

                                            GUI.color = Color.white;
                                        }
                                    }
                                }

                                id++;
                            }
                        }

                        //Crafting Item List
                        GUI.DrawTexture(new Rect(backGround.x, (spacing / 1.5f), backGround.width, spacing * 4.5f), craftingImages.wodeen_Craft);
                        {
                            int id = 0;
                            for(int x = 0; x < 4; x++)
                            {
                                for(int y = 0; y < 4; y++)
                                {
                                    float itemH = spacing;
                                    GUI.DrawTexture(new Rect((backGround.x + spacing / 3) + (itemH * x) - (spacing / 6), ((spacing / 1.5f) + (spacing / 3) - (spacing / 6)) + itemH * y, itemH, itemH), craftingImages.itemSlot);

                                    if(craftingImages.tabs_Names[selectedCat] == "Tools")
                                    {
                                        if (id < CraftingTools.Count)
                                        {
                                            Rect slo = new Rect((backGround.x + spacing / 3) + (itemH * x) - (spacing / 6), ((spacing / 1.5f) + (spacing / 3) - (spacing / 6)) + itemH * y, itemH, itemH);
                                            GUI.DrawTexture(slo, CraftingTools[id].Item.ItemIcon);

                                            if(slo.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0)
                                            {
                                                selectedCrafting = CraftingTools[id];
                                            }
                                        }
                                    }
                                    else if (craftingImages.tabs_Names[selectedCat] == "Miscellaneous ")
                                    {
                                        if (id < CraftingMisc.Count)
                                        {
                                            Rect slo = new Rect((backGround.x + spacing / 3) + (itemH * x) - (spacing / 6), ((spacing / 1.5f) + (spacing / 3) - (spacing / 6)) + itemH * y, itemH, itemH);
                                            GUI.DrawTexture(slo, CraftingMisc[id].Item.ItemIcon);

                                            if (slo.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0)
                                            {
                                                selectedCrafting = CraftingMisc[id];
                                            }
                                        }
                                    }
                                    else if (craftingImages.tabs_Names[selectedCat] == "Weapons")
                                    {
                                        if (id < CraftingWep.Count)
                                        {
                                            Rect slo = new Rect((backGround.x + spacing / 3) + (itemH * x) - (spacing / 6), ((spacing / 1.5f) + (spacing / 3) - (spacing / 6)) + itemH * y, itemH, itemH);
                                            GUI.DrawTexture(slo, CraftingWep[id].Item.ItemIcon);

                                            if (slo.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0)
                                            {
                                                selectedCrafting = CraftingWep[id];
                                            }
                                        }
                                    }
                                    else if (craftingImages.tabs_Names[selectedCat] == "Structures")
                                    {
                                        if (id < CraftingStruc.Count)
                                        {
                                            Rect slo = new Rect((backGround.x + spacing / 3) + (itemH * x) - (spacing / 6), ((spacing / 1.5f) + (spacing / 3) - (spacing / 6)) + itemH * y, itemH, itemH);
                                            GUI.DrawTexture(slo, CraftingStruc[id].Item.ItemIcon);

                                            if (slo.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0)
                                            {
                                                selectedCrafting = CraftingStruc[id];
                                            }
                                        }
                                    }

                                    id++;
                                }
                            }
                        }
                    }
                }

                if (r.Contains(e.mousePosition))
                {
                    if (showItemsAbleToCraft && selectedCat == i)
                    {
                        if (e.button == 0 && e.type == EventType.mouseDown)
                        {
                            showItemsAbleToCraft = !showItemsAbleToCraft;
                            selectedCrafting = null;
                        }
                    }
                    else if (!showItemsAbleToCraft)
                    {
                        selectedCat = i;
                        if (e.button == 0 && e.type == EventType.mouseDown)
                        {
                            showItemsAbleToCraft = true;
                        }
                    }
                }

                if (selectedCat == i)
                {
                    if (!showItemsAbleToCraft)
                    {
                        GUILayout.BeginArea(new Rect(r.x + spacing, r.y, r.width + 50, r.height / 2), "", "box");
                        {
                            GUILayout.Label(craftingImages.tabs_Names[i]);
                        }
                        GUILayout.EndArea();
                    }
                    GUI.DrawTexture(r, craftingImages.tabs_Selected[i]);
                }
                else
                {
                    GUI.DrawTexture(r, craftingImages.tabs[i]);
                }
            }
        }
        GUI.EndGroup();

        if (showCrafting)
        {
            cSize = Vector2.Lerp(cSize, new Vector2(20, 0), 0.1f);
            if (showItemsAbleToCraft)
                cWidth = Vector2.Lerp(cWidth, new Vector2(spacing * 8, spacing * 8), 0.1f);
            else
                cWidth = Vector2.Lerp(cWidth, new Vector2(spacing * 3.5f, spacing * 8), 0.1f);
        }
        else
        {
            cSize = Vector2.Lerp(cSize, new Vector2(-100, 0), 0.1f);
            showItemsAbleToCraft = false;
            selectedCat = -1;
            cWidth = Vector2.Lerp(cWidth, new Vector2(spacing, spacing * 8), 0.1f);
        }
    }
    public int GetItemNum(Item i)
    {
        int count = 0;

        for (int x = 0; x < NetworkManager.invWidth; x++)
        {
            for (int y = 0; y < NetworkManager.invHeight; y++)
            {
                if (InventoryContent[x, y] == null) continue;
                if (InventoryContent[x,y].ItemID == i.ItemID)
                {
                    count += InventoryNum[x,y];
                }
            }
        }
        //Hotbar
        for (int x = 0; x < 8; x++)
        {
            if (InventoryHotbarContent[x, 0] == null) continue;
            if (InventoryHotbarContent[x, 0].ItemID == i.ItemID)
            {
                count += InventoryHotbarNum[x, 0];
            }
        }

        return count;
    }

    [RPC]
    public void Server_CraftItem(string Itemname, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);
        bool hasItemsNeeded = true;

        foreach(ItemCraft ic in NetworkManager.Crafting)
        {
            if(ic.Item.ItemName == Itemname)
            {
                for (int i = 0; i < ic.ItemsNeeded.Length; i++)
                {
                    if (pl.GetItemNum(ic.ItemsNeeded[i].item) < ic.ItemsNeeded[i].itemNum)
                    {
                        hasItemsNeeded = false;
                    }
                }
            }
        }

        if(hasItemsNeeded)
        {
            Item it = new Item();
            int num = 0;
            foreach (ItemCraft ic in NetworkManager.Crafting)
            {
                if (ic.Item.ItemName == Itemname)
                {
                    it = ic.Item;
                    num = ic.itemCount;

                    for (int i = 0; i < ic.ItemsNeeded.Length; i++)
                    {
                        pl.RemoveItem(ic.ItemsNeeded[i].item, ic.ItemsNeeded[i].itemNum);
                    }
                }
            }
            pl.AddItem(it.ItemName, num);
        }
    }
    #endregion
    #region RPC's
    [RPC]
    public void Server_TeleportPlayer(NetworkPlayer pl, NetworkPlayer plToTP, Vector3 pos)
    {
        if(pl == Network.player)
        {
            MPPlayer.GetPlayer(plToTP).PlayerTransform.position = pos;
            posOld = MPPlayer.GetPlayer(plToTP).PlayerTransform.position;
            networkView.RPC("Client_Teleport", plToTP, pos);
        }
        else
        {
            Network.CloseConnection(pl, true);
        }
    }
    [RPC]
    public void Client_Teleport(Vector3 pos)
    {
        transform.position = pos;
    }
    //Update inventory RPC's
    [RPC]
    public void Server_UpdateInventory(NetworkPlayer pl)
    {
        string invS = InventoryToString(MPPlayer.GetPlayer(pl).PlayerInventory, MPPlayer.GetPlayer(pl).PlayerInventoryNum);
        string invSH = InventoryToString(MPPlayer.GetPlayer(pl).PlayerInventory_Hotbar, MPPlayer.GetPlayer(pl).PlayerInventoryNum_Hotbar, true);

        string invA = ArmorToString(MPPlayer.GetPlayer(pl).InventoryArmorContent, MPPlayer.GetPlayer(pl).InventoryArmorNum);

        if(pl == Network.player)
        {
            Client_UpdateInventory(invS, invSH, invA);
        }
        else
        {
            networkView.RPC("Client_UpdateInventory", pl, invS, invSH, invA);
        }
    }
    [RPC]
    public void Client_UpdateInventory(string invS, string invH, string invA)
    {
        StringToInventory(invS);
        StringToInventory(invH, true);
        StringToArmor(invA);
    }
    //Move item RPC's
    [RPC]
    public void Server_MoveItem(int slotid, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);
        Item itemToPickup = pl.getItem(slotid).item; int itemToPickupNum = pl.getItem(slotid).itemNum;

        if (pl.isChestSlot(slotid) && pl.PlayerInsideChest == false)
        {
            draggingItem = true;
            if (pln == Network.player)
                Client_MoveItem(draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, true);
            else
                networkView.RPC("Client_MoveItem", pln, draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, true);
            return;
        }

        draggingItem = false;

        if (itemToPickup.ItemID == pl.PlayerItemDragging.ItemID)
        {
            if(itemToPickupNum + pl.PlayerItemDraggingNum <= itemToPickup.ItemMaxStack)
            {
                pl.SetItem(slotid, pl.PlayerItemDragging, itemToPickupNum + pl.PlayerItemDraggingNum);

                if (pln == Network.player)
                    Client_MoveItem(draggingItem, itemToPickup.ItemID, pl.PlayerItemDraggingNum, false);
                else
                    networkView.RPC("Client_MoveItem", pln, draggingItem, itemToPickup.ItemID, pl.PlayerItemDraggingNum, false);

                pl.PlayerItemDragging = new Item();
                pl.PlayerItemDraggingNum = 0;

                return;
            }
            else
            {
                int rest = 0;

                rest = (itemToPickupNum + pl.PlayerItemDraggingNum) - itemToPickup.ItemMaxStack;
                itemToPickupNum = itemToPickup.ItemMaxStack;

                pl.SetItem(slotid, pl.PlayerItemDragging, itemToPickup.ItemMaxStack);

                if (rest > 0)
                {
                    pl.SetItemDragging(itemToPickup, rest);
                    draggingItem = true;
                }
            }
        }
        else
        {
            pl.SetItem(slotid, pl.PlayerItemDragging, pl.PlayerItemDraggingNum);
            pl.SetItemDragging(itemToPickup, itemToPickupNum);

            if (itemToPickup.ItemID >= 0)
                draggingItem = true;
        }

        if (pln == Network.player)
            Client_MoveItem(draggingItem, itemToPickup.ItemID, pl.PlayerItemDraggingNum, false);
        else
            networkView.RPC("Client_MoveItem", pln, draggingItem, itemToPickup.ItemID, pl.PlayerItemDraggingNum, false);
    }
    [RPC]
    public void Client_MoveItem(bool draggingItem, int itemid, int num, bool clearAnim)
    {
        if(clearAnim)
            itemsLeftToAnimate.Clear();
        this.draggingItem = draggingItem;
        selecteditem = Item.getItem(itemid);
        selecteditemnum = num;
        UpdateInventory();
    }
    //Dragging Item RPC's
    [RPC]
    public void Server_DragItem(int slotid, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);

        if (pl.isChestSlot(slotid) && pl.PlayerInsideChest == false) return;

        Item slotItem = pl.getItem(slotid).item; int slotNum = pl.getItem(slotid).itemNum;
        pl.SetItemDragging(slotItem, slotNum);

        pl.SetItem(slotid, new Item(), 0);

        if (pln == Network.player)
            Client_DragItem(slotid, slotItem.ItemID, slotNum);
        else
            networkView.RPC("Client_DragItem", pln, slotid, slotItem.ItemID, slotNum);
    }
    [RPC]
    public void Client_DragItem(int slotid, int itemID, int itemnum)
    {
        this.draggingItem = true;

        selecteditem = Item.getItem(itemID);
        selecteditemnum = itemnum;
        slotidd = slotid;
        
        UpdateInventory();
    }
    //Slipt item rpc's
    [RPC]
    public void Server_SplitItem(int slotid, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);

        if (pl.isChestSlot(slotid) && pl.PlayerInsideChest == false) return;

        Item slotItem = pl.getItem(slotid).item; int slotNum = pl.getItem(slotid).itemNum;

        if (slotNum > 1 && draggingItem == false)
        {
            int slotnumberhalf = slotNum / 2;
            int slotRest = slotNum - slotnumberhalf;

            pl.SetItem(slotid, slotItem, slotRest);
            pl.SetItemDragging(slotItem, slotnumberhalf);

            if (pln == Network.player)
                Client_SplitItem(slotid, slotItem.ItemID, slotnumberhalf);
            else
                networkView.RPC("Client_SplitItem", pln, slotid, slotItem.ItemID, slotnumberhalf);
        }
        else
        {

        }
    }
    [RPC]
    public void Client_SplitItem(int slotid, int itemID, int slotnum)
    {
        draggingItem = true;

        selecteditem = Item.getItem(itemID);
        selecteditemnum = slotnum;
        slotidd = slotid;

        UpdateInventory();
    }
    //Hot bar move item
    [RPC]
    public void Server_MoveHotbarItem(int slotid, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);
        Item itemToPickup = pl.getHotBarItem(slotid).item; int itemToPickupNum = pl.getHotBarItem(slotid).itemNum;

        draggingItem = false;

        if (itemToPickup == null)
        {
            itemToPickup = new Item();
        }

        if (itemToPickup.ItemID == pl.PlayerItemDragging.ItemID)
        {
            if (pl.SelectedItem.slot == slotid)
            {
                pl.SelectedItem = new SelectedItem();
                    networkView.RPC("Client_DisableAllItems",RPCMode.All, pln);
            }
            if (itemToPickupNum + pl.PlayerItemDraggingNum <= itemToPickup.ItemMaxStack)
            {
                pl.SetHotbarItem(slotid, pl.PlayerItemDragging, itemToPickupNum + pl.PlayerItemDraggingNum);

                if (pln == Network.player)
                    Client_MoveHotbarItem(draggingItem, itemToPickup.ItemID, pl.PlayerItemDraggingNum);
                else
                    networkView.RPC("Client_MoveHotbarItem", pln, draggingItem, itemToPickup.ItemID);

                pl.PlayerItemDragging = new Item();
                pl.PlayerItemDraggingNum = 0;

                return;
            }
            else
            {
                int rest = 0;

                rest = (itemToPickupNum + pl.PlayerItemDraggingNum) - itemToPickup.ItemMaxStack;
                itemToPickupNum = itemToPickup.ItemMaxStack;

                pl.SetHotbarItem(slotid, pl.PlayerItemDragging, itemToPickup.ItemMaxStack);

                if (rest > 0)
                {
                    pl.SetItemDragging(itemToPickup, rest);
                    draggingItem = true;
                }
            }
        }
        else
        {
            if (pl.SelectedItem.slot == slotid)
            {
                pl.SelectedItem = new SelectedItem();
                networkView.RPC("Client_DisableAllItems", RPCMode.All, pln);
            }

            pl.SetHotbarItem(slotid, pl.PlayerItemDragging, pl.PlayerItemDraggingNum);
            pl.SetItemDragging(itemToPickup, itemToPickupNum);

            if (itemToPickup.ItemID >= 0)
                draggingItem = true;
        }

        if (pln == Network.player)
            Client_MoveHotbarItem(draggingItem, itemToPickup.ItemID, pl.PlayerItemDraggingNum);
        else
            networkView.RPC("Client_MoveHotbarItem", pln, draggingItem, itemToPickup.ItemID, pl.PlayerItemDraggingNum);
    }
    [RPC]
    public void Client_MoveHotbarItem(bool draggingItem, int itemid, int num)
    {
        this.draggingItem = draggingItem;
        selecteditem = Item.getItem(itemid);
        selecteditemnum = num;
        UpdateInventory();
    }
    //horbar drag item rpc's
    [RPC]
    public void Server_DragHotbarItem(int slotid, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);

        Item slotItem = pl.getHotBarItem(slotid).item; int slotNum = pl.getHotBarItem(slotid).itemNum;
        pl.SetItemDragging(slotItem, slotNum);

        if (pl.SelectedItem.slot == slotid)
        {
            pl.SelectedItem = new SelectedItem();
                networkView.RPC("Client_DisableAllItems",RPCMode.All, pln);
        }

        pl.SetHotbarItem(slotid, new Item(), 0);

        if (pln == Network.player)
            Client_DragHotbarItem(slotid, slotItem.ItemID, slotNum);
        else
            networkView.RPC("Client_DragHotbarItem", pln, slotid, slotItem.ItemID, slotNum);
    }
    [RPC]
    public void Client_DragHotbarItem(int slotid, int itemID, int itemnum)
    {
        this.draggingItem = true;

        selecteditem = Item.getItem(itemID);
        selecteditemnum = itemnum;
        slotidd = slotid;

        UpdateInventory();
    }
    //RPC slipt item hotbar
    [RPC]
    public void Server_SplitHotbarItem(int slotid, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);

        Item slotItem = pl.getHotBarItem(slotid).item; int slotNum = pl.getHotBarItem(slotid).itemNum;

        if (slotNum > 1 && draggingItem == false)
        {
            int slotnumberhalf = slotNum / 2;
            int slotRest = slotNum - slotnumberhalf;

            pl.SetHotbarItem(slotid, slotItem, slotRest);
            pl.SetItemDragging(slotItem, slotnumberhalf);

            if (pln == Network.player)
                Client_SplitItem(slotid, slotItem.ItemID, slotnumberhalf);
            else
                networkView.RPC("Client_SplitItem", pln, slotid, slotItem.ItemID, slotnumberhalf);
        }
        else
        {

        }
    }
    [RPC]
    public void Client_SplitHotbarItem(int slotid, int itemID, int slotnum)
    {
        draggingItem = true;

        selecteditem = Item.getItem(itemID);
        selecteditemnum = slotnum;
        slotidd = slotid;

        UpdateInventory();
    }
    //RPC to move items inside CHESTS
    [RPC]
    public void Server_MoveChestItem(int id, int slot, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);
        ChestContainer chestInv = NetworkManager.Chests[id];

        if (chestInv.playerInsideChest != pln || chestInv.chestOpen == false)
            return;

        draggingItem = true;

        if (Vector3.Distance(chestInv.transform.position, pl.PlayerTransform.position) > 10)
        {
            if (pln == Network.player)
            {
                Client_OutofChest();
            }
            else
            {
                networkView.RPC("Client_OutofChest", pln);
            }

            if (pln == Network.player)
                Client_UpdateChestItems(draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, false, "-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;");
            else
                networkView.RPC("Client_UpdateChestItems", pln, draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, false, "-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;-1:-1;");

            return;
        }

        Item itemToPickup = chestInv.getItem(slot).item;
        int itemToPickupNum = chestInv.getItem(slot).itemNum;

        if (itemToPickup == null)
        {
            itemToPickup = new Item();
        }

        /*if(pl.PlayerItemDragging.ItemID == -1)
        {
            pl.PlayerItemDragging = chestInv.getItem(slot).item;
            pl.PlayerItemDraggingNum = chestInv.getItem(slot).itemNum;

            chestInv.SetItem(slot, new Item(), 0);
        }
        else
        {

        }*//*

        draggingItem = false;

        if (itemToPickup.ItemID == pl.PlayerItemDragging.ItemID)
        {
            if (itemToPickupNum + pl.PlayerItemDraggingNum <= itemToPickup.ItemMaxStack)
            {
                chestInv.SetItem(slot, pl.PlayerItemDragging, itemToPickupNum + pl.PlayerItemDraggingNum);

                string chestInvv = NetworkManager.Chests[id].ChestInvToString();

                pl.PlayerItemDragging = new Item();
                pl.PlayerItemDraggingNum = 0;

                if (pln == Network.player)
                    Client_UpdateChestItems(draggingItem, itemToPickup.ItemID, pl.PlayerItemDraggingNum, false, chestInvv);
                else
                    networkView.RPC("Client_UpdateChestItems", pln, draggingItem, itemToPickup.ItemID, pl.PlayerItemDraggingNum, false, chestInvv);

                return;
            }
            else
            {
                int rest = 0;

                rest = (itemToPickupNum + pl.PlayerItemDraggingNum) - itemToPickup.ItemMaxStack;
                itemToPickupNum = itemToPickup.ItemMaxStack;

                chestInv.SetItem(slot, pl.PlayerItemDragging, itemToPickup.ItemMaxStack);

                if (rest > 0)
                {
                    pl.SetItemDragging(itemToPickup, rest);
                    draggingItem = true;
                }
            }
        }
        else
        {
            chestInv.SetItem(slot, pl.PlayerItemDragging, pl.PlayerItemDraggingNum);
            pl.SetItemDragging(itemToPickup, itemToPickupNum);

            if (itemToPickup.ItemID >= 0)
                draggingItem = true;
        }

        string chestInvvv = NetworkManager.Chests[id].ChestInvToString();
        if(Network.player == pln)
        {
            Client_UpdateChestItems(draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, false, chestInvvv);
        }
        else
        {
            networkView.RPC("Client_UpdateChestItems", pln, draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, false, chestInvvv);
        }
    }
    [RPC]
    public void Client_UpdateChestItems(bool draggingItem, int itemid, int num, bool clearAnim, string chestItems)
    {
        if (clearAnim)
            itemsLeftToAnimate.Clear();

        this.draggingItem = draggingItem;
        selecteditem = Item.getItem(itemid);
        selecteditemnum = num;

        UpdateInventory();
        StringToChest(chestItems);
    }
    //chest drag item rpc's
    [RPC]
    public void Server_DragChestItem(int id, int slot, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);
        ChestContainer chestInv = NetworkManager.Chests[id];

        if (chestInv.playerInsideChest != pln || chestInv.chestOpen == false)
            return;

        if (Vector3.Distance(chestInv.transform.position, pl.PlayerTransform.position) > 10)
        {
            if (pln == Network.player)
            {
                Client_OutofChest();
            }
            else
            {
                networkView.RPC("Client_OutofChest", pln);
            }
            return;
        }

        Item slotItem = chestInv.getItem(slot).item;
        int slotNum = chestInv.getItem(slot).itemNum;

        pl.SetItemDragging(slotItem, slotNum);

        chestInv.SetItem(slot, new Item(), 0);

        string chestInvv = NetworkManager.Chests[id].ChestInvToString();
        if (pln == Network.player)
            Client_DragChestItem(slot, slotItem.ItemID, slotNum, chestInvv);
        else
            networkView.RPC("Client_DragChestItem", pln, slot, slotItem.ItemID, slotNum, chestInvv);
    }
    [RPC]
    public void Client_DragChestItem(int slotid, int itemID, int itemnum, string chestItems)
    {
        this.draggingItem = true;

        selecteditem = Item.getItem(itemID);
        selecteditemnum = itemnum;
        slotidd = slotid;

        UpdateInventory();
        StringToChest(chestItems);
    }
    //RPC slipt item chest
    [RPC]
    public void Server_SplitChestItem(int id, int slotid, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);
        ChestContainer chestInv = NetworkManager.Chests[id];

        if (chestInv.playerInsideChest != pln || chestInv.chestOpen == false)
            return;

        Item slotItem = chestInv.getItem(slotid).item;
        int slotNum = chestInv.getItem(slotid).itemNum;

        if (slotNum > 1 && draggingItem == false)
        {
            int slotnumberhalf = slotNum / 2;
            int slotRest = slotNum - slotnumberhalf;

            chestInv.SetItem(slotid, slotItem, slotRest);
            pl.SetItemDragging(slotItem, slotnumberhalf);

            string chestInvv = NetworkManager.Chests[id].ChestInvToString();
            if (pln == Network.player)
                Client_SplitChestItem(slotid, slotItem.ItemID, slotnumberhalf, chestInvv);
            else
                networkView.RPC("Client_SplitChestItem", pln, slotid, slotItem.ItemID, slotnumberhalf, chestInvv);
        }
        else
        {

        }
    }
    [RPC]
    public void Client_SplitChestItem(int slotid, int itemID, int slotnum, string chestItems)
    {
        draggingItem = true;

        selecteditem = Item.getItem(itemID);
        selecteditemnum = slotnum;
        slotidd = slotid;

        UpdateInventory();
        StringToChest(chestItems);
    }
    //RPC to move AMOR
    [RPC]
    public void Server_MoveArmorItem(int slot, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);

        Item itemToPickup = pl.getArmorItem(slot).item;
        int itemToPickupNum = pl.getArmorItem(slot).itemNum;

        if (itemToPickup == null)
        {
            itemToPickup = new Item();
            itemToPickup.ItemType = Item.ItemTypeE.None;
        }

        draggingItem = true;
        if (slot == 0)
        {
            if (pl.PlayerItemDragging.ItemType != Item.ItemTypeE.HeadArmor)
            {
                string armorString = ArmorToString(pl.InventoryArmorContent, pl.InventoryArmorNum);
                if (pln == Network.player)
                    Client_UpdateArmorItems(draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, true, armorString);
                else
                    networkView.RPC("Client_UpdateArmorItems", pln, draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, true, armorString);
                return;
            }
        }
        if (slot == 1)
        {
            if (pl.PlayerItemDragging.ItemType != Item.ItemTypeE.BodyArmor)
            {
                string armorString = ArmorToString(pl.InventoryArmorContent, pl.InventoryArmorNum);
                if (pln == Network.player)
                    Client_UpdateArmorItems(draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, true, armorString);
                else
                    networkView.RPC("Client_UpdateArmorItems", pln, draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, true, armorString);

                return;
            }
        }
        if (slot == 2)
        {
            if (pl.PlayerItemDragging.ItemType != Item.ItemTypeE.LegsArmor)
            {
                string armorString = ArmorToString(pl.InventoryArmorContent, pl.InventoryArmorNum);
                if (pln == Network.player)
                    Client_UpdateArmorItems(draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, true, armorString);
                else
                    networkView.RPC("Client_UpdateArmorItems", pln, draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, true, armorString);

                return;
            }
        }
        if (slot == 3)
        {
            if (pl.PlayerItemDragging.ItemType != Item.ItemTypeE.ShooesArmor)
            {
                string armorString = ArmorToString(pl.InventoryArmorContent, pl.InventoryArmorNum);
                if (pln == Network.player)
                    Client_UpdateArmorItems(draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, true, armorString);
                else
                    networkView.RPC("Client_UpdateArmorItems", pln, draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, true, armorString);

                return;
            }
        }

        draggingItem = false;

        if (itemToPickup.ItemID == pl.PlayerItemDragging.ItemID)
        {
            if (itemToPickupNum + pl.PlayerItemDraggingNum <= itemToPickup.ItemMaxStack)
            {
                pl.SetArmorItem(slot, pl.PlayerItemDragging, itemToPickupNum + pl.PlayerItemDraggingNum);

                //string chestInvv = NetworkManager.Chests[id].ChestInvToString();
                string armorString = ArmorToString(pl.InventoryArmorContent, pl.InventoryArmorNum);

                pl.PlayerItemDragging = new Item();
                pl.PlayerItemDraggingNum = 0;

                if (pln == Network.player)
                    Client_UpdateArmorItems(draggingItem, itemToPickup.ItemID, pl.PlayerItemDraggingNum, false, armorString);
                else
                    networkView.RPC("Client_UpdateArmorItems", pln, draggingItem, itemToPickup.ItemID, pl.PlayerItemDraggingNum, false, armorString);

                return;
            }
            else
            {
                int rest = 0;

                rest = (itemToPickupNum + pl.PlayerItemDraggingNum) - itemToPickup.ItemMaxStack;
                itemToPickupNum = itemToPickup.ItemMaxStack;

                pl.SetArmorItem(slot, pl.PlayerItemDragging, itemToPickup.ItemMaxStack);

                if (rest > 0)
                {
                    pl.SetItemDragging(itemToPickup, rest);
                    draggingItem = true;
                }
            }
        }
        else
        {
            pl.SetArmorItem(slot, pl.PlayerItemDragging, pl.PlayerItemDraggingNum);
            pl.SetItemDragging(itemToPickup, itemToPickupNum);

            if (itemToPickup.ItemID >= 0)
                draggingItem = true;
        }

        string armorStringg = ArmorToString(pl.InventoryArmorContent, pl.InventoryArmorNum);
        if (Network.player == pln)
        {
            Client_UpdateArmorItems(draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, false, armorStringg);
        }
        else
        {
            networkView.RPC("Client_UpdateArmorItems", pln, draggingItem, pl.PlayerItemDragging.ItemID, pl.PlayerItemDraggingNum, false, armorStringg);
        }
    }
    [RPC]
    public void Client_UpdateArmorItems(bool draggingItem, int itemid, int num, bool clearAnim, string chestItems)
    {
        if (clearAnim)
            itemsLeftToAnimate.Clear();

        this.draggingItem = draggingItem;
        selecteditem = Item.getItem(itemid);
        selecteditemnum = num;

        UpdateInventory();
        StringToArmor(chestItems);
    }
    //chest drag AMOR rpc's
    [RPC]
    public void Server_DragArmorItem(int slot, NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);

        Item slotItem = pl.getArmorItem(slot).item;
        int slotNum = pl.getArmorItem(slot).itemNum;

        pl.SetItemDragging(slotItem, slotNum);
        pl.SetArmorItem(slot, new Item(), 0);

        string armorString = ArmorToString(pl.InventoryArmorContent, pl.InventoryArmorNum);
        if (pln == Network.player)
            Client_DragArmorItem(slotItem.ItemID, slotNum, armorString);
        else
            networkView.RPC("Client_DragArmorItem", pln, slotItem.ItemID, slotNum, armorString);
    }
    [RPC]
    public void Client_DragArmorItem(int itemID, int itemnum, string chestItems)
    {
        this.draggingItem = true;

        selecteditem = Item.getItem(itemID);
        selecteditemnum = itemnum;

        UpdateInventory();
        StringToArmor(chestItems);
    }

    //status
    [RPC]
    public void Server_UpdateStatus(NetworkPlayer pln)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);

        float health = pl.PlayerHealth;
        float stamina = pl.PlayerStamina;
        float hunger = pl.PlayerHunger;

        if(Network.player == pln)
        {
            Client_UpdateStatus(health, stamina, hunger);
        }
        else
        {
            networkView.RPC("Client_UpdateStatus", pln, health, stamina, hunger);
        }
    }
    [RPC]
    public void Client_UpdateStatus(float h, float st, float hu)
    {
        Health = h;
        Stamina = st;
        hu = Hunger;
    }

    /*//*/void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {/*
        if (stream.isWriting)
        {
            Vector3 tp = transform.position;
            Quaternion tr = transform.rotation;
            //bool sprinting = controller.isSpritning;

            bool r = running;
            bool w = walking;
            bool s = slashing;
            bool j = jumping;
            bool d = defending;

            stream.Serialize(ref tp);
            stream.Serialize(ref tr);
            stream.Serialize(ref sprinting);

            stream.Serialize(ref r);
            stream.Serialize(ref w);
            stream.Serialize(ref s);
            stream.Serialize(ref j);
            stream.Serialize(ref d);
        }
        else
        {
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
            stream.Serialize(ref sprinting);

            stream.Serialize(ref running);
            stream.Serialize(ref walking);
            stream.Serialize(ref slashing);
            stream.Serialize(ref jumping);
            stream.Serialize(ref defending);
        }
    }*//*
    Vector3 pos;
    Quaternion rot;
    bool sprinting;
    Vector3 posOld;
    Quaternion rotOld;
    void UpdateMovement()
    {
        if (!networkView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, pos, 0.5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.5f);

            if(Network.isServer)
            {
                MPPlayer.GetPlayer(networkView.owner).PlayerSprinting = sprinting;
            }

            anim.SetBool("Walk", walking);
            anim.SetBool("Run", running);
            anim.SetBool("Slash", slashing);
            anim.SetBool("Jump", jumping);
            anim.SetBool("Defend", defending);
        }
    }

    public void UpdateInventory()
    {
        if (Network.isServer)
        {
            Server_UpdateInventory(Network.player);
        }
        else
        {
            networkView.RPC("Server_UpdateInventory", RPCMode.Server, Network.player);
        }
    }
    public void MoveItem(int slotID_A)
    {
        if(Network.isServer)
            Server_MoveItem(slotID_A, Network.player);
        else
            networkView.RPC("Server_MoveItem", RPCMode.Server, slotID_A, Network.player);
    }
    public void DragItem(int slotID_A)
    {
        if (Network.isServer)
            Server_DragItem(slotID_A, Network.player);
        else
            networkView.RPC("Server_DragItem", RPCMode.Server, slotID_A, Network.player);
    }
    public void SplitItem(int slotid)
    {
        if (Network.isServer)
            Server_SplitItem(slotid, Network.player);
        else
            networkView.RPC("Server_SplitItem", RPCMode.Server, slotid, Network.player);
    }

    public void MoveHotbarItem(int slotid)
    {
        if (Network.isServer)
            Server_MoveHotbarItem(slotid, Network.player);
        else
            networkView.RPC("Server_MoveHotbarItem", RPCMode.Server, slotid, Network.player);
    }
    public void DragHotbarItem(int slotID_A)
    {
        if (Network.isServer)
            Server_DragHotbarItem(slotID_A, Network.player);
        else
            networkView.RPC("Server_DragHotbarItem", RPCMode.Server, slotID_A, Network.player);
    }
    public void SplitHotbarItem(int slotid)
    {
        if (Network.isServer)
            Server_SplitHotbarItem(slotid, Network.player);
        else
            networkView.RPC("Server_SplitHotbarItem", RPCMode.Server, slotid, Network.player);
    }

    public void MoveItemChest(int id, int slot)
    {
        if (Network.isServer)
            Server_MoveChestItem(id, slot, Network.player);
        else
            networkView.RPC("Server_MoveChestItem", RPCMode.Server, id, slot, Network.player);
    }
    public void DragItemChest(int id, int slot)
    {
        if (Network.isServer)
            Server_DragChestItem(id, slot, Network.player);
        else
            networkView.RPC("Server_DragChestItem", RPCMode.Server, id, slot, Network.player);
    }
    public void SplitItemChest(int id, int slot)
    {
        if (Network.isServer)
            Server_SplitChestItem(id, slot, Network.player);
        else
            networkView.RPC("Split_MoveChestItem", RPCMode.Server, id, slot, Network.player);
    }

    public void MoveItemArmor(int slot)
    {
        if (Network.isServer)
            Server_MoveArmorItem(slot, Network.player);
        else
            networkView.RPC("Server_MoveArmorItem", RPCMode.Server, slot, Network.player);
    }
    public void DragItemArmor(int slot)
    {
        if (Network.isServer)
            Server_DragArmorItem(slot, Network.player);
        else
            networkView.RPC("Server_DragArmorItem", RPCMode.Server, slot, Network.player);
    }

    public void UpdatePlayerStatus()
    {
        if (Network.isServer)
            Server_UpdateStatus(Network.player);
        else
            networkView.RPC("Server_UpdateStatus", RPCMode.Server, Network.player);
    }
    public void Teleport(NetworkPlayer pl, Vector3 pos)
    {
        if (Network.isServer)
            Server_TeleportPlayer(Network.player, pl, pos);
        else
            networkView.RPC("Server_TeleportPlayer", RPCMode.Server, Network.player, pl, pos);
    }

    public float ToScaleWidth(float originalPixels, Rect currentPixels, int orignialWidth)
    {
        return (originalPixels * currentPixels.width) / orignialWidth;
    }
    public float ToScaleHeight(float originalPixels, Rect currentPixels, int orignialHeight)
    {
        return (originalPixels * currentPixels.height) / orignialHeight;
    }

    public enum GUIState
    {
        None,
        Chest,
        Door
    }
    #endregion
    #region Player take damage/give damage & status management

    public float HungerLossAmmount = 2f;
    public float TimeToConsumeHunger = 8f;
    
    private float timepast = 0f;
    private float timepast2;
    private float sprintingLast = 0f;
    //Need a better system..
    public void HungerConsume()
    {
        if (Time.time > sprintingLast)
        {
            foreach (MPPlayer pl in NetworkManager.PlayerList)
            {
                if (pl.PlayerSprinting)
                {
                    pl.PlayerStamina -= 1f * 10;
                }
                else if (!pl.PlayerSprinting && pl.PlayerStamina < 100)
                {
                    pl.PlayerStamina += 0.5f * 10;
                }

            }
            sprintingLast = Time.time + 0.5f;
        }
        if (Time.time > timepast)
        {
            foreach (MPPlayer pl in NetworkManager.PlayerList)
            {
                if (Network.player == pl.PlayerNetwork)
                {
                    Client_UpdateStatus(pl.PlayerHealth, pl.PlayerStamina, pl.PlayerHunger);
                }
                else
                {
                    networkView.RPC("Client_UpdateStatus", pl.PlayerNetwork, pl.PlayerHealth, pl.PlayerStamina, pl.PlayerHunger);
                }
            }
            timepast = Time.time + 1f;
        }
        if (Time.time > timepast2)
        {
            foreach(MPPlayer pl in NetworkManager.PlayerList)
            {
                if (pl.PlayerHunger < 50)
                {
                    pl.PlayerHunger -= HungerLossAmmount * 2;
                }
                else
                    pl.PlayerHunger -= HungerLossAmmount;
            }
            timepast2 = Time.time + TimeToConsumeHunger;
        }
    }

    public GameObject SparksParticle;
    public AudioClip[] Sounds;
    [RPC]
    public void Server_ReceiveDamage(NetworkPlayer giver, NetworkPlayer victim, Vector3 pos)
    {
        MPPlayer damage_giver = MPPlayer.GetPlayer(giver);
        MPPlayer damage_receiver = MPPlayer.GetPlayer(victim);

        if (Vector3.Distance(damage_giver.PlayerTransform.position, damage_receiver.PlayerTransform.position) > damage_giver.SelectedItem.item.ItemRange)
            return;

        if (Time.time < damage_giver.LastHit)
        {
            return;
        }

        damage_giver.PlayerStamina -= 10;
        damage_giver.LastHit = Time.time + 1.5f;

        if(damage_receiver.PlayerDefending)
        {
            if(damage_receiver.PlayerDefendingStartTime + 1 < Time.time)
            {
                damage_receiver.PlayerHealth -= damage_giver.SelectedItem.item.ItemAttackDamage;
            }
            else
            {
                PlaySound(Random.Range(0, 3));
                Network.Instantiate(SparksParticle, pos, Quaternion.identity, 1);
                return;
            }
        }
        else
        {
            damage_receiver.PlayerHealth -= damage_giver.SelectedItem.item.ItemAttackDamage;
        }

        if (victim == Network.player)
            Client_DamageScreen();
        else
            networkView.RPC("Client_DamageScreen", victim);

        if (victim == Network.player)
        {
            Client_UpdateStatus(damage_receiver.PlayerHealth, damage_receiver.PlayerStamina, damage_receiver.PlayerHunger);
        }
        else
        {
            networkView.RPC("Client_UpdateStatus", victim, damage_receiver.PlayerHealth, damage_receiver.PlayerStamina, damage_receiver.PlayerHunger);
        }
    }
    [RPC]
    public void Client_DamageScreen()
    {
        BleedBehavior.BloodAmount += 0.2f * 5;
        camera.GetComponent<BleedBehavior>().TestingBloodAmount += 0.2f * 5;
    }
    [RPC]
    public void Client_PlaySound(int id)
    {
        audio.PlayOneShot(Sounds[id]);
    }

    public void PlaySound(int id)
    {
        networkView.RPC("Client_PlaySound", RPCMode.All, id);
    }
    void giveDamage(HitInfo giver)
    {
        if(Network.isServer)
        {
            Server_ReceiveDamage(giver.pl, networkView.owner, giver.pos);
        }
        else
        {
            networkView.RPC("Server_ReceiveDamage", RPCMode.Server, giver.pl, networkView.owner, giver.pos);
        }
    }

    [RPC]
    public void Server_PlayerDefending(NetworkPlayer pln, bool defending)
    {
        MPPlayer pl = MPPlayer.GetPlayer(pln);
        pl.PlayerDefending = defending;

        if (defending)
            pl.PlayerDefendingStartTime = Time.time;
    }

    public GameObject ProjectilePrefab;
    public Collider[] ThirdPersonColliders;
    public Collider[] FPSCollider;
    [RPC]
    public void Server_FireArrow(Vector3 pos, Quaternion rot, NetworkPlayer shooter)
    {
        MPPlayer pl = MPPlayer.GetPlayer(shooter);
        if (pl.GetItemNum(Item.getItem("Arrow")) > 0)
        {
            GameObject arrow = Network.Instantiate(ProjectilePrefab, pos, rot, 1) as GameObject;
            if (Network.player != shooter)
                arrow.GetComponent<ArrowBehavior>().pl = MPPlayer.GetPlayer(shooter).PlayerTransform.GetComponent<PlayerManager>().ThirdPersonColliders;
            else
                arrow.GetComponent<ArrowBehavior>().pl = FPSCollider;
            arrow.rigidbody.AddForce(arrow.transform.forward * 45, ForceMode.Impulse);

            pl.RemoveItem(Item.getItem("Arrow"), 1);

            Server_UpdateInventory(shooter);
        }
    }

    #endregion
	*/
}

public class ItemAnimation
{
    public Vector2 initialPos;
    public Vector2 initialPosNum;
    public Vector2 pos;
    public Texture2D text;
    public int num;
    public bool showNum;

    public ItemAnimation(Vector2 iniposition, Vector2 position, Vector2 initialPosNumm, Texture2D texture, int itemcount, bool shownum)
    {
        initialPosNum = initialPosNumm;
        initialPos = iniposition;
        pos = position;
        text = texture;
        num = itemcount;
        showNum = shownum;
    }
}
[System.Serializable]
public class StateImages
{
    public Texture2D Background;
    public Texture2D Chest;
    public Texture2D Door;
    public Texture2D Skin;
}
[System.Serializable]
public class CraftingImages
{
    public Texture2D WoodenTab;
    public string[] tabs_Names;
    public Texture2D[] tabs;
    public Texture2D[] tabs_Selected;
    public Texture2D[] tabs_Click;
    public Texture2D wodeen_Craft;
    public Texture2D itemSplitTop;
    public Texture2D itemSplitBottom;
    public Texture2D itemSlot;
    public Texture2D itemInfo;
    public Texture2D craftButton;
}