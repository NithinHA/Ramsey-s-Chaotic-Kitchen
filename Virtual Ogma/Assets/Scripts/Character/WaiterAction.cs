using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class WaiterAction : CharacterAction
{
	[SerializeField] List<string> keywords_list = new List<string>();
	public List<InteractableItem> customers;

	void Start()
	{
		inventory = Inventory.Instance;             // get the Singleton instance of Inventory Class
		keywords_data = KeywordsData.Instance;      // get the Singleton instance of KeywordsData Class
		characterType = CharacterType.waiter;

		keywords_list = keywords_data.waiter_keywords_2;

		foreach (string keyword in keywords_list)
		{
			string[] word_list = keyword.Split();
			keywords_dict.Add(keyword, () => ActionSelection(word_list));
		}
		// THERE IS SOME PROBLEM HERE! I used to get null reference exception until I Debug.Log'ed the contents of the dictionary, after which the errors simply disappeared.. NANI?

		keyword_recognizer = new KeywordRecognizer(keywords_dict.Keys.ToArray(), ConfidenceLevel.Low);
		keyword_recognizer.OnPhraseRecognized += OnKeywordsRecognized;
	}

    public override void Init(GameObject ch)
    {
        if (Input.GetKey(KeyCode.Q))
        {
			base.Init(ch);
        }
    }

    protected override void ActionSelection(string[] word_list)
	{
		base.ActionSelection(word_list);
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
				return;
		}
	}

	IEnumerator take_order(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		string table_name = word_list[word_list.Length - 1];
		GameObject customer_GO = customers.Find(item => item.itemKeyword == table_name.ToLower()).itemGO;
		if (!player_GO.GetComponent<Player>().is_busy && customer_GO.GetComponent<Customer>().is_ordering)
		{
			character.GetComponent<Player>().PlayVoiceOver(VoiceOverTypes.ActionPositive);
			Transform[] all_transforms = { player_GO.GetComponent<WaiterData>().waiter_interactable_positions[customer_GO.transform], player_GO.GetComponent<Player>().starting_transform };    // array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			Customer cu = customer_GO.GetComponent<Customer>();
			ch.is_busy = true;          // set character.is_busy true
			cu.is_ordering = false;      // set customer.is_ordering true

            Animator anim = ch.GetComponent<Animator>();

            //move player to take order
            anim.SetBool("is_walking", true);       // play walking anim
            ch.target = all_transforms[0].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[0], 2));

            anim.SetTrigger("is_serving");      // take order anim
            yield return new WaitForSeconds(2);
			//call cu.order_food()
			cu.order_food();

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
		else
			character.GetComponent<Player>().PlayVoiceOver(VoiceOverTypes.ActionNegative);
	}

	IEnumerator serving(string[] word_list)
	{
		GameObject player_GO = character;          // A copy of character is made for use within the coroutine since, the global variable character might change before this coroutine ends.
		string table_name = word_list[word_list.Length - 1];
		GameObject customer_GO = customers.Find(item => item.itemKeyword == table_name.ToLower()).itemGO;
		if (!player_GO.GetComponent<Player>().is_busy && !customer_GO.GetComponent<Customer>().is_ordering && !customer_GO.GetComponent<Customer>().is_served)
		{
			Item food_item = customer_GO.GetComponent<Customer>().dish;
			if (!inventory.isItemPresent(food_item))			// check if the ordered dish is present in inventory
			{
				character.GetComponent<Player>().PlayVoiceOver(VoiceOverTypes.ActionNegative);
				yield break;
			}
			if (!Utensils.Instance.checkUtensilAvailability(food_item.served_in))       // check if utensil to serve food in is available.
			{
				character.GetComponent<Player>().PlayVoiceOver(VoiceOverTypes.ActionNegative);
				Utensils.Instance.utensilUnavailableIndicator(food_item.served_in);     //indicate utensil are unavailable by blinking utensil_slot
				yield break;
			}
			character.GetComponent<Player>().PlayVoiceOver(VoiceOverTypes.ActionPositive);
			Transform[] all_transforms = { player_GO.GetComponent<WaiterData>().waiter_interactable_positions[customer_GO.transform], player_GO.GetComponent<Player>().starting_transform };    // array of positions where character needs to go

			Player ch = player_GO.GetComponent<Player>();
			Customer cu = customer_GO.GetComponent<Customer>();
			ch.is_busy = true;          // set character.is_busy true

            Animator anim = ch.GetComponent<Animator>();

            //move player to serve food
            anim.SetBool("is_walking", true);       // play walking anim
            Debug.Log("serve " + food_item.name + " to table " + table_name);
			InstructionPanel.Instance.DisplayInstruction("serve " + food_item.name + " to table " + table_name);
			ch.target = all_transforms[0].position;
			ch.target_reached = false;
			while (!ch.target_reached)
				yield return null;
            anim.SetBool("is_walking", false);      // stop walking anim
            //resolve rotations
            StartCoroutine(ch.invokeResolveRotation(all_transforms[0], 2));

            anim.SetTrigger("is_serving");      // serve food anim
            yield return new WaitForSeconds(2);

			//call cu.food_served()
			if (!customer_GO.GetComponent<Customer>().is_served)	// makes sure that the customer was not served the dish by some other waiter while this waiter was moving and waiting for 2 seconds
			{
				cu.food_served();			// removes the dish from orders_list and some other actions take place 
				//remove dish from inventory and one utensil instance
				inventory.removeItem(food_item);
				Utensils.Instance.removeOneUtensil(food_item.served_in);
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
		else
			character.GetComponent<Player>().PlayVoiceOver(VoiceOverTypes.ActionNegative);
	}

	void default_method()
	{
		Debug.Log("Invalid input command!");
	}
}
