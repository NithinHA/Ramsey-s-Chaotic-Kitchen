using TMPro;
using UnityEngine;
using DG.Tweening;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private RectTransform _bgPanel;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Start()
    {
        LevelController.Instance.OnGameStateChange += OnGameStateChange;
    }

    private void OnDestroy()
    {
        if (LevelController.Instance != null)
            LevelController.Instance.OnGameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(GameState prevState, GameState curState)
    {
        if (curState != GameState.End)
            return;

        _container.gameObject.SetActive(true);
        _scoreText.text = $"Total earnings\n<color=yellow>${Score.score}</color>";

        _bgPanel.localScale = Vector3.zero;
        _bgPanel.DOScale(1, 1f).SetEase(Ease.OutExpo);
    }

    public void OnClick_Restart()       // incomplete
    {
        SceneTransition.Instance.RestartLevel();
    }

    public void OnClick_MainMenu()      // incomplete
    {
        SceneTransition.Instance.MainMenu();
    }
}
