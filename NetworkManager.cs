using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System;
using Ini;

public class NetworkManager : MonoBehaviour
{
    bool ConnectedToServer = false;
    public bool ServerCanPlay = true;
    public bool StartServerOnStart = false;
    public static int invWidth = 8, invHeight = 4;
    public static List<MPPlayer> PlayerList = new List<MPPlayer>();
    public static List<Item> ItemList = new List<Item>();
    public static List<ItemCraft> Crafting = new List<ItemCraft>();

    public static string Username;

    public static int walkSpeedRestriction;
    public static int runSpeedRestriction;
    public static int airSpeedRestriction;

    public static string serverIP = "localhost";

    public static List<ChestContainer> Chests = new List<ChestContainer>();

    public GameObject PlayerPref;
    public PlayerManager myPlayer;

    public GameObject Twister;

    public GameObject ChestModel;
    public GameObject Door;
    public GameObject Wall1;
    public GameObject Foundation;
    public GameObject Roof;
    public Material buildingMaterial;

    public static List<GameObject> foundations = new List<GameObject>();
    public static List<GameObject> walls = new List<GameObject>();
    public static List<GameObject> ceillings = new List<GameObject>();

    public static NetworkManager instance;

    public static List<Collider> blockedAreas = new List<Collider>();

    void Awake()
    {
        /*MasterServer.ipAddress = "88.198.160.222";
        MasterServer.port = 23466;
        Network.natFacilitatorIP = "88.198.160.222";
        Network.natFacilitatorPort = 50005;*/

        LoadIni();
        if (StartServerOnStart)
        {
            Network.InitializeServer(24, ServerConnector.port, false);
            MasterServer.RegisterHost("bm_gamehungergames_test12345", ServerConnector.serverName, "A game server! and it is online!");
        }
        DontDestroyOnLoad(gameObject);
        //RegisterItems();
        //RegisterCraftings();

        instance = this;
    }
    /*void RegisterItems()
    {
        //Weapons
        ItemList.Add(new Item("Crossbow", 1, Item.ItemTypeE.Weapon));
        ItemList.Add(new Item("S2", 1, Item.ItemTypeE.Weapon, 25));

        //Ammo
        ItemList.Add(new Item("Arrow", 25, Item.ItemTypeE.Ammo));
        ItemList.Add(new Item("Poison Arrow", 25, Item.ItemTypeE.Ammo));
        ItemList.Add(new Item("Fire Arrow", 25, Item.ItemTypeE.Ammo));

        //Armor

        //Grenades

        //Tools
        ItemList.Add(new Item("hatchet", 1, Item.ItemTypeE.Tool));

        //Food

        //Building
        ItemList.Add(new Item("container", 5, Item.ItemTypeE.Building, ChestModel));
        ItemList.Add(new Item("Door", 25, Item.ItemTypeE.Building, Door));
        ItemList.Add(new Item("Wall1", 25, Item.ItemTypeE.Building, Wall1));
        ItemList.Add(new Item("Foundation", 25, Item.ItemTypeE.Building, Foundation));
        ItemList.Add(new Item("Roof", 25, Item.ItemTypeE.Building, Roof));

        //Misc
        ItemList.Add(new Item("healthshot", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("staminashot", 250, Item.ItemTypeE.None));

        //Others
        ItemList.Add(new Item("Wood", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("Stone", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("syringe", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("redplant", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("blueplant", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("leather", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("cloth", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("rope", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("leaves", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("fur", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("metal", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("nails", 250, Item.ItemTypeE.None));
        ItemList.Add(new Item("Expl_Barrel", 250, Item.ItemTypeE.None));
    }
    void RegisterCraftings()
    {
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(Item.getItem("Wood"), 2), new ItemInfo(Item.getItem("Stone"), 3) }, Item.getItem("hatchet"), ItemCraft.ItemT.Tools, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(Item.getItem("redplant"), 40), new ItemInfo(Item.getItem("syringe"), 1) }, Item.getItem("healthshot"), ItemCraft.ItemT.Miscellaneous, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(Item.getItem("blueplant"), 40), new ItemInfo(Item.getItem("syringe"), 1) }, Item.getItem("staminashot"), ItemCraft.ItemT.Miscellaneous, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(Item.getItem("Arrow"), 2), new ItemInfo(Item.getItem("cloth"), 1) }, Item.getItem("Fire Arrow"), ItemCraft.ItemT.Weapons, 2));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(Item.getItem("Wood"), 4), new ItemInfo(Item.getItem("nails"), 4) }, Item.getItem("container"), ItemCraft.ItemT.Structures, 2));
    }
	*/
    void LoadIni()
    {
        IniFile ini = new IniFile(Application.dataPath + "/settings.ini");

        if (ini.IniReadValue("Server", "initiateServer") == null ||
           ini.IniReadValue("Server", "initiateServer") == "")
        {
            ini.IniWriteValue("Server", "initiateServer", "false");
        }
        if (ini.IniReadValue("Server", "dedicatedServer") == null ||
           ini.IniReadValue("Server", "dedicatedServer") == "")
        {
            ini.IniWriteValue("Server", "dedicatedServer", "true");
        }
        if (ini.IniReadValue("Server", "serverPort") == null ||
           ini.IniReadValue("Server", "serverPort") == "")
        {
            ini.IniWriteValue("Server", "serverPort", "1900");
        }
        if (ini.IniReadValue("Server", "serverName") == null ||
           ini.IniReadValue("Server", "serverName") == "")
        {
            ini.IniWriteValue("Server", "serverName", "My server!");
        }
        if (ini.IniReadValue("AntiHack", "runSpeedRestriction") == null ||
           ini.IniReadValue("AntiHack", "runSpeedRestriction") == "")
        {
            ini.IniWriteValue("AntiHack", "runSpeedRestriction", "7");
        }
        if (ini.IniReadValue("AntiHack", "walkSpeedRestriction") == null ||
           ini.IniReadValue("AntiHack", "walkSpeedRestriction") == "")
        {
            ini.IniWriteValue("AntiHack", "walkSpeedRestriction", "9");
        }
        if (ini.IniReadValue("AntiHack", "airSpeedRestriction") == null ||
           ini.IniReadValue("AntiHack", "airSpeedRestriction") == "")
        {
            ini.IniWriteValue("AntiHack", "airSpeedRestriction", "15");
        }

        ServerCanPlay = !bool.Parse(ini.IniReadValue("Server", "dedicatedServer"));
        StartServerOnStart = bool.Parse(ini.IniReadValue("Server", "initiateServer"));
        ServerConnector.port = int.Parse(ini.IniReadValue("Server", "serverPort"));
        ServerConnector.serverName = ini.IniReadValue("Server", "serverName");
        walkSpeedRestriction = int.Parse(ini.IniReadValue("AntiHack", "walkSpeedRestriction"));
        runSpeedRestriction = int.Parse(ini.IniReadValue("AntiHack", "runSpeedRestriction"));
        airSpeedRestriction = int.Parse(ini.IniReadValue("AntiHack", "airSpeedRestriction"));
    }

    /*int numberOfPlayer = 0;
    int ping = 0;
    void OnGUI()
    {
        if (!ConnectedToServer) return;

        GUILayout.Label("Connected to server. Players online: " + numberOfPlayer + " of 24.");
        //GUILayout.Label("Ping: " + pg + "");
        GUILayout.Label("Latency " + ping + " ms");
    }

    UniStormWeatherSystem_C uniStorm;

    float timeToUpdate = 3;
    float timeToUpdate2 = 0;
    void Update()
    {
        if(Network.isServer)
        {
            if (Time.time > timeToUpdate)
            {
                ping = Network.GetLastPing(Network.player);
                numberOfPlayer = Network.connections.Length;

                Server_updatePings();

                timeToUpdate = Time.time + 3;
            }
        }
        if (uniStorm == null)
        {
            try
            {
                uniStorm = GameObject.Find("UniStormSystemEditor").GetComponent<UniStormWeatherSystem_C>();
                //Network.Instantiate(Twister, new Vector3(400, 300, 275), Quaternion.identity, 1);

                foreach(GameObject go in GameObject.FindGameObjectsWithTag("BlockedArea"))
                {
                    blockedAreas.Add(go.collider);
                }
            }
            catch { }
        }
        else
        {
            if (myPlayer != null)
            {
                uniStorm.cameraThing = myPlayer.camera;
                uniStorm.butterflies = myPlayer.Butterflies;
                uniStorm.rain = myPlayer.Rain;
                uniStorm.snow = myPlayer.Snow;
                uniStorm.snowMistFog = myPlayer.SnowObject;
                uniStorm.windyLeaves = myPlayer.WindyLeaves;
                uniStorm.mistFog = myPlayer.MistCloud;
            }
            if (Network.isServer)
            {
                uniStorm.staticWeather = false;
            }

            if (Time.time > timeToUpdate2)
            {
                if (Network.isServer)
                    Server_UpdateTime();
                else
                    networkView.RPC("Server_UpdateTime", RPCMode.Server);
                timeToUpdate2 = Time.time + 1;
            }
        }

    }
    [RPC]
    public void Server_UpdateTime()
    {
        networkView.RPC("Client_UpdateTime", RPCMode.All, uniStorm.startTime, uniStorm.weatherForecaster, uniStorm.daySize);
    }
    [RPC]
    public void Client_UpdateTime(float time, int weather, int times)
    {
        uniStorm.startTime = time;
        uniStorm.weatherForecaster = weather;
        uniStorm.daySize = times;
    }

    public void Server_updatePings()
    {
        int latency = 0;
        foreach(MPPlayer pl in PlayerList)
        {
            if (pl.PlayerNetwork != Network.player)
            {
                latency = Network.GetLastPing(pl.PlayerNetwork);
                networkView.RPC("Client_setPing", pl.PlayerNetwork, latency, Network.connections.Length);
            }
        }
    }
    [RPC]
    public void Client_setPing(int ping, int players)
    {
        numberOfPlayer = players;
        this.ping = ping;
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        if (stream.isWriting)
        {
            if (Network.isServer)
            {
                int tp = Network.connections.Length;

                stream.Serialize(ref tp);
            }
        }
        else
        {
            stream.Serialize(ref numberOfPlayer);
        }
    }

    void OnServerInitialized()
    {
        if (ServerCanPlay)
            PlayerList.Add(new MPPlayer("Test_" + UnityEngine.Random.Range(100, 999), Network.player));

        ConnectedToServer = true;
        Application.LoadLevelAsync(1);
    }
    void OnConnectedToServer()
    {
        if (Username == "" || Username == null)
            Username = "Nameless";
        networkView.RPC("PlayerJoinRequest", RPCMode.Server, Network.player, Username);
        ConnectedToServer = true;
        Application.LoadLevelAsync(1);
    }

    [RPC]
    public void PlayerJoinRequest(NetworkPlayer pl, string name)
    {
        PlayerList.Add(new MPPlayer(name, pl));
    }
    void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            if(Network.isServer && ServerCanPlay)
            {
                Server_SpawnPlayer(Network.player);
            }
            else
            {
                networkView.RPC("Server_SpawnPlayer", RPCMode.Server, Network.player);
            }

            foreach(GameObject go in GameObject.FindGameObjectsWithTag("Chest"))
            {
                Chests.Add(go.GetComponent<ChestContainer>());
            }
        }
    }

    void OnDisconnectedFromServer()
    {
        Destroy(gameObject);
    }

    void OnPlayerDisconnected(NetworkPlayer pl)
    {
        PlayerList.Remove(MPPlayer.GetPlayer(pl));
        Network.DestroyPlayerObjects(pl);
    }

    [RPC]
    public void Server_SpawnPlayer(NetworkPlayer pl)
    {
        MPPlayer player = MPPlayer.GetPlayer(pl);
        if (player.PlayerAlive == false)
        {
            if (Network.player == pl)
                Client_SpawnPlayer(new Vector3(205, 31, 275), Quaternion.identity);
            else
                networkView.RPC("Client_SpawnPlayer", pl, new Vector3(205, 31, 275), Quaternion.identity);

            player.PreparePlayer(pl);
            player.AddItem("S2", 1);
            player.AddItem("Crossbow", 1);
            player.AddItem("Foundation", 25);
            player.AddItem("Wall1", 25);
            player.AddItem("Door", 25);
            player.AddItem("Arrow", 25);
        }
    }
    [RPC]
    public void Client_SpawnPlayer(Vector3 pos, Quaternion rot)
    {
        GameObject PlayerObj = Network.Instantiate(PlayerPref, pos, rot, 0) as GameObject;
        PlayerObj.GetComponent<PlayerManager>().UpdateInventory();
        myPlayer = PlayerObj.GetComponent<PlayerManager>();
    }*/
}

