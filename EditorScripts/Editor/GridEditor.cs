using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor
{
    Grid grid;

    void OnEnable()
    {
        grid = (Grid)target;
    }

    void OnSceneGUI()
    {
        int controlid = GUIUtility.GetControlID(FocusType.Passive);
        Event e = Event.current;
        Ray ray = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
        Vector3 mousePos = ray.origin;
        if (e.isMouse && e.type == EventType.MouseDown && e.button == 0)
        {
            GUIUtility.hotControl = controlid;
            e.Use();

            Undo.IncrementCurrentGroup();

            GameObject gameObject = new GameObject();
            Vector3 aligned = new Vector3(Mathf.Floor(mousePos.x / grid.width) * grid.width + (grid.width / 2),
                                          0 ,
                                          (Mathf.Floor(mousePos.z / grid.height) * grid.height + (grid.height / 2)) /*- 11 * grid.height*/);

            gameObject.transform.position = aligned;
            gameObject.transform.parent = grid.transform;
            gameObject.tag = "BlockedArea";
            gameObject.layer = 16;
            gameObject.AddComponent<BoxCollider>().size = new Vector3(grid.width, 500, grid.width);

            foreach (GameObject blocked in GameObject.FindGameObjectsWithTag("BlockedArea"))
            {
                if (Mathf.FloorToInt(blocked.transform.position.x) == Mathf.FloorToInt(aligned.x) &&
                    Mathf.FloorToInt(blocked.transform.position.z) == Mathf.FloorToInt(aligned.z) &&
                    blocked != gameObject)
                {
                    DestroyImmediate(gameObject);
                    return;
                }
            }

            Undo.RegisterCreatedObjectUndo(gameObject, "Create" + gameObject.name);
        }
        if (e.isMouse && e.type == EventType.MouseDown && e.button == 1)
        {
            GUIUtility.hotControl = controlid;
            e.Use();

            Undo.IncrementCurrentGroup();

            Vector3 aligned = new Vector3(Mathf.Floor(mousePos.x / grid.width) * grid.width + (grid.width / 2),
                                          0,
                                          (Mathf.Floor(mousePos.z / grid.height) * grid.height + (grid.height / 2)) /*- 11 * grid.height*/);

            foreach (GameObject blocked in GameObject.FindGameObjectsWithTag("BlockedArea"))
            {
                if (Mathf.FloorToInt(blocked.transform.position.x) == Mathf.FloorToInt(aligned.x) &&
                    Mathf.FloorToInt(blocked.transform.position.z) == Mathf.FloorToInt(aligned.z))
                {
                    DestroyImmediate(blocked);
                    return;
                }
            }
        }
        if (e.isMouse && e.type == EventType.MouseUp && e.button == 0 || e.button == 1)
        {
            GUIUtility.hotControl = 0;
        }
    }
}