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