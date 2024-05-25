using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using SingletonBase;

public class SceneTransition : Singleton<SceneTransition>
{
    [SerializeField] private Animator m_Animator;

    private const string ScreenClose = "st_close";
    private const string ScreenOpen = "st_open";

    public void restartLevel()
    {
        int scene_index = SceneManager.GetActiveScene().buildIndex;     // get current scene index
        TransitScene(scene_index);
    }

    public void mainMenu()
    {
        TransitScene("main_menu");
    }

    public async void TransitScene(int sceneIndex)        // assign this method to buttons in the end-game canvas
    {
        await SceneTransitBase();
        SceneManager.LoadScene(sceneIndex);
    }

    public async void TransitScene(string sceneName)
    {
        await SceneTransitBase();
        SceneManager.LoadScene(sceneName);
    }

    private async Task SceneTransitBase()
    {
        m_Animator.SetTrigger(ScreenClose);
        await Task.Delay(1000);
        SceneManager.sceneLoaded += PostSceneTransit;
    }

    private void PostSceneTransit(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= PostSceneTransit;
        m_Animator.SetTrigger(ScreenOpen);
    }
}
