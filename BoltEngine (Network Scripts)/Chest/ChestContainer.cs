using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChestContainer : Bolt.EntityBehaviour<ITFSChestState>
{
    public NetworkPlayer playerInsideChest;
    
    public bool chestOpen = false;

    [System.NonSerialized]
    public List<ItemSpawnInfo> ItemTypes = new List<ItemSpawnInfo>();

    ItemsDatabase ItemsDatabase;
    public BuildingObjects buildingModels;

    public bool suplyDrop = false;

    public List<Item> ItemList = new List<Item>();
    public List<ItemCraft> Crafting = new List<ItemCraft>();

    public LayerMask lm;
    
    public ITFSChestState state;

    public override void Attached()
    {
        if (BoltNetwork.isClient) return;

        ItemTypes.Clear();

        int tier = Random.Range(0, 5);
        if(tier <= 1)
        {
            ItemTypes.Add(new ItemSpawnInfo(Item.ItemTypeE.Material, 1, 16, 1));
        }
        else if (tier == 2)
        {
            ItemTypes.Add(new ItemSpawnInfo(Item.ItemTypeE.Material, 1, 8, 1));
            ItemTypes.Add(new ItemSpawnInfo(Item.ItemTypeE.Material2, 1, 8, 1));
        }
        else if (tier >= 3)
        {
            ItemTypes.Add(new ItemSpawnInfo(Item.ItemTypeE.Material, 1, 5, 1));
            ItemTypes.Add(new ItemSpawnInfo(Item.ItemTypeE.Material2, 1, 10, 1));
            ItemTypes.Add(new ItemSpawnInfo(Item.ItemTypeE.Material2, 1, 2, 2));
        }
        if (suplyDrop)
        {
            ItemTypes.Clear();

            ItemTypes.Add(new ItemSpawnInfo(Item.ItemTypeE.Material, 1, 5, 1));
            ItemTypes.Add(new ItemSpawnInfo(Item.ItemTypeE.Material2, 1, 10, 1));
            ItemTypes.Add(new ItemSpawnInfo(Item.ItemTypeE.Material2, 1, 4, 2));
        }

        this.GetComponent<BoltEntity>().TakeControl();
        state = this.GetComponent<BoltEntity>().GetState<ITFSChestState>();

        for (int i = 0; i < 16; i++)
        {
            state.inventory[i].ItemID = 0;
        }

        ItemList = ItemsDatabase.instance.ItemList;
        Crafting = ItemsDatabase.instance.Crafting;

        RecaulculateItems();
    }

    void RecaulculateItems()
    {
        List<Item> ItemListAbleToChoose = new List<Item>();
        ItemListAbleToChoose = ItemsToPlace();

        int[] slotsToPlaceX = new int[ItemListAbleToChoose.Count];
        int[] slotsToPlaceY = new int[ItemListAbleToChoose.Count];

        for(int i = 0; i < ItemListAbleToChoose.Count; i++)
        {
            slotsToPlaceX[i] = Random.Range(0, 4);
            slotsToPlaceY[i] = Random.Range(0, 4);
        }

        for (int index = 0; index < ItemListAbleToChoose.Count; index++)
        {
            int num = 1;

            if (ItemListAbleToChoose[index].ItemMaxStack > 1)
                num = Random.Range(1, 3);

            state.inventory[slotID(slotsToPlaceX[index], slotsToPlaceY[index])].ItemID = ItemListAbleToChoose[index].ItemID;
            state.inventory[slotID(slotsToPlaceX[index], slotsToPlaceY[index])].ItemNum = num;
        }
    }

    public int slotID(int x, int z)
    {
        int slotid= 0;

        for (int ix = 0; ix < 4; ix++)
        {
            for (int iz = 0; iz < 4; iz++)
            {
                if (ix == x && iz == z)
                    return slotid;
                slotid++;
            }
        }

        return slotid;
    }

    public bool ItemTypesConatains(Item.ItemTypeE e)
    {
        foreach(ItemSpawnInfo isi in ItemTypes)
        {
            if (isi.ItemType == e)
                return true;
        }
        return false;
    }
    public List<Item> ItemsToPlace()
    {
        List<Item> ItemListAbleToChooseE = new List<Item>();
        foreach(ItemSpawnInfo i in ItemTypes)
        {
            if(i.rarity == 1)
            {
                int r = Random.Range(i.minItems, i.maxItems); // Number of items to put
                int[] ids = GetIDs(i.ItemType, r);

                if (ids.Length == 0) continue;
                foreach (int idss in ids)
                {
                    Item item = getItemD(idss);
                    ItemListAbleToChooseE.Add(item);
                }
            }
            else if (Random.Range(0, i.rarity) == 1)
            {
                int r = Random.Range(i.minItems, i.maxItems); // Number of items to put
                int[] ids = GetIDs(i.ItemType, r);

                if (ids.Length == 0) continue;
                foreach (int idss in ids)
                {
                    Item item = getItemD(idss);
                    ItemListAbleToChooseE.Add(item);
                }
            }
            //items++;
        }
        return ItemListAbleToChooseE;
    }

    public int[] GetIDs(Item.ItemTypeE e, int ammount)
    {
        try
        {
            if (ammount <= 0)
                return new int[] { 0, 0, 0 };

            List<int> AvailableIds = new List<int>();
            foreach (Item i in ItemList)
            {
                if (i.ItemType == e)
                    AvailableIds.Add(i.ItemID);
            }

            List<int> ids = new List<int>();
            for (int x = 0; x < ammount; x++)
            {
                ids.Add(AvailableIds[Random.Range(0, AvailableIds.Count)]);
            }

            return ids.ToArray();
        }
        catch
        {
            return null;
        }
    }

    public Item getItemD(int id)
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
    public Item getItemD(string name)
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

    public ItemInfo getItem(int slot)
    {
        int slotid = 0;
        for (int x = 0; x < 4; x++)
        {
            for (int z = 0; z < 4; z++)
            {
                if (slot == slotid)
                {
                    return new ItemInfo(getItemD(state.inventory[slotID(x, z)].ItemID), state.inventory[slotID(x, z)].ItemNum);
                }
                slotid++;
            }
        }

        return new ItemInfo();
    }
    public bool SetItem(int slot, Item item, int num)
    {
        int slotid = 0;
        for (int x = 0; x < 4; x++)
        {
            for (int z = 0; z < 4; z++)
            {
                if (slot == slotid)
                {
                    state.inventory[slotID(x, z)].ItemID = item.ItemID;
                    state.inventory[slotID(x, z)].ItemNum = num;

                    return true;
                }
                slotid++;
            }
        }

        return false;
    }
}
