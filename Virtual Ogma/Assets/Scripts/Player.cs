using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
	//private Camera cam;
	private NavMeshAgent agent;

	[HideInInspector] public Vector3 starting_position;
	public Transform preparing_position;			// used in ChefAction script
	[HideInInspector] public bool is_busy = false;

	[SerializeField] public bool target_reached = true;
	public Vector3 target;
	public float min_dist = 1f;

	public GameObject player_highlighter;
	[SerializeField] GameObject highlighter;

	void Start()
    {
		agent = GetComponent<NavMeshAgent>();
		starting_position = transform.position;
		target = starting_position;
		//cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}
	
    void Update()
    {
		if (Vector3.Distance(target, transform.position) > min_dist)
		{
			agent.SetDestination(target);
			target_reached = false;
		}
		else
		{
			if (!target_reached)
				target_reached = true;
		}
	}

	public void move_player(Vector3 pos)
	{
		//target = new Vector3();
		agent.SetDestination(pos);
	}

	public void wait_for_seconds(float seconds)
	{
		StartCoroutine(waiting(seconds));
	}

	IEnumerator waiting(float seconds)
	{
		yield return new WaitForSeconds(seconds);
	}

	public void highlight_player()
	{
		highlighter = Instantiate(player_highlighter, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
		highlighter.transform.SetParent(transform);
	}
	public void remove_highlighter()
	{
		if (highlighter != null)
			Destroy(highlighter);
	}
}
