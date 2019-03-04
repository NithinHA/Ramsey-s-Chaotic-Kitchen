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

}
