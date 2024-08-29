using Heroicsolo.DI;
using Heroicsolo.Heroicsolo.Player;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public class GameController : MonoBehaviour, IGameController
    {
        [Inject] private RuntimeDungeonGenerator dungeonGenerator;
        [Inject] private IPlayerProgressionManager playerProgressionManager;
        [Inject] private ScenesLoader scenesLoader;
        
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
            scenesLoader.LoadSceneAsync("LevelComplete", null);
        }

        public void OnLevelWasLoaded(int level)
        {
            dungeonGenerator.GenerateDungeon(4, 3, 5, 2);

            playerController = FindObjectOfType<PlayerController>(true);
        }
    }
}