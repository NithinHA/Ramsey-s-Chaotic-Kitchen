using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI time_text;
	public float game_time;
	private string minutes;
	private string seconds;

    void Start()
    {
        
    }
	
    void Update()
    {
		if (LevelController.Instance.CurrentGameState != GameState.Running)
			return;

		if (game_time > 0.0f)
		{
			game_time -= Time.deltaTime;
		}
		else
		{
			game_time = 0.0f;
			LevelController.Instance.ChangeGameState(GameState.End);
			//has_game_ended = true;
		}

		// get minute-second format from seconds format
		minutes = Mathf.Floor(game_time / 60).ToString("00");
		seconds = (game_time % 60).ToString("00");

		// display minutes and seconds on the screen
		time_text.text = string.Format("{0}:{1}", minutes, seconds);

		//else			// game is over
		//{
		//	if (Input.GetKeyDown(KeyCode.Space))			// restart the level
		//	{
		//		Score.score = 0;
		//		Time.timeScale = 1;
		//		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		//	}
		//}
    }
}
