using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LobbyController : Bolt.EntityBehaviour<ILobby>
{
    void OnGUI()
    {
        GUILayout.Space(100);
        GUI.color = Color.green;
        GUILayout.Label("Estimated wait time: " + state.timeLeft);
        GUILayout.Label("Players: " + state.playersC);
        GUI.color = Color.clear;
    }
    /*List<PlayerStruct> players = new List<PlayerStruct>();

    public Text countDown;

    bool startUpdating;

    public override void Attached()
    {
        if (BoltNetwork.isServer)
        {
            entity.TakeControl();

            offset = 60 * 5;
            startTime = Time.time;
        }

        startUpdating = true; 
    }

    bool once = false;
    float startTime = 0;
    int offset;

    bool once2 = true;
    void Update()
    {
        if (countDown == null)
        {
            countDown = GameObject.FindGameObjectWithTag("Lobby").GetComponent<Text>();
            return;
        }

        if(startUpdating)
        {

            //if(once2 && state.players.Length > 15)
            {
                startTime = Time.time;
                offset = 15;
                once2 = false;
            }

            state.timeLeft = offset - Mathf.FloorToInt(Time.time - startTime);

            countDown.text = state.timeLeft.ToString();

            players.Clear();
            /*foreach(PlayerStruct s in state.players)
            {
                if(s != null && s.playerName !=null &&  s.playerName.Length > 0)
                    players.Add(s);
            }*/
    /*
            LobbyControl lc= countDown.GetComponentInParent<LobbyControl>();
            for (int i = 0; i < lc.maxPlayers; i++)
            {
                lc.players[i].name = "";
                lc.players[i].rank = "";
                lc.players[i].ping = 0;
            }

            int ic = 0;
            foreach(PlayerStruct pl in players)
            {
                PlayerBar pb = new PlayerBar();

                pb.name = pl.playerName;
                pb.rank = pl.rank;

                lc.players[ic] = new PlayerBar();
                ic++;
            }
        }
    }*/
}
