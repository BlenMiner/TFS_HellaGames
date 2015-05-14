using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour
{
    public float width = 3f;
    public float height = 3f;

    public Color gridColor = Color.green;
    public Color color = Color.green;

    void OnDrawGizmos()
    {
        Vector3 pos = Camera.current.transform.position;
        Gizmos.color = gridColor;

        for (float y = pos.y - 5800f; y < pos.y + 5800f; y += height)
        {
            Gizmos.DrawLine(new Vector3(Mathf.Floor(y / height) * height, 0, -1000000f),
                            new Vector3(Mathf.Floor(y / height) * height, 0, 1000000f));
        }

        for (float x = pos.x - 5800f; x < pos.x + 5800f; x += width)
        {
            Gizmos.DrawLine(new Vector3(-1000000f, 0, Mathf.Floor(x / width) * width),
                            new Vector3(1000000f, 0, Mathf.Floor(x / width) * width));
        }

        Gizmos.color = color;
        foreach(GameObject blocked in GameObject.FindGameObjectsWithTag("BlockedArea"))
        {
            Gizmos.DrawCube(blocked.transform.position, new Vector3(blocked.GetComponent<BoxCollider>().size.x, blocked.GetComponent<BoxCollider>().size.y, blocked.GetComponent<BoxCollider>().size.z));
        }
    }
}
