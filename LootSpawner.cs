using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootSpawner : Bolt.EntityBehaviour<IAirDrop>
{
    public List<ItemInfo> items = new List<ItemInfo>();

    int tier = 0;
    public override void Attached()
    {
        tier = Random.Range(1, 4);
        if( tier == 1)
        {
            SpawnTier1();
        }
        else if( tier == 2)
        {
            SpawnTier2();
        }
        else if (tier == 3)
        {
            SpawnTier3();
        }
        else if (tier == 4)
        {
            SpawnTier4();
        }
    }
    void SpawnTier1()
    {
        int[] randomC = new int[6];
        int[] randomID = new int[6];

        List<Item> materials = new List<Item>();
        foreach(Item item in ItemsDatabase.instance.ItemList)
        {
            if (item.ItemType == Item.ItemTypeE.Material)
                materials.Add(item);
        }
        if (materials.Count == 0) return;

        for(int i = 0; i < 6; i++)
        {
            randomC[i] = Random.Range(0, 9);
            randomID[i] = materials[Random.Range(0, materials.Count)].ItemID;

            state.inventory[i].ItemID = randomID[i];
            state.inventory[i].ItemNum = randomC[i];

            if(AllToguether(randomC) > 15)
            {
                continue;
            }
        }
    }
    void SpawnTier2()
    {
        int[] randomC = new int[6];
        int[] randomID = new int[6];

        List<Item> materials = new List<Item>();
        foreach (Item item in ItemsDatabase.instance.ItemList)
        {
            if (item.ItemType == Item.ItemTypeE.Material)
                materials.Add(item);
        }
        if (materials.Count == 0) return;

        for (int i = 0; i < 6; i++)
        {
            randomC[i] = Random.Range(0, 9);
            randomID[i] = materials[Random.Range(0, materials.Count)].ItemID;

            state.inventory[i].ItemID = randomID[i];
            state.inventory[i].ItemNum = randomC[i];

            if (AllToguether(randomC) > 15)
            {
                continue;
            }
        }
    }
    void SpawnTier3()
    {
        int[] randomC = new int[6];
        int[] randomID = new int[6];

        List<Item> materials = new List<Item>();
        foreach (Item item in ItemsDatabase.instance.ItemList)
        {
            if (item.ItemType == Item.ItemTypeE.BodyArmor)
                materials.Add(item);
        }

        if (materials.Count == 0) return;

        for (int i = 0; i < 6; i++)
        {
            randomC[i] = Random.Range(-10, 1);
            randomID[i] = materials[Random.Range(0, materials.Count)].ItemID;

            if (randomC[i] > 0)
            {
                state.inventory[i].ItemID = randomID[i];
                state.inventory[i].ItemNum = randomC[i];
            }

            if (AllToguether(randomC) > 15)
            {
                continue;
            }
        }
    }
    void SpawnTier4()
    {
        int[] randomC = new int[6];
        int[] randomID = new int[6];

        List<Item> materials = new List<Item>();
        foreach (Item item in ItemsDatabase.instance.ItemList)
        {
            if (item.ItemType == Item.ItemTypeE.BodyArmor)
                materials.Add(item);
            if (item.ItemType == Item.ItemTypeE.Ammo)
                materials.Add(item);
        }
        if (materials.Count == 0) return;

        for (int i = 0; i < 6; i++)
        {
            randomC[i] = Random.Range(-10, 5);
            randomID[i] = materials[Random.Range(0, materials.Count)].ItemID;

            if (randomC[i] > 0)
            {
                state.inventory[i].ItemID = randomID[i];
                state.inventory[i].ItemNum = randomC[i];
            }

            if (AllToguether(randomC) > 15)
            {
                continue;
            }
        }
    }

    int AllToguether(int[] array)
    {
        int c = 0;
        foreach(int i in array)
        {
            c += i;
        }
        return c;
    }
}
