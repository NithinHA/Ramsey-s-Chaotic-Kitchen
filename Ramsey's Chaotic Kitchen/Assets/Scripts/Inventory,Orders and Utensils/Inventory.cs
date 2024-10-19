using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonBase;
using System;

public class Inventory : Singleton<Inventory>
{
	public delegate void onItemChanged();					// invoke this delegate whenever a new item is added to inventory, or an existing item is removed from inventory
	public Action<Item> OnItemAdded;
	public Action<Item, int> OnItemRemoved;

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

		OnItemAdded?.Invoke(item);
		return true;
	}

	public void removeItem(Item item, int slotIndex = -1)
	{
		food_items.Remove(item);
		OnItemRemoved?.Invoke(item, slotIndex);
	}

	public bool isItemPresent(Item food_item)
	{
		foreach(Item item in food_items)
		{
			if (item.name == food_item.name)
			{
				return true;
			}
		}
		return false;
	}

	public bool areIngredientsAvailable(Item dish)			// checks if all the ingredients required to prepare a dish are available in the inventory
	{														// consider an example of sushi
		foreach(Item ingredient in dish.ingredients)		// for all Items in ingreients list of sushi ie.- boiled rice, chopped meat, sea weed
		{
			bool item_available = false;
			foreach(Item food_item in food_items) {			// for all items in the inventory list
				if (ingredient.name == food_item.name) {	// if ingredient name = item name, then set item_available to true and break
					item_available = true;
					break;
				}
			}
			if (!item_available)			// if this ingredient is unavailable, then return false 
				return false;
		}
		return true;
	}

	public void removeIngredientsUsed(Item dish)			// this method removes the items present in ingredients[] of dish item from food_items list
	{
		foreach(Item ingredient in dish.ingredients) {
			foreach(Item food_item in food_items)
			{
				if(ingredient.name == food_item.name) {
					removeItem(food_item);
					break;
				}
			}
		}
	}
}
