using UnityEngine;
using System.Collections;

public class OutsidePlayerWeapon : MonoBehaviour
{
    ITFSPlayerState state;
    public BoltEntity entity;
    public CameraObj cameraObj;

    public PlayerInventoryShow pis;
    public GameObject[] weapons;

    public Animator character;

    int lastID;
	void Update ()
    {
        if (cameraObj.Camera.activeSelf == true)
            return;

        if (cameraObj.Camera.transform.parent.gameObject.activeSelf == true && BoltNetwork.isClient)
        {
            cameraObj.Camera.transform.parent.gameObject.SetActive(false);
        }

        if (entity.isAttached)
        {
            if (state == null)
            {
                state = entity.GetState<ITFSPlayerState>();
                return;
            }
            if (!lastID.Equals(state.selectedItemID))
            {
                diselectAll();
                GameObject weapon = getWeapon(pis.getItem(state.selectedItemID).ItemName);

                if (weapon != null)
                {
                    getWeapon(pis.getItem(state.selectedItemID).ItemName).SetActive(true);

                    if(pis.getItem(state.selectedItemID).WeaponType == WeaponType.H1)
                    {
                        for (int i = 1; i < character.layerCount; i++)
                        {
                            character.SetLayerWeight(i, 0f);
                        }
                        character.SetLayerWeight(1, 1f);
                    }
                    else if (pis.getItem(state.selectedItemID).WeaponType == WeaponType.H2)
                    {

                    }
                    else if (pis.getItem(state.selectedItemID).WeaponType == WeaponType.Spear)
                    {
                        for (int i = 1; i < character.layerCount; i++)
                        {
                            character.SetLayerWeight(i, 0f);
                        }
                        character.SetLayerWeight(3, 1f);
                    }
                    else if (pis.getItem(state.selectedItemID).WeaponType == WeaponType.CrossBow)
                    {

                    }
                }
                else
                {
                    for (int i = 1; i < character.layerCount; i++)
                    {
                        character.SetLayerWeight(i, 0f);
                    }
                    character.SetLayerWeight(2, 1f);
                }

                lastID = state.selectedItemID;
            }
        }
	}

    GameObject getWeapon(string name)
    {
        foreach(GameObject go in weapons)
        {
            if(go.name == name)
            {
                return go;
            }
        }
        return null;
    }
    void diselectAll()
    {
        foreach (GameObject go in weapons)
        {
           go.SetActive(false);
        }
    }
}
