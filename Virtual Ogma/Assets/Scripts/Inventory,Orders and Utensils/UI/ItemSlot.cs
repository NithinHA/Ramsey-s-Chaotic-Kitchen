using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
	Item item;

	public Image icon;						// applies to both inventory and orders

	public Text name_text;					// applies to only orders
	public GameObject ingredients_panel;		// applies to only orders

	public Button close_button;				// applies to only inventory

	public void addItem(Item new_item)		// when a new item is added to INVENTORY or new ORDER is made by a customer
	{
		this.item = new_item;

		icon.sprite = item.icon;			// changes the sprite of icon in both inventory and orders to icon of this.item
		icon.enabled = true;

		if (name_text != null)
			name_text.text = item.name + " -> ₹" + KeywordsData.instance.dish_cost[item];		// display name and cost of of the order

		if(ingredients_panel != null)
		{
			Image[] ingredient_images = ingredients_panel.GetComponentsInChildren<Image>();		// array of all the null images present inside ingredients_panel
			for (int i = 0; i < ingredient_images.Length - 1; i++)
			{
				if (i < item.ingredients.Length)
				{
					ingredient_images[i + 1].sprite = item.ingredients[i].icon;         // set sprite of each of those ingredient images to icons of all ingredients of this order 
					ingredient_images[i + 1].enabled = true;                // set ingredient icons to visible, since they are not null anymore
				}
				else
				{
					ingredient_images[i + 1].sprite = null;
					ingredient_images[i + 1].enabled = false;
				}
			}
		}

		if (close_button != null)
			close_button.interactable = true;				// to remove unwanted item from inventory
	}

	public void clearSlot()					// when an item is removed from INVENTORY or an ORDER is served to customer
	{
		item = null;				// remove the item alloted to this slot

		icon.sprite = null;			// remove the sprite
		icon.enabled = false;		// make the sprite not visible since null value will disply an ugly white square

		if (name_text != null)
			name_text.text = "";     // remove the display name

		if (ingredients_panel != null)
		{
			Image[] ingredient_images = ingredients_panel.GetComponentsInChildren<Image>();     // array of all the images present inside ingredients_panel
			for (int i = 1; i < ingredient_images.Length; i++)		// starts from 1 because we don't want to set the first image to null, since the first image is within orders_panel
			{
				ingredient_images[i].sprite = null;			// set sprite of each of those ingredient images to null 
				ingredient_images[i].enabled = false;		// set ingredient icons to not visible. This prevents null images(white square) from being displayed
			}
		}

		if (close_button != null)
			close_button.interactable = false;			// disable remove item button from inventory
	}

	/////////////// Buttons ///////////////

	public void onInventoryButton()
	{
		if (this.item != null)
		{
			Debug.Log("Item selected: " + item.name);
			Test_script2.ts2.applyText("Item selected: " + item.name);
		}
	}
	
	public void onCloseButton()
	{
		Inventory.instance.removeItem(item);
	}

	public void onOrderButton()
	{
		if (item != null)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < item.ingredients.Length; i++)
			{
				if (i > 0)
					sb.Append(",");
				sb.Append(" " + item.ingredients[i].name);
			}
			Debug.Log("Order selected: " + item.name + "\nIngredients:" + sb.ToString());
			Test_script2.ts2.applyText("Order selected: " + item.name + "\nIngredients:" + sb.ToString());
		}
	}
}
