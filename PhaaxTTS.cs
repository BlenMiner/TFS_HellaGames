using UnityEngine;
using System.Collections;

/*
 * PhaaxTTS 1.7 for Unity
 * Samuel Johansson 2014
 * samuel@phaaxgames.com
 * 
 * Free to use as long as you give some credit in your application/game.
 * 
 * Works on mobile devices, uses Google TTS.
 * There's a limit of 100 characters (by Google).
 * This obviously uses network traffic.
 * 
 * Usage:
 * Drop it into your assets folder somewhere.
 * Don't attach it to any object.
 *
 * You need to call PhaaxTTS.Say like this: 
 * PhaaxTTS.Say("en_gb", "Text-to-speech is silly. I wear a hat, sometimes. Sometimes!", 1.0f, 1.0f);
 * 
 * First parameter is your language code (en_us = US english, en_gb = british english, sv = swedish)
 * Second parameter is what you want it to say.
 * Third parameter is pitch, which is a float value.
 * Fourth parameter is volume, which is a float value (0.0f-1.0f).
 * 
 * Use PhaaxTTS.IsPlaying() to see if anything is playing at the moment (returns true or false).
 * Use PhaaxTTS.IsLoading() to see if anything is loading at the moment (returns true or false).
 * 
 */

public class PhaaxTTS : MonoBehaviour {
	private static PhaaxTTS myInstance = null;
	private AudioSource aSource;
	private bool isLoading = false;

	private static PhaaxTTS instance {
		get	{
			if (myInstance == null) {
				myInstance = GameObject.FindObjectOfType(typeof(PhaaxTTS)) as PhaaxTTS;
				if (myInstance == null) {
					myInstance = new GameObject("PhaaxTTS").AddComponent<PhaaxTTS>();
				}
			}
			return myInstance;
		}
	}
	
	void Awake () {
		if (myInstance == null) {
			myInstance = this as PhaaxTTS;
		}
	}

	void Die () {
		myInstance = null;
		Destroy(gameObject);
	}
	
	void OnApplicationQuit () {
		myInstance = null;
	}

	private IEnumerator DestroyObject (GameObject gObject, AudioSource aSource) {
		while (aSource.isPlaying) {
			yield return new WaitForSeconds(0.1f);
		}
		Destroy (gObject);
	}

	public static bool IsPlaying() {
		if (instance.aSource != null) {
			return instance.aSource.isPlaying;
		} else {
			return false;
		}
	}

	public static bool IsLoading() {
		if (instance != null) {
			return instance.isLoading;
		} else {
			return false;
		}
	}

	public static void Say (string countryCode, string words, float pitch, float volume) {
		instance.StartCoroutine (instance.Speak (countryCode, words, pitch, volume));
	}

	private IEnumerator Speak (string countryCode, string words, float pitch, float volume) {
		if (words.Length <= 100) {
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY
			string result = WWW.EscapeURL (words, System.Text.Encoding.UTF8);
			string url = "http://translate.google.com/translate_tts?ie=UTF-8&tl="+countryCode+"&q="+result;

			instance.isLoading = true;

			WWW www = new WWW (url);
			Debug.Log ("<<PhaaxTTS>> Fetching TTS audio.");
			yield return www;
			Debug.Log ("<<PhaaxTTS>> Audio download complete.");

			AudioClip sound = www.GetAudioClip(false, true, AudioType.MPEG);

			if (sound == null) {
				Debug.Log("<<PhaaxTTS>> Error: Failed to download sound properly.");
			} else {
				GameObject TTSObject = new GameObject ();
				if (TTSObject != null) {
					TTSObject.name = "PhaaxTTS_Playing";
					TTSObject.transform.parent = instance.transform;

					instance.aSource = TTSObject.AddComponent<AudioSource> ();
					if (instance.aSource != null) {
						instance.aSource.loop = false;
						instance.aSource.playOnAwake = false;
						instance.aSource.clip = sound;
						instance.aSource.pitch = pitch;
						instance.aSource.volume = volume;
						instance.aSource.Play ();
						instance.StartCoroutine(instance.DestroyObject(TTSObject, instance.aSource));
					} else {
						Debug.Log("<<PhaaxTTS>> Error: Failed to create AudioSource!");
					}
				} else {
					Debug.Log("<<PhaaxTTS>> Error: Could not create a GameObject.");
				}
			}
			instance.isLoading = false;

			#else
			Debug.Log ("<<PhaaxTTS>> Streaming MP3 only works on IPhone, Android and Blackberry devices");
			yield break;
			#endif
		} else {
			Debug.Log ("<<PhaaxTTS>> Error: Text is too long ("+words.Length+"/100 chars)");
			yield break;
		}
	}
}