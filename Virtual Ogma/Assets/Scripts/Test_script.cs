using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_script : MonoBehaviour
{
	public GameObject[] tables;

	//Coroutine temp_coroutine;

    void Start()
    {
		//temp_coroutine = StartCoroutine(temp(5));
	}

    void Update()
    {

		//if (Input.GetKeyDown(KeyCode.Space))
		//	StopCoroutine(temp_coroutine);

		if (tables[0].GetComponent<Customer>().is_ordering && Input.GetKeyDown(KeyCode.A))
			tables[0].GetComponent<Customer>().order_food();
		if (!tables[0].GetComponent<Customer>().is_ordering && !tables[0].GetComponent<Customer>().is_served && Input.GetKeyDown(KeyCode.Z))
			tables[0].GetComponent<Customer>().food_served();

		if (tables[1].GetComponent<Customer>().is_ordering && Input.GetKeyDown(KeyCode.S))
			tables[1].GetComponent<Customer>().order_food();
		if (!tables[1].GetComponent<Customer>().is_ordering && !tables[1].GetComponent<Customer>().is_served && Input.GetKeyDown(KeyCode.X))
			tables[1].GetComponent<Customer>().food_served();

		if (tables[2].GetComponent<Customer>().is_ordering && Input.GetKeyDown(KeyCode.D))
			tables[2].GetComponent<Customer>().order_food();
		if (!tables[2].GetComponent<Customer>().is_ordering && !tables[2].GetComponent<Customer>().is_served && Input.GetKeyDown(KeyCode.C))
			tables[2].GetComponent<Customer>().food_served();

		if (tables[3].GetComponent<Customer>().is_ordering && Input.GetKeyDown(KeyCode.F))
			tables[3].GetComponent<Customer>().order_food();
		if (!tables[3].GetComponent<Customer>().is_ordering && !tables[3].GetComponent<Customer>().is_served && Input.GetKeyDown(KeyCode.V))
			tables[3].GetComponent<Customer>().food_served();
	}

	//IEnumerator temp(int num)
	//{
	//	yield return new WaitForSeconds(num);
	//	Debug.Log("destroyed");
	//	Destroy(gameObject);
	//}
}
