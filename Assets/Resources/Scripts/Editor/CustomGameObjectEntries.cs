using Heroicsolo.Scripts.Logics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Heroicsolo.Scripts.Editor
{
    public static class CustomGameObjectEntries
    {
        [MenuItem("GameObject/Game Logic/Spawner")]
        public static void PlaceSpawner(MenuCommand command)
        {
            GameObject spawner = ObjectFactory.CreateGameObject("Spawner", typeof(MobsSpawner));
            Utils.PrefabUtils.PlaceInViewport(spawner);
        }
    }
}
