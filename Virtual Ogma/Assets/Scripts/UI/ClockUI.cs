using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private RectTransform _container;
    [SerializeField] private RectTransform _pauseGamePos;
    [SerializeField] private RectTransform _resumeGamePos;
    [SerializeField] private TextMeshProUGUI _timeText;

    private GameTime _gameTime;
    private string _minutes;
    private string _seconds;

    private Vector3 _simpleAlertShakeStrength = new Vector3(0, 0, 25);
    private int _simpleAlertShakeVibrato = 6;
    private float _simpleAlertShakeRandomness = 5f;
    private ShakeRandomnessMode _simpleAlertShakeMode = ShakeRandomnessMode.Harmonic;
    private float _simpleAlertScale = 1.2f;

    private bool _isLast10sAlert = false;
    private float _countdownAlertScale = 1.5f;

    private void Awake()
    {
        _gameTime = FindObjectOfType<GameTime>();
    }

    private IEnumerator Start()
    {
        GameTime.OnGameTimeUpdate += OnGameTimeUpdate;
        GameTime.OnGameTimeAlert += OnGameTimeAlert;
        yield return new WaitForSeconds(.5f);
        ClockAnimationSequence();
    }

    private void OnDestroy()
    {
        GameTime.OnGameTimeUpdate -= OnGameTimeUpdate;
        GameTime.OnGameTimeAlert -= OnGameTimeAlert;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //    _isLast10sAlert = !_isLast10sAlert;
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
            .Join(_container.DORotate(_pauseGamePos.rotation.eulerAngles, 1.5f).SetEase(Ease.OutExpo))
            .AppendInterval(1.5f)
            .Append(_container.DOMove(_resumeGamePos.position, 1f).SetEase(Ease.OutExpo))
            .Join(_container.DORotate(_resumeGamePos.rotation.eulerAngles, 1f).SetEase(Ease.OutExpo));
    }

    private void OnGameTimeUpdate(float remainingTime)
    {
        // get minute-second from remainingTime
        remainingTime = Mathf.Floor(remainingTime);
        _minutes = Mathf.Floor(remainingTime / 60).ToString("00");
        _seconds = (remainingTime % 60).ToString("00");

        // display minutes and seconds on the screen
        _timeText.text = string.Format("{0}:{1}", _minutes, _seconds);
        if (_isLast10sAlert)
            ClockCountdownAlert();
    }

    private void OnGameTimeAlert(GameTime.GameTimeAlert type)
    {
        switch (type)
        {
            case GameTime.GameTimeAlert.Alert_2m:
            case GameTime.GameTimeAlert.Alert_1m:
            case GameTime.GameTimeAlert.Alert_30s:
                SimpleClockAlert();
                break;
            case GameTime.GameTimeAlert.Alert_10s:
                _isLast10sAlert = true;
                ClockCountdownAlert();      // this makes sure to trigger the alert for the time=10s
                break;
            case GameTime.GameTimeAlert.Alert_5s:   // nothing
                break;
            case GameTime.GameTimeAlert.TimeUp:
                TimeUpAlert();
                break;
        }
    }

    /// <summary>
    /// Simple shake animation: Scale up by 1.2x and perform a z-axis-shake-rotation for 2 sec, and scale back.
    /// </summary>
    private void SimpleClockAlert()
    {
        AudioManager.Instance?.PlaySound(Constants.Audio.ClockAlert);
        Sequence simpleAlertSequence = DOTween.Sequence();
        simpleAlertSequence.Append(_container.DOScale(_simpleAlertScale, .1f))
            .Join(_container.DOShakeRotation(2f, _simpleAlertShakeStrength, _simpleAlertShakeVibrato, _simpleAlertShakeRandomness, true, _simpleAlertShakeMode))
            .Append(_container.DOScale(Vector3.one, .2f));
    }
    
    /// <summary>
    /// Simple scale-up followed by scale-down with an ease mode.
    /// </summary>
    private void ClockCountdownAlert()
    {
        AudioManager.Instance.PlaySound(Constants.Audio.ClockCountdownAlert);
        _container.DOScale(_countdownAlertScale, .1f).SetEase(Ease.OutExpo).
            OnComplete(() => _container.DOScale(Vector3.one, .2f).SetEase(Ease.OutExpo));
    }

    private void TimeUpAlert()
    {
        _isLast10sAlert = false;
        AudioManager.Instance.PlaySound(Constants.Audio.TimeUp);
    }
}