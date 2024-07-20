using UnityEngine;

public class PauseGamePanel : MonoBehaviour
{
    [SerializeField] private GameObject m_Container;

    private GameState _cachedState;

    /// <summary>
    /// Pause game timer
    /// Change game state to pause
    /// Enable container
    /// </summary>
    public void OnClick_Pause()
    {
        Time.timeScale = 0;
        _cachedState = LevelController.Instance.CurrentGameState;
        LevelController.Instance.ChangeGameState(GameState.Pause);
        m_Container.SetActive(true);
    }

    /// <summary>
    /// Resume game timer
    /// Change game state to running
    /// Disable container
    /// </summary>
    public void OnClick_Resume()
    {
        Time.timeScale = 1;
        LevelController.Instance.ChangeGameState(_cachedState);
        m_Container.SetActive(false);
    }

    public void OnClick_Restart()
    {
        Time.timeScale = 1;
        SceneTransition.Instance.RestartLevel();
    }

    public void OnClick_MainMenu()
    {
        Time.timeScale = 1;
        SceneTransition.Instance.MainMenu();
    }
}
