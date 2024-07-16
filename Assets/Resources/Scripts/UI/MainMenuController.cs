using Heroicsolo.DI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Scripts.UI
{
    public class MainMenuController : MonoBehaviour, ISystem
    {
        [Inject] private MainMenuView menuView;

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

        }

        public void ProccessGameContinue()
        {

        }
    }
}