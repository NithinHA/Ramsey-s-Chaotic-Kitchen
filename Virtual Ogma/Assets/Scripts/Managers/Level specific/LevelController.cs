using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using SingletonBase;

public class LevelController : Singleton<LevelController>
{
    public GameState CurrentGameState = GameState.Pause;
    public Action<GameState, GameState> OnGameStateChange;

    protected override void Start()
    {
        base.Start();
        AudioManager.Instance?.PlaySound(Constants.Audio.AmbientBg);
        StartCoroutine(StartLevelWithDelay());
    }

    IEnumerator StartLevelWithDelay()
    {
        yield return new WaitForSeconds(5);
        ChangeGameState(GameState.Running);
    }

    private void Update()
    {
        if (CurrentGameState != GameState.End)
            return;

        if (Input.GetKeyDown(KeyCode.Space))            // restart the level
        {
            //Score.score = 0;
            //Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ChangeGameState(GameState newState)
    {
        GameState prevGameState = CurrentGameState;
        CurrentGameState = newState;
        OnGameStateChange?.Invoke(prevGameState, newState);
    }
}

public enum GameState
{
    Running, Pause, End
}