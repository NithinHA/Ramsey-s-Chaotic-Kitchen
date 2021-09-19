using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
	[Header("VoiceOver clips")]
	public CharacterVoiceOvers vo_NameCalled;
	public CharacterVoiceOvers vo_ActionPositive;
	public CharacterVoiceOvers vo_ActionNegative;
}

[System.Serializable]
public struct CharacterVoiceOvers
{
	public VoiceOverTypes voiceOverType;
	public AudioClip[] voiceOverClips;
}

public enum VoiceOverTypes
{
	NameCalled, ActionPositive, ActionNegative
}

[System.Serializable]
public struct InteractableItem
{
	public string itemKeyword;
	public GameObject itemGO;
}
