using Dungeonizer;
using Heroicsolo.DI;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public class RuntimeDungeonGenerator : MonoBehaviour, ISystem
    {
        [SerializeField] private Dungeonizer.Dungeonizer dungeonizer;
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private float dungeonAngle = 45f;

        private bool dungeonReady;

        public NavMeshSurface NavMeshSurface => navMeshSurface;

        public bool IsDungeonReady()
        {
            return dungeonReady;
        }

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

            dungeonizer.transform.rotation = Quaternion.Euler(0f, dungeonAngle, 0f);

            dungeonizer.Generate();

            navMeshSurface.BuildNavMesh();

            dungeonReady = true;
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