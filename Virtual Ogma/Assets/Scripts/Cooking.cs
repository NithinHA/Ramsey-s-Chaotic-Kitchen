using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class Cooking : MonoBehaviour
{
	GameObject character;
	bool is_listening = false;

	public void init_cooking(GameObject ch)
	{
		character = ch;
		ch.GetComponent<Player>().highlight_player();
		is_listening = true;
	}

	//public GameObject keywords_data;
	List<string> keywords_list = new List<string>();
	Dictionary<Transform, Vector3> place_position = new Dictionary<Transform, Vector3>();
	Dictionary<string, Vector3> item_positions = new Dictionary<string, Vector3>();

	KeywordRecognizer keyword_recognizer;
	Dictionary<string, Action> keywords_dict = new Dictionary<string, Action>();

	void Start()
	{
		GameObject keywords_data = GameObject.FindGameObjectWithTag("character data");					// !!!!!! Find GameObject with tag !!!!!!
		keywords_list = keywords_data.GetComponent<KeywordsData>().chef_keywords_2;
		place_position = keywords_data.GetComponent<KeywordsData>().chef_place_positions;
		item_positions = keywords_data.GetComponent<KeywordsData>().chef_item_positions;

		foreach (string keyword in keywords_list)
		{
			Action method_call;
			string[] word_list = keyword.Split();
			switch (word_list[0])
			{
				case "chop":
					method_call = () => chopping(word_list);
					break;
				case "boil":
					method_call = () => boiling(word_list);
					break;
				case "get":
					method_call = () => getting_supplies(word_list);
					break;
				case "wash":
					method_call = () => washing(word_list);
					break;
				case "prepare":
					method_call = () => preparing(word_list);
					break;
				case "turn":
					method_call = () => turn_off(word_list);
					break;
				default:
					method_call = () => default_method();
					break;
			}
			keywords_dict.Add(keyword, method_call);
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
			if (!keyword_recognizer.IsRunning && is_listening)		// !keyword_recognizer.IsRunning not required, is_listening will alone do the trick
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
	}



	void chopping(string[] word_list)
	{
		string cutting_board_name = "cutting_board_" + word_list[3];
		GameObject cutting_board_GO = GameObject.Find(cutting_board_name);                  // !!!!!! Find GameObject with name !!!!!!
		if (!character.GetComponent<Player>().is_busy && !cutting_board_GO.GetComponent<CuttingBoard>().is_chopping)
		{
			string item = word_list[1];						//received item to be chopped vegetables/fruits/meat
			GameObject place = cutting_board_GO;			//access cutting_board_a/b GameObject
			Vector3[] positions = { item_positions[item], place_position[place.transform] };        //array of positions where character needs to go

			character.GetComponent<Player>().is_busy = true;		//set character is_busy true
			//cutting_board_GO.GetComponent<CuttingBoard>().chop_item(character, item, positions);

			//move player to fetch item
			//wait player for 1s
			//move player to chopping board
			//call cb.chop()
			//wait player for chopping_delay seconds
			//move player to starting position
			//set chef and cutting board not busy

			//destroy gameobject if is_listening is false
		}
	}
	void boiling(string[] word_list)
	{
		string cooker_name = "cooker_" + word_list[3];
		GameObject cooker_GO = GameObject.Find(cooker_name);                  // !!!!!! Find GameObject with name !!!!!!
		if (!character.GetComponent<Player>().is_busy && !cooker_GO.GetComponent<CuttingBoard>().is_chopping)
		{
			string item = word_list[1];
			GameObject place = cooker_GO;
			Vector3[] positions = { item_positions[item], place_position[place.transform] };

			Player ch = character.GetComponent<Player>();
			Cooker co = place.GetComponent<Cooker>();
			ch.is_busy = true;
			co.is_cooking = true;       // set chef and cutting board is busy

			//move player to fetch item
			//wait player for 1s
			//move player to cooker
			//call co.cook()
			//wait player for 1s
			//move player to starting position
			//set chef not busy

			//destroy gameobject if is_listening is false
		}
	}
	void getting_supplies(string[] word_list)
	{
		if (!character.GetComponent<Player>().is_busy)
		{
			string item = word_list[1];
			Vector3[] positions = { item_positions[item] };

			Player ch = character.GetComponent<Player>();
			ch.is_busy = true;

			//move player to fetch item
			//wait player for 1s
			//add item to inventory
			//move player to starting position
			//set chef not busy

			//destroy gameobject if is_listening is false
		}
	}
	void washing(string[] word_list)
	{
		if (!character.GetComponent<Player>().is_busy)
		{
			string item = word_list[1];
			Vector3[] positions = { item_positions[item] };
			GameObject sink_instance = GameObject.Find("sink");

			Player ch = character.GetComponent<Player>();
			ch.is_busy = true;

			//move player to item_position sink
			//call sink_instance.wash_plates()
			//wait player for washing delay
			//move player to starting position
			//set chef not busy

			//destroy gameobject if is_listening is false
		}
	}
	void preparing(string[] word_list)
	{
		if (!character.GetComponent<Player>().is_busy)
		{

		}
	}
	void turn_off(string[] word_list)
	{
		string place_name = "cooker_" + word_list[3];
		GameObject place = GameObject.Find(place_name);
		if (!character.GetComponent<Player>().is_busy && place.GetComponent<Cooker>().is_cooking)
		{
			Vector3[] positions = { place_position[place.transform] };

			Player ch = character.GetComponent<Player>();
			Cooker co = place.GetComponent<Cooker>();
			ch.is_busy = true;

			//move player to cooker position
			//call co.turn_off_cooker()
			//wait player 1s
			//move player to starting position
			//set chef not busy

			//destroy gameobject if is_listening is false
		}
	}

	void default_method()
	{
		Debug.Log("Invalid input command!");
	}

}
