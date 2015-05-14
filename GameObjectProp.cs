using UnityEngine;
using System.Collections;

public class GameObjectProp : MonoBehaviour
{
    public bool HideIfMine = false;
    public bool DeleteIfNotMine = false;
    public bool EnableColliderIfNotMine = false;
    public NetworkView nv;
    public Collider[] c;
    public bool DestroyAfterSeconds = false;
    public float DestroyTimer = 2f;

	void Start () {
        if (nv != null)
        {
            if (HideIfMine && nv.isMine)
            {
                gameObject.SetActive(false);
            }
            if (!nv.isMine && DeleteIfNotMine)
            {
                Destroy(gameObject);
            }
            if (EnableColliderIfNotMine && !nv.isMine)
            {
                foreach (Collider co in c)
                    co.enabled = true;
            }
            else
            {
                foreach (Collider co in c)
                    co.enabled = false;
            }
        }
        if(DestroyAfterSeconds)
        {
            Destroy(gameObject, DestroyTimer);
        }
	}
}
