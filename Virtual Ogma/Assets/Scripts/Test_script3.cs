using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class Test_script3 : MonoBehaviour
{
	[SerializeField] GameObject character;
	bool is_listening = false;

	public void init_cooking(GameObject ch)
	{
		character = ch;
		ch.GetComponent<Player>().highlight_player();
		// pan the camera towards the selected character and follow the player until 'Q' is released
		is_listening = true;
	}

	//public GameObject keywords_data;
	[SerializeField] List<string> keywords_list = new List<string>();
	Dictionary<Transform, Vector3> place_position = new Dictionary<Transform, Vector3>();
	Dictionary<string, Vector3> item_positions = new Dictionary<string, Vector3>();

	KeywordRecognizer keyword_recognizer;
	Dictionary<string, Action> keywords_dict = new Dictionary<string, Action>();

	void Start()
	{
		GameObject keywords_data = GameObject.FindGameObjectWithTag("character data");                  // !!!!!! Find GameObject with tag !!!!!!
		keywords_list = keywords_data.GetComponent<KeywordsData>().chef_keywords_2;
		place_position = keywords_data.GetComponent<KeywordsData>().chef_place_positions;
		item_positions = keywords_data.GetComponent<KeywordsData>().chef_item_positions;

		foreach (string keyword in keywords_list)
		{
			string[] word_list = keyword.Split();
			keywords_dict.Add(keyword, () => ActionSelection(word_list));
		}
		// THERE IS SOME PROBLEM HERE! I used to get null reference exception until I Debug.Log'ed the contents of the dictionary, after which the errors simply disappeared.. NANI?

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
			if (!keyword_recognizer.IsRunning && is_listening)
			{
				Debug.Log("give a cooking command");
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
				// pan the camera to the default camera position
			}
		}
	}


	void ActionSelection(string[] word_list)
	{
		switch (word_list[0])
		{
			case "chop":
				StartCoroutine(chopping(word_list));
				break;
			case "boil":
				StartCoroutine(boiling(word_list));
				break;
			case "turn":
				StartCoroutine(turn_off(word_list));
				break;
			case "get":
				StartCoroutine(getting_supplies(word_list));
				break;
			case "wash":
				StartCoroutine(washing(word_list));
				break;
			case "prepare":
				StartCoroutine(preparing(word_list));
				break;
			default:
				//StartCoroutine(chopping(word_list));
				break;
		}
	}

	IEnumerator chopping(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		string cutting_board_name = word_list[word_list.Length - 1];
		GameObject cutting_board_GO = GameObject.Find("/Props/cutting_boards/" + cutting_board_name.ToLower());                  // !!!!!! Find GameObject with name !!!!!!
		if (!player_GO.GetComponent<Player>().is_busy && !cutting_board_GO.GetComponent<CuttingBoard>().is_chopping)
		{
			string item = word_list[1];                     // received item to be chopped vegetables/fruits/meat
			Vector3[] positions = { item_positions[item], place_position[cutting_board_GO.transform], player_GO.GetComponent<Player>().starting_position };        // array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			CuttingBoard cb = cutting_board_GO.GetComponent<CuttingBoard>();
			ch.is_busy = true;			// set character.is_busy true
			cb.is_chopping = true;		// set cutting_board.is_chopping true

			//move player to fetch item
			Debug.Log("fetch item for chopping");
			ch.target = positions[0];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;

			//wait player for 1s
			yield return new WaitForSeconds(1);

			//move player to chopping board
			Debug.Log("move to chopping board");
			ch.target = positions[1];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;

			//call cb.chop()
			Debug.Log("reached chopping board");
			cb.chop(item);

			//wait player for chopping_delay seconds
			while (cb.is_chopping)
				yield return null;

			//move player to starting position
			ch.target = positions[2];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
			Debug.Log("player returns");

			//set chef not busy
			ch.is_busy = false;
		}
	}

	IEnumerator boiling(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		string cooker_name = word_list[word_list.Length - 1];
		GameObject cooker_GO = GameObject.Find("/Props/cookers/" + cooker_name.ToLower());                  // !!!!!! Find GameObject with name !!!!!!
		if (!player_GO.GetComponent<Player>().is_busy && !cooker_GO.GetComponent<Cooker>().is_cooking)		// if character is free and cooker is not cooking anything
		{
			string item = word_list[1];                     //received item to be cooked rice/noodles
			Vector3[] positions = { item_positions[item], place_position[cooker_GO.transform], player_GO.GetComponent<Player>().starting_position };        //array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			Cooker co = cooker_GO.GetComponent<Cooker>();
			ch.is_busy = true;          // set character.is_busy true
			co.is_cooking = true;		// set cooker.is_cooking true

			//move player to fetch item
			Debug.Log("fetch item for boiling");
			ch.target = positions[0];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;

			//wait player for 1s
			yield return new WaitForSeconds(1);

			//move player to cooker
			Debug.Log("move to cooker");
			ch.target = positions[1];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;

			//call co.cook()
			Debug.Log("reached cooker");
			co.cook(item);

			//DO NOT wait player for cooking_delay seconds. Instead, wait player at cooker for 1s
			yield return new WaitForSeconds(1);

			//move player to starting position
			ch.target = positions[2];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
			Debug.Log("player returns");

			//set chef not busy
			ch.is_busy = false;
		}
	}

	IEnumerator turn_off(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		string cooker_name = word_list[word_list.Length - 1];
		GameObject cooker_GO = GameObject.Find("/Props/cookers/" + cooker_name.ToLower());                  // !!!!!! Find GameObject with name !!!!!!
		if (!player_GO.GetComponent<Player>().is_busy && cooker_GO.GetComponent<Cooker>().is_cooking)		// if character is free and cooker is cooking something
		{
			Vector3[] positions = { place_position[cooker_GO.transform], player_GO.GetComponent<Player>().starting_position };        //array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			Cooker co = cooker_GO.GetComponent<Cooker>();
			ch.is_busy = true;          // set character.is_busy true, and cooker.is_cooking is already true

			//move player to cooker
			Debug.Log("move to cooker");
			ch.target = positions[0];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;

			//call co.turn_off_cooker()
			Debug.Log("reached cooker");
			co.turn_off_cooker();

			yield return new WaitForSeconds(.5f);		// player takes 0.5s to turn off cooker

			//move player to starting position
			ch.target = positions[1];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
			Debug.Log("player returns");

			//set chef not busy
			ch.is_busy = false;
		}
	}

	IEnumerator getting_supplies(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		if (!player_GO.GetComponent<Player>().is_busy)
		{
			string item = word_list[1];                     // item to be get (buns)
			Vector3[] positions = { item_positions[item], player_GO.GetComponent<Player>().starting_position };        // array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			ch.is_busy = true;          // set character.is_busy true

			//move player to fetch item
			Debug.Log("get item from " + item_positions[item]);
			ch.target = positions[0];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;

			//wait player for 1s
			yield return new WaitForSeconds(1);

			//add item to inventory!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			//move player to starting position
			ch.target = positions[1];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
			Debug.Log("player returns");

			//set chef not busy
			ch.is_busy = false;
		}
	}

	IEnumerator washing(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		GameObject sink_GO = GameObject.Find("/Props/sink");                  // !!!!!! Find GameObject with name !!!!!!
		if (!player_GO.GetComponent<Player>().is_busy && !sink_GO.GetComponent<Sink>().is_washing)
		{
			string item = word_list[1];                     // received item to be washed (dishes)
			Vector3[] positions = { item_positions[item], place_position[sink_GO.transform], player_GO.GetComponent<Player>().starting_position };        // array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			Sink sink = sink_GO.GetComponent<Sink>();
			ch.is_busy = true;              // set character.is_busy true
			sink.is_washing = true;         // set sink.is_washing true

			//move player to fetch item
			Debug.Log("fetch plates");
			ch.target = positions[0];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;

			//wait player for 1s
			yield return new WaitForSeconds(1);

			//move player to sink
			Debug.Log("move to sink");
			ch.target = positions[1];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;

			//call sink.wash_plates()
			Debug.Log("reached sink");
			sink.wash_plates();

			//wait player for washing_time seconds
			while (sink.is_washing)
				yield return null;

			//move player to starting position
			ch.target = positions[2];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
			Debug.Log("player returns");

			//set chef not busy
			ch.is_busy = false;
		}
	}
	
	IEnumerator preparing(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.

		yield return null;				//////////////////////// !!!
	}
	void _preparing(string[] word_list)
	{

		if (!character.GetComponent<Player>().is_busy)
		{

		}
	}
	

	void default_method()
	{
		Debug.Log("Invalid input command!");
	}
}