public class MPPlayer
{
    public string PlayerName = "";
    public NetworkPlayer PlayerNetwork;
    public float PlayerHealth;
    public bool PlayerAlive = false;
    public Item[,] PlayerInventory;
    public int[,] PlayerInventoryNum;

    public Item[,] PlayerInventory_Hotbar;
    public int[,] PlayerInventoryNum_Hotbar;

    public Item[,] InventoryArmorContent = new Item[1, 4];
    public int[,] InventoryArmorNum = new int[1, 4];

    public Item PlayerItemDragging;
    public int PlayerItemDraggingNum;

    public SelectedItem SelectedItem = new SelectedItem();

    public bool PlayerInsideChest = false;

    public float PlayerHunger;
    public float PlayerStamina;

    public float LastHit;
    public bool PlayerSprinting = false;
    public bool PlayerDefending = false;
    public float PlayerDefendingStartTime = 0;

    public Transform PlayerTransform;

    public MPPlayer()
    {
    }
    public MPPlayer(string name, NetworkPlayer pl)
    {
        PlayerName = name;
        PlayerNetwork = pl;
        PlayerHealth = 100;
        PlayerAlive = false;
        PlayerInventory = new Item[NetworkManager.invWidth, NetworkManager.invHeight];
        PlayerInventory_Hotbar = new Item[NetworkManager.invWidth, 1];
        InventoryArmorContent = new Item[1, 4];
        InventoryArmorNum = new int[1, 4];
    }

