using UnityEngine;
using System.Collections;

public class DeleteWhenNeeded : MonoBehaviour 
{
    public static bool working = false;

    public string tags = "Roof";

    /*void Update()
    {
        if (working == false)
            StartCoroutine(checkIfShouldDisable());
    }*/


    IEnumerator checkIfShouldDisable()
    {
        working = true;

        GameObject[] roofs = GameObject.FindGameObjectsWithTag(tags);
        foreach (GameObject t in roofs)
        {
            if (Vector3.Distance(transform.position, t.transform.position) < 3)
            {
                working = false;
                Destroy(gameObject);
                break;
            }
            yield return new WaitForEndOfFrame();
        }

        working = false;
        yield return new WaitForEndOfFrame();
    }
}
