using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UdpKit;
using Ini;

public class ServerConnector : Bolt.GlobalEventListener
{
    public static int port = 25565;
    public static string serverName = "";

    public string map;

    void Start()
    {
        IniFile ini = new IniFile(Application.dataPath + "/settings.ini");

        if (ini.IniReadValue("Server", "initiateServer") == null ||
           ini.IniReadValue("Server", "initiateServer") == "")
        {
            ini.IniWriteValue("Server", "initiateServer", "false");
        }
        if (ini.IniReadValue("Server", "serverName") == null ||
           ini.IniReadValue("Server", "serverName") == "")
        {
            ini.IniWriteValue("Server", "serverName", "BlenMiner's home server :D");
        }

        bool initializeServer = bool.Parse(ini.IniReadValue("Server", "initiateServer"));
        serverName = ini.IniReadValue("Server", "serverName");

        if(initializeServer)
        {
            CreateServer();
        }
    }

    public void CreateServer()
    {
        //BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, (ushort)port));
        BoltLauncher.StartServer(port);
        /*while (!BoltNetwork.isClient && !BoltNetwork.isServer)
        {

        }*/

        //BoltNetwork.LoadScene("Lobby2 Scene");
        //BoltNetwork.LoadScene("Lobby2 Scene");

        //BoltNetwork.LoadScene(map);
    }
    public override void BoltStartDone()
    {
        if(BoltNetwork.isServer)
            BoltNetwork.LoadScene("Lobby2 Scene");
    }

    public void JoinMatch()
    {
        Application.LoadLevel(1);
    }

    public void CloseAPP()
    {
        Application.Quit();
    }
}
