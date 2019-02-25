using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
	public bool is_chopping = false;
	private Dictionary<string, float> chopping_time = new Dictionary<string, float>();

	string food_item;
	// gameobject food_item; -> this is the item cooked rice/noodles that is to be added to inventory

	void Start()
    {
		chopping_time.Add("vegetables", 5);
		chopping_time.Add("fruits", 5);
		chopping_time.Add("meat", 7);
    }

    void Update()
    {
		
    }

	public void chop(string food_item)
	{
		Debug.Log("chopping " + food_item);
		this.food_item = food_item;
		StartCoroutine(chopping_delay(food_item));
	}

	IEnumerator chopping_delay(string food_item)
	{
		yield return new WaitForSeconds(chopping_time[food_item]);
		Debug.Log(food_item + " chopped");
		is_chopping = false;
		// add chopped fruits, vegetables or meat (food_item) to inventory!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	}

}
