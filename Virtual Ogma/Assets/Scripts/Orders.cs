using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orders : MonoBehaviour
{
	#region Singleton
	public static Orders instance;       // singleton
	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogWarning("More than one Orders instance found in the scene");
			return;
		}
		instance = this;
	}
	#endregion

	public delegate void onOrderChanged();                   // invoke this delegate whenever a new dish is added to orders_list, or an existing item is removed from orders_list
	public onOrderChanged on_order_changed_callback;

	public int order_space = 20;
	public List<Item> orders_list = new List<Item>();

	public bool addItem(Item item)
	{
		if (orders_list.Count >= order_space)        // if number of dishes in orders_list exceeds the max orders_list size, then return false
		{
			Debug.Log("cannot make any more orders!");
			return false;                           // REMEMBER to catch this returned bool value in the invoking function
		}
		orders_list.Add(item);

		if (on_order_changed_callback != null)
			on_order_changed_callback.Invoke();      // invoke delegate on adding new dish to orders_list

		return true;
	}

	public void removeItem(Item item)
	{
		orders_list.Remove(item);
		if (on_order_changed_callback != null)
			on_order_changed_callback.Invoke();      // invoke delegate on removing a dish from orders_list
	}

	public bool isItemPresent(Item dish)			// probably would not need this method.. but let's see
	{
		foreach (Item order in orders_list)
		{
			if (order.name == dish.name)
			{
				return true;
			}
		}
		return false;
	}
}
