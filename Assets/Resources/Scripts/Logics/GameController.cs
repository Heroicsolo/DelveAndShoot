using Heroicsolo.DI;
using Heroicsolo.Heroicsolo.Player;
using Heroicsolo.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heroicsolo.Logics
{
    public class GameController : MonoBehaviour, IGameController
    {
        [Inject] private RuntimeDungeonGenerator dungeonGenerator;
        [Inject] private IPlayerProgressionManager playerProgressionManager;
        [Inject] private ScenesLoader scenesLoader;
        [Inject] private IGameUIController gameUIController;
        
        private PlayerController playerController;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public PlayerController GetPlayerController()
        {
            playerController ??= FindObjectOfType<PlayerController>(true);

            return playerController;
        }

        public void LevelCompleted()
        {
            gameUIController.HideIngameUI();
            scenesLoader.LoadSceneAsync("LevelComplete", null);
        }

        public void OnLevelWasLoaded(int level)
        {
            if (SceneManager.GetActiveScene().name != "GameScene")
            {
                return;
            }

            dungeonGenerator.GenerateDungeon(4, 3, 5, 2);

            playerController = FindObjectOfType<PlayerController>(true);

            gameUIController.ShowIngameUI();
        }
    }
}