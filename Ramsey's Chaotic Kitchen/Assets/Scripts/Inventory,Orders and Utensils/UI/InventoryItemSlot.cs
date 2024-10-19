using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryItemSlot : MonoBehaviour
{
	public Item Item { get; private set; }

	[SerializeField] private RectTransform m_Button;
	[SerializeField] private Image m_Icon;                      // applies to both inventory and orders
	[SerializeField] private Button m_CloseButton;             // applies to only inventory

	private Tween _ongoingTween = null;
	private Color _defaultIconColor = Color.white;
	private int _slotIndex = -1;

    private void Start()
    {
		_defaultIconColor = m_Icon.color;
	}

	public void SetSlotIndex(int index) => _slotIndex = index;

    public void RefreshSlot(Item new_item)      // when a new item is added to INVENTORY or new ORDER is made by a customer
	{
		this.Item = new_item;

		m_Icon.sprite = Item.icon;            // changes the sprite of icon in both inventory and orders to icon of this.item
		m_Icon.enabled = true;
		m_Icon.color = _defaultIconColor;

		m_CloseButton.gameObject.SetActive(true);               // to remove unwanted item from inventory
	}

	public void ClearSlot()                 // when an item is removed from INVENTORY or an ORDER is served to customer
	{
		Item = null;                // remove the item alloted to this slot

		m_Icon.sprite = null;         // remove the sprite
		m_Icon.enabled = false;       // make the sprite not visible since null value will disply an ugly white square
		m_Icon.color = _defaultIconColor;

		m_CloseButton.gameObject.SetActive(false);          // disable remove item button from inventory
	}

	public void OnItemAdded(Item new_item, Action onComplete = null)
    {
		// play effect
		if (_ongoingTween != null)
			_ongoingTween.Kill();

		_ongoingTween = FadeImageAlpha(m_Icon, 0, 1, .5f, onComplete);
		m_Button.DOShakeRotation(0.5f, 10f, 10, 90f);
		// play sfx
		AudioManager.Instance?.PlaySound(Constants.Audio.InventoryAdd);
		RefreshSlot(new_item);		// this is required because, the image should have a sprite attached before FadeImageAlpha anim.
    }

	public void OnItemRemoved(Action onComplete = null)
	{
		if (_ongoingTween != null)
			_ongoingTween.Kill();

		// play effect
		_ongoingTween = FadeImageAlpha(m_Icon, 1, 0, .5f, onComplete);
		m_Button.DOShakeRotation(0.5f, 10f, 10, 90f);
		// play sfx
		AudioManager.Instance?.PlaySound(Constants.Audio.InventoryRemove);
	}

	public Tween FadeImageAlpha(Image targetImage, float sourceAlpha, float targetAlpha, float duration, Action onComplete = null)
	{
		var col = targetImage.color;
		col.a = sourceAlpha;
		targetImage.color = col;
		return targetImage.DOFade(targetAlpha, duration).OnComplete(() => onComplete?.Invoke());
	}

#region Button OnClicks

	public void onInventoryButton()
	{
		if (this.Item != null)
		{
			Debug.Log("Item selected: " + Item.name);
			InstructionPanel.Instance.DisplayInstruction("Item selected: " + Item.name);
		}
	}

	public void onCloseButton()
	{
		Inventory.Instance.removeItem(Item, _slotIndex);
	}

#endregion
}
