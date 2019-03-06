using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UtensilSlot : MonoBehaviour
{
	[Header("Utensil item")]
	public Item utensil_item;

	[Header("UI")]
	public Image image;
	public Text name_text;
	public Text count_text;

	[Header("Empty utensil indicator")]
	[SerializeField] private Color blink_color;
	[SerializeField] private float blink_time = .1f;
	Color default_color;
	[SerializeField] private float blink_count = 5;

	Utensils utensils;

    void Start()
    {
		utensils = Utensils.instance;
		image.sprite = utensil_item.icon;
		name_text.text = utensil_item.name;
		count_text.text = utensils.clean_utensil_arr[utensils.utensil_index_dict[utensil_item.name]].ToString();

		utensils.on_utensil_changed_callback += updateUtensilsUI;
	}
	
    void Update()
    {
        
    }
	
	public void updateUtensilsUI()
	{
		count_text.text = utensils.clean_utensil_arr[utensils.utensil_index_dict[utensil_item.name]].ToString();
	}

	public void blinkSlot()
	{
		StartCoroutine(blink_slot());
	}
	IEnumerator blink_slot()
	{
		default_color = GetComponent<Image>().color;
		for (int i = 0; i < blink_count; i++)
		{
			if (i % 2 == 0)
			{
				GetComponent<Image>().color = blink_color;
				yield return new WaitForSeconds(blink_time);
			}
			else
			{
				GetComponent<Image>().color = default_color;
				yield return new WaitForSeconds(blink_time);
			}
		}
		GetComponent<Image>().color = default_color;
	}
}
