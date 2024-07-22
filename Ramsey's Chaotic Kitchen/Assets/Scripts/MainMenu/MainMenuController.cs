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
    private enum MainMenuStage
    {
        Street, LevelSelect, InfoScreen
    }

    [SerializeField] private Image m_Eyelids;
    [SerializeField] private Transform m_MainMenuT;
    [SerializeField] private Transform m_LevelSelectT;
    [SerializeField] private Transform m_InfoScreenT;
    [Space]
    [SerializeField] private float m_BlinkTime = 1f;

    private Color _cachedEyelidsColor;
    private float _eyelidsAlpha;

    private MainMenuStage _currentMainMenuStage = MainMenuStage.Street;

    public Camera MainCam { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        MainCam = Camera.main;
    }

    protected override void Start()
    {
        base.Start();
        AudioManager.Instance.PlaySound(Constants.Audio.MainMenuBg);
    }

    private void Update()
    {
        if(_currentMainMenuStage == MainMenuStage.Street)
        {
            if(Input.GetKeyDown(KeyCode.Return))
                OnEnter();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        AudioManager.Instance.StopSound(Constants.Audio.MainMenuBg);
    }

    public void OnEnter()
    {
        _currentMainMenuStage = MainMenuStage.LevelSelect;
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
            _currentMainMenuStage = MainMenuStage.Street;
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

    public void OnHelpButton()
    {
        _currentMainMenuStage = MainMenuStage.InfoScreen;
        EyesToggle(true, m_BlinkTime, () =>
        {
            MainCam.transform.position = m_InfoScreenT.position;
            MainCam.transform.rotation = m_InfoScreenT.rotation;
            EyesToggle(false, m_BlinkTime);
        });
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
