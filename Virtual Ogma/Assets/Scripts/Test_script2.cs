using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class Test_script2 : MonoBehaviour
{
	GameObject character;
	bool is_listening = false;

	public void initiate(GameObject ch)
	{
		character = ch;
		ch.GetComponent<Player>().highlight_player();
		is_listening = true;
	}
	
	//[SerializeField] private List<string> keywords = new List<string>();
	//[SerializeField]  private List<Transform> item_pos = new List<Transform>();
	Dictionary<string, Vector3> item_positions = new Dictionary<string, Vector3>();

	KeywordRecognizer keyword_recognizer;
	Dictionary<string, Action> keywords_dict = new Dictionary<string, Action>();

	private void Awake()
	{
		GameObject keywords_data = GameObject.FindGameObjectWithTag("character data");                  // !!!!!! Find GameObject with tag !!!!!!
		//keywords = keywords_data.GetComponent<KeywordsData>().item_names;
		//item_pos = keywords_data.GetComponent<KeywordsData>().item_positions;
		item_positions = keywords_data.GetComponent<KeywordsData>().chef_item_positions;
	}

	void Start()
	{
		//for (int i = 0; i < keywords.Length; i++)
		//{
		//	keywords_dict.Add(keywords[i], () => MoveToPosition(item_pos[i].position));
		//}
		foreach(KeyValuePair<string, Vector3> item_pos in item_positions)
		{
			keywords_dict.Add(item_pos.Key, () => MoveToPosition(item_pos.Value));
		}

		keyword_recognizer = new KeywordRecognizer(keywords_dict.Keys.ToArray(), ConfidenceLevel.Low);
		keyword_recognizer.OnPhraseRecognized += OnKeywordsRecognized;
	}

	public void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
	{
		//Debug.Log("Recognized");
		Action keyword_action;
		if (keywords_dict.TryGetValue(args.text, out keyword_action))
		{
			//Debug.Log("recognized");
			keyword_action.Invoke();
		}
	}

	void Update()
	{

		if (Input.GetKey(KeyCode.Q))
		{
			if (!keyword_recognizer.IsRunning && is_listening)      // !keyword_recognizer.IsRunning not required, is_listening will alone do the trick
			{
				Debug.Log("give a cooking command");
				keyword_recognizer.Start();
				//is_listening = false;
			}
		}
		else
		{
			if (keyword_recognizer.IsRunning)               // if(keyword_recognizer.IsRunning) could be replaced with if(is_listening)
			{
				is_listening = false;
				keyword_recognizer.Stop();
				character.GetComponent<Player>().remove_highlighter();
			}
			//Destroy(gameObject);		//not here do this later
		}
		//else
			//Debug.Log("NOT listening");
	}


	public void MoveToPosition(Vector3 position)
	{
		Player player = character.GetComponent<Player>();
		Debug.Log("moving ");
		if (!player.is_busy)
		{
			player.is_busy = true;
			player.move_player(position);
			player.is_busy = false;
		}
	}
	
}
