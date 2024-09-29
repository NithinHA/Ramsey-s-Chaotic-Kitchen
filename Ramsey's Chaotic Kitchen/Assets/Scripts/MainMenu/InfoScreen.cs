using System.Collections.Generic;
using UnityEngine;

namespace UI.MainMenu
{
    public class InfoScreen : MonoBehaviour
    {
        [SerializeField] private InfoScreenItem[] m_InfoScreenItems;

        private Dictionary<InfoScreenType, GameObject> _infoScreenMap = new Dictionary<InfoScreenType, GameObject>();
        private GameObject _activeInfoScreen = null;

        private void Start()
        {
            foreach (var item in m_InfoScreenItems)
            {
                _infoScreenMap.Add(item.Type, item.Screen);
            }

            _activeInfoScreen = _infoScreenMap[InfoScreenType.General];
            _activeInfoScreen.SetActive(true);
        }

        public void OnClick_InfoTabButton(int infoScreenType)
        {
            InfoScreenType type = (InfoScreenType)infoScreenType;
            if (_activeInfoScreen != null)
                _activeInfoScreen.SetActive(false);

            _activeInfoScreen = _infoScreenMap[type];
            _activeInfoScreen.SetActive(true);
        }

    }

    [System.Serializable]
    public enum InfoScreenType
    {
        General,
        HowToPlay,
        ChefCommands,
        WaiterCommands,
        MiscCommands,
        Recipies
    }

    [System.Serializable]
    public class InfoScreenItem
    {
        public InfoScreenType Type;
        public GameObject Screen;
    }
}