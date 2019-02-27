using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_script2 : MonoBehaviour
{
	Text display_text;

	public static Test_script2 ts2;		//singleton

	private void Start()
	{
		ts2 = this;
		display_text = GetComponent<Text>();
	}

	public void applyText(string text)
	{
		display_text.text = text;
	}
}
