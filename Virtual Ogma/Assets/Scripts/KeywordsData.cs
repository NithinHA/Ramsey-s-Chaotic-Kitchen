using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeywordsData : MonoBehaviour
{
	//-------------option:1-------------//
	// create a structure and read all the calls from csv file
	public struct keywords_data
	{
		string call;				// string that KeywordRecognizer will recognize
		int action_number;			// method number
		string item;				// food item such as vegetables, fruits, rice, etc
		int action_position;		// position A/B
	}

	public static List<keywords_data> chef_keywords_1 = new List<keywords_data>();
	public static List<keywords_data> waiter_keywords_1 = new List<keywords_data>();

	//-------------option:2-------------//
	// create a list of all the calls and decipher the call by using stringbuilder

	[Header("Chef data")]
	public List<string> chef_keywords_2 = new List<string>();

	public List<Transform> chef_places = new List<Transform>();
	public List<Transform> chef_positions = new List<Transform>();
	public Dictionary<Transform, Vector3> chef_place_positions = new Dictionary<Transform, Vector3>();

	[Header("Waiter data")]
	public List<string> waiter_keywords_2 = new List<string>();
	public List<Transform> waiter_places = new List<Transform>();
	public List<Transform> waiter_positions = new List<Transform>();
	public Dictionary<Transform, Vector3> waiter_place_positions = new Dictionary<Transform, Vector3>();

	[Header("Items data")]
	public List<string> item_names = new List<string>();
	public List<Transform> item_positions = new List<Transform>();
	public Dictionary<string, Vector3> chef_item_positions = new Dictionary<string, Vector3>();


	void Start()
    {
		for (int i = 0; i < chef_places.Count; i++)
		{
			chef_place_positions.Add(chef_places[i], chef_positions[i].position);
		}
		for (int i = 0; i < waiter_places.Count; i++)
		{
			waiter_place_positions.Add(waiter_places[i], waiter_positions[i].position);
		}
		for (int i = 0; i < item_names.Count; i++)
		{
			chef_item_positions.Add(item_names[i], item_positions[i].position);
		}
    }
	
}
