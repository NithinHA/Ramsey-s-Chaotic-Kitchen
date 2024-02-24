using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTime : MonoBehaviour
{
	public float RemainingTime;
	public static Action<float> OnGameTimeUpdate;

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
		if (valueBefore != valueAfter)
			OnGameTimeUpdate?.Invoke(RemainingTime);	// gets invoked if the RemainingTime as integer changes.
    }
}
