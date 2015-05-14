using UnityEngine;
using System.Collections;

public class Speak : MonoBehaviour {

	void Update ()
    {
        if (!PhaaxTTS.IsPlaying())
            PhaaxTTS.Say("en_gb", "Text-to-speech is silly. I wear a hat, sometimes. Sometimes!", 1.0f, 1.0f);
	}
}