    public bool SetItemDragging(Item i, int num)
    {
        PlayerItemDragging = i;
        PlayerItemDraggingNum = num;

        return true;
    }

    public static MPPlayer GetPlayer(NetworkPlayer pl)
    {
        foreach(MPPlayer mpl in NetworkManager.PlayerList)
        {
            if (mpl.PlayerNetwork == pl)
                return mpl;
        }
        return null;
    }
    public bool PreparePlayer(NetworkPlayer pl)
    {
        foreach (MPPlayer mpl in NetworkManager.PlayerList)
        {
            if (mpl.PlayerNetwork == pl)
            {
                mpl.PlayerAlive = true;
                mpl.PlayerHealth = 100;
                mpl.PlayerInventory = new Item[NetworkManager.invWidth, NetworkManager.invHeight];
                mpl.PlayerInventoryNum = new int[NetworkManager.invWidth, NetworkManager.invHeight];
                PlayerInventory_Hotbar = new Item[NetworkManager.invWidth, 2];
                PlayerInventoryNum_Hotbar = new int[NetworkManager.invWidth, 2];
                InventoryArmorContent = new Item[1, 4];
                InventoryArmorNum = new int[1, 4];

                for (int x = 0; x < NetworkManager.invWidth; x++)
                {
                    for (int z = 0; z < NetworkManager.invHeight; z++)
                    {
                        mpl.PlayerInventory[x, z] = new Item();
                        mpl.PlayerInventoryNum[x, z] = 0;
                    }
                }

                PlayerHealth = 100;
                PlayerHunger = 100;
                PlayerStamina = 100;

                return true;
            }
        }

        return false;
    }

