using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private RectTransform m_Container;
    [SerializeField] private TextMeshProUGUI m_ScoreText;
    [Space]
    [SerializeField] private Image m_ScoreFill;
    [SerializeField] private float m_HueMin = 0;
    [SerializeField] private float m_HueMax = 120;


    private float _scoreProgress = 0;

    private float _hue;
    private float _saturation;
    private float _val;

    private float _alertScale = 1.1f;
    private Vector3 _shakeStrength = new Vector3(0, 0, 25);
    private int _shakeVibraton = 6;
    private float _shakeRandomness = 5f;

    void Start()
    {
        Score.OnScoreUpdate += UpdateScore;
        Color.RGBToHSV(m_ScoreFill.color, out _hue, out _saturation, out _val);
    }

    void OnDestroy()
    {
        Score.OnScoreUpdate -= UpdateScore;
    }

    private void UpdateScore(int score, bool animate)
    {
        if(!animate)
        {
            SetScoreText(score);
            _scoreProgress = 0;
            OnUpdateProgress();
            return;
        }

        SetScoreText(score);
        PerformShakeAnimation(() =>
        {
            FillProgressBar(score);
        });
    }

    private void SetScoreText(int score)
    {
        m_ScoreText.text = $"${score}";
    }

    private void PerformShakeAnimation(Action onComplete = null)
    {
        Sequence simpleAlertSequence = DOTween.Sequence();
        simpleAlertSequence.Append(m_Container.DOScale(_alertScale, .1f))
            .Join(m_Container.DOShakeRotation(1f, _shakeStrength, _shakeVibraton, _shakeRandomness, true, ShakeRandomnessMode.Harmonic))
            .Append(m_Container.DOScale(Vector3.one, .1f))
            .OnComplete(() => onComplete.Invoke());
    }

    private void FillProgressBar(int score)
    {
        float newFillAmount = Mathf.Clamp01((float)score / Score.Instance.TargetScore);
        DOTween.To(() => _scoreProgress, x => _scoreProgress = x, newFillAmount, 1f)
            .OnUpdate(OnUpdateProgress);
    }

    private void OnUpdateProgress()
    {
        m_ScoreFill.fillAmount = _scoreProgress;
        _hue = (m_HueMin + (m_HueMax - m_HueMin) * _scoreProgress)/360;
        m_ScoreFill.color = Color.HSVToRGB(_hue, _saturation, _val);
    }
}
