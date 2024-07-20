using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Customer : MonoBehaviour
{
	[Header("Time for first/ next order")]
	public float start_time;
	public float order_time;
	private float cur_time;

	float time_of_order = -1;                    // time at which was made. Used to compute total waiting time of a customer

	Item[] dishes;						// list of all dish Items taken from KeywordsData class
	public Item dish;					//current dish

	public bool is_ordering = false;	// if is_ordering is true, then only the waiter will take order. So this condition will be used in take_order()
	public bool is_served = true;

    [Header("Customer lego model")]
    public GameObject lego_customer;
    Animator anim;

	[Header("Materials for table top indication")]
	public Material food_order_indication_mat;
	public Material food_to_be_served_indication_mat;
	private Material default_mat;

	[Header("table_top GameObject")]
	public GameObject table_top;
	[SerializeField] private CustomerOrderBubble orderBubblePrefab;
	[SerializeField] private Transform bubbleParent;
	[SerializeField] private ParticleSystem m_CoinParticles;

	public Action<Item> onFoodOrdered;
	public Action onFoodServed;

	private CustomerOrderBubble orderBubble;

    void Start()
    {
		cur_time = start_time;

		dishes = KeywordsData.Instance.dish_arr;

        anim = lego_customer.GetComponent<Animator>();
		default_mat = table_top.GetComponent<Renderer>().material;

		orderBubble = Instantiate(orderBubblePrefab, bubbleParent);
		orderBubble.Init(this, transform);
    }
	
    void Update()
    {
		if (LevelController.Instance.CurrentGameState != GameState.Running)
			return;

		if (is_served)
		{
			if (cur_time > 0)
			{
				cur_time -= Time.deltaTime;
			}
			else
			{
				Debug.Log("I want to order something..");
				InstructionPanel.Instance.DisplayInstruction("I want to order something");

                anim.SetBool("is_eating", false);    // stop eating animation
                anim.SetBool("is_ordering", true);   // play ordering animation

				is_ordering = true;
				table_top.GetComponent<Renderer>().material = food_order_indication_mat;	// make the table pink color indicating waiter has to take order from that table
				cur_time = order_time;
				is_served = false;
				time_of_order = Time.time;		// TO MAKE THINGS CHALLENGING, start a timer here and if food is served before 40s => $30 tips, 20s => $50 tips, 10s => $80 tips
			}
		}
	}

	public void order_food()		// called when player says, "TAKE ORDER FROM TABLE __"
	{
		this.dish = dishes[UnityEngine.Random.Range(0, dishes.Length)];
		bool has_added = Orders.Instance.addItem(this.dish);      // add this.dish to orders list
		if (has_added)
		{
			Debug.Log(this.dish.name + " added to orders list");
			InstructionPanel.Instance.DisplayInstruction(this.dish.name + " added to orders list");
		}
		else
		{
			Debug.Log("can not add " + this.dish.name + " to orders list");
			InstructionPanel.Instance.DisplayInstruction("can not add " + this.dish.name + " to orders list");
			is_ordering = true;				// so that the waiter can come back and take order some other time
			return;
		}
        anim.SetBool("is_ordering", false);      // stop ordering animation and go to idle animation

        table_top.GetComponent<Renderer>().material = food_to_be_served_indication_mat;     // make table orange color indicating customer is waiting for the dish to be served

		onFoodOrdered?.Invoke(dish);
	}

	public void food_served()       // called when player says, "SERVE TABLE __"
	{
		is_served = true;
		Orders.Instance.removeItem(this.dish);           // remove dish from orders list

        anim.SetBool("is_eating", true);        // play eating animation

        int serv_delay = (int)(Time.time - time_of_order);

		// customer gives tips depending upon the time customer was waiting
		int tips = 0;
        foreach (var item in Constants.GameConstants.CustomerServDelayTipMap)
        {
			if (serv_delay < item.Key)
            {
				tips = item.Value;
				break;
            }
        }

		InstructionPanel.Instance.DisplayInstruction($"Serve delayed by {serv_delay}s, tips received ${tips} ");
		m_CoinParticles.Play();
		Score.Instance.PayBill(this.dish, tips);		// update score text

		this.dish = null;				// reset dish to null
		tips = 0;						// reset tips value to 0
		// removing dish from inventory and decrementing clean utensil count is done in WaiterAction class
		table_top.GetComponent<Renderer>().material = default_mat;  // make table original color indicating customer has been served and is eating the dish
		onFoodServed?.Invoke();
	}
}
