using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordsData : MonoBehaviour
{
	public static KeywordsData instance;	// Singleton instance of class KeywordsData

	[Header("Chef data")]
	public List<string> chef_keywords_2 = new List<string>();
	[SerializeField] private List<Transform> chef_places = new List<Transform>();			// List of cheff places like cooker A & B, cutting_board A & B, etc
	[SerializeField] private List<Transform> chef_positions = new List<Transform>();		// List of positions that cheff has to be to interact with above objects
	public Dictionary<Transform, Vector3> chef_place_positions = new Dictionary<Transform, Vector3>();		// A Dictionary formed out of above info

	[Header("Waiter data")]
	public List<string> waiter_keywords_2 = new List<string>();
	[SerializeField] private List<Transform> waiter_places = new List<Transform>();         // List of waiter places like Dining table A,B,C & D
	[SerializeField] private List<Transform> waiter_positions = new List<Transform>();      // List of positions that waiter has to be to interact with above objects
	public Dictionary<Transform, Vector3> waiter_place_positions = new Dictionary<Transform, Vector3>();	// A dictionary formed out of above info

	[Header("Items data")]
	[SerializeField] private List<Item> item_list = new List<Item>();						// List of Items that go in the inventory
	[SerializeField] private List<Transform> item_positions = new List<Transform>();		// List of positions that player has to be on to interact with items
	public Dictionary<Item, Vector3> game_item_positions = new Dictionary<Item, Vector3>();             // A Dictionary formed out of above info

	[SerializeField] private List<Item> dish_list = new List<Item>();

	void Awake()
    {
		if (instance != null)
		{
			Debug.LogWarning("More than one KeywordsData instance found in the scene");
			return;
		}
		instance = this;

		for (int i = 0; i < chef_places.Count; i++)
		{
			chef_place_positions.Add(chef_places[i], chef_positions[i].position);
		}
		for (int i = 0; i < waiter_places.Count; i++)
		{
			waiter_place_positions.Add(waiter_places[i], waiter_positions[i].position);
		}
		for (int i = 0; i < item_list.Count; i++)
		{
			game_item_positions.Add(item_list[i], item_positions[i].position);
		}
    }

	public Item findItemWithRawMaterial(string item_name)           // keywords_data.findItemWithRawMaterial("chopped " + item);	Use this format in calling function
	{
		foreach(Item item in item_list)
		{
			if (item.name == item_name)
				return item;
		}
		Debug.LogWarning("No item with the name " + item_name + "exists");
		return null;
	}
}
