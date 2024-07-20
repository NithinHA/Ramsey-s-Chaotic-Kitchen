using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class OrdersUI : MonoBehaviour
{
	public Transform orders_panel;

	ItemSlot[] slots;

	[SerializeField] private TextMeshProUGUI m_OrdersButtonText;
	[SerializeField] private RectTransform m_OrdersContainerRect;
	[Space]
	[SerializeField] private float m_TransitTime = .5f;
	[SerializeField] private float m_ContainerMin = 75f;
	[SerializeField] private float m_ContainerMax = 250f;
	[SerializeField] private string m_OrdersBtTextHide = "H\nI\nD\nE";
	[SerializeField] private string m_OrdersBtTextShow = "S\nH\nO\nW";

	private bool _isShrink = true;

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
		_isShrink = !_isShrink;
		foreach (ItemSlot slot in slots)
			slot.ToggleSlot(!_isShrink, m_TransitTime);

		Vector2 targetSize = new Vector2(_isShrink ? m_ContainerMin : m_ContainerMax, m_OrdersContainerRect.sizeDelta.y);
		m_OrdersContainerRect.DOSizeDelta(targetSize, m_TransitTime)
			.SetEase(Ease.OutExpo)
			.OnComplete(() => m_OrdersButtonText.text = _isShrink ? m_OrdersBtTextShow : m_OrdersBtTextHide);
	}
}
