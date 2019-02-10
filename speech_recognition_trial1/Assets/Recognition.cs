using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class Recognition : MonoBehaviour {

	int i = 0;
	

	KeywordRecognizer keyword_recognizer;       // used to listen to specific words
	Dictionary<string, Action> keywords = new Dictionary<string, Action>();

	void Start () {
		keywords.Add("where am i", Call_where_am_i);
		keywords.Add("who are you", Call_who_are_you);
		keywords.Add("what should i do", Call_what_should_i_do);
		keywords.Add("what to do", Call_what_should_i_do);
		keywords.Add("direction", Call_direction);
		keywords.Add("shoot", Call_fire);

		keyword_recognizer = new KeywordRecognizer(keywords.Keys.ToArray());
        keyword_recognizer.OnPhraseRecognized += OnKeywordsRecognized;
        keyword_recognizer.Start();
	}

	private void Update()
	{
		
	}

	void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
	{
        if (Input.GetKey(KeyCode.Q))
        {
            Action keyword_action;
            if (keywords.TryGetValue(args.text, out keyword_action))
            {
                keyword_action.Invoke();
            }
        }
	}

	void Call_where_am_i()
	{
		string[] reply = { "You're at your home", "You're at college", "I dont know. Look around you", "Ask someone else" };
		Debug.Log(reply[UnityEngine.Random.Range(0,reply.Length)]);
	}
	void Call_who_are_you()
	{
		string[] reply = { "I'm Elfen", "I'm your best bud", "I should be asking you that. Who are you?", "You tell me" };
		Debug.Log(reply[UnityEngine.Random.Range(0, reply.Length)]);
	}
	void Call_what_should_i_do()
	{
		string[] reply = { "Test me", "Modify me", "What do you normally do at this time?", "How bout sleep?" };
		Debug.Log(reply[UnityEngine.Random.Range(0, reply.Length)]);
	}

	void Call_clockwise()
	{
		gameObject.transform.rotation = Quaternion.EulerRotation(Vector3.up * 5);
		Debug.Log("clockwise rotate");
	}
	void Call_anticlockwise()
	{
		gameObject.transform.rotation = Quaternion.EulerRotation(Vector3.up * -5);
		Debug.Log("anti-clockwise rotate");
	}

	void Call_direction()
	{
		if (i % 2 == 0)
		{
			Call_clockwise();
			i++;
		}
		else
		{
			Call_anticlockwise();
			i++;
		}
	}

	void Call_fire()
	{
		Debug.Log("Shoot!!!!!!!");
	}
}
