using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonBase;

public class KeywordsData : Singleton<KeywordsData>
{
	[Header("Chef data")]
	public List<string> chef_keywords_2 = new List<string>();

	[Header("Waiter data")]
	public List<string> waiter_keywords_2 = new List<string>();

	[Header("Items data")]
	[SerializeField] private Item[] item_arr;					// List of Items that go in the inventory
	[SerializeField] private Transform[] item_positions;		// List of positions that player has to be on to interact with items
	public Dictionary<Item, Transform> game_item_positions = new Dictionary<Item, Transform>();             // A Dictionary formed out of above info
	
	[Header("Dish data")]
	public Item[] dish_arr;						// this array is cloned in Customer script for choosing a random dish from array of available dishes
	[SerializeField] private int[] cost_arr;
	public Dictionary<Item, int> dish_cost = new Dictionary<Item, int>();

	[Header("Utensil data")]
	public Item[] utensil_arr;
	//public int[] utensil_count_arr;               // total numer of utensil instances present for serving... eg.- 4 plates, 2 bowls and 2 cups

	[Header("Particle Effects data")]
	[SerializeField] private Item[] item_for_particles;
	[SerializeField] private GameObject[] particle_effects;
	public Dictionary<Item, GameObject> item_particles_dict = new Dictionary<Item, GameObject>();

	[SerializeField] List<Item> all_items = new List<Item>();

	protected override void Awake()
    {
		base.Awake();

		for (int i = 0; i < item_arr.Length; i++)                                           // initialize item_positoins dictionary
		{
			game_item_positions.Add(item_arr[i], item_positions[i]);
		}
		for (int i = 0; i < dish_arr.Length; i++)
		{
			dish_cost.Add(dish_arr[i], cost_arr[i]);
		}
		for (int i = 0; i < item_for_particles.Length; i++)
		{
			item_particles_dict.Add(item_for_particles[i], particle_effects[i]);
		}

		foreach (Item item in item_arr)
			all_items.Add(item);
		foreach (Item item in dish_arr)
			all_items.Add(item);
		foreach (Item item in utensil_arr)
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
