using Heroicsolo.DI;
using UnityEngine;

namespace Heroicsolo.Scripts.UI
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject continueButton;
        [SerializeField] private MainMenuController menuController;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void OnQuitButtonClicked()
        {
            menuController.ProccessGameQuit();
        }

        public void OnNewGameButtonClicked()
        {
            menuController.ProccessNewGameStart();
        }

        public void OnContinueButtonClicked()
        {
            menuController.ProccessGameContinue();
        }
    }
}