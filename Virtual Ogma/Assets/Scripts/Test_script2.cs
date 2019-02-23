using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class Test_script2 : MonoBehaviour
{
	[SerializeField] GameObject character;
	bool is_listening = false;

	public void init_cooking(GameObject ch)
	{
		character = ch;
		ch.GetComponent<Player>().highlight_player();
		is_listening = true;
	}
	
	Dictionary<string, Vector3> item_positions = new Dictionary<string, Vector3>();

	KeywordRecognizer keyword_recognizer;
	Dictionary<string, Action> keywords_dict = new Dictionary<string, Action>();

	[SerializeField] string[] item_list;		//never actually used, but is used to check for the items in dictionary. Since dictionary cannot be serialized

	void Start()
	{
		GameObject keywords_data = GameObject.FindGameObjectWithTag("character data");                  // !!!!!! Find GameObject with tag !!!!!!
		item_positions = keywords_data.GetComponent<KeywordsData>().chef_item_positions;
		item_list = item_positions.Keys.ToArray();		//just to check in inspector weather items are added

		foreach (KeyValuePair<string, Vector3> item_pos in item_positions)
		{
			keywords_dict.Add(item_pos.Key, () => MoveToPosition(item_pos.Value));
		}

		keyword_recognizer = new KeywordRecognizer(keywords_dict.Keys.ToArray(), ConfidenceLevel.Low);
		keyword_recognizer.OnPhraseRecognized += OnKeywordsRecognized;
	}

	public void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
	{
		Action keyword_action;
		if (keywords_dict.TryGetValue(args.text, out keyword_action))
		{
			keyword_action.Invoke();
		}
	}

	void Update()
	{

		if (Input.GetKey(KeyCode.Q))
		{
			if (!keyword_recognizer.IsRunning && is_listening)      // !keyword_recognizer.IsRunning not required, is_listening will alone do the trick
			{
				Debug.Log("give a cooking command");
				keyword_recognizer.Start();
				//is_listening = false;
			}
		}
		else
		{
			if (keyword_recognizer.IsRunning)               // if(keyword_recognizer.IsRunning) could be replaced with if(is_listening)
			{
				is_listening = false;
				keyword_recognizer.Stop();
				character.GetComponent<Player>().remove_highlighter();
			}
			//Destroy(gameObject);		//not here do this later
		}
		//else
			//Debug.Log("NOT listening");
	}

	void MoveToPosition(Vector3 position)
	{
		Player player = character.GetComponent<Player>();
		if (!player.is_busy)
		{
			player.is_busy = true;
			StartCoroutine(move_player(position));
			//player.move_player(position);
		}
	}

	IEnumerator move_player(Vector3 position)
	{
		Player player = character.GetComponent<Player>();
		player.target = position;
		player.target_reached = false;
		while (!player.target_reached)
		{
			yield return null;
		}

		player.target = player.starting_position;
		player.target_reached = false;
		while (!player.target_reached)
		{
			yield return null;
		}

		//player.target = position;
		//player.target_reached = false;
		//while (!player.target_reached)
		//{
		//	yield return null;
		//}

		//player.target = player.starting_position;
		//player.target_reached = false;
		//while (!player.target_reached)
		//{
		//	yield return null;
		//}

		Debug.Log("journey ends here");
		player.is_busy = false;
		
	}
}
