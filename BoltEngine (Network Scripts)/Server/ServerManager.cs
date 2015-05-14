using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ServerManager : Bolt.EntityBehaviour<IServerSettings>
{
    bool controller = false;
    bool attached = false;

    AudioClip countdown;
    AudioClip aftercountdown;

    GameObject textParent;
    Text uiCounter;

    void Awake()
    {
        if (ItemsDatabase.instance == null)
            Destroy(this);

        gameObject.AddComponent<AudioSource>().maxDistance = 100000;
        gameObject.AddComponent<AudioSource>().minDistance = 90000;

        countdown = ItemsDatabase.instance.countdown;
        countdown = ItemsDatabase.instance.aftercountdown;
    }
    public static bool matchStarted = false;

    float sTime;

    float time;
    bool canUseUI = true;
    public override void Attached()
    {
        attached = true;
	    if(BoltNetwork.isServer)
        {
            entity.TakeControl();
            state.timer = 25;
            controller = true;
        }

        sTime = Time.time + 25f;
        time = Time.time;

        try
        {
            uiCounter = GameObject.FindGameObjectWithTag("timer").GetComponent<Text>();
            textParent = GameObject.FindGameObjectWithTag("timerParent");
        }
        catch
        {
            canUseUI = false;
        }
	}

    bool once = true;
    void Update()
    {
        if (!attached)
        {
            return;
        }

        if (controller)
        {
            state.timer = Mathf.FloorToInt(sTime - Time.time);
            //if (state.timer <= 0)
                state.matchStarted = true;
        }

        if (uiCounter != null)
            uiCounter.text = state.timer.ToString();

        if (state.timer == 20 && once)
        {
            once = false;
            audio.PlayOneShot(aftercountdown);
        }

        if (state.matchStarted && matchStarted == false)
        {
            matchStarted = true;
            audio.PlayOneShot(aftercountdown);

            if (textParent != null)
                textParent.SetActive(false);
        }


        if (state.matchStarted)
        {
            bool isOver = true;

            int aliveCounter = 0;
            foreach (TutorialPlayerObject p in TutorialPlayerObjectRegistry.allPlayers)
            {
                if (p.character != null && p.character.isAttached)
                {
                    ITFSPlayerState pl = p.character.GetState<ITFSPlayerState>();
                    if (!pl.dead)
                    {
                        isOver = false;
                        aliveCounter++;
                    }
                }
                if(p.connection == null && p.character != null)
                {
                    isOver = false;
                }
            }

            if (isOver || aliveCounter <= 1 && TutorialPlayerObjectRegistry.allPlayers.Count > 1)
            {
                matchStarted = false;
                if(BoltNetwork.isServer)
                    BoltNetwork.LoadScene("Lobby2 Scene");
            }
        }
    }
}
