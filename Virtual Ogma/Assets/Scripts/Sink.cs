using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
	public bool is_washing = false;

	public float time_to_wash = 1.5f;
	public int no_of_plates = 4;
	public static int clean_plates;

    void Start()
    {
		clean_plates = no_of_plates;
    }
	
    void Update()
    {
        
    }

	public void wash_plates()
	{
		int dirty_plates = no_of_plates - clean_plates;
		float washing_time = time_to_wash * dirty_plates;
		Debug.Log("plates to wash:" + dirty_plates + "\ntime taken:" + washing_time);
		StartCoroutine(washing_plates(washing_time));
	}

	IEnumerator washing_plates(float washing_time)
	{
		yield return new WaitForSeconds(washing_time);
		clean_plates = no_of_plates;
		is_washing = false;
	}
}