    /*public bool AddItem(string name, int count)
    {
        int slotID = 0;
        for (int z = 0; z < NetworkManager.invHeight; z++)
        {
            for (int x = 0; x < NetworkManager.invWidth / 2; x++)
            {
                if (PlayerInventory[x, z].ItemID == -1 || PlayerInventory[x, z].ItemName == name)
                {
                    if (PlayerInventoryNum[x, z] != Item.getItem(name).ItemMaxStack)
                    {
                        int rest = 0;

                        PlayerInventory[x, z] = Item.getItem(name);

                        if (PlayerInventoryNum[x, z] + count > Item.getItem(name).ItemMaxStack)
                        {
                            rest = (PlayerInventoryNum[x, z] + count) - Item.getItem(name).ItemMaxStack;
                            PlayerInventoryNum[x, z] = Item.getItem(name).ItemMaxStack;
                        }
                        else
                        {
                            PlayerInventoryNum[x, z] += count;
                        }

                        if (rest > 0)
                        {
                            AddItem(name, rest);
                        }

                        return true;
                    }
                }
                slotID++;
            }
        }

        return false;
    }
    
    public ItemInfo getItem(int slot)
    {
        int slotID = 0;
        for (int x = 0; x < NetworkManager.invWidth; x++)
        {
            for (int z = 0; z < NetworkManager.invHeight; z++)
            {
                if (slotID == slot)
                {
                    return new ItemInfo(PlayerInventory[x, z], PlayerInventoryNum[x,z]);
                }
                slotID++;
            }
        }
        return new ItemInfo();
    }
    public ItemInfo getHotBarItem(int slot)
    {
        int slotID = 0;
        for (int x = 0; x < NetworkManager.invWidth; x++)
        {
            if (slotID == slot)
            {
                return new ItemInfo(PlayerInventory_Hotbar[slotID, 0], PlayerInventoryNum_Hotbar[slotID, 0]);
            }
            slotID++;
        }
        return new ItemInfo();
    }
    public ItemInfo getArmorItem(int slot)
    {
        if (slot > 4)
            slot = 4;
        return new ItemInfo(InventoryArmorContent[0, slot], InventoryArmorNum[0, slot]);
    }

    public int GetItemNum(Item i)
    {
        int count = 0;

        for (int x = 0; x < NetworkManager.invWidth; x++)
        {
            for (int y = 0; y < NetworkManager.invHeight; y++)
            {
                if (PlayerInventory[x, y] == null) continue;
                if (PlayerInventory[x, y].ItemID == i.ItemID)
                {
                    count += PlayerInventoryNum[x, y];
                }
            }
        }
        //Hotbar
        for (int x = 0; x < 8; x++)
        {
            if (PlayerInventory_Hotbar[x, 0] == null) continue;
            if (PlayerInventory_Hotbar[x, 0].ItemID == i.ItemID)
            {
                count += PlayerInventoryNum_Hotbar[x, 0];
            }
        }

        return count;
    }
    public bool RemoveItem(Item i, int num)
    {
        if (GetItemNum(i) < num) return false;
        for (int x = 0; x < NetworkManager.invWidth + 8; x++)
        {
            for (int y = 0; y < NetworkManager.invHeight; y++)
            {
                if (x > 7)
                {
                    if (PlayerInventory_Hotbar[x - 8, 0] == i)
                    {
                        if (PlayerInventoryNum_Hotbar[x - 8, 0] >= num)
                        {
                            if (PlayerInventoryNum_Hotbar[x - 8, 0] == num)
                            {
                                PlayerInventory_Hotbar[x - 8, 0] = new Item();
                                PlayerInventoryNum_Hotbar[x - 8, 0] = 0;
                            }
                            else
                            {
                                PlayerInventoryNum_Hotbar[x - 8, 0] -= num;
                            }
                            return true;
                        }
                        else
                        {
                            int rest = PlayerInventoryNum_Hotbar[x - 8, 0] - num;

                            PlayerInventory_Hotbar[x - 8, 0] = new Item();
                            PlayerInventoryNum_Hotbar[x - 8, 0] = 0;

                            RemoveItem(i, rest);
                        }
                    }
                }
                else
                {
                    if (PlayerInventory[x, y] == i)
                    {
                        if (PlayerInventoryNum[x, y] >= num)
                        {
                            if (PlayerInventoryNum[x, y] == num)
                            {
                                PlayerInventory[x, y] = new Item();
                                PlayerInventoryNum[x, y] = 0;
                            }
                            else
                            {
                                PlayerInventoryNum[x, y] -= num;
                            }
                            return true;
                        }
                        else
                        {
                            int rest = PlayerInventoryNum[x, y] - num;

                            PlayerInventory[x, y] = new Item();
                            PlayerInventoryNum[x, y] = 0;

                            RemoveItem(i, rest);
                        }
                    }
                }
            }
        }
        return false;
    }
    public bool isChestSlot(int slot)
    {
        int slotID = 0;
        for (int x = 0; x < NetworkManager.invWidth; x++)
        {
            for (int z = 0; z < NetworkManager.invHeight; z++)
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

    public bool SetItem(int slot, Item i, int num)
    {
        int slotID = 0;
        for (int x = 0; x < NetworkManager.invWidth; x++)
        {
            for (int z = 0; z < NetworkManager.invHeight; z++)
            {
                if (slotID == slot)
                {
                    PlayerInventory[x, z] = i;
                    PlayerInventoryNum[x, z] = num;
                    return true;
                }
                slotID++;
            }
        }
        return false;
    }
    public bool SetHotbarItem(int slot, Item i, int num)
    {
        PlayerInventory_Hotbar[slot, 0] = i;
        PlayerInventoryNum_Hotbar[slot, 0] = num;
        return true;
    }
    public bool SetArmorItem(int slot, Item i, int num)
    {
        if (slot > 4)
            slot = 4;

        InventoryArmorContent[0, slot] = i;
        InventoryArmorNum[0, slot] = num;

        return true;
    }*/
}
public class Item
{
    public string ItemName;
    public int ItemID = -1;
    public Texture ItemIcon;
    public int ItemMaxStack;
    public ItemTypeE ItemType;
    public float ItemAttackDamage;
    public float ItemRange = 7f;
    public GameObject ItemObjectWorld;
    public WeaponType WeaponType = WeaponType.None;
    public float SlowValue;
    public int Height;
    public string ItemDescription;
    public float ProtectionValue = 0f;

