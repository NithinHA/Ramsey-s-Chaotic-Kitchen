using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private RectTransform _container;
    [SerializeField] private RectTransform _pauseGamePos;
    [SerializeField] private RectTransform _resumeGamePos;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private Ease _easeMode = Ease.Linear;

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

    private void ClockAnimationSequence()
    {
        if (_gameTime != null)
            OnGameTimeUpdate(_gameTime.RemainingTime);

        _container.SetPositionAndRotation(_resumeGamePos.position, _resumeGamePos.rotation);

        Sequence clockAnimSequence = DOTween.Sequence();
        clockAnimSequence.Append(_container.DOMove(_pauseGamePos.position, 1.5f).SetEase(Ease.OutBack))
            .Join(_container.DORotate(_pauseGamePos.rotation.eulerAngles, 1.5f).SetEase(Ease.OutBack))
            .AppendInterval(1.5f)
            .Append(_container.DOMove(_resumeGamePos.position, 1.5f).SetEase(Ease.InBack))
            .Join(_container.DORotate(_resumeGamePos.rotation.eulerAngles, 1.5f).SetEase(Ease.InBack));
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
