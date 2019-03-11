using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownDisplay : MonoBehaviour
{
	private float total_time = -1;
	private float countdown_time;

    void Start()
    {

    }

	public void setTimer(float _time)
	{
		total_time = _time;
		countdown_time = total_time;
	}
	
    void Update()
    {
		if (total_time > 0)
		{
			if (countdown_time > 0)
			{
				countdown_time -= Time.deltaTime;
			}
			//else
			//{
			//	Destroy(gameObject, .5f);		// need to change this!! It should destroy parent gameobject that is the prefab!
			//}
			GetComponent<Image>().fillAmount = countdown_time / total_time;
		}
    }
}