    [System.Serializable]
    public enum ItemTypeE
    {
        None,
        Material,
        Material2,
        Weapon,
        Food,
        Building,
        HeadArmor,
        BodyArmor,
        LegsArmor,
        ShooesArmor,
        Shields,
        Ammo,
        Grenades,
        Trap,
        Tool
    }

    public Item()
    {
        ItemIcon = Resources.Load<Texture>("default");
        ItemID = -1;
        ItemType = ItemTypeE.None;
    }
    public Item(string name, int maxstack, ItemTypeE it, int height, List<Item> ItemList, string desc)
    {
        ItemName = name;
		ItemID = NextID(ItemList);
        ItemIcon = Resources.Load<Texture>("" + name);
        if( ItemIcon == null) ItemIcon = Resources.Load<Texture>("default");
        ItemMaxStack = maxstack;
        this.ItemType = it;
        Height = height;
        ItemDescription = desc;
    }
    public Item(string name, int maxstack, ItemTypeE it, int height, float protectionValue, List<Item> ItemList, string desc)
    {
        ItemName = name;
        ItemID = NextID(ItemList);
        ItemIcon = Resources.Load<Texture>("" + name);
        if (ItemIcon == null) ItemIcon = Resources.Load<Texture>("default");
        ItemMaxStack = maxstack;
        this.ItemType = it;
        Height = height;
        ItemDescription = desc;
        ProtectionValue = protectionValue;
    }


