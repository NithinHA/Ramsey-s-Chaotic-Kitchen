using System;
using System.Collections.Generic;
using UnityEngine;
using SingletonBase;

public class Orders : Singleton<Orders>
{
	public Action onOrderListUpdate;

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

		onOrderListUpdate?.Invoke();
		return true;
	}

	public void removeItem(Item item)
	{
		orders_list.Remove(item);
		onOrderListUpdate?.Invoke();
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
