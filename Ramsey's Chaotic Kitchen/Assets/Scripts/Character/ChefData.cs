using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefData : CharacterData
{
	[Header("Chef data")]
	[SerializeField] private List<Transform> chef_interactable = new List<Transform>();         // List of cheff places like cooker A & B, cutting_board A & B, etc
	[SerializeField] private List<Transform> chef_positions = new List<Transform>();        // List of positions that cheff has to be to interact with above objects
	public Dictionary<Transform, Transform> chef_interactable_positions = new Dictionary<Transform, Transform>();       // A Dictionary formed out of above info

	//[Header("Items data")]
	//[SerializeField] private List<Item> item_list = new List<Item>();                       // List of Items that go in the inventory
	//[SerializeField] private List<Transform> item_positions = new List<Transform>();        // List of positions that player has to be on to interact with items
	//public Dictionary<Item, Vector3> game_item_positions = new Dictionary<Item, Vector3>();                         // A Dictionary formed out of above info

	//public List<Item> dish_list = new List<Item>();
	//public List<Item> utensil_list = new List<Item>();

	//[SerializeField] List<Item> all_food_items = new List<Item>();

	void Awake()
    {
		for (int i = 0; i < chef_interactable.Count; i++)                                   // initialize chef_interactable_positoins dictionary
		{
			chef_interactable_positions.Add(chef_interactable[i], chef_positions[i]);
		}

		//for (int i = 0; i < item_list.Count; i++)                                           // initialize item_positoins dictionary
		//{
		//	game_item_positions.Add(item_list[i], item_positions[i].position);
		//}

		//foreach (Item item in item_list)
		//	all_food_items.Add(item);
		//foreach (Item item in dish_list)
		//	all_food_items.Add(item);
	}

	//public Item findItemWithRawMaterial(string item_name)           // keywords_data.findItemWithRawMaterial("chopped " + item);	Use this format in calling function
	//{
	//	foreach (Item item in all_food_items)
	//	{
	//		if (item.name == item_name)
	//			return item;
	//	}
	//	Debug.LogWarning("No item with the name " + item_name + "exists");
	//	return null;
	//}
}
