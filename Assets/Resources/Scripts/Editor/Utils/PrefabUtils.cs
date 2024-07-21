﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Heroicsolo.Scripts.Editor.Utils
{
    internal static class PrefabUtils
    {
        internal static void Place(GameObject prefab, Vector3 pos)
        {
            prefab.transform.position = pos;
            StageUtility.PlaceGameObjectInCurrentStage(prefab);
            GameObjectUtility.EnsureUniqueNameForSibling(prefab);
            Undo.RegisterCreatedObjectUndo(prefab, $"Create GO {prefab.name}");
            Selection.activeGameObject = prefab;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        internal static void PlaceInViewport(GameObject prefab)
        {
            var view = SceneView.lastActiveSceneView;
            Place(prefab,
                  view ? view.pivot : Vector3.zero);
        }
    }
}