    public Item(string name, int maxstack, ItemTypeE it, GameObject worldModel, int height, List<Item> ItemList, string desc)
    {
        ItemName = name;
		ItemID = NextID(ItemList);
        ItemIcon = Resources.Load<Texture>("" + name);
        if (ItemIcon == null) ItemIcon = Resources.Load<Texture>("default");
        ItemMaxStack = maxstack;
        this.ItemType = it;
        ItemObjectWorld = worldModel;
        Height = height;
        ItemDescription = desc;
    }
    public Item(string name, int maxstack, ItemTypeE it, float attackDmg, float heavy, WeaponType weaponType, GameObject go, int height, List<Item> ItemList, string desc)
    {
        ItemName = name;
		ItemID = NextID(ItemList);
        ItemIcon = Resources.Load<Texture>("" + name);
        if (ItemIcon == null) ItemIcon = Resources.Load<Texture>("default");
        ItemMaxStack = maxstack;
        this.ItemType = it;
        ItemAttackDamage = attackDmg;
        WeaponType = weaponType;
        ItemObjectWorld = go;
        SlowValue = heavy;
        Height = height;
        ItemDescription = desc;
    }

    public static int NextID(List<Item> ItemList)
    {
		return ItemList.Count + 1;
    }
	public static Item getItem(int id, List<Item> ItemList)
    {
		foreach(Item i in ItemList)
        {
            if(i.ItemID == id)
            {
                return i;
            }
        }

        return null;
    }
	public static Item getItem(string name, List<Item> ItemList)
    {
		foreach (Item i in ItemList)
        {
            if (i.ItemName == name)
            {
                return i;
            }
        }

        return null;
    }
}

public class ItemInfo
{
    public Item item;
    public int itemNum;

    public ItemInfo()
    {
        item = new Item();
        itemNum = 0;
    }
    public ItemInfo(Item i, int inn)
    {
        item = i;
        itemNum = inn;
    }
}
[System.Serializable]
public class ItemSpawnInfo
{
    public Item.ItemTypeE ItemType;
    public int minItems = 3;
    public int maxItems = 8;
    public int rarity = 5;

    public ItemSpawnInfo()
    {

    }
    public ItemSpawnInfo(Item.ItemTypeE it, int minI, int maxI, int rar)
    {
        ItemType = it;
        minItems = minI;
        maxItems = maxI;
        rarity = rar;
    }
}
public class SelectedItem
{
    public Item item;
    public bool active;
    public int slot;

    public SelectedItem()
    {
        slot = -1;
        item = new Item();
    }
    public SelectedItem(Item i , bool a, int s)
    {
        item = i;
        active = a;
        slot = s;
    }
}
public class ItemCraft
{
    public ItemInfo[] ItemsNeeded;
    public Item Item;
    public int itemCount;
    public ItemT ItemType;

    public ItemCraft(ItemInfo[] i, Item it,ItemT t, int iCount)
    {
        ItemsNeeded = i;
        Item = it;
        ItemType = t;
        itemCount = iCount;
    }

    public enum ItemT
    {
        Tools,
        Weapons,
        Structures,
        Vehicles,
        Consumables,
        Traps,
        Miscellaneous,
        Armor
    }
}

namespace Ini
{
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public IniFile(string INIPath)
        {
            path = INIPath;
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            255, this.path);
            return temp.ToString();

        }
    }
}