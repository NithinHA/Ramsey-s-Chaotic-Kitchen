using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
	public bool is_chopping = false;

	Item food_item;			// this is the item cooked rice/noodles that is to be added to inventory

	void Start()
    {

    }

    void Update()
    {
		
    }

	public void chop(Item food_item)
	{
		string food_item_name = food_item.name.Split()[1];
		Debug.Log("chopping " + food_item_name);
		this.food_item = food_item;
		StartCoroutine(chopping_delay(food_item_name));
	}

	IEnumerator chopping_delay(string food_item_name)
	{
		yield return new WaitForSeconds(food_item.time_to_prepare);
		Debug.Log(food_item_name + " chopped");
		is_chopping = false;

		// add chopped fruits, vegetables or meat (food_item) to inventory... DONE
		bool has_added = Inventory.instance.addItem(food_item);
		if (has_added)
			Debug.Log(food_item.name + " added to inventory");
		else
			Debug.Log("can not add " + food_item.name + " to inventory");
	}

}
