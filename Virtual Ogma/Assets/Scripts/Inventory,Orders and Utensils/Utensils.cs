using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonBase;

public class Utensils : Singleton<Utensils>
{
	public delegate void onUtensilChanged();                   // invoke this delegate whenever any change occurs in the number of plates
	public onUtensilChanged on_utensil_changed_callback;

	public Dictionary<string, int> utensil_index_dict = new Dictionary<string, int>();         // a dictionary with keys utensil_name and values utensil_index

	public int[] utensil_count_arr;             // total numer of utensil instances present for serving... eg.- 4 plates, 2 bowls and 2 cups
	public int[] clean_utensil_arr;             // tells how many of the utensils are clean at a given time

	protected override void Awake()
	{
		base.Awake();

		utensil_index_dict.Add("plates", 0);
		utensil_index_dict.Add("bowls", 1);
		utensil_index_dict.Add("cups", 2);

		clean_utensil_arr = new int[utensil_count_arr.Length];      // done here since static members cannot be edited in inspector
		for (int i = 0; i < utensil_count_arr.Length; i++)
		{
			clean_utensil_arr[i] = utensil_count_arr[i];            // at start of game, all the utensils are clean
		}

		clean_utensil_arr[0] = 1;			// for testing
		clean_utensil_arr[1] = 1;
		clean_utensil_arr[2] = 1;
	}

	public bool checkUtensilAvailability(Item utensil)              // if there are any clean utensil of that instance, returns true; else false
	{
		int utensil_index;
		utensil_index_dict.TryGetValue(utensil.name, out utensil_index);
		if (clean_utensil_arr[utensil_index] > 0)
			return true;
		else
			return false;
	}

	public void removeOneUtensil(Item utensil)                      // once the food is served, decrease clean utensil count by 1
	{
		int utensil_index;
		utensil_index_dict.TryGetValue(utensil.name, out utensil_index);
		clean_utensil_arr[utensil_index]--;

		if (on_utensil_changed_callback != null)
			on_utensil_changed_callback.Invoke();			// invoke delegate on removing a utensil
	}

	public void utensilUnavailableIndicator(Item utensil)			// cause blinking of utensil_slot when utensil is unavailable
	{
		int utensil_index;
		utensil_index_dict.TryGetValue(utensil.name, out utensil_index);
		UtensilSlot[] utensil_slot_arr = gameObject.GetComponentsInChildren<UtensilSlot>();
		utensil_slot_arr[utensil_index].blinkSlot();

		// play an indication sound
	}
}
