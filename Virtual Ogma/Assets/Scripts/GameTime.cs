using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameTime : MonoBehaviour
{
	[SerializeField] private Text time_text;
	public float game_time;

	[SerializeField] private GameObject end_game_canvas;

	private string minutes;
	private string seconds;

	bool has_game_ended = false;

    void Start()
    {
        
    }
	
    void Update()
    {
		if (!has_game_ended)
		{
			if (game_time > 0.0f)
			{
				game_time -= Time.deltaTime;
			}
			else
			{
				game_time = 0.0f;
				endGame();
				has_game_ended = true;
			}

			// get minute-second format from seconds format
			minutes = Mathf.Floor(game_time / 60).ToString("00");
			seconds = (game_time % 60).ToString("00");

			// display minutes and seconds on the screen
			time_text.text = string.Format("{0}:{1}", minutes, seconds);
		}
		else			// game is over
		{
			if (Input.GetKeyDown(KeyCode.Space))			// restart the level
			{
				Score.score = 0;
				Time.timeScale = 1;
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}
    }

	void endGame()
	{
		Time.timeScale = 0;					// this pauses the game.. no time related functions can operate. Therefore, we can not even use yield return waitForSeconds()
		end_game_canvas.SetActive(true);
		end_game_canvas.GetComponentInChildren<Text>().text = "Score: " + Score.score;      // displays score at end of the game
	}
}
