using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UtensilSlot : MonoBehaviour
{
	[Header("Utensil item")]
	public Item utensil_item;

	[Header("UI")]
	public Image image;
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI countText;
	[SerializeField] private Image m_Bg;

	[Header("Utensil indicators")]
	[SerializeField] private Color m_EmptyBlinkColor;
	[SerializeField] private float m_EmptyBlinkTime = .1f;
	[SerializeField] private float m_EmptyBlinkCount = 5;
	[Space]
	[SerializeField] private Color m_RefreshColor;

	Color _defaultColor;
	Utensils _utensils;
	Coroutine _blinkCoroutine;

    void Start()
    {
		_utensils = Utensils.Instance;
		image.sprite = utensil_item.icon;
		nameText.text = utensil_item.name;
		countText.text = _utensils.clean_utensil_arr[_utensils.utensil_index_dict[utensil_item.name]].ToString();

		_utensils.on_utensil_changed_callback += updateUtensilsUI;
		_defaultColor = m_Bg.color;
	}
	
    void Update()
    {
        
    }
	
	public void updateUtensilsUI()
	{
		countText.text = _utensils.clean_utensil_arr[_utensils.utensil_index_dict[utensil_item.name]].ToString();
	}

	public void BlinkSlot()
	{
		if (_blinkCoroutine != null)
			StopCoroutine(_blinkCoroutine);

		_blinkCoroutine = StartCoroutine(BlinkSlotRoutine());
	}
	IEnumerator BlinkSlotRoutine()
	{
		m_Bg.fillAmount = 1;
		for (int i = 0; i < m_EmptyBlinkCount; i++)
		{
			if (i % 2 == 0)
			{
				m_Bg.color = m_EmptyBlinkColor;
				yield return new WaitForSeconds(m_EmptyBlinkTime);
			}
			else
			{
				m_Bg.color = _defaultColor;
				yield return new WaitForSeconds(m_EmptyBlinkTime);
			}
		}

		m_Bg.color = _defaultColor;
		_blinkCoroutine = null;
	}

	public void OnSlotRefresh(float duration)
	{
		if (_blinkCoroutine != null)
			StopCoroutine(_blinkCoroutine);

		_blinkCoroutine = StartCoroutine(RefreshSlotRoutine(duration));
	}
	IEnumerator RefreshSlotRoutine(float duration)
	{
		m_Bg.color = m_RefreshColor;
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			m_Bg.fillAmount = Mathf.Lerp(0, 1, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		yield return new WaitForSeconds(.2f);
		m_Bg.color = _defaultColor;
		m_Bg.fillAmount = 1;
	}
}
