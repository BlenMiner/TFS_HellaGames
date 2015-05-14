using UnityEngine;
using System.Collections;

public class ClientSoundCallbacks : Bolt.GlobalEventListener
{
    public AudioClip[] weaponSounds;
    public GameObject bloodEffect;

    public static ClientSoundCallbacks instance;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnEvent(playSound e)
    {
        if (e.entity == null || e.soundID == null || e.soundID >= weaponSounds.Length) return;

        e.entity.gameObject.audio.PlayOneShot(weaponSounds[e.soundID]);
        if (e.soundID == 16)
            Instantiate(bloodEffect, e.entity.gameObject.transform.position, Quaternion.identity);

        if (BoltNetwork.isServer && e.soundID > 21 && e.soundID < 204)
        {
            ITFSPlayerState tfs = e.entity.GetState < ITFSPlayerState>();
            tfs.steps++;
        }
    }
}
