using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
	public bool is_chopping = false;
	private Dictionary<string, float> chopping_time = new Dictionary<string, float>();

	public Player player;
	public Vector3[] waypoints;
	int currentWP = 0;
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
	
	//public void chop_item(GameObject chef, string item, Vector3[] positions)		//we might have to delete this method since we're using coroutines in test_script3 itself
	//{
	//	//player = chef.GetComponent<Player>();
	//	//waypoints = positions;
	//	is_chopping = true;


	//}

	public void chop(string food_item)
	{
		//is_chopping = true;     // if player movement to different positions works appropriately, then this step will be done in cooking.cs where variable 
		// is_chopping will be used as is_busy and chop() will be called once player has reached near cutting_board

		Debug.Log("chopping " + food_item);
		StartCoroutine(chopping_delay(food_item));
		//player.wait_for_seconds(chopping_time);
	}

	IEnumerator chopping_delay(string food_item)
	{
		yield return new WaitForSeconds(chopping_time[food_item]);
		Debug.Log(food_item + " chopped");
		is_chopping = false;
		// add chopped fruits, vegetables or meat to inventory
	}

}
