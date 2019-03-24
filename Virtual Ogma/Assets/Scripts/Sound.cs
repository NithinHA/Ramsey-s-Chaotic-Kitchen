using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]		// allows non-monobehaviour classes to be edited on the inspector
public class Sound {

	public string name;
	public AudioClip clip;

	[Range(0, 1)] public float volume;
	[Range(.1f, 3)] public float pitch;

	public bool is_looping;

	[HideInInspector] public AudioSource source;
}
