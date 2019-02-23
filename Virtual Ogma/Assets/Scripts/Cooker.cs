using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooker : MonoBehaviour
{
	public bool is_cooking = false;
	public bool is_cooked = false;
	Dictionary<string, float> cooking_time = new Dictionary<string, float>();       //later make this public or serializefield and control the dictionary from inspector

	string food_item;
	// gameobject food_item; -> this is the item cooked rice/noodles that is to be added to inventory
	
	public float time_to_spoil = 10;
	float cur_time;

	private Coroutine cooking_delay_coroutine;
	private Coroutine spoiling_delay_coroutine;

	void Start()
	{
		cooking_time.Add("rice", 12);
		cooking_time.Add("noodles", 15);

		cur_time = time_to_spoil;
	}

	void Update()
	{
		//if (is_cooked)
		//{
		//	if (cur_time > 0) {
		//		cur_time -= Time.deltaTime;
		//	} else {
		//		is_cooking = false;
		//		is_cooked = false;
		//		Debug.Log("overcooked");
		//		cur_time = time_to_spoil;
		//	}
		//}
	}

	public void cook(string food_item)		// called when player says, "COOK food_item AT __"
	{
		Debug.Log("cooking " + food_item);
		this.food_item = food_item;
		cooking_delay_coroutine = StartCoroutine(cooking_delay(food_item));
	}

	IEnumerator cooking_delay(string food_item)
	{
		yield return new WaitForSeconds(cooking_time[food_item]);
		Debug.Log(food_item + " cooked");
		is_cooked = true;       // at this time, both is_cooking and is_cooked  will be true and system waits for user to turn off cooker
		Debug.Log("indicate turn off cooker!");
		spoiling_delay_coroutine = StartCoroutine(spoiling_delay());
	}

	IEnumerator spoiling_delay()
	{
		yield return new WaitForSeconds(time_to_spoil);
		//instantiate smoke particle effects at cooker position that self destroy after 1s indicating food has spoiled
		is_cooking = false;
		is_cooked = false;
		Debug.Log("Overcooked!");
	}

	public void turn_off_cooker()       // called when player says, "TURN OFF COOKER __"
	{
		if (cooking_delay_coroutine != null)
		{
			StopCoroutine(cooking_delay_coroutine);
		}
		if (spoiling_delay_coroutine != null)
		{
			StopCoroutine(spoiling_delay_coroutine);
		}

		if (is_cooked)
		{
			Debug.Log("adding cooked " + food_item + "to inventory");
			// add cooked rice or cooked noodles to inventory
		}
		is_cooking = false;
		is_cooked = false;
		Debug.Log("cooking stops");
	}
}
