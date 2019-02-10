using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour {

	NavMeshAgent agent;
	Transform target;

	void Start () {
		agent = GetComponent<NavMeshAgent>();
	}
	
	void Update () {
		if (target)
		{
			agent.SetDestination(target.position);
		}
	}

	public void MovePlayer(PositionScript destination)
	{
		agent.stoppingDistance = destination.radius;
		agent.SetDestination(destination.transform.position);
	}

	public void Follow(PositionScript ps)
	{
		target = ps.transform;
		//StartCoroutine(FollowTarget());
	}
	///////////working with coroutines to increase performance////////////
	//IEnumerator FollowTarget()
	//{
	//	if (target)
	//	{
	//		agent.SetDestination(target.position);
	//	}
	//	yield return new WaitForSeconds(0.1f);
	//	FollowTarget();
	//}

	public void StopFollowing()
	{
		target = null;
	}
}
