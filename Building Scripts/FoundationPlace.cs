using UnityEngine;
using System.Collections;

public class FoundationPlace : MonoBehaviour
{
    public Transform mainF;
    public Vector3 Position;

    void Start()
    {
        StartCoroutine(checkIfShouldDisable());
    }

    public Vector3 GetPosition()
    {
        return mainF.transform.position + Position;
    }
    public Transform GetFoundationP()
    {
        return mainF.transform;
    }

    IEnumerator checkIfShouldDisable()
    {
        for(int i = 0; i < ServerInventoryCallbacks.foundations.Count; i++)
        {
            if ((ServerInventoryCallbacks.foundations.Count - 1 < i) || ServerInventoryCallbacks.foundations[i] == null)
                break;
            if (Vector3.Distance(transform.position, ServerInventoryCallbacks.foundations[i].position) < 2)
            {
                Destroy(gameObject);
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(checkIfShouldDisable());
    }
}
