using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_script : MonoBehaviour
{
	new Transform camera;
	[SerializeField] float rotation_speed = 20f;

    void Start()
    {
		camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
	}

    void Update()
    {
		//find the vector pointing from our position to the target
		Vector3  _direction = (camera.position - transform.position).normalized;

		//create the rotation we need to be in to look at the target
		Quaternion _lookRotation = Quaternion.LookRotation(_direction);

		//rotate us over time according to speed until we are in the required rotation
		transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * rotation_speed);
	}

}
