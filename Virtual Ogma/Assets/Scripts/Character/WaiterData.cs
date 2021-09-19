using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiterData : CharacterData
{
	[Header("Waiter data")]
	[SerializeField] private List<Transform> waiter_interactable = new List<Transform>();         // List of waiter places like Dining table A,B,C & D
	[SerializeField] private List<Transform> waiter_positions = new List<Transform>();      // List of positions that waiter has to be to interact with above objects
	public Dictionary<Transform, Transform> waiter_interactable_positions = new Dictionary<Transform, Transform>();     // A dictionary formed out of above info

	void Awake()
    {
		for (int i = 0; i < waiter_interactable.Count; i++)                                 // initialize waiter_interactable_positoins dictionary
		{
			waiter_interactable_positions.Add(waiter_interactable[i], waiter_positions[i]);
		}
	}
	
    void Update()
    {
        
    }
}
