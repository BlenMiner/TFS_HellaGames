using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUITEST : MonoBehaviour 
{
    public Texture2D pic1;
    public Texture2D pic2;
    public Texture2D pic3;
    public Texture2D pic4;
    public Texture2D pic5;
    public Texture2D pic6;
    public Texture2D pic7;

    public bool showCrafingInv = false;
    public bool tabOpened = false;
    public bool itemSelected = false;

    Vector2 offset = new Vector2(26.5f, 77.3f);
    float spacing = 12.65f;
    float size = 45.4f;

    Color c = Color.white;
    Color c1 = Color.white;
    Color c2 = Color.white;

    public Image[] im;
    public Image[] icraftingTab;
    public Image[] itemTab;

    public PlayerCraftingSystem pcs;

    void Start()
    {
        c.a = 0;
    }

    void Update()
    {
        if(!showCrafingInv)
        {
            tabOpened = false;
            itemSelected = false;
            c.a = Vector2.Lerp(new Vector2(c.a, 0), new Vector2(0, 0), 0.3f).x;

            DiselectItem();

        }
        else
        {
            c.a = Vector2.Lerp(new Vector2(c.a, 0), new Vector2(Color.white.a, 0), 0.3f).x;
        }

        if (!showCrafingInv || !tabOpened)
        {
            itemSelected = false;
            c1.a = Vector2.Lerp(new Vector2(c1.a, 0), new Vector2(0, 0), 0.8f).x;
        }
        else
        {
            c1.a = Vector2.Lerp(new Vector2(c1.a, 0), new Vector2(Color.white.a, 0), 0.8f).x;
        }

        if (!showCrafingInv || !tabOpened || !itemSelected)
        {
            c2.a = Vector2.Lerp(new Vector2(c2.a, 0), new Vector2(0, 0), 0.8f).x;
        }
        else
        {
            c2.a = Vector2.Lerp(new Vector2(c2.a, 0), new Vector2(Color.white.a, 0), 0.8f).x;
        }
    }
    void OnGUI()
    {
        GUI.color = c;

        foreach(Image i in im)
        {
            i.color = c;
        }
        foreach (Image i in icraftingTab)
        {
            i.color = c1;
        }
        foreach (Image i in itemTab)
        {
            i.color = c2;
        }
        foreach (GameObject i in slots)
        {
            i.GetComponent<Image>().color = c2;
        }

        ShowTabIcons();

        GUI.color = Color.white;
    }
    void ShowTabIcons()
    {
        GUI.DrawTexture(new Rect(offset.x, offset.y, size, size), pic1);
        GUI.DrawTexture(new Rect(offset.x, offset.y + size + spacing, size, size), pic2);
        GUI.DrawTexture(new Rect(offset.x, offset.y + (size * 2) + (spacing * 2), size, size), pic3);
        GUI.DrawTexture(new Rect(offset.x, offset.y + (size * 3) + (spacing * 3), size, size), pic4);
        GUI.DrawTexture(new Rect(offset.x, offset.y + (size * 4) + (spacing * 4), size, size), pic5);
        GUI.DrawTexture(new Rect(offset.x, offset.y + (size * 5) + (spacing * 5), size, size), pic6);
        GUI.DrawTexture(new Rect(offset.x, offset.y + (size * 6) + (spacing * 6), size, size), pic7);

        if(selectedC != null)
        {
            itemSelected = true;
            showCraftInfo();
        }

        Event e = Event.current;
        if (selectedTab == "tools" && tabOpened)
        {
            ShowTools(e);
        }
        else if (selectedTab == "weapons" && tabOpened)
        {
            Showweapons(e);
        }
        else if (selectedTab == "structures" && tabOpened)
        {
            Showstructures(e);
        }
        else if (selectedTab == "vehicles" && tabOpened)
        {
            Showvehicles(e);
        }
        else if (selectedTab == "consumables" && tabOpened)
        {
            Showconsumables(e);
        }
        else if (selectedTab == "miscellaneous" && tabOpened)
        {
            Showmiscellaneous(e);
        }
        else if (selectedTab == "traps" && tabOpened)
        {
            ShowmTraps(e);
        }
    }

    public GameObject[] slots;
    Vector2 infoOffset = new Vector2(131, 382.9f);
    float infosize = 41.3f;
    float infosapce = 19f;
    float yO = 66.86f;
    void showCraftInfo()
    {
        int i = 0;
        for (int x = 0; x < 4; x++ )
        {
            Rect r = new Rect(infoOffset.x + (infosapce * x) + (infosize * x), infoOffset.y, infosize, infosize);

            if (selectedC.ItemsNeeded.Length >= (i + 1))
            {
                GUI.DrawTexture(r, selectedC.ItemsNeeded[i].item.ItemIcon);
                GUI.Label(new Rect(r.x + (infosize / 2) / 2, r.y + infosize + 3, infosize, infosize), pcs.pis.GetItemNum(selectedC.ItemsNeeded[i].item) + "/" + selectedC.ItemsNeeded[i].itemNum);
            }

            i++;
        }
        for (int x = 0; x < 4; x++)
        {
            Rect r = new Rect(infoOffset.x + (infosapce * x) + (infosize * x), infoOffset.y + yO, infosize, infosize);

            if (selectedC.ItemsNeeded.Length >= (i + 1))
            {
                GUI.DrawTexture(r, selectedC.ItemsNeeded[i].item.ItemIcon);
                GUI.Label(new Rect(r.x + (infosize / 2), r.y + infosize + 10, infosize, infosize), pcs.pis.GetItemNum(selectedC.ItemsNeeded[i].item) + "/" + selectedC.ItemsNeeded[i].itemNum);
            }

            i++;
        }
    }

    List<ItemCraft> tools = new List<ItemCraft>();
    List<ItemCraft> weapons = new List<ItemCraft>();
    List<ItemCraft> structures = new List<ItemCraft>();
    List<ItemCraft> vehicles = new List<ItemCraft>();
    List<ItemCraft> consumables = new List<ItemCraft>();
    List<ItemCraft> traps = new List<ItemCraft>();
    List<ItemCraft> miscellaneous = new List<ItemCraft>();

    ItemCraft selectedC = null;

    void ShowTools(Event e)
    {
        int i = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (tools.Count >= (i + 1))
                {
                    ItemCraft c = tools[i];
                    Rect rect = new Rect(tabOffset.x + (x * tabSize) + (tabSpacing * x), tabOffset.y + (y * tabSize) + (tabSpacing * y), tabSize, tabSize);

                    if (rect.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0){selectedC = c;}if (c.Item.ItemIcon != null)
                        GUI.DrawTexture(rect, c.Item.ItemIcon);
                }
                i++;
            }
        }
    }
    void Showweapons(Event e)
    {
        int i = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (weapons.Count >= (i + 1))
                {
                    ItemCraft c = weapons[i];
                    Rect rect = new Rect(tabOffset.x + (x * tabSize) + (tabSpacing * x), tabOffset.y + (y * tabSize) + (tabSpacing * y), tabSize, tabSize);

                    if (rect.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0){selectedC = c;}if (c.Item.ItemIcon != null)
                        GUI.DrawTexture(rect, c.Item.ItemIcon);
                }
                i++;
            }
        }
    }
    void Showstructures(Event e)
    {
        int i = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (structures.Count >= (i + 1))
                {
                    ItemCraft c = structures[i];
                    Rect rect = new Rect(tabOffset.x + (x * tabSize) + (tabSpacing * x), tabOffset.y + (y * tabSize) + (tabSpacing * y), tabSize, tabSize);

                    if (rect.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0){selectedC = c;}if (c.Item.ItemIcon != null)
                        GUI.DrawTexture(rect, c.Item.ItemIcon);
                }
                i++;
            }
        }
    }
    void Showvehicles(Event e)
    {
        int i = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (vehicles.Count >= (i + 1))
                {
                    ItemCraft c = vehicles[i];
                    Rect rect = new Rect(tabOffset.x + (x * tabSize) + (tabSpacing * x), tabOffset.y + (y * tabSize) + (tabSpacing * y), tabSize, tabSize);

                    if (rect.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0){selectedC = c;}if (c.Item.ItemIcon != null)
                        GUI.DrawTexture(rect, c.Item.ItemIcon);
                }
                i++;
            }
        }
    }
    void Showconsumables(Event e)
    {
        int i = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (consumables.Count >= (i + 1))
                {
                    ItemCraft c = consumables[i];
                    Rect rect = new Rect(tabOffset.x + (x * tabSize) + (tabSpacing * x), tabOffset.y + (y * tabSize) + (tabSpacing * y), tabSize, tabSize);

                    if (rect.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0){selectedC = c;}if (c.Item.ItemIcon != null)
                        GUI.DrawTexture(rect, c.Item.ItemIcon);
                }
                i++;
            }
        }
    }
    void Showmiscellaneous(Event e)
    {
        int i = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (miscellaneous.Count >= (i + 1))
                {
                    ItemCraft c = miscellaneous[i];
                    Rect rect = new Rect(tabOffset.x + (x * tabSize) + (tabSpacing * x), tabOffset.y + (y * tabSize) + (tabSpacing * y), tabSize, tabSize);

                    if (rect.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0){selectedC = c;}if (c.Item.ItemIcon != null)
                        GUI.DrawTexture(rect, c.Item.ItemIcon);
                }
                i++;
            }
        }
    }
    void ShowmTraps(Event e)
    {
        int i = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (traps.Count >= (i + 1))
                {
                    ItemCraft c = traps[i];
                    Rect rect = new Rect(tabOffset.x + (x * tabSize) + (tabSpacing * x), tabOffset.y + (y * tabSize) + (tabSpacing * y), tabSize, tabSize);

                    if (rect.Contains(e.mousePosition) && e.type == EventType.mouseDown && e.button == 0){selectedC = c;}if (c.Item.ItemIcon != null)
                        GUI.DrawTexture(rect, c.Item.ItemIcon);
                }
                i++;
            }
        }
    }

    Vector2 tabOffset = new Vector2(147.2f, 129.8f);
    float tabSpacing = 12.65f;
    float tabSize = 38.9f;

    string selectedTab = "";
    public void OpenTabTools()
    {
        DiselectItem();
        tools.Clear();
        if (showCrafingInv == false) return;

        selectedTab = "tools";
        tabOpened = true;

        List<ItemCraft> craftings = PlayerCraftingSystem.crafting;
        foreach(ItemCraft c in craftings)
        {
            if (c.ItemType == ItemCraft.ItemT.Tools)
                tools.Add(c);
        }        
    }
    public void OpenTabWeapons()
    {
        DiselectItem();
        weapons.Clear();
        if (showCrafingInv == false) return;

        selectedTab = "weapons";
        tabOpened = true;

        List<ItemCraft> craftings = PlayerCraftingSystem.crafting;
        foreach (ItemCraft c in craftings)
        {
            if (c.ItemType == ItemCraft.ItemT.Weapons)
                weapons.Add(c);
        }
    }
    public void OpenTabStructures()
    {
        DiselectItem();
        structures.Clear();
        if (showCrafingInv == false) return;

        selectedTab = "structures";
        tabOpened = true;

        List<ItemCraft> craftings = PlayerCraftingSystem.crafting;
        foreach (ItemCraft c in craftings)
        {
            if (c.ItemType == ItemCraft.ItemT.Structures)
                structures.Add(c);
        }
    }
    public void OpenTabVehicles()
    {
        DiselectItem();
        vehicles.Clear();
        if (showCrafingInv == false) return;

        selectedTab = "vehicles";
        tabOpened = true;

        List<ItemCraft> craftings = PlayerCraftingSystem.crafting;
        foreach (ItemCraft c in craftings)
        {
            if (c.ItemType == ItemCraft.ItemT.Vehicles)
                vehicles.Add(c);
        }
    }
    public void OpenTabConsumables()
    {
        DiselectItem();
        consumables.Clear();
        if (showCrafingInv == false) return;

        selectedTab = "consumables";
        tabOpened = true;

        List<ItemCraft> craftings = PlayerCraftingSystem.crafting;
        foreach (ItemCraft c in craftings)
        {
            if (c.ItemType == ItemCraft.ItemT.Consumables)
                consumables.Add(c);
        }
    }
    public void OpenTabMiscellaneous()
    {
        DiselectItem();
        miscellaneous.Clear();
        if (showCrafingInv == false) return;

        selectedTab = "miscellaneous";
        tabOpened = true;

        List<ItemCraft> craftings = PlayerCraftingSystem.crafting;
        foreach (ItemCraft c in craftings)
        {
            if (c.ItemType == ItemCraft.ItemT.Miscellaneous)
                miscellaneous.Add(c);
        }
    }
    public void OpenTabTraps()
    {
        DiselectItem();
        traps.Clear();
        if (showCrafingInv == false) return;

        selectedTab = "traps";
        tabOpened = true;

        List<ItemCraft> craftings = PlayerCraftingSystem.crafting;
        foreach (ItemCraft c in craftings)
        {
            if (c.ItemType == ItemCraft.ItemT.Traps)
                traps.Add(c);
        }
    }

    void DiselectItem()
    {
        selectedC = null;
        itemSelected = false;
    }

    public void Craft()
    {
        CraftItem craft = new CraftItem();
        craft.entity = pcs.entity;
        craft.itemID = selectedC.Item.ItemID;
        craft.Send();

        DiselectItem();
    }
}