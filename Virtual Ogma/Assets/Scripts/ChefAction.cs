using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class ChefAction : MonoBehaviour
{
	[SerializeField] GameObject character;
	bool is_listening = false;

	KeywordsData keywords_data;
	Inventory inventory;

	List<string> keywords_list = new List<string>();
	Dictionary<Transform, Transform> place_transforms = new Dictionary<Transform, Transform>();
	Dictionary<Item, Transform> item_transforms = new Dictionary<Item, Transform>();

	KeywordRecognizer keyword_recognizer;
	Dictionary<string, Action> keywords_dict = new Dictionary<string, Action>();

	[SerializeField] private GameObject countdown_display_prefab;
	[SerializeField] private Vector3 countdown_display_position_offset;

	public void init_cooking(GameObject ch)
	{
		if (Input.GetKey(KeyCode.Q))
		{
			character = ch;
			ch.GetComponent<Player>().highlight_player();
			place_transforms = ch.GetComponent<ChefData>().chef_interactable_positions;
			// pan the camera towards the selected character and follow the player until 'Q' is released
			is_listening = true;
		}
	}

	void Start()
	{
		inventory = Inventory.instance;             // get the Singleton instance of Inventory Class
		keywords_data = KeywordsData.instance;      // get the Singleton instance of KeywordsData Class
		
		keywords_list = keywords_data.chef_keywords_2;
		item_transforms = keywords_data.game_item_positions;

		foreach (string keyword in keywords_list)
		{
			string[] word_list = keyword.Split();
			keywords_dict.Add(keyword, () => actionSelection(word_list));
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
			Test_script2.ts2.applyText(args.text);
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
				Test_script2.ts2.applyText("give a cooking command");
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


	void actionSelection(string[] word_list)
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
				default_method();
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
			string item_name = word_list[1];                     // received item to be chopped vegetables/fruits/meat
			Item food_item = keywords_data.findItemWithRawMaterial("chopped " + item_name);
			Transform[] all_transforms = { item_transforms[food_item], place_transforms[cutting_board_GO.transform], player_GO.GetComponent<Player>().starting_transform };        // array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			CuttingBoard cb = cutting_board_GO.GetComponent<CuttingBoard>();
			ch.is_busy = true;          // set character.is_busy true
			cb.is_chopping = true;      // set cutting_board.is_chopping true

            Animator anim = ch.GetComponent<Animator>();

            //move player to fetch item
            anim.SetBool("is_walking", true);       // play walking anim
			ch.target = all_transforms[0].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[0], 1));
			//play get_supplies sound
			AudioManager.instance.playSound("get_supplies");
            anim.SetTrigger("is_serving");      // get supplies anim
            //wait player for 1s
            yield return new WaitForSeconds(1);

            //move player to chopping board
            anim.SetBool("is_walking", true);       // play walking anim
			ch.target = all_transforms[1].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            Coroutine resolve_rotations_cor = StartCoroutine(ch.resolveRotations(all_transforms[1].eulerAngles));
			//call cb.chop()
			cb.chop(food_item);
            //wait player for chopping_delay seconds
            anim.SetBool("is_working", true);       // play working anim
            while (cb.is_chopping)
				yield return null;
            anim.SetBool("is_working", false);      // stop working anim
            //stop resolve_rotations_coroutine
            if (resolve_rotations_cor != null)
				StopCoroutine(resolve_rotations_cor);

            //move player to starting position
            anim.SetBool("is_walking", true);       // play walking anim
            ch.target = all_transforms[2].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[2], 1));
			yield return new WaitForSeconds(1);

			//set chef not busy
			ch.is_busy = false;
		}
	}

	IEnumerator boiling(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		string cooker_name = word_list[word_list.Length - 1];
		GameObject cooker_GO = GameObject.Find("/Props/cookers/" + cooker_name.ToLower());                  // !!!!!! Find GameObject with name !!!!!!
		if (!player_GO.GetComponent<Player>().is_busy && !cooker_GO.GetComponent<Cooker>().is_cooking)      // if character is free and cooker is not cooking anything
		{
			string item_name = word_list[1];                     //received item to be cooked rice/noodles
			Item food_item = keywords_data.findItemWithRawMaterial("boiled " + item_name);
			Transform[] all_transforms = { item_transforms[food_item], place_transforms[cooker_GO.transform], player_GO.GetComponent<Player>().starting_transform };        //array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			Cooker co = cooker_GO.GetComponent<Cooker>();
			ch.is_busy = true;          // set character.is_busy true
			co.is_cooking = true;       // set cooker.is_cooking true

            Animator anim = ch.GetComponent<Animator>();

            //move player to fetch item
            anim.SetBool("is_walking", true);       // play walking anim
			ch.target = all_transforms[0].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[0], 1));
            //play get_supplies sound
            AudioManager.instance.playSound("get_supplies");
            anim.SetTrigger("is_serving");      // get supplies anim
            //wait player for 1s
            yield return new WaitForSeconds(1);

            //move player to cooker
            anim.SetBool("is_walking", true);       // play walking anim
            ch.target = all_transforms[1].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[1], 1));
			//call co.cook()
			co.cook(food_item);
            anim.SetTrigger("is_serving");      // turn on cooker anim
            //DO NOT wait player for cooking_delay seconds. Instead, wait player at cooker for 1s
            yield return new WaitForSeconds(1);

            //move player to starting position
            anim.SetBool("is_walking", true);       // play walking anim
            ch.target = all_transforms[2].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[2], 1));
            yield return new WaitForSeconds(1);

            //set chef not busy
            ch.is_busy = false;
		}
	}

	IEnumerator turn_off(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		string cooker_name = word_list[word_list.Length - 1];
		GameObject cooker_GO = GameObject.Find("/Props/cookers/" + cooker_name.ToLower());                  // !!!!!! Find GameObject with name !!!!!!
		if (!player_GO.GetComponent<Player>().is_busy && cooker_GO.GetComponent<Cooker>().is_cooking)       // if character is free and cooker is cooking something
		{
			Transform[] all_transforms = { place_transforms[cooker_GO.transform], player_GO.GetComponent<Player>().starting_transform };        //array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			Cooker co = cooker_GO.GetComponent<Cooker>();
			ch.is_busy = true;          // set character.is_busy true, and cooker.is_cooking is already true

            Animator anim = ch.GetComponent<Animator>();

            //move player to cooker
            anim.SetBool("is_walking", true);       // play walking anim
			ch.target = all_transforms[0].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[0], .5f));
			//call co.turn_off_cooker()
			co.turn_off_cooker();
            anim.SetTrigger("is_serving");      // turn off cooker anim
            yield return new WaitForSeconds(.5f);       // player takes 0.5s to turn off cooker

            //move player to starting position
            anim.SetBool("is_walking", true);       // play walking anim
            ch.target = all_transforms[1].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[1], 1));
            yield return new WaitForSeconds(1);

            //set chef not busy
            ch.is_busy = false;
		}
	}

	IEnumerator getting_supplies(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		if (!player_GO.GetComponent<Player>().is_busy)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 1; i < word_list.Length; i++)
			{
				sb.Append(" " + word_list[i]);          // for items such as "sea weed", item name consist of 2 words. Therefore, we have to use StringBuilder to set item name of any size
			}
			string item_name = sb.ToString().Trim();         // item to be get buns/sea weed
			Item food_item = keywords_data.findItemWithRawMaterial(item_name);
			Transform[] all_transforms = { item_transforms[food_item], player_GO.GetComponent<Player>().starting_transform };        // array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			ch.is_busy = true;          // set character.is_busy true

            Animator anim = ch.GetComponent<Animator>();

            //move player to fetch item
            anim.SetBool("is_walking", true);       // play walking anim
            ch.target = all_transforms[0].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[0], food_item.time_to_prepare));
            //play get_supplies sound
            AudioManager.instance.playSound("get_supplies");
            anim.SetTrigger("is_serving");      // get supplies anim
			//wait player for time required to fetch the item
			yield return new WaitForSeconds(food_item.time_to_prepare);

			//add item to inventory... DONE
			bool has_added = inventory.addItem(food_item);
			if (has_added) {
				Debug.Log(food_item.name + " added to inventory");
			}
			else {
				Debug.Log("can not add " + food_item.name + " to inventory");
				Test_script2.ts2.applyText("can not add " + food_item.name + " to inventory");
			}

            //move player to starting position
            anim.SetBool("is_walking", true);       // play walking anim
            ch.target = all_transforms[1].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[1], 1));
            yield return new WaitForSeconds(1);

            //set chef not busy
            ch.is_busy = false;
		}
	}

	IEnumerator washing(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		GameObject sink_GO = GameObject.Find("/Props/sink");                  // !!!!!! Find GameObject with name !!!!!!
		GameObject serving_table_GO = GameObject.Find("/Props/serving_table");
		if (!player_GO.GetComponent<Player>().is_busy && !sink_GO.GetComponent<Sink>().is_washing)
		{
			string item_name = word_list[1];                     // received item to be washed (dishes)
			Item utensil_item = keywords_data.findItemWithRawMaterial(item_name);
			Transform[] all_transforms = { place_transforms[serving_table_GO.transform], place_transforms[sink_GO.transform], player_GO.GetComponent<Player>().starting_transform };        // array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			Sink sink = sink_GO.GetComponent<Sink>();
			ch.is_busy = true;              // set character.is_busy true
			sink.is_washing = true;         // set sink.is_washing true

            Animator anim = ch.GetComponent<Animator>();

            //move player to fetch item
            anim.SetBool("is_walking", true);       // play walking anim
            ch.target = all_transforms[0].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[0], .5f));
            //wait player fetching item
            anim.SetTrigger("is_serving");      // fetch utensil anim
            yield return new WaitForSeconds(.5f);

            //move player to sink
            anim.SetBool("is_walking", true);       // play walking anim
            ch.target = all_transforms[1].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            Coroutine resolve_rotations_cor = StartCoroutine(ch.resolveRotations(all_transforms[1].eulerAngles));
			//call sink.washUtensils()
			sink.washUtensils(utensil_item);
            anim.SetBool("is_working", true);       // play washing anim
			//wait player for washing_time seconds
			while (sink.is_washing)
				yield return null;
            anim.SetBool("is_working", false);       // stop washing anim
			//stop resolve_rotations_coroutine
			if (resolve_rotations_cor != null)
				StopCoroutine(resolve_rotations_cor);

            //move player to starting position
            anim.SetBool("is_walking", true);       // play walking anim
            ch.target = all_transforms[2].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[2], 1));
            yield return new WaitForSeconds(1);

            //set chef not busy
            ch.is_busy = false;
		}
	}

	IEnumerator preparing(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		if (!player_GO.GetComponent<Player>().is_busy)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 1; i < word_list.Length; i++)
			{
				sb.Append(" " + word_list[i]);          // for dishes such as "french fries", dish name consist of 2 words. Therefore, we have to use StringBuilder to set dish name of any size
			}
			string dish_name = sb.ToString().Trim();         // dish to prepare eg.- salad,biryani,sushi,etc..
			Item dish_item = keywords_data.findItemWithRawMaterial(dish_name);
			
			bool are_ingredients_available = inventory.areIngredientsAvailable(dish_item);  // check if raw materials for dish are available in inventory
			if (are_ingredients_available)
			{
				Transform[] all_transforms = { player_GO.GetComponent<Player>().preparing_position, player_GO.GetComponent<Player>().starting_transform };
				Player ch = player_GO.GetComponent<Player>();
				ch.is_busy = true;          // set character.is_busy true

                Animator anim = ch.GetComponent<Animator>();

                //move player to player's preparing_position
                anim.SetBool("is_walking", true);       // play walking anim
                ch.target = all_transforms[0].position;
				ch.target_reached = false;
				while (!ch.target_reached)
					yield return null;
                anim.SetBool("is_walking", false);      // stop walking anim

                //resolve rotations
                StartCoroutine(ch.invokeResolveRotation(all_transforms[0], dish_item.time_to_prepare));

                //remove ingredients used to prepare the dish from inventory
                inventory.removeIngredientsUsed(dish_item);

				//wait player for preparing delay
				// displpay the countdown_timer and then delete it after the countdown is over
				GameObject countdown_display = Instantiate(countdown_display_prefab, all_transforms[0].position + countdown_display_position_offset, Quaternion.Euler(new Vector3(45, 0, 0)));
				countdown_display.GetComponentInChildren<CountdownDisplay>().setTimer(dish_item.time_to_prepare);
				Destroy(countdown_display, dish_item.time_to_prepare);

                anim.SetBool("is_working", true);       // play preparing anim
                yield return new WaitForSeconds(dish_item.time_to_prepare);
                anim.SetBool("is_working", false);       // stop preparing anim

                //add item to inventory
                bool has_added = inventory.addItem(dish_item);
				if (has_added)
				{
					Debug.Log(dish_item.name + " added to inventory");
				}
				else
				{
					Debug.Log("can not add " + dish_item.name + " to inventory");
					Test_script2.ts2.applyText("can not add " + dish_item.name + " to inventory");
				}

                //move player to starting position
                anim.SetBool("is_walking", true);       // play walking anim
                ch.target = all_transforms[1].position;
				ch.target_reached = false;
				while (!ch.target_reached)
					yield return null;
                anim.SetBool("is_walking", false);      // stop walking anim
                //resolve rotations
                StartCoroutine(ch.invokeResolveRotation(all_transforms[1], 1));
                yield return new WaitForSeconds(1);

                //set chef not busy
                ch.is_busy = false;
			}
		}
	}

	void default_method()
	{
		Debug.Log("Invalid input command!");
	}
}
