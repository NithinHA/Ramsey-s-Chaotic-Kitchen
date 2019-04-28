using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
	//private Camera cam;
	private NavMeshAgent agent;

	public Transform starting_transform;		// use transform instead
	//[HideInInspector] public Vector3 starting_rotation;

	public Transform preparing_position;			// used in ChefAction script
	[HideInInspector] public bool is_busy = false;

	[SerializeField] public bool target_reached = true;
	public Vector3 target;
	public float min_dist = 1f;

	public GameObject player_highlighter;
	[SerializeField] GameObject highlighter;

	[SerializeField] private float resolve_rotation_speed = 3f;
	public Coroutine resolve_rotations_coroutine;

	void Start()
    {
		agent = GetComponent<NavMeshAgent>();
        target = starting_transform.position;
		
		//starting_rotation = transform.eulerAngles;
		//cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}
	
    void Update()
    {
		if (Vector3.Distance(target, transform.position) > min_dist)
		{
			agent.SetDestination(target);
			target_reached = false;
			if (!GetComponent<AudioSource>().isPlaying)
				GetComponent<AudioSource>().Play();
		}
		else
		{
			if (!target_reached)
			{
				target_reached = true;
				GetComponent<AudioSource>().Stop();
			}
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

	public void resolveRotationAfterNavigation(Vector3 direction)
	{
		Quaternion look_rotation = Quaternion.LookRotation(direction, Vector3.up);
		Vector3 dir = look_rotation.eulerAngles;
		Debug.Log("Dir:" + dir);
		resolve_rotations_coroutine = StartCoroutine(resolveRotations(new Vector3(0, dir.y, 0)));
	}
    public IEnumerator invokeResolveRotation(Transform destination, float time_to_stop)
    {
        Coroutine resolve_rotations_cor = StartCoroutine(resolveRotations(destination.eulerAngles));
        yield return new WaitForSeconds(time_to_stop);
        if (resolve_rotations_cor != null)
            StopCoroutine(resolve_rotations_cor);
    }
	public IEnumerator resolveRotations(Vector3 direction)
	{
		if (direction.y > transform.eulerAngles.y)
		{
			Debug.Log("increase");
			while (transform.eulerAngles.y < direction.y)
			{
				Debug.Log("R: " + direction + "\nT" + transform.eulerAngles);
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(direction), Time.deltaTime * resolve_rotation_speed);
				yield return null;
			}
			Debug.Log("R: " + direction + "\nT" + transform.eulerAngles);
		}
		else
		{
			Debug.Log("decrease");
			while (transform.eulerAngles.y > direction.y)
			{
				Debug.Log("R: " + direction + "\nT" + transform.eulerAngles);
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(direction), Time.deltaTime * resolve_rotation_speed);
				yield return null;
			}
			Debug.Log("R: " + direction + "\nT" + transform.eulerAngles);
		}
	}

	//public void resolveRotationAfterNavigation(Vector3 direction)
	//{
	//	Vector3 relative_direction = new Vector3(direction.x*0, direction.y , direction.z * 0);
	//	//Debug.Log(direction);
	//	Quaternion look_rotation = Quaternion.LookRotation(direction, Vector3.up);
	//	Debug.Log(look_rotation);
	//	//transform.rotation = look_rotation;
	//	StartCoroutine(resolveRotations(look_rotation));
	//}
	//IEnumerator resolveRotations(Quaternion look_rotation)
	//{
	//	if (look_rotation.y > transform.rotation.y)
	//	{
	//		while (transform.rotation.y < look_rotation.y)
	//		{
	//			transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, Time.deltaTime * resolve_rotation_speed);
	//			yield return null;
	//		}
	//	}
	//	else
	//	{
	//		while (transform.rotation.y > look_rotation.y)
	//		{
	//			transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, Time.deltaTime * resolve_rotation_speed);
	//			yield return null;
	//		}
	//	}
	//}
}
