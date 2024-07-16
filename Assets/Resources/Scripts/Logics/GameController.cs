using Heroicsolo.DI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Scripts.Logics
{
    public class GameController : MonoBehaviour
    {
        [Inject] private RuntimeDungeonGenerator dungeonGenerator;
        [Inject] private IPlayerProgressionManager playerProgressionManager;

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);

            dungeonGenerator.GenerateDungeon(4, 3, 5, 2);
        }
    }
}