using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTime : MonoBehaviour
{
	[SerializeField] private Text time_text;
	public float game_time;

	private string minutes;
	private string seconds;

    void Start()
    {
        
    }
	
    void Update()
    {
        if(game_time > 0.0f)
		{
			game_time -= Time.deltaTime;
		}
		else
		{
			game_time = 0.0f;
		}

		minutes = Mathf.Floor(game_time / 60).ToString("00");
		seconds = (game_time % 60).ToString("00");

		time_text.text = string.Format("{0}:{1}", minutes, seconds);
    }
}
