using Dungeonizer;
using Heroicsolo.DI;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace Heroicsolo.Scripts.Logics
{
    public class RuntimeDungeonGenerator : MonoBehaviour, ISystem
    {
        [SerializeField] private Dungeonizer.Dungeonizer dungeonizer;
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private float dungeonAngle = 45f;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void GenerateDungeon(int roomsCount, int minRoomSize, int maxRoomSize, int roomMargin)
        {
            dungeonizer.roomMargin = roomMargin;
            dungeonizer.maximumRoomCount = roomsCount;
            dungeonizer.minRoomSize = minRoomSize;
            dungeonizer.maxRoomSize = maxRoomSize;

            dungeonizer.Generate();

            dungeonizer.transform.rotation = Quaternion.Euler(0f, dungeonAngle, 0f);

            navMeshSurface.BuildNavMesh();
        }

        public void DeleteCurrentDungeon()
        {
            dungeonizer.ClearOldDungeon();
        }

        private void Awake()
        {
            SystemsManager.RegisterSystem(this);
        }
    }
}