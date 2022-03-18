using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrdersUI : MonoBehaviour
{
	public Transform orders_panel;

	ItemSlot[] slots;

	[SerializeField] private Button info_toggle_button;

	void Start()
	{
		Orders.Instance.onOrderListUpdate += UpdateUI;

		slots = orders_panel.GetComponentsInChildren<ItemSlot>();
	}

    private void OnDestroy()
    {
		if(Orders.Instance != null)
			Orders.Instance.onOrderListUpdate -= UpdateUI;
    }

    void Update()
	{

	}

	void UpdateUI()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (i < Orders.Instance.orders_list.Count)
			{
				slots[i].addItem(Orders.Instance.orders_list[i]);
			}
			else
			{
				slots[i].clearSlot();
			}
		}
	}

	public void toggleOrdersInfo()
	{
		bool is_shrink = !slots[0].transform.Find("info_panel").gameObject.activeSelf;					// !!!!!! Find GameObject with name !!!!!!
		foreach (ItemSlot slot in slots)
		{
			slot.transform.Find("info_panel").gameObject.SetActive(!slot.transform.Find("info_panel").gameObject.activeSelf);       // !!!!!! Find GameObject with name !!!!!!
		}

		if (is_shrink)
		{
			transform.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 260);
			info_toggle_button.GetComponentInChildren<TextMeshProUGUI>().text = "H\nI\nD\nE";
		}
		else
		{
			transform.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 260);
			info_toggle_button.GetComponentInChildren<TextMeshProUGUI>().text = "S\nH\nO\nW";
		}
	}
}
