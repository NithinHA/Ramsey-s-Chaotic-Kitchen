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
	
	public GameObject empty_gameobject;

	void Start()
    {
		foreach (GameObject chef in chefs)
			keywords.Add(chef.name, () => init_chef(chef));
		foreach (GameObject waiter in waiters)
			keywords.Add(waiter.name, () => init_waiter(waiter));
		keywords.Add("Pause game", () => open_pause_menu());

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
	public GameObject ob;
	GameObject ob_;
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
			//if (selected_player != null)        // to remove player highlighter a player was selected
			//{
			//	selected_player.GetComponent<Player>().remove_highlighter();
			//	selected_player = null;
			//}
		}

		//control test script.. DELETE it
		if (Input.GetKeyDown(KeyCode.K))
		{
			ob_ = Instantiate<GameObject>(ob, Vector3.zero, Quaternion.identity);
			ob_.AddComponent<Test_script>();
		}
	}

	GameObject ch_action;
	void init_chef(GameObject chef)
	{
		//CharacterScript ch = new CharacterScript(chef, "chef");
		
		ch_action = Instantiate(empty_gameobject, Vector3.zero, Quaternion.identity);
		ch_action.AddComponent<Cooking>();
		ch_action.GetComponent<Cooking>().init_cooking(chef);
		
		keyword_recognizer.Stop();
		
	}

	void init_waiter(GameObject waiter)
	{
		if (!waiter.GetComponent<Player>().is_busy)
		{
			//CharacterScript ch = new CharacterScript(waiter, "waiter");

		}
	}

	void open_pause_menu()
	{

	}
}
