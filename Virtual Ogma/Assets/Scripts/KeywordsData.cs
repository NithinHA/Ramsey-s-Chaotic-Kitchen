using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordsData : MonoBehaviour
{
	public static KeywordsData instance;	// Singleton instance of class KeywordsData

	[Header("Chef data")]
	public List<string> chef_keywords_2 = new List<string>();

	[Header("Waiter data")]
	public List<string> waiter_keywords_2 = new List<string>();

	[Header("Items data")]
	[SerializeField] private List<Item> item_list = new List<Item>();                       // List of Items that go in the inventory
	[SerializeField] private List<Transform> item_positions = new List<Transform>();        // List of positions that player has to be on to interact with items
	public Dictionary<Item, Vector3> game_item_positions = new Dictionary<Item, Vector3>();                         // A Dictionary formed out of above info

	public List<Item> dish_list = new List<Item>();
	[SerializeField] private List<int> cost_list = new List<int>();
	public Dictionary<Item, int> dish_cost = new Dictionary<Item, int>();

	public List<Item> utensil_list = new List<Item>();
	public int[] utensil_count_arr;               // total numer of utensil instances present for serving... eg.- 4 plates, 2 bowls and 2 cups
	
	[SerializeField] List<Item> all_items = new List<Item>();

	void Awake()
    {
		if (instance != null)
		{
			Debug.LogWarning("More than one KeywordsData instance found in the scene");
			return;
		}
		instance = this;
		
		for (int i = 0; i < item_list.Count; i++)                                           // initialize item_positoins dictionary
		{
			game_item_positions.Add(item_list[i], item_positions[i].position);
		}
		for (int i = 0; i < dish_list.Count; i++)
		{
			dish_cost.Add(dish_list[i], cost_list[i]);
		}

		foreach (Item item in item_list)
			all_items.Add(item);
		foreach (Item item in dish_list)
			all_items.Add(item);
		foreach (Item item in utensil_list)
			all_items.Add(item);
	}

	public Item findItemWithRawMaterial(string item_name)           // keywords_data.findItemWithRawMaterial("chopped " + item);	Use this format in calling function
	{
		foreach (Item item in all_items)
		{
			if (item.name == item_name)
				return item;
		}
		Debug.LogWarning("No item with the name " + item_name + " exists");
		return null;
	}
}
