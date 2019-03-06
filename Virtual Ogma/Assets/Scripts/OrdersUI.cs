using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrdersUI : MonoBehaviour
{
	public Transform orders_panel;
	Orders orders;

	ItemSlot[] slots;

	void Start()
	{
		orders = Orders.instance;
		orders.on_order_changed_callback += updateUI;

		slots = orders_panel.GetComponentsInChildren<ItemSlot>();
	}

	void Update()
	{

	}

	void updateUI()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (i < orders.orders_list.Count)
			{
				slots[i].addItem(orders.orders_list[i]);
			}
			else
			{
				slots[i].clearSlot();
			}
		}
	}

	public void toggleOrdersInfo(Transform button)
	{
		bool is_shrink = !slots[0].transform.Find("info_panel").gameObject.activeSelf;					// !!!!!! Find GameObject with name !!!!!!
		foreach (ItemSlot slot in slots)
		{
			slot.transform.Find("info_panel").gameObject.SetActive(!slot.transform.Find("info_panel").gameObject.activeSelf);       // !!!!!! Find GameObject with name !!!!!!
		}

		if (is_shrink)
		{
			transform.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 260);
			button.transform.GetComponentInChildren<Text>().text = "H\nI\nD\nE";
		}
		else
		{
			transform.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 260);
			button.transform.GetComponentInChildren<Text>().text = "S\nH\nO\nW";
		}
	}
}
