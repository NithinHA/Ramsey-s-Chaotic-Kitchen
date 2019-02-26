using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	#region Singleton
	public static Inventory instance;		// singleton
	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one Inventory instance found in the scene");
			return;
		}
		instance = this;
	}
	#endregion

	public delegate void onItemChanged();					// invoke this delegate whenever a new item is added to inventory, or an existing item is removed from inventory
	public onItemChanged on_item_changed_callback;

	public int inventory_space = 20;
	public List<Item> food_items = new List<Item>();

	public bool addItem(Item item)
	{
		if(food_items.Count >= inventory_space)		// if number of items in inventory exceeds the max inventory size, then return false
		{
			Debug.Log("No space in inventory!");
			return false;                           // REMEMBER to catch this returned bool value in the invoking function
		}
		food_items.Add(item);

		if (on_item_changed_callback != null)
			on_item_changed_callback.Invoke();		// invoke delegate on adding new food_item

		return true;
	}

	public void removeItem(Item item)
	{
		food_items.Remove(item);
		if (on_item_changed_callback != null)
			on_item_changed_callback.Invoke();		// invoke delegate on removing a food_item
	}
}
