using DG.Tweening;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
	public Transform inventory_panel;
	[SerializeField] private float m_TransitTime = .5f;
	[SerializeField] private InventoryItemSlot[] m_Slots;

	private Inventory inventory;
	private bool _isShrink = true;

	void Start()
    {
		inventory = Inventory.Instance;
		inventory.OnItemAdded += OnInventoryItemAdded;
		inventory.OnItemRemoved += OnInventoryItemRemoved;

		if (m_Slots == null || m_Slots.Length == 0)
			m_Slots = inventory_panel.GetComponentsInChildren<InventoryItemSlot>();

        for (int i = 0; i < m_Slots.Length; i++)
        {
            m_Slots[i].SetSlotIndex(i);
        }
    }
	
	private void OnInventoryItemAdded(Item item)
    {
		InventoryItemSlot nextSlot = GetNextEmptySlot();
		if (nextSlot == null)
		{
			Debug.LogError("Could not find the corresponding item! Item-Add failed.");
			return;
		}

		nextSlot.OnItemAdded(item, UpdateUI);
    }

	private void OnInventoryItemRemoved(Item item, int index)
    {
		InventoryItemSlot correspondingSlot = index == -1 ? FindItemSlotWithItem(item) : m_Slots[index];
		if(correspondingSlot == null)
        {
			Debug.LogError("Could not find the corresponding item! Item-Remove failed.");
			return;
        }

		correspondingSlot.OnItemRemoved(UpdateUI);
	}

	private InventoryItemSlot FindItemSlotWithItem(Item item)
    {
		for (int i = 0; i < m_Slots.Length; i++)
		{
			if (m_Slots[i].Item == item)
			{
				return m_Slots[i];
			}
		}

		Debug.LogError($"Could not find InventorySlot with the item: {item.name}");
		return null;
	}

	private InventoryItemSlot GetNextEmptySlot()
    {
		int index = inventory.food_items.Count - 1;
		return m_Slots[index];
    }

	private void UpdateUI()
	{
		for (int i = 0; i < m_Slots.Length; i++)
		{
			if (i < inventory.food_items.Count)
			{
				m_Slots[i].RefreshSlot(inventory.food_items[i]);
			}
			else
			{
				m_Slots[i].ClearSlot();
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
