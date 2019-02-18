using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
	[Header("Time for first/ next order")]
	public float start_time;
	public float order_time;
	private float cur_time;

	/*[HideInInspector]*/ public bool is_ordering = false;	// if is_ordering is true, then only the waiter will take order. So this condition will be used in take_order()
	/*[HideInInspector]*/ public bool is_served = true;

	[Header("Materials for table top indication")]
	public Material food_order_indication_mat;
	public Material food_to_be_served_indication_mat;
	private Material default_mat;

	[Header("table_top GameObject")]
	public GameObject table_top;

    void Start()
    {
		cur_time = start_time;

		default_mat = table_top.GetComponent<Renderer>().material;
    }
	
    void Update()
    {
		if (is_served)
		{
			if (cur_time > 0)
			{
				cur_time -= Time.deltaTime;
			}
			else
			{
				Debug.Log("I want to order something..");
				is_ordering = true;
				table_top.GetComponent<Renderer>().material = food_order_indication_mat;	// make the table pink color indicating waiter has to take order from that table
				cur_time = order_time;
				is_served = false;
			}
		}
    }

	public void order_food()		// called when player says, "TAKE ORDER FROM TABLE __"
	{
		string[] dishes = { "salad", "sushi", "biryani", "fruit juice", "burger" };
		Debug.Log("I'll have " + dishes[Random.Range(0, dishes.Length)]);
		is_ordering = false;
		table_top.GetComponent<Renderer>().material = food_to_be_served_indication_mat;		// make table orange color indicating customer is waiting for the dish to be served
		// select a random dish from the list and add it to orders list
	}

	public void food_served()       // called when player says, "SERVE TABLE __"
	{
		Debug.Log("Now I start eating!");
		is_served = true;
		Sink.clean_plates--;
		table_top.GetComponent<Renderer>().material = default_mat;	// make table original color indicating customer has been served and is eating the dish
		// pop that dish from orders list. Also remember to pop it from inventory list
	}

}
