using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTime : MonoBehaviour
{
	public enum GameTimeAlert
    {
		Alert_2m, Alert_1m, Alert_30s, Alert_10s, Alert_5s, TimeUp
    }

	public float RemainingTime;
	public static Action<float> OnGameTimeUpdate;
	public static Action<GameTimeAlert> OnGameTimeAlert; 

    void Start()
    {
        
    }
	
    void Update()
    {
		if (LevelController.Instance.CurrentGameState != GameState.Running)
			return;

		int valueBefore = Mathf.FloorToInt(RemainingTime);
		
		if (RemainingTime > 0.0f)
		{
			RemainingTime -= Time.deltaTime;
		}
		else
		{
			RemainingTime = 0.0f;
			LevelController.Instance.ChangeGameState(GameState.End);
		}

		int valueAfter = Mathf.FloorToInt(RemainingTime);
		if (valueBefore > valueAfter && valueAfter >= 0)
        {
			OnGameTimeUpdate?.Invoke(RemainingTime);    // gets invoked if the RemainingTime as integer changes.
			CheckForGameTimeAlert(valueAfter);
        }
    }

	private void CheckForGameTimeAlert(int remainingTimeInSec)
    {
        switch (remainingTimeInSec)
        {
			case 120:
				OnGameTimeAlert?.Invoke(GameTimeAlert.Alert_2m);
				break;
			case 60:
				OnGameTimeAlert?.Invoke(GameTimeAlert.Alert_1m);
				break;
			case 30:
				OnGameTimeAlert?.Invoke(GameTimeAlert.Alert_30s);
				break;
			case 10:
				OnGameTimeAlert?.Invoke(GameTimeAlert.Alert_10s);
				break;
			case 5:
				OnGameTimeAlert?.Invoke(GameTimeAlert.Alert_5s);
				break;
            case 0:
                OnGameTimeAlert?.Invoke(GameTimeAlert.TimeUp);
                break;
        }
    }
}
