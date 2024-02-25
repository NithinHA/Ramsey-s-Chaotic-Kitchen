using TMPro;
using UnityEngine;
using DG.Tweening;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private RectTransform _container;
    [SerializeField] private RectTransform _pauseGamePos;
    [SerializeField] private RectTransform _resumeGamePos;
    [SerializeField] private TextMeshProUGUI _timeText;

    private GameTime _gameTime;
    private string _minutes;
    private string _seconds;

    private void Awake()
    {
        _gameTime = FindObjectOfType<GameTime>();
    }

    private void Start()
    {
        ClockAnimationSequence();
        GameTime.OnGameTimeUpdate += OnGameTimeUpdate;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ClockAnimationSequence();
    }

    /// <summary>
    /// Set original position and Time.
    /// Move from _resumeGamePos to _pauseGamePos; Wait for few frames; Move from _pauseGamePos to _resumeGamePos.
    /// </summary>
    private void ClockAnimationSequence()
    {
        if (_gameTime != null)
            OnGameTimeUpdate(_gameTime.RemainingTime);

        _container.SetPositionAndRotation(_resumeGamePos.position, _resumeGamePos.rotation);

        Sequence clockAnimSequence = DOTween.Sequence();
        clockAnimSequence.AppendInterval(.5f)
            .Append(_container.DOMove(_pauseGamePos.position, 1.5f).SetEase(Ease.OutExpo))
            .Join(_container.DORotate(_pauseGamePos.rotation.eulerAngles, 1.5f).SetEase(Ease. OutExpo))
            .AppendInterval(1.5f)
            .Append(_container.DOMove(_resumeGamePos.position, 1f).SetEase(Ease.            OutExpo))
            .Join(_container.DORotate(_resumeGamePos.rotation.eulerAngles, 1f).SetEase(Ease.OutExpo));
    }

    private void OnGameTimeUpdate(float remainingTime)
    {
        // get minute-second from remainingTime
        _minutes = Mathf.Floor(remainingTime / 60).ToString("00");
        _seconds = (remainingTime % 60).ToString("00");

        // display minutes and seconds on the screen
        _timeText.text = string.Format("{0}:{1}", _minutes, _seconds);
    }
}
