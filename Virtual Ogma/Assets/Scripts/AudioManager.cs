using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;		// singleton class

	public Sound[] sounds;

	private void Awake()
	{
		if (instance == null)
			instance = this;
		else
		{
			Destroy(gameObject);				// if an instance of AudioManager exists in the scene, destroy this gameobject
			return;
		}
		DontDestroyOnLoad(gameObject);			// does not destroy existing gameobject when new scene is loaded

		foreach(Sound sound in sounds)			// for each sounds that can be played in game, add it to sounds array and set desired properties in the inspector
		{
			sound.source = gameObject.AddComponent<AudioSource>();		// add a new AudioSource component for each sound that can be played in-game
			sound.source.clip = sound.clip;

			sound.source.loop = sound.is_looping;
			sound.source.volume = sound.volume;
			sound.source.pitch = sound.pitch;
		}
	}

	void Start()
    {
        
    }
	
    public void playSound(string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);		// find the sound with the name that matches to the name of sound to be played
		if (s == null)
		{
			Debug.LogWarning("Sound " + s.name + " not found");
			return;
		}
		s.source.Play();			// play the sound
	}

	public void stopSound(string name)
	{
		Sound s = Array.Find(sounds, sound => sound.name == name);      // find the sound with the name that matches to the name of sound to be stopped playing
		if (s == null)
		{
			Debug.LogWarning("Sound " + s.name + " not found");
			return;
		}
		if (!s.source.isPlaying)			// if the sound is not being played, display warning message that sound that is not being played cannot be stopped
		{
			Debug.LogWarning("Sound " + s.name + " is not being played");
			return;
		}
		s.source.Stop();			// stop the sound
	}
}
