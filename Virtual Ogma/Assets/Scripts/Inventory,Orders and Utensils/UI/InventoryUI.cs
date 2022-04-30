using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
	public Transform inventory_panel;
	[SerializeField] private float m_TransitTime = .5f;

	Inventory inventory;
	ItemSlot[] slots;

	private bool _isShrink = true;

	void Start()
    {
		inventory = Inventory.Instance;
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
		_isShrink = !_isShrink;
		if (!_isShrink)
			inventory_panel.gameObject.SetActive(true);

		float target = _isShrink ? 0 : 1;
		inventory_panel.DOScale(target, m_TransitTime)
			.SetEase(Ease.OutExpo)
			.OnComplete(() =>
			{
				if (_isShrink)
					inventory_panel.gameObject.SetActive(false);
			});
	}
}
