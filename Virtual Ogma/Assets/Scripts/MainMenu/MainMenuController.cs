using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using DG.Tweening;
using UnityEngine.UI;
using System;
using SingletonBase;

public class MainMenuController : Singleton<MainMenuController>
{
    [SerializeField] private Image m_Eyelids;
    [SerializeField] private Transform m_MainMenuT;
    [SerializeField] private Transform m_LevelSelectT;
    [Space]
    [SerializeField] private float m_BlinkTime = 1f;

    private Color _cachedEyelidsColor;
    private float _eyelidsAlpha;

    public Camera MainCam { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        MainCam = Camera.main;
    }

    public void OnEnter()
    {
        EyesToggle(true, m_BlinkTime, () =>
        {
            MainCam.transform.position = m_LevelSelectT.position;
            MainCam.transform.rotation = m_LevelSelectT.rotation;
            EyesToggle(false, m_BlinkTime);
        });

    }

    public void ReturnToMainMenu()
    {
        EyesToggle(true, m_BlinkTime, () =>
        {
            MainCam.transform.position = m_MainMenuT.position;
            MainCam.transform.rotation = m_MainMenuT.rotation;
            EyesToggle(false, m_BlinkTime);
        });
    }

    public void UpdateProfile()
    {
        _cachedEyelidsColor = m_Eyelids.color;
        _cachedEyelidsColor.a = _eyelidsAlpha;
        m_Eyelids.color = _cachedEyelidsColor;
    }

    public void EyesToggle(bool isClose, float tweenTime,Action onComplete = null)
    {
        if (isClose)
            m_Eyelids.raycastTarget = true;

        float target = isClose ? 1 : 0;

        Tween exposureTween = DOTween.To(() => _eyelidsAlpha, (x) => _eyelidsAlpha = x, target, tweenTime)
            .OnUpdate(UpdateProfile)
            .OnComplete(() => 
            { 
                onComplete?.Invoke();
                if (!isClose)
                    m_Eyelids.raycastTarget = false;
            });
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
