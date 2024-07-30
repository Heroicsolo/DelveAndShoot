using Assets.FantasyInventory.Scripts.Data;
using Heroicsolo.UI.UElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Heroicsolo.Scripts.Editor.CustomInspectors
{
    [CustomPropertyDrawer(typeof(Property))]
    internal class Property_Inspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            #region Props variables
            var idProp = property.FindPropertyRelative("Id");
            var valueProp = property.FindPropertyRelative("Value");
            #endregion
            StyleSheet editorUSS = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/UI/USS/Editor.uss");
            VisualElement root = new();
            #region Content
            var row = new Row(
                    new PropertyField(idProp, ""),
                    new PropertyField(valueProp)
                    );
            //row.AddToClassList("compact");
            root.Add(row);
            #endregion
            root.styleSheets.Add(editorUSS);
            return root;
        }
    }
}
