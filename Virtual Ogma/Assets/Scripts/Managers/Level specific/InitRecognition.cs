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
	
	public Player[] chefs;
	public Player[] waiters;

	[SerializeField] private InventoryUI inventory_UI;
	[SerializeField] private OrdersUI orders_UI;

    void Start()
    {
		foreach (Player chef in chefs)
			keywords.Add(chef.characterData.CharacterName, () => InitCharacter(chef, CharacterType.chef));
		foreach (Player waiter in waiters)
			keywords.Add(waiter.characterData.CharacterName, () => InitCharacter(waiter, CharacterType.waiter));
		keywords.Add(Constants.Keywords.Inventory, () => openInventory());
		keywords.Add(Constants.Keywords.Orders, () => showOrders());
        keywords.Add(Constants.Keywords.Help, () => showManual());
        keywords.Add(Constants.Keywords.Restart, () => restartGame());
        keywords.Add(Constants.Keywords.MainMenu, () => loadMainMenu());

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
        //if (LevelController.Instance.CurrentGameState != GameState.Running)
        //    return;

        if (Input.GetKey(KeyCode.Q) && LevelController.Instance.CurrentGameState == GameState.Running)
		{
			if (!keyword_recognizer.IsRunning && is_listening)
			{
				keyword_recognizer.Start();
				is_listening = false;
				AudioManager.Instance?.PlaySound(Constants.Audio.ListenerOn);
			}
		}
		else
		{
			if (keyword_recognizer.IsRunning)
			{
				keyword_recognizer.Stop();
				is_listening = true;
				AudioManager.Instance?.PlaySound(Constants.Audio.ListenerOff);
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

	private void InitCharacter(Player character, CharacterType type)
    {
		keyword_recognizer.Stop();
		Debug.Log(character.name + " called");
		InstructionPanel.Instance.DisplayInstruction(character.name + " called");
		CharacterAction characterAction = type == CharacterType.chef ? GetComponent<ChefAction>() as CharacterAction : 
			GetComponent<WaiterAction>() as CharacterAction;
		characterAction.Init(character);
	}

	void openInventory()
	{
		inventory_UI.toggleInventory();
	}

	void showOrders()
	{
		orders_UI.toggleOrdersInfo();
	}

    void showManual()
    {
        ManualUI.Instance.showManual();
    }

    void restartGame()
	{
        SceneTransition.Instance.restartLevel();
    }
	
    void loadMainMenu()
    {
        SceneTransition.Instance.mainMenu();
    }
}
