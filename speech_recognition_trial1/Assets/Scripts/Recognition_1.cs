using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class Recognition_1 : MonoBehaviour {

	KeywordRecognizer keyword_recognizer;
	Dictionary<string, Action> keywords = new Dictionary<string, Action>();

	public GameObject players;
	public GameObject[] all_players = new GameObject[4];

	public string[] player_names = { "Alice", "Bob", "Cera", "Dave" };
	
	void Start () {
		//add keywords to actions
		keywords.Add(player_names[0], () => CallPlayer(player_names[0], 0));
		keywords.Add(player_names[1], () => CallPlayer(player_names[1], 1));
		keywords.Add(player_names[2], () => CallPlayer(player_names[2], 2));
		keywords.Add(player_names[3], () => CallPlayer(player_names[3], 3));

		//keywords.Add("who am i", () => call_player("Elfen"));

		keyword_recognizer = new KeywordRecognizer(keywords.Keys.ToArray(), ConfidenceLevel.Low);
		keyword_recognizer.OnPhraseRecognized += OnKeywordsRecognized;
		//keyword_recognizer.Start();
	}


	public void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
	{
		Action keyword_action;
		if (keywords.TryGetValue(args.text, out keyword_action))
		{
			keyword_action.Invoke();
		}
	}

	bool is_listening = false;
	
	void Update () {
		if (Input.GetKey(KeyCode.Q)) {
			if (!is_listening) {
				keyword_recognizer.Start();
				is_listening = true;
			}
		}
		else {
			if (is_listening) {
				if(keyword_recognizer.IsRunning)
					keyword_recognizer.Stop();
				is_listening = false;
			}
		}
	}

	void CallPlayer(string player_name, int player_index)
	{
		Debug.Log("hello " + player_name);

		PlayerScript player_obj = players.GetComponent<PlayerScript>();
		player_obj.Initiate(all_players[player_index]);


		keyword_recognizer.Stop();
	}
}
