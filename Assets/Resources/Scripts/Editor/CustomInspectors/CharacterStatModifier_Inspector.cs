using Heroicsolo.Scripts.Logics;
using Heroicsolo.Scripts.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Heroicsolo.Scripts.Editor.CustomInspectors
{
    [CustomPropertyDrawer(typeof(CharacterStatModifier))]
    internal class CharacterStatModifier_Inspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            #region Props variables
            var statTypeProp = property.FindPropertyRelative("statType");
            var modFuncProp = property.FindPropertyRelative("modifierType");
            var valueProp = property.FindPropertyRelative("modifierValue");
            var affectTypesProp = property.FindPropertyRelative("affectTypes");
            #endregion
            StyleSheet editorUSS = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/UI/USS/Editor.uss");
            VisualElement root = new();
            #region Content
            root.Add(new PropertyField(statTypeProp));
            var modRow = new Row(
                new PropertyField(modFuncProp, "Modifier"),
                new PropertyField(valueProp, "")
                );
            root.Add(modRow);
            root.Add(new PropertyField(affectTypesProp));
            #endregion
            root.styleSheets.Add(editorUSS);
            return root;
        }
    }
}
