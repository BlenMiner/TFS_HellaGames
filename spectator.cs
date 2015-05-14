using UnityEngine;
using System.Collections;

public class spectator : Bolt.EntityBehaviour<ISpectator> 
{
    bool isMine = false;
    public override void ControlGained()
    {
        isMine = true;
    }

    bool done = false;
    void Update()
    {
        if (isMine == false)
            return;

        if(camera.enabled == false)
        {
            camera.enabled = true;
        }

        GameObject go = GameObject.FindGameObjectWithTag("ALLUI");
        if (go != null && done == false)
        {
            go.SetActive(false);
            done = true;
        }
    }
    void OnGUI()
    {
        if (isMine == false)
            return;

        GUILayout.Label("Spectating isnt a feature yet...");
        GUILayout.Label("Wait for all players to die.");
    }
}
