using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class InitRecognition : MonoBehaviour
{
	KeywordRecognizer keyword_recognizer;
	Dictionary<string, Action> keywords = new Dictionary<string, Action>();
	
	public GameObject[] chefs;
	public GameObject[] waiters;

	[SerializeField] private InventoryUI inventory_UI;
	[SerializeField] private OrdersUI orders_UI;
	

	void Start()
    {
		foreach (GameObject chef in chefs)
			keywords.Add(chef.name, () => initChef(chef));
		foreach (GameObject waiter in waiters)
			keywords.Add(waiter.name, () => initWaiter(waiter));
		keywords.Add("Inventory", () => openInventory());
		keywords.Add("Orders", () => showOrders());
		keywords.Add("Pause game", () => openPauseMenu());

		keyword_recognizer = new KeywordRecognizer(keywords.Keys.ToArray(), ConfidenceLevel.Low);
		keyword_recognizer.OnPhraseRecognized += OnKeywordsRecognized;
	}

	public void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
	{
		Action keyword_action;
		if (keywords.TryGetValue(args.text, out keyword_action))
		{
			keyword_action.Invoke();
		}
	}
	//public GameObject ob;
	//GameObject ob_;
	bool is_listening = true;		// prevents keyword_recognizer to start and stop repeatedly every frame
	void Update()
    {
		if (Input.GetKey(KeyCode.Q))
		{
			if (!keyword_recognizer.IsRunning && is_listening)
			{
				keyword_recognizer.Start();
				is_listening = false;
			}
		}
		else
		{
			if (keyword_recognizer.IsRunning)
			{
				keyword_recognizer.Stop();
				is_listening = true;
			}
			if (!is_listening)
			{
				is_listening = true;
			}
		}

		//control test script.. DELETE it
		//if (Input.GetKeyDown(KeyCode.K))
		//{
		//	ob_ = Instantiate<GameObject>(ob, Vector3.zero, Quaternion.identity);
		//	ob_.AddComponent<Test_script>();
		//}
	}

	GameObject ch_action;
	void initChef(GameObject chef)
	{
		keyword_recognizer.Stop();

		Debug.Log(chef.name + " called");
		Test_script2.ts2.applyText(chef.name + " called");
		ChefAction chef_action = GetComponent<ChefAction>();
		chef_action.init_cooking(chef);

		//keyword_recognizer.Stop();
	}

	void initWaiter(GameObject waiter)
	{
		keyword_recognizer.Stop();

		Debug.Log(waiter.name + " called");
		Test_script2.ts2.applyText(waiter.name + " called");
		WaiterAction waiter_action = GetComponent<WaiterAction>();
		waiter_action.init_serving(waiter);

		//keyword_recognizer.Stop();
	}

	void openInventory()
	{
		inventory_UI.toggleInventory();
	}

	void showOrders()
	{
		orders_UI.toggleOrdersInfo();
	}

	void openPauseMenu()
	{

	}
	
}
