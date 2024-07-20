using UnityEngine;
using DG.Tweening;

public class UILevelSelect : MonoBehaviour
{
    [SerializeField] private Animator m_ElevatorAnim;
    [SerializeField] private Transform m_ElevatorDestination;

    private const string ElevatorAnimKey = "ElevatorEnter";
    private bool _isLevelSelected = false;

    public void SelectLevel(int index)
    {
        if (_isLevelSelected)
            return;

        _isLevelSelected = true;
        Transform mainCamT = MainMenuController.Instance.MainCam.transform;

        m_ElevatorAnim.SetTrigger(ElevatorAnimKey);
        mainCamT.DORotate(m_ElevatorDestination.rotation.eulerAngles, .5f)
            .OnComplete(() => {
            mainCamT.DOMove(m_ElevatorDestination.position, .8f)
                .OnComplete(() => SceneTransition.Instance.TransitScene(index));
            });
    }

    public void OnBackButton()
    {
        MainMenuController.Instance.ReturnToMainMenu();
    }
}
