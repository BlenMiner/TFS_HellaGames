using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemsDatabase : MonoBehaviour
{
	public List<Item> ItemList = new List<Item>();
    public List<ItemCraft> Crafting = new List<ItemCraft>();

    public BuildingObjects building;
    public GameObject[] WeaponModels;

    public GameObject chopwood;

    public static ItemsDatabase instance;

    public AudioClip countdown;
    public AudioClip aftercountdown;

    void Awake()
    {
        instance = this;
        LoadItemsAndCraftings();
    }

	public void LoadItemsAndCraftings () {
        RegisterItems();
		RegisterCraftings();
	}

    GameObject getGameObject(string i, GameObject[] weapons)
    {
        foreach(GameObject go in weapons)
        {
            if (go.name == i)
                return go;
        }
        return null;
    }
    void RegisterItems()
	{
		//Weapons
        ItemList.Add(new Item("M8", 1, Item.ItemTypeE.Weapon, 45, 1f, WeaponType.H1, WeaponModels[0], 2, ItemList, "Pretty ... thing"));
        ItemList.Add(new Item("S2", 1, Item.ItemTypeE.Weapon, 40, 1f, WeaponType.H1, WeaponModels[1], 2, ItemList, "Pretty sword"));

        ItemList.Add(new Item("SpearP4", 1, Item.ItemTypeE.Weapon, 45, 1.8f, WeaponType.Spear, WeaponModels[2], 3, ItemList, "Pretty spear"));
        ItemList.Add(new Item("Crossbow", 1, Item.ItemTypeE.Weapon, 80, 2f, WeaponType.CrossBow, WeaponModels[3], 3, ItemList, "Pretty crossbow"));

        ItemList.Add(new Item("Torch", 1, Item.ItemTypeE.Weapon, 0, 1f, WeaponType.Torch, WeaponModels[4], 3, ItemList, "The light in the end of the tunel."));
        ItemList.Add(new Item("Machete", 1, Item.ItemTypeE.Weapon, 20f, 1f, WeaponType.H1, WeaponModels[4], 3, ItemList, "Used mainly to skin animals."));
		
		//Ammo
        ItemList.Add(new Item("Arrow", 25, Item.ItemTypeE.Ammo, 1, ItemList, "Multi-use stick !"));
        ItemList.Add(new Item("Poison Arrow", 25, Item.ItemTypeE.Ammo, 1, ItemList, "Multi-use stick !"));
        ItemList.Add(new Item("Fire Arrow", 25, Item.ItemTypeE.Ammo, 1, ItemList, "Multi-use stick !"));
		
		//Armor
        ItemList.Add(new Item("DesertHelmet", 1, Item.ItemTypeE.HeadArmor, 1, 0.2f, ItemList, "Keep your self warm."));
        ItemList.Add(new Item("DesertBody", 1, Item.ItemTypeE.BodyArmor, 1, 0.9f, ItemList, "Dont show your tities."));
        ItemList.Add(new Item("DesertLegs", 1, Item.ItemTypeE.LegsArmor, 1, 0.5f, ItemList, "Keep your self from peeing."));
        ItemList.Add(new Item("DesertShoes", 1, Item.ItemTypeE.ShooesArmor, 1, 0.1f, ItemList, "Keep your feet warm."));
		
		//Grenades
		
		//Tools
        ItemList.Add(new Item("hatchet", 1, Item.ItemTypeE.Tool, 1, ItemList, "Is this a tool or a weapon?"));
		
		//Food
		
		//Building
		//ItemList.Add(new Item("container", 5, Item.ItemTypeE.Building, building.ChestModel,ItemList));
        ItemList.Add(new Item("Door", 5, Item.ItemTypeE.Building, building.Door, 1, ItemList, "Its used for privacy."));
        ItemList.Add(new Item("Wall1", 5, Item.ItemTypeE.Building, building.Wall1, 1, ItemList, "Protects you agains harmeless survivors."));
        ItemList.Add(new Item("Foundation", 5, Item.ItemTypeE.Building, building.Foundation, 1, ItemList, "Something to step on."));
        ItemList.Add(new Item("Roof", 5, Item.ItemTypeE.Building, building.Roof, 1, ItemList, "To cover your head from an airdrop.."));
        ItemList.Add(new Item("WoodDoor", 5, Item.ItemTypeE.Building, building.WoodDoor, 1, ItemList, "Its used to close your self from the world."));
        ItemList.Add(new Item("Window", 5, Item.ItemTypeE.Building, building.Window, 1, ItemList, "Used to watch your neightbor get naked."));
        ItemList.Add(new Item("Fence", 5, Item.ItemTypeE.Building, building.Fence, 1, ItemList, "Blocks the passage of inanimated objects."));
        ItemList.Add(new Item("StairWay", 5, Item.ItemTypeE.Building, building.StairWay, 1, ItemList, "Idk what this is used for."));
        ItemList.Add(new Item("Stick", 5, Item.ItemTypeE.Building, building.Stick, 1, ItemList, "Pillar."));

        ItemList.Add(new Item("ClothTent", 1, Item.ItemTypeE.Building, building.ClothTent, 1, ItemList, "Keep your self warm."));
        ItemList.Add(new Item("ClothHut", 1, Item.ItemTypeE.Building, building.ClothHut, 1, ItemList, "Keep your self warm."));
        
        ItemList.Add(new Item("campfire", 5, Item.ItemTypeE.Building, building.campFire, 1, ItemList, "Pillar."));
        
        //Trap        
        ItemList.Add(new Item("DonkeyMeat", 16, Item.ItemTypeE.None, building.Trap1, 1, ItemList, "Get your calories."));
        ItemList.Add(new Item("Trap1", 5, Item.ItemTypeE.Trap, building.Trap1, 1, ItemList, "Nothing to worry."));

		//Misc
        ItemList.Add(new Item("healthshot", 250, Item.ItemTypeE.None, 1, ItemList, "Increases health."));
        ItemList.Add(new Item("staminashot", 250, Item.ItemTypeE.None, 1, ItemList, "Increases stamina!"));
		
		//Others
        ItemList.Add(new Item("Wood", 250, Item.ItemTypeE.Material, 1, ItemList, ""));
        ItemList.Add(new Item("Stone", 250, Item.ItemTypeE.Material, 1, ItemList, ""));
        ItemList.Add(new Item("syringe", 250, Item.ItemTypeE.Material2, 1, ItemList, ""));
        ItemList.Add(new Item("redplant", 250, Item.ItemTypeE.None, 1, ItemList, ""));
        ItemList.Add(new Item("blueplant", 250, Item.ItemTypeE.None, 1, ItemList, ""));
        ItemList.Add(new Item("leather", 250, Item.ItemTypeE.Material2, 1, ItemList, ""));
        ItemList.Add(new Item("cloth", 250, Item.ItemTypeE.Material, 1, ItemList, ""));
        ItemList.Add(new Item("string", 250, Item.ItemTypeE.Material2, 1, ItemList, ""));
        ItemList.Add(new Item("glass", 250, Item.ItemTypeE.Material2, 1, ItemList, ""));
        ItemList.Add(new Item("rope", 250, Item.ItemTypeE.Material, 1, ItemList, ""));
        ItemList.Add(new Item("leaves", 250, Item.ItemTypeE.Material, 1, ItemList, ""));
        ItemList.Add(new Item("fur", 250, Item.ItemTypeE.None, 1, ItemList, ""));
        ItemList.Add(new Item("metal", 250, Item.ItemTypeE.Material2, 1, ItemList, ""));
        ItemList.Add(new Item("nail", 250, Item.ItemTypeE.Material, 1, ItemList, ""));
        ItemList.Add(new Item("gause", 250, Item.ItemTypeE.None, 1, ItemList, ""));
        ItemList.Add(new Item("Expl_Barrel", 250, Item.ItemTypeE.None, 1, ItemList, ""));
	}
	void RegisterCraftings()
	{
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 2), new ItemInfo(getItem("Stone"), 3) }, getItem("hatchet"), ItemCraft.ItemT.Tools, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Stone"), 1), new ItemInfo(getItem("Wood"), 6) }, getItem("SpearP4"), ItemCraft.ItemT.Weapons, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Stone"), 2), new ItemInfo(getItem("Wood"), 10) }, getItem("Campfire"), ItemCraft.ItemT.Miscellaneous, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Leather"), 5), new ItemInfo(getItem("String"), 10) }, getItem("Parachute"), ItemCraft.ItemT.Miscellaneous, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("String"), 2), new ItemInfo(getItem("Leaves"), 30) }, getItem("Camoflauge"), ItemCraft.ItemT.Armor, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 8), new ItemInfo(getItem("Stone"), 8) }, getItem("Firepit"), ItemCraft.ItemT.Miscellaneous, 1));
        //Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Rope"), 1), new ItemInfo(getItem("Leather"), 10) }, getItem("DonkeySaddle"), ItemCraft.ItemT.Tools, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Arrow"), 2), new ItemInfo(getItem("Poison"), 1) }, getItem("Poison Arrow"), ItemCraft.ItemT.Miscellaneous, 2));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Leather"), 1), new ItemInfo(getItem("String"), 1) }, getItem("LegArmorPants"), ItemCraft.ItemT.Armor, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 2), new ItemInfo(getItem("Cloth"), 1) }, getItem("Torch"), ItemCraft.ItemT.Armor, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 2), new ItemInfo(getItem("Stone"), 2) }, getItem("Arrow"), ItemCraft.ItemT.Weapons, 1));


        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Glass"), 5) }, getItem("3b1"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Metal"), 2), new ItemInfo(getItem("Rope"), 3), new ItemInfo(getItem("Wood"), 4) }, getItem("3b2"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Rope"), 2)}, getItem("3b3"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Metal"), 2), new ItemInfo(getItem("Stone"), 12), new ItemInfo(getItem("Rope"), 4) }, getItem("3b4"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Stone"), 2), new ItemInfo(getItem("Rope"), 3), new ItemInfo(getItem("Wood"), 4) }, getItem("2a"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Rope"), 3), new ItemInfo(getItem("Wood"), 8) }, getItem("2b"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Rope"), 3), new ItemInfo(getItem("Wood"), 8) }, getItem("2c"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Metal"), 2), new ItemInfo(getItem("Rope"), 3), new ItemInfo(getItem("Wood"), 4) }, getItem("A1"), ItemCraft.ItemT.Traps, 1));


        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 8), new ItemInfo(getItem("Nail"), 4)}, getItem("Fence"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 20), new ItemInfo(getItem("Nail"), 10) }, getItem("Wall1"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 16), new ItemInfo(getItem("Nail"), 4) }, getItem("Window"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 4), new ItemInfo(getItem("Nail"), 4) }, getItem("Door"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 24), new ItemInfo(getItem("Nail"), 16) }, getItem("Foundation"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 8), new ItemInfo(getItem("Nail"), 4) }, getItem("Stairway"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 16), new ItemInfo(getItem("Nail"), 8) }, getItem("WoodDoor"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Leaves"), 30), new ItemInfo(getItem("Wood"), 4), new ItemInfo(getItem("Rope"), 10) }, getItem("Roof"), ItemCraft.ItemT.Structures, 1));


		Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("redplant"), 40), new ItemInfo(getItem("syringe"), 1) }, getItem("healthshot"), ItemCraft.ItemT.Miscellaneous, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("blueplant"), 40), new ItemInfo(getItem("syringe"), 1) }, getItem("staminashot"), ItemCraft.ItemT.Miscellaneous, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Arrow"), 2), new ItemInfo(getItem("cloth"), 1) }, getItem("Fire Arrow"), ItemCraft.ItemT.Weapons, 2));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 4), new ItemInfo(getItem("nails"), 4) }, getItem("container"), ItemCraft.ItemT.Structures, 2));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Cloth"), 5), new ItemInfo(getItem("String"), 1) }, getItem("Gauze"), ItemCraft.ItemT.Miscellaneous, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 2), new ItemInfo(getItem("Stone"), 3) }, getItem("Hatchet"), ItemCraft.ItemT.Tools, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 6), new ItemInfo(getItem("Stone"), 1) }, getItem("Spear"), ItemCraft.ItemT.Tools, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 8) }, getItem("Stick"), ItemCraft.ItemT.Tools, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 10), new ItemInfo(getItem("Cloth"), 1) }, getItem("FireArrow"), ItemCraft.ItemT.Weapons, 2));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Leather"), 5), new ItemInfo(getItem("String"), 4) }, getItem("Parachute"), ItemCraft.ItemT.Miscellaneous, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 4), new ItemInfo(getItem("Nail"), 4) }, getItem("Box"), ItemCraft.ItemT.Structures, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("String"), 2), new ItemInfo(getItem("Leaves"), 30) }, getItem("Camoflauge"), ItemCraft.ItemT.Armor, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 4), new ItemInfo(getItem("Stone"), 1) }, getItem("Chest"), ItemCraft.ItemT.Structures, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 8), new ItemInfo(getItem("Stone"), 8) }, getItem("Firepit"), ItemCraft.ItemT.Structures, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Arrow"), 2), new ItemInfo(getItem("Poison"), 1) }, getItem("Poisonarrow"), ItemCraft.ItemT.Weapons, 2));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Cloth"), 8), new ItemInfo(getItem("String"), 10) }, getItem("LegArmorPants"), ItemCraft.ItemT.Armor, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Cloth"), 8), new ItemInfo(getItem("String"), 16) }, getItem("LegArmorShirt"), ItemCraft.ItemT.Armor, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Cloth"), 1), new ItemInfo(getItem("Wood"), 2) }, getItem("Torch"), ItemCraft.ItemT.Tools, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Stone"), 2), new ItemInfo(getItem("Wood"), 2) }, getItem("arrow"), ItemCraft.ItemT.Weapons, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Glass"), 5) }, getItem("3b1"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Rope"), 3), new ItemInfo(getItem("Wood"), 8) }, getItem("2C"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Rope"), 3), new ItemInfo(getItem("Wood"), 8) }, getItem("2B"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Rope"), 6), new ItemInfo(getItem("Stone"), 1) }, getItem("C3"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Metal"), 4), new ItemInfo(getItem("Wood"), 4) }, getItem("Trap"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 2), new ItemInfo(getItem("Smokecan"), 1), new ItemInfo(getItem("Rope"), 3), new ItemInfo(getItem("Stone"), 2) }, getItem("3A"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("String"), 1), new ItemInfo(getItem("Rope"), 10), new ItemInfo(getItem("Cloth"), 18), new ItemInfo(getItem("Metal"), 1) }, getItem("Hanglider"), ItemCraft.ItemT.Miscellaneous, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 15), new ItemInfo(getItem("Cloth"), 8), new ItemInfo(getItem("Rope"), 12) }, getItem("Raft2B"), ItemCraft.ItemT.Traps, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 4), new ItemInfo(getItem("Nail"), 4) }, getItem("FenceSection"), ItemCraft.ItemT.Structures, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 8), new ItemInfo(getItem("Nail"), 4) }, getItem("EntranceBarricade"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 8), new ItemInfo(getItem("Nail"), 4) }, getItem("WindowBarricade"), ItemCraft.ItemT.Structures, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 24), new ItemInfo(getItem("Nail"), 26) }, getItem("CraftableTower"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 24), new ItemInfo(getItem("Nail"), 16), new ItemInfo(getItem("Metal"), 2) }, getItem("CraftableCampGate"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 24), new ItemInfo(getItem("Nail"), 10) }, getItem("CraftableCampWall"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 4), new ItemInfo(getItem("leather"), 10) }, getItem("Tepee"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Cloth"), 2), new ItemInfo(getItem("Wood"), 4), new ItemInfo(getItem("leather"), 4) }, getItem("Halftent"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 6), new ItemInfo(getItem("Cloth"), 4), new ItemInfo(getItem("Rope"), 6) }, getItem("Tent"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 15) }, getItem("Halfwoodentent"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 30) }, getItem("WoodenTent"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Wood"), 6), new ItemInfo(getItem("Leaves"), 15), new ItemInfo(getItem("Rope"), 6) }, getItem("StrawTent"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Cloth"), 8), new ItemInfo(getItem("Wood"), 20), new ItemInfo(getItem("Rope"), 6) }, getItem("Tepee"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Leaves"), 60), new ItemInfo(getItem("Wood"), 60), new ItemInfo(getItem("Rope"), 6) }, getItem("WoodenHut"), ItemCraft.ItemT.Structures, 1));

        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Cloth"), 4), new ItemInfo(getItem("Wood"), 4), new ItemInfo(getItem("Rope"), 4) }, getItem("ClothTent"), ItemCraft.ItemT.Structures, 1));
        Crafting.Add(new ItemCraft(new ItemInfo[] { new ItemInfo(getItem("Cloth"), 2), new ItemInfo(getItem("Wood"), 6), new ItemInfo(getItem("Rope"), 6) }, getItem("ClothHut"), ItemCraft.ItemT.Structures, 1));

	}

	public static Item getItem(int id)
	{
		foreach (Item i in ItemsDatabase.instance.ItemList)
		{
			if (i.ItemID == id)
			{
				return i;
			}
		}
		
		return null;
	}
    public static Item getItem(string name)
	{
        foreach (Item i in ItemsDatabase.instance.ItemList)
		{
            if (i.ItemName.ToLower() == name.ToLower())
            {
                return i;
            }
            if (i.ItemName == name)
            {
                return i;
            }
		}
        foreach (Item i in ItemsDatabase.instance.ItemList)
        {
            if (i.ItemName.ToLower() == name.ToLower())
            {
                return i;
            }
            if (i.ItemName == name)
            {
                return i;
            }
            if (i.ItemName.StartsWith(name))
            {
                return i;
            }
            if (i.ItemName.EndsWith(name))
            {
                return i;
            }
            if (i.ItemName.Contains(name))
            {
                return i;
            }
            if (name.Contains(i.ItemName))
            {
                return i;
            }
        }
		
		return new Item();
	}
}
