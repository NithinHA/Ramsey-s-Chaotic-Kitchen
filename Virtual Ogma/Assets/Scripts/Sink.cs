using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
	public bool is_washing = false;

	Dictionary<string, int> utensil_index_dict = new Dictionary<string, int>();			// a dictionary with keys utensil_name and values utensil_index

	[SerializeField] int[] utensil_count_arr;				// total numer of utensil instances present for serving... eg.- 4 plates, 2 bowls and 2 cups
	static int[] clean_utensil_arr;							// tells how many of the utensils are clean at a given time

    void Start()
    {
		utensil_index_dict.Add("plates", 0);
		utensil_index_dict.Add("bowls", 1);
		utensil_index_dict.Add("cups", 2);

		clean_utensil_arr = new int[utensil_count_arr.Length];		// done here since static members cannot be edited in inspector
		for (int i = 0; i < utensil_count_arr.Length; i++)
		{
			clean_utensil_arr[i] = utensil_count_arr[i];			// at start of game, all the utensils are clean
		}

		// clean_utensil_arr[0] -= 3;
		// clean_utensil_arr[1] -= 1;
		// clean_utensil_arr[2] -= 2;
    }
	
    void Update()
    {
        
    }

	public void washUtensils(Item utensil)				// called when user says "WASH utensil" where utensil can be "plates","bowls" or "cups" 
	{
		int utensil_index = utensil_index_dict[utensil.name];

		int dirty_utensils = utensil_count_arr[utensil_index] - clean_utensil_arr[utensil_index];		// compute number of dirty utensils
		float washing_time = utensil.time_to_prepare * dirty_utensils;									// compute time taken to wash dirty utensils
		Debug.Log(utensil.name + " to wash:" + dirty_utensils + "\ntime taken:" + washing_time);
		Test_script2.ts2.applyText(utensil.name + " to wash:" + dirty_utensils + "\ntime taken:" + washing_time);
		StartCoroutine(washing_utensils(washing_time, utensil_index));
	}

	IEnumerator washing_utensils(float washing_time, int utensil_index)
	{
		yield return new WaitForSeconds(washing_time);
		clean_utensil_arr[utensil_index] = utensil_count_arr[utensil_index];			// cleans all utensil of that instance. ie.- cleans all plates or bowls or cups
		is_washing = false;
	}

	public bool checkUtensilAvailability(Item utensil)				// if there are any clean utensil of that instance, returns true; else false
	{
		int utensil_index;
		utensil_index_dict.TryGetValue(utensil.name, out utensil_index);
		if (clean_utensil_arr[utensil_index] > 0)
			return true;
		else
			return false;
	}

	public void removeOneUtensil(Item utensil)						// once the food is served, decrease clean utensil count by 1
	{
		int utensil_index;
		utensil_index_dict.TryGetValue(utensil.name, out utensil_index);
		clean_utensil_arr[utensil_index]--;
	}
}
