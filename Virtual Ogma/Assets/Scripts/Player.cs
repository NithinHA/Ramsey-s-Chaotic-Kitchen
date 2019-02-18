using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
	//private Camera cam;
	private NavMeshAgent agent;

	[HideInInspector] public Vector3 starting_position;
	[HideInInspector] public bool is_busy = false;

	public bool target_reached = true;
	public Vector3 target;
	public float min_dist = 0.1f;

	public GameObject player_highlighter;
	GameObject highlighter;

	void Start()
    {
		agent = GetComponent<NavMeshAgent>();
		starting_position = transform.position;
		target = starting_position;
		//cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}
	
    void Update()
    {
		if(Vector3.Distance(target, transform.position) > min_dist)
		{
			agent.SetDestination(target);
			target_reached = false;
		}
		else
		{
			if(!target_reached)
				target_reached = true;
		}

		//if (Input.GetMouseButtonDown(0))
		//{
		//	Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		//	RaycastHit hit;
		//	if (Physics.Raycast(ray, out hit))
		//	{
		//		move_player(hit.point);
		//	}
		//}

		//if (Input.GetKeyDown(KeyCode.Space))
		//{
		//	wait_for_seconds(4);
		//}

		//if (is_busy)
		//	Debug.Log("Busy");
		//else
		//	Debug.Log("Free");
		
	}

	public void path(Vector3 init_pos, Vector3 final_pos, float time_dealy)
	{

	}
	public void path(Vector3 init_pos, Vector3 mid_pos, Vector3 final_pos, float time_1, float time_2)
	{

	}


	public void move_player(Vector3 pos)
	{
		target = new Vector3();
		//agent.SetDestination(pos);
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
		highlighter = Instantiate(player_highlighter, transform.position + new Vector3(0, -1, 0), Quaternion.identity);
		highlighter.transform.SetParent(transform);
	}
	public void remove_highlighter()
	{
		if (highlighter != null)
			Destroy(highlighter);
	}

	//public bool is_player_idle()
	//{
	//	if (agent.velocity.x <= 0 || agent.velocity.z <= 0)
	//		return true;
	//	else
	//		return false;
	//}

}
