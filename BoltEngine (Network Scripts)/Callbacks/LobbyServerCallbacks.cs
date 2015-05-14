using UnityEngine;
using System.Collections;
using Ini;

[BoltGlobalBehaviour(BoltNetworkModes.Host, "Lobby2 Scene")]
public class LobbyServerCallbacks : Bolt.GlobalEventListener
{
    /*ILobby lobby;
    public override void SceneLoadLocalDone(string map)
    {
        IniFile ini = new IniFile(Application.dataPath + "/settings.ini");
        BoltNetwork.SetHostInfo(ini.IniReadValue("Server", "serverName"), null);

        if (BoltNetwork.isServer)
        {
            BoltEntity lobby = BoltNetwork.Instantiate(BoltPrefabs.Lobby);
            lobby.TakeControl();

            this.lobby = lobby.GetState<ILobby>();
        }
    }*/

    /*int c = 0;
    public override void Connected(BoltConnection connection)
    {
        string name = "testPlayer_" + Random.Range(100000, 900000);

        lobby.players[c].playerName = name;
        lobby.players[c].rank = "TFS_Pre-orderer";
        lobby.players[c].speaking = false;

        c++;

        Debug.Log("Player connected");
    }*/

    float time;

    ILobby lobby;
    void Awake()
    {
        IniFile ini = new IniFile(Application.dataPath + "/settings.ini");
        BoltNetwork.SetHostInfo(ini.IniReadValue("Server", "serverName"), null);

        if (BoltNetwork.isServer)
        {
            BoltEntity lobby = BoltNetwork.Instantiate(BoltPrefabs.Lobby);
            lobby.TakeControl();

            this.lobby = lobby.GetState<ILobby>(); 
        }

        time = Time.time;
    }

    bool once = true;
    bool once2 = true;
    bool once3 = true;

    int playersToStart = 10;

    void Update()
    {
        int c = 0;
        foreach (var p in BoltNetwork.connections)
        {
            if(p != null)
                c++;
        }

        lobby.playersC = c;
        if (c < playersToStart)
        {
            lobby.timeLeft = (60 * 2) - (Time.time - time);
        }

        if (c >= 2 && once)
        {
            time = Time.time;
            once = false;
        }
        if (c >= 2 && Time.time - time > 60 * 2)
        {
            BoltNetwork.LoadScene("Main Demo");
        }

        if (c >= playersToStart)
        {
            if (once3)
            {
                BoltNetwork.LoadScene("Main Demo");
                once3 = false;
            }
        }
    }
    void OnGUI()
    {
        GUILayout.Space(500);
        if(GUILayout.Button("Start"))
        {
            BoltNetwork.LoadScene("Main Demo");
        }
    }

    IEnumerator LoadScene()
    {
        lobby.timeLeft = 0;
        yield return new WaitForSeconds(3.5f);
        BoltNetwork.LoadScene("Main Demo");
    }
}
