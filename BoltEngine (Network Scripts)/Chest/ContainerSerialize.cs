using UnityEngine;
using System.Collections;

public class ContainerSerialize : Bolt.EntityBehaviour<ITFSChestState>
{
    public int[,] Inventory = new int[4, 4];
    public int[,] InventoryNum = new int[4, 4];

    int id = 0;

    public override void Attached()
    {
        state.AddCallback("inventory[]", UpdateChest);
        //state.AddCallback("id", UpdateID);
    }

    public void UpdateChest(Bolt.IState state, string path, Bolt.ArrayIndices indices)
    {
        int index = indices[0];
        ITFSChestState actorState = (ITFSChestState)state;

        int slotID = 0;
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                if (index == slotID)
                {
                    Inventory[x, y] = actorState.inventory[index].ItemID;
                    InventoryNum[x, y] = actorState.inventory[index].ItemNum;
                }
                slotID++;
            }
        }
    }
}
