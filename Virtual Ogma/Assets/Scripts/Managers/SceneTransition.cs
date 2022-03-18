using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using SingletonBase;

public class SceneTransition : Singleton<SceneTransition>
{
    [SerializeField] private Transform m_LeftWall;
    [SerializeField] private Transform m_RightWall;
    [SerializeField] private float m_TransitDuration = .7f;

    public void restartLevel()
    {
        int scene_index = SceneManager.GetActiveScene().buildIndex;     // get current scene index
        TransitScene(scene_index);
    }

    public void mainMenu()
    {
        TransitScene("main_menu");
    }

    public void TransitScene(int sceneIndex)        // assign this method to buttons in the end-game canvas
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Join(m_LeftWall.DOScaleX(1, m_TransitDuration))
        .Join(m_RightWall.DOScaleX(1, m_TransitDuration))
        .SetEase(Ease.OutSine)
        .AppendCallback(() =>
        {
            SceneManager.sceneLoaded += PostSceneTransit;
            SceneManager.LoadScene(sceneIndex);
        });
    }

    public void TransitScene(string sceneName)
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Join(m_LeftWall.DOScaleX(1, m_TransitDuration))
        .Join(m_RightWall.DOScaleX(1, m_TransitDuration))
        .SetEase(Ease.OutSine)
        .AppendCallback(() =>
        {
            SceneManager.sceneLoaded += PostSceneTransit;
            SceneManager.LoadScene(sceneName);
        });
    }

    private void PostSceneTransit(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= PostSceneTransit; 

        Sequence mySequence = DOTween.Sequence();
        mySequence.Join(m_LeftWall.DOScaleX(0, m_TransitDuration))
        .Join(m_RightWall.DOScaleX(0, m_TransitDuration))
        .SetEase(Ease.InSine);
    }
}
