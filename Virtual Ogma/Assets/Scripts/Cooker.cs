using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooker : MonoBehaviour
{
	public bool is_cooking = false;
	public bool is_cooked = false;
	Dictionary<string, float> item = new Dictionary<string, float>();		//later make this public or serializefield and control the dictionary from inspector

	// gameobject food_item; -> this is the item cooked rice/noodles that is to be added to inventory

	private float time_to_cook;
	public float time_to_spoil = 10;
	float cur_time;

	private Coroutine cooking_delay_coroutine;

	void Start()
	{
		item.Add("rice", 8);
		item.Add("noodles", 10);

		cur_time = time_to_spoil;
	}

	void Update()
	{
		if (is_cooked)
		{
			if (cur_time > 0) {
				cur_time -= Time.deltaTime;
			} else {
				is_cooking = false;
				is_cooked = false;
				Debug.Log("overcooked");
				cur_time = time_to_spoil;
			}
		}
	}

	public void cook(string food_item)		// called when player says, "COOK food_item AT __"
	{
		is_cooking = true;  // if player movement to different positions works appropriately, then this step will be done in cooking.cs where variable is_cooking 
							// will be used as is_busy and cook() will be called once player has reached near cooker
		Debug.Log("cooking starts");
		time_to_cook = item[food_item];
		cooking_delay_coroutine = StartCoroutine(cooking_delay(time_to_cook));
	}

	IEnumerator cooking_delay(float time_to_cook)
	{
		yield return new WaitForSeconds(time_to_cook);
		//if(is_cooking)
		is_cooked = true;       // at this time, both is_cooking and is_cooked  will be true and system waits for user to turn off cooker
		Debug.Log("indicate turn off cooker!");
	}

	public void turn_off_cooker()       // called when player says, "TURN OFF COOKER __"
	{
		if (cooking_delay_coroutine != null)
		{
			StopCoroutine(cooking_delay_coroutine);
		}

		if (is_cooked)
		{
			// add cooked rice or cooked noodles to inventory
		}
		is_cooking = false;
		is_cooked = false;
		Debug.Log("cooking stops");
	}
}
