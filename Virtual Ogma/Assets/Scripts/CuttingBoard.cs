using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
	public bool is_chopping = false;
	public float chopping_time = 5;

	public Player player;
	public Vector3[] waypoints;
	int currentWP = 0;
	// gameobject food_item; -> this is the item cooked rice/noodles that is to be added to inventory

	void Start()
    {

    }

    void Update()
    {
		
    }
	
	public void chop_item(GameObject chef, string item, Vector3[] positions)
	{
		//player = chef.GetComponent<Player>();
		//waypoints = positions;
		is_chopping = true;


	}

	public void chop()
	{
		//is_chopping = true;     // if player movement to different positions works appropriately, then this step will be done in cooking.cs where variable 
								// is_chopping will be used as is_busy and chop() will be called once player has reached near cutting_board
		StartCoroutine(chopping_delay());
		//player.wait_for_seconds(chopping_time);
	}

	IEnumerator chopping_delay()
	{
		yield return new WaitForSeconds(chopping_time);
		is_chopping = false;
		// add chopped fruits, vegetables or meat to inventory
	}

}
