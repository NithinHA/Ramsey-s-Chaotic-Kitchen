using TMPro;
using UnityEngine;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Start()
    {
        LevelController.Instance.OnGameStateChange += OnGameStateChange;
    }

    private void OnDestroy()
    {
        LevelController.Instance.OnGameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(GameState prevState, GameState curState)
    {
        if(curState == GameState.End)
        {
            _container.SetActive(true);
            _scoreText.text = "Total earnings\n$" + Score.score;
        }
    }
}
