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
    [SerializeField] private GraphicRaycaster m_MenuInteraction;
    [SerializeField] private Transform m_MainMenuT;
    [SerializeField] private Transform m_LevelSelectT;
    [SerializeField] private PostProcessingProfile profile;
    [Space]
    [SerializeField] [Range(-20, 0)] private float m_ExposureLow = -15;
    [SerializeField] [Range(-4f, 1f)] private float m_ExposureHigh = 0;
    [SerializeField] private float m_ExposureTweenTime = 1f;
    private float _currentExposure;

    public Camera MainCam;

    protected override void Awake()
    {
        base.Awake();
        MainCam = Camera.main;
    }

    public void OnEnter()
    {
        m_MenuInteraction.enabled = false;
        EyesToggle(true, m_ExposureTweenTime, () =>
        {
            MainCam.transform.position = m_LevelSelectT.position;
            MainCam.transform.rotation = m_LevelSelectT.rotation;
            EyesToggle(false, m_ExposureTweenTime, () =>
            {
                m_MenuInteraction.enabled = true;
            });
        });

    }

    public void ReturnToMainMenu()
    {
        m_MenuInteraction.enabled = false;
        EyesToggle(true, m_ExposureTweenTime, () =>
        {
            MainCam.transform.position = m_MainMenuT.position;
            MainCam.transform.rotation = m_MainMenuT.rotation;
            EyesToggle(false, m_ExposureTweenTime, () =>
            {
                m_MenuInteraction.enabled = true;
            });
        });
    }

    public void UpdateProfile(bool reset = false)
    {
        var setting = profile.colorGrading.settings;
        setting.basic.postExposure = reset ? m_ExposureHigh : _currentExposure;
        profile.colorGrading.settings = setting;
    }

    public void EyesToggle(bool isClose, float tweenTime,Action onComplete = null)
    {
        Tween exposureTween = DOTween.To(() => _currentExposure, (x) => _currentExposure = x, isClose ? m_ExposureLow : m_ExposureHigh, tweenTime)
            .OnUpdate(() => UpdateProfile())
            .OnComplete(() => onComplete?.Invoke());
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
