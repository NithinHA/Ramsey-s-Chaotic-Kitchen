using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_script2 : MonoBehaviour
{
	public bool bool_var = true;
	//public int a = 5;

    void Start()
    {
        
    }

    void Update()
    {
		if (!bool_var)
		{
			//Debug.Log("false");
			StartCoroutine(temp());
		}
		else
		{
			;//Debug.Log("true");
		}
    }
	
	IEnumerator temp()
	{
		yield return new WaitForSeconds(2);
		bool_var = true;
	}
}
