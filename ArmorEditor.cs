using UnityEngine;
using System.Collections;
using Bolt;

public class ArmorEditor : EntityBehaviour<ITFSPlayerState> 
{
    public DesertClothes desertClothes;
    
    public override void Attached()
    {
        desertClothes.DisableAll();
    }

    int[] armor = new int[4];
    void Update()
    {
        for(int i = 0; i < 4; i++)
        {
            if(armor[i] != state.armorSlots[i].ItemID)
            {
                armor[i] = state.armorSlots[i].ItemID;
                UpdateArmor();
            }
        }
    }

    public void UpdateArmor()
    {
        ITFSPlayerState actorState = (ITFSPlayerState)state;

        if (actorState.armorSlots[0].ItemID > 0)
        {
            desertClothes.Head.SetActive(true);
        }
        else
        {
            desertClothes.Head.SetActive(false);
        }
        if (actorState.armorSlots[1].ItemID > 0)
        {
            desertClothes.Body.SetActive(true);
        }
        else
        {
            desertClothes.Body.SetActive(false);
        }
        if (actorState.armorSlots[2].ItemID > 0)
        {
            desertClothes.Pants.SetActive(true);
        }
        else
        {
            desertClothes.Pants.SetActive(false);
        }
        if (actorState.armorSlots[3].ItemID > 0)
        {
            desertClothes.Shoes.SetActive(true);
        }
        else
        {
            desertClothes.Shoes.SetActive(false);
        }
    }
}

[System.Serializable]
public class DesertClothes
{
    public GameObject Head;
    public GameObject Body;
    public GameObject Pants;
    public GameObject Shoes;

    public void DisableAll()
    {
        Head.SetActive(false);
        Body.SetActive(false);
        Pants.SetActive(false);
        Shoes.SetActive(false);
    }
}
