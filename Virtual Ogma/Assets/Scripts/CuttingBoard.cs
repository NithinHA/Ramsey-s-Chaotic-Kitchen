using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
	public bool is_chopping = false;

	[SerializeField] private GameObject countdown_display_prefab;

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
		// displpay the countdown_timer and then delete it after the countdown is over
		GameObject countdown_display = Instantiate(countdown_display_prefab, transform.position + new Vector3(0, 1, .5f), Quaternion.Euler(new Vector3(45, 0, 0)));
		countdown_display.transform.SetParent(transform);
		countdown_display.GetComponentInChildren<CountdownDisplay>().setTimer(food_item.time_to_prepare);
		Destroy(countdown_display, food_item.time_to_prepare);

		// display particle effects for chopping action
		GameObject chopping_particles = Instantiate(KeywordsData.instance.item_particles_dict[food_item], transform.position + new Vector3(0, .5f, 0), Quaternion.identity);
		chopping_particles.transform.SetParent(transform);

		// play chopping sound
		GetComponent<AudioSource>().Play();

		yield return new WaitForSeconds(food_item.time_to_prepare);
		Debug.Log(food_item_name + " chopped");
		is_chopping = false;

		Destroy(chopping_particles);

		// stop chopping sound
		GetComponent<AudioSource>().Stop();

		// add chopped fruits, vegetables or meat (food_item) to inventory... DONE
		bool has_added = Inventory.instance.addItem(food_item);
		if (has_added)
			Debug.Log(food_item.name + " added to inventory");
		else
			Debug.Log("can not add " + food_item.name + " to inventory");
	}

}
