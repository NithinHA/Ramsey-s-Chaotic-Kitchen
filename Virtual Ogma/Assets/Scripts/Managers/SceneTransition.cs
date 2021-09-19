using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    Animator anim;

    public static SceneTransition instance;
    void Awake()
    {
        instance = this;

        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        
    }

    public void restartLevel()
    {
        int scene_index = SceneManager.GetActiveScene().buildIndex;     // get current scene index
        StartCoroutine(sceneTransitionCoroutine(scene_index));
    }

    public void mainMenu()
    {
        StartCoroutine(sceneTransitionCoroutine("main_menu"));
    }

    public void sceneTransition(int scene_index)        // assign this method to buttons in the end-game canvas
    {
        StartCoroutine(sceneTransitionCoroutine(scene_index));
    }

    public void sceneTransition(string scene_name)
    {
        StartCoroutine(sceneTransitionCoroutine(scene_name));
    }

    IEnumerator sceneTransitionCoroutine(int scene_index)
    {
        anim.SetTrigger("end");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene_index);
    }

    IEnumerator sceneTransitionCoroutine(string scene_name)
    {
        anim.SetTrigger("end");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene_name);
    }
}
