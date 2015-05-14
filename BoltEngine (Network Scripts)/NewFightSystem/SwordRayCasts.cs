using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordRayCasts : MonoBehaviour 
{
    public Transform point0;
    public Transform point1;

    public int pointCount = 10;

    public Transform[] points;
    public Vector3[] lastPosition;

    public GameObject bulletHole;

    void Update()
    {
        int i = 0;
        foreach (Transform t in points)
        {
            float d = Vector3.Distance(lastPosition[i], t.position);

            Vector3 fwd = (t.position - lastPosition[i]).normalized;

            if (d > 0.08f)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Ray(t.position, fwd), out hit, 1f))
                {
                    Destroy(GameObject.Instantiate(bulletHole, hit.point, Quaternion.identity) as GameObject, 0.5f);
                }
            }

            lastPosition[i] = t.position;
            i++;
        }
    }

    [ContextMenu("Create RayCastPoints")]
    void SetupSkinnedMesh()
    {
        foreach(Transform t in points)
        {
            if(t != null)
            {
                DestroyImmediate(t.gameObject);
            }
        }

        points = new Transform[pointCount + 1];
        lastPosition = new Vector3[pointCount + 1];

        Vector3 distance = ((point1.position) - point0.position) / pointCount;

        for (int i = 0; i <= pointCount; i++)
        {
            GameObject point = new GameObject("point" + i);

            point.transform.parent = point0.transform;

            point.transform.position = point0.position + (distance * i);
            points[i] = point.transform;
            lastPosition[i] = point.transform.position;
        }
    }
}
