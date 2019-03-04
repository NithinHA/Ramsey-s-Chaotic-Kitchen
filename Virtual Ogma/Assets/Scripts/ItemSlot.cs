using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
	Item item;

	public Image icon;
	public Button close_button;

	public void addItem(Item new_item)
	{
		this.item = new_item;

		icon.sprite = item.icon;
		icon.enabled = true;

		if (close_button != null)
			close_button.interactable = true;
	}

	public void clearSlot()
	{
		item = null;

		icon.sprite = null;
		icon.enabled = false;

		if (close_button != null)
			close_button.interactable = false;
	}

	/////////////// Buttons ///////////////

	public void onInventoryButton()
	{
		Debug.Log("Item selected: " + item.name);
		Test_script2.ts2.applyText("Item selected: " + item.name);
	}
	
	public void onCloseButton()
	{
		Inventory.instance.removeItem(item);
	}

	public void onOrderButton()
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < item.ingredients.Length; i++)
		{
			sb.Append(" " + item.ingredients[i]);
		}
		Debug.Log("Order selected: " + item.name + "\nIngredients:" + sb.ToString());
		Test_script2.ts2.applyText("Order selected: " + item.name + "\nIngredients:" + sb.ToString());
	}
}
