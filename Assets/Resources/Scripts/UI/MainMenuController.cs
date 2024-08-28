using Heroicsolo.DI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Scripts.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private MainMenuView menuView;

        [Inject] private ScenesLoader scenesLoader;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void ProccessGameQuit()
        {
            //TODO: Add popup with player approval
            Application.Quit();
        }

        public void ProccessNewGameStart()
        {
            scenesLoader.LoadSceneAsync("GameScene", null);
        }

        public void ProccessGameContinue()
        {
            scenesLoader.LoadSceneAsync("GameScene", null);
        }

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);
        }
    }
}