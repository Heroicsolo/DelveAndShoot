using Heroicsolo.Logics.Mobs;
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
