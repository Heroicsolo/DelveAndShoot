using Heroicsolo.DI;
using Heroicsolo.Heroicsolo.Player;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public class GameController : MonoBehaviour, IGameController
    {
        [Inject] private RuntimeDungeonGenerator dungeonGenerator;
        [Inject] private IPlayerProgressionManager playerProgressionManager;
        
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

        private void Start()
        {
            dungeonGenerator.GenerateDungeon(4, 3, 5, 2);

            playerController = FindObjectOfType<PlayerController>(true);
        }
    }
}