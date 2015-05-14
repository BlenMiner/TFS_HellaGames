using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Equipmentizer : MonoBehaviour
{
    public GameObject parent;
    public GameObject joinTo;

    List<Transform> originalBones = new List<Transform>();
    List<Transform> myBones = new List<Transform>();

    [ContextMenu ("Autopopulate")]
    void SetupSkinnedMesh()
    {
        foreach (SkinnedMeshRenderer m in joinTo.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            foreach(Transform t in m.bones)
            {
                originalBones.Add(t);
            }
        }
        foreach (SkinnedMeshRenderer m in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            m.transform.parent = parent.transform;
            foreach (Transform t in m.bones)
            {
                myBones.Add(t);
            }
        }

        foreach(Transform t in myBones)
        {
            Transform oBone = getBone(t.name);
            if (oBone != null)
            {
                t.parent = oBone;
                t.rotation = oBone.rotation;
                t.position = oBone.position;
            }
            else
            {
                Debug.Log("Cant find : " + t.name);
            }
        }
    }
    Transform getBone(string n)
    {
        foreach(Transform t in originalBones)
        {
            if(t.name.Contains(n))
            {
                return t;
            }
        }
        return null;
    }
}
