using UnityEngine;
using System.Collections;
using Bolt;
using BoltInternal;

[BoltGlobalBehaviour(BoltScenes.Main_Demo)]
public class TutorialPlayerCallbacks : GlobalEventListener
{
    public override void ControlOfEntityGained(BoltEntity arg)
    {
        if (!arg.gameObject.name.Contains("TFS"))
            return;

        Material m = arg.gameObject.GetComponent<CameraObj>().Material;

        arg.gameObject.GetComponent<CameraObj>().Camera.SetActive(true);
        arg.gameObject.GetComponent<CameraObj>().Weapons.SetActive(true);

        arg.gameObject.GetComponent<CharacterControls>().enabled = true;
        //arg.gameObject.GetComponent<MouseLook>().enabled = true;

        foreach (GameObject g in arg.gameObject.GetComponent<CameraObj>().Models)
        {
            foreach (Renderer r in g.GetComponentsInChildren<Renderer>(true))
            {
                r.material = m;
            }
        }
    }
    public override void ConnectRequest(UdpKit.UdpEndPoint endpoint, IProtocolToken token)
    {
        BoltNetwork.Accept(endpoint);
    }
}
