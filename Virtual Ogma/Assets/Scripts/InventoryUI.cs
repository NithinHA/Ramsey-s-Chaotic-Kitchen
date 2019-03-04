using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
	public Transform inventory_panel;
	Inventory inventory;

	ItemSlot[] slots;

    void Start()
    {
		inventory = Inventory.instance;
		inventory.on_item_changed_callback += updateUI;

		slots = inventory_panel.GetComponentsInChildren<ItemSlot>();
    }
	
    void Update()
    {
        
    }

	void updateUI()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if(i < inventory.food_items.Count)
			{
				slots[i].addItem(inventory.food_items[i]);
			}
			else
			{
				slots[i].clearSlot();
			}
		}
	}

	public void toggleInventory()
	{
		inventory_panel.gameObject.SetActive(!inventory_panel.gameObject.activeSelf);
	}
}
