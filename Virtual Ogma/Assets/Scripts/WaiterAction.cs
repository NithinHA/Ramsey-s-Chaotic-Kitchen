using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class WaiterAction : MonoBehaviour
{
	[SerializeField] GameObject character;
	bool is_listening = false;

	KeywordsData keywords_data;
	Inventory inventory;

	[SerializeField] List<string> keywords_list = new List<string>();
	Dictionary<Transform, Vector3> place_position = new Dictionary<Transform, Vector3>();
	Dictionary<Item, Vector3> item_positions = new Dictionary<Item, Vector3>();

	KeywordRecognizer keyword_recognizer;
	Dictionary<string, Action> keywords_dict = new Dictionary<string, Action>();


	public void init_serving(GameObject ch)
	{
		character = ch;
		ch.GetComponent<Player>().highlight_player();
		place_position = ch.GetComponent<WaiterData>().waiter_interactable_positions;
		// pan the camera towards the selected character and follow the player until 'Q' is released
		is_listening = true;
	}

	void Start()
	{
		inventory = Inventory.instance;             // get the Singleton instance of Inventory Class
		keywords_data = KeywordsData.instance;      // get the Singleton instance of KeywordsData Class

		keywords_list = keywords_data.waiter_keywords_2;

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
			case "take":
				StartCoroutine(take_order(word_list));
				break;
			case "serve":
				StartCoroutine(serving(word_list));
				break;
			default:
				default_method();
				break;
		}
	}

	IEnumerator take_order(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		string table_name = word_list[word_list.Length - 1];
		Debug.Log(table_name + "called");
		GameObject customer_GO = GameObject.Find("/Props/dining_tables/" + table_name.ToLower());                  // !!!!!! Find GameObject with name !!!!!!
		if (!player_GO.GetComponent<Player>().is_busy && customer_GO.GetComponent<Customer>().is_ordering)
		{
			Vector3[] positions = { player_GO.GetComponent<WaiterData>().waiter_interactable_positions[customer_GO.transform], player_GO.GetComponent<Player>().starting_position };        // array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			Customer cu = customer_GO.GetComponent<Customer>();
			ch.is_busy = true;          // set character.is_busy true
			cu.is_ordering = false;      // set customer.is_ordering true

			//move player to take order
			Debug.Log("taking order from table " + table_name);
			Test_script2.ts2.applyText("taking order from table " + table_name);
			ch.target = positions[0];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;

			//wait waiter for 2 seconds
			yield return new WaitForSeconds(2);

			//call cu.order_food()
			cu.order_food();

			//move player to starting position
			ch.target = positions[1];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
			Debug.Log("waiter returns");
			Test_script2.ts2.applyText("waiter returns");

			//set chef not busy
			ch.is_busy = false;
		}
	}

	IEnumerator serving(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		string table_name = word_list[word_list.Length - 1];
		GameObject customer_GO = GameObject.Find("/Props/dining_tables/" + table_name.ToLower());			// !!!!!! Find GameObject with name !!!!!!
		if (!player_GO.GetComponent<Player>().is_busy && !customer_GO.GetComponent<Customer>().is_ordering && !customer_GO.GetComponent<Customer>().is_served)
		{
			Item food_item = customer_GO.GetComponent<Customer>().dish;
			if (!inventory.isItemPresent(food_item))
			{
				yield break;
			}
			if (!Utensils.instance.checkUtensilAvailability(food_item.served_in))		// check if utensil to serve food in is available.
			{
				yield break;
			}
			Vector3[] positions = { player_GO.GetComponent<WaiterData>().waiter_interactable_positions[customer_GO.transform], player_GO.GetComponent<Player>().starting_position };        // array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			Customer cu = customer_GO.GetComponent<Customer>();
			ch.is_busy = true;          // set character.is_busy true

			//move player to serve food
			Debug.Log("serve " + food_item.name + " to table " + table_name);
			Test_script2.ts2.applyText("serve " + food_item.name + " to table " + table_name);
			ch.target = positions[0];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;

			//wait player for 2s
			yield return new WaitForSeconds(2);

			//call cu.food_served()
			if (!customer_GO.GetComponent<Customer>().is_served)	// makes sure that the customer was not served the dish by some other waiter while this waiter was moving and waiting for 2 seconds
			{
				cu.food_served();			// removes the dish from orders_list and some other actions take place 

				//remove dish from inventory and one utensil instance
				inventory.removeItem(food_item);
				Utensils.instance.removeOneUtensil(food_item.served_in);
			}				
			
			//move player to starting position
			ch.target = positions[1];
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
			Debug.Log("waiter returns");
			Test_script2.ts2.applyText("waiter returns");

			//set chef not busy
			ch.is_busy = false;
		}
	}

	void default_method()
	{
		Debug.Log("Invalid input command!");
	}
}
