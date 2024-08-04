using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.UI.UElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Heroicsolo.Scripts.Editor.CustomInspectors
{
    //[CustomPropertyDrawer(typeof(ItemParams))]
    internal class ItemParams_Inspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            #region Props variables
            var tagsProp = property.FindPropertyRelative("Tags");
            #endregion
            StyleSheet editorUSS = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/UI/USS/Editor.uss");
            VisualElement root = new();
            #region Content

            #endregion
            root.styleSheets.Add(editorUSS);
            return root;
        }
    }
}
