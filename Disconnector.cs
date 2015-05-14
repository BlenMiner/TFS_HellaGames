using UnityEngine;
using System.Collections;

public class Disconnector : MonoBehaviour {

	public void Disconnect()
    {
        BoltLauncher.Shutdown();
        Application.LoadLevel(0);
    }
}
