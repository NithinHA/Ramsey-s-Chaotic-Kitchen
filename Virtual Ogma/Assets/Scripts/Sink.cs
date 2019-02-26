using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
	public bool is_washing = false;

	[SerializeField] int[] utensil_count_arr;
	static int[] clean_utensil_arr;

    void Start()
    {
		clean_utensil_arr = new int[utensil_count_arr.Length];
		for (int i = 0; i < utensil_count_arr.Length; i++)
		{
			clean_utensil_arr[i] = utensil_count_arr[i];
		}

		clean_utensil_arr[0] -= 3;
		clean_utensil_arr[1] -= 1;
		clean_utensil_arr[2] -= 2;
    }
	
    void Update()
    {
        
    }

	public void washUtensils(Item utensil)
	{
		int utensil_index = 0;
		switch (utensil.name)
		{
			case "plates":
				utensil_index = 0;
				break;
			case "bowls":
				utensil_index = 1;
				break;
			case "cups":
				utensil_index = 2;
				break;
			default:
				Debug.LogWarning("invalid utensil index!");
				break;
		}

		int dirty_utensils = utensil_count_arr[utensil_index] - clean_utensil_arr[utensil_index];
		float washing_time = utensil.time_to_prepare * dirty_utensils;
		Debug.Log(utensil.name + " to wash:" + dirty_utensils + "\ntime taken:" + washing_time);
		StartCoroutine(washing_utensils(washing_time, utensil_index));
	}

	IEnumerator washing_utensils(float washing_time, int utensil_index)
	{
		yield return new WaitForSeconds(washing_time);
		clean_utensil_arr[utensil_index] = utensil_count_arr[utensil_index];
		is_washing = false;
	}
}
