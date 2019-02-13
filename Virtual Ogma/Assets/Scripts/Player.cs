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

    void Start()
    {
		agent = GetComponent<NavMeshAgent>();
		starting_position = transform.position;
		//cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}
	
    void Update()
    {
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

	public void move_player(Vector3 pos)
	{
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

}
