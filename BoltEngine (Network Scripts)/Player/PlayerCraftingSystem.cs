using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCraftingSystem : Bolt.EntityBehaviour<ITFSPlayerState> 
{
    public bool debugMod = false;

    bool mine = false;
    public PlayerInventoryShow pis;

    public static List<Item> items;
    public static List<ItemCraft> crafting;

    public GameObject craftingMenu;
    public GUITEST craftingController;
    
    public void AttachedD()
    {
        items = pis.ItemList;
        crafting = pis.Crafting;

        mine = true;

        craftingMenu = GameObject.FindGameObjectWithTag("CraftingMenu");
        if (craftingMenu == null)
        {
            this.enabled = false;
            return;
        }
        craftingController = craftingMenu.GetComponent<GUITEST>();

        craftingController.pcs = this;
    }
    
    void Update()
    {
        if (!mine) return;
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCraftingMenu tCM = ToggleCraftingMenu.Create(Bolt.GlobalTargets.OnlyServer);
            //using (var evnt = ToggleCraftingMenu.Raise(Bolt.GlobalTargets.OnlyServer))
            {
                tCM.entity = entity;
            }
            tCM.Send();
           
        }
        if (craftingController != null)
            craftingController.showCrafingInv = state.crafting;
    }

    void OnGUI()
    {
        if (mine || debugMod)
        {
            Event e = Event.current;
            RenderCraftingGUI(e);
        }
    }
    void RenderCraftingGUI(Event e)
    {

    }

    public static Item getItem(int id)
    {
        foreach (Item i in items)
        {
            if (i.ItemID == id)
            {
                return i;
            }
        }

        return new Item();
    }
    public static Item getItem(string name)
    {
        foreach (Item i in items)
        {
            if (i.ItemName == name)
            {
                return i;
            }
        }

        return new Item();
    }
}
