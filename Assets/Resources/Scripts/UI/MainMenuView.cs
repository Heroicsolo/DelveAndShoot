using Heroicsolo.DI;
using UnityEngine;

namespace Heroicsolo.Scripts.UI
{
    public class MainMenuView : MonoBehaviour, ISystem
    {
        [SerializeField] GameObject continueButton;

        [Inject] private MainMenuController menuController;

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