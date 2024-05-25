using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class CharacterAction : MonoBehaviour
{
    protected Player character;
    protected bool is_listening = false;

    protected KeywordsData keywords_data;
    protected Inventory inventory;

    protected KeywordRecognizer keyword_recognizer;
    protected Dictionary<string, Action> keywords_dict = new Dictionary<string, Action>();

	public CharacterType characterType;

	public virtual void Init(Player ch)
	{
		character = ch;
		Player player = ch.GetComponent<Player>();
		player.highlight_player();
		player.PlayVoiceOver(VoiceOverTypes.NameCalled);
		// pan the camera towards the selected character and follow the player until 'Q' is released
		is_listening = true;
	}

	protected virtual void Update()
    {
		if (Input.GetKey(KeyCode.Q) && LevelController.Instance.CurrentGameState == GameState.Running)
		{
			if (!keyword_recognizer.IsRunning && is_listening)
			{
				InstructionPanel.Instance.DisplayInstruction("give a command");
				keyword_recognizer.Start();
			}
		}
		else
		{
			if (keyword_recognizer.IsRunning && is_listening)               // (keyword_recognizer.IsRunning) could be replaced with (is_listening)
			{
				is_listening = false;
				keyword_recognizer.Stop();
				character.GetComponent<Player>().remove_highlighter();
				AudioManager.Instance?.PlaySound(Constants.Audio.ListenerOff);
				// pan the camera to the default camera position
			}
		}
	}

	protected void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
	{
		Action keyword_action;
		if (keywords_dict.TryGetValue(args.text, out keyword_action))
		{
			InstructionPanel.Instance.DisplayInstruction(args.text);
			keyword_action.Invoke();
		}
	}

	protected virtual void ActionSelection(string[] word_list)
    {

    }
}

public enum CharacterType
{
    chef, waiter
}
