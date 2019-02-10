using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class PlayerScript : MonoBehaviour {

	//for speech recognition
	KeywordRecognizer keyword_recognizer;
	Dictionary<string, Action> keywords = new Dictionary<string, Action>();

	//for positions
	public Transform[] positions = new Transform[4];
	public string[] position_names = { "alpha positive", "alpha negative", "beta positive", "beta negative" };

	//

	//others
	GameObject player;
	PlayerMovement player_movement;
	public GameObject player_highlighter;
	GameObject highlighted_player;

	void Start()
	{
		////////////position commands////////////
		keywords.Add(position_names[0], () => MoveToPosition("alpha +", 0));
		keywords.Add(position_names[1], () => MoveToPosition("alpha -", 1));
		keywords.Add(position_names[2], () => MoveToPosition("beta +", 2));
		keywords.Add(position_names[3], () => MoveToPosition("beta -", 3));

		////////////some other commands////////////
		//keywords.Add(command_string, () => method())

		keyword_recognizer = new KeywordRecognizer(keywords.Keys.ToArray(), ConfidenceLevel.Low);
		keyword_recognizer.OnPhraseRecognized += OnKeywordsRecognized;
	}

	public void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
	{
		Action keyword_action;
		if (keywords.TryGetValue(args.text, out keyword_action))
		{
			keyword_action.Invoke();
		}
	}

	bool is_player_selected = false;
	bool is_listening = false;

	void Update()
	{
		if (is_player_selected)
		{
			if (Input.GetKey(KeyCode.Q))
			{
				if (!is_listening)
				{
					keyword_recognizer.Start();
					is_listening = true;
				}
			}
			else
			{
				//if (is_listening)
				{
					keyword_recognizer.Stop();
					is_listening = false;

					Destroy(highlighted_player);
					is_player_selected = false;
				}
			}
		}
	}

	public void Initiate(GameObject player)
	{
		this.player = player;
		player_movement = player.GetComponent<PlayerMovement>();
		//highlight player
		highlighted_player = Instantiate(player_highlighter, player.transform.position + new Vector3(0, -1, 0), Quaternion.identity);
		highlighted_player.transform.SetParent(player.transform);
		is_player_selected = true;
	}

	public void MoveToPosition(string pos, int pos_index)
	{
		Debug.Log("Player moving to: " + pos);
		
		PositionScript ps = positions[pos_index].GetComponent<PositionScript>();
		player_movement.MovePlayer(ps);
	}

	public void FollowTarget()
	{

	}

}
