using Heroicsolo.Inventory;
using Heroicsolo.Scripts.Logics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Heroicsolo.Scripts.Editor.CustomInspectors
{
    [CustomPropertyDrawer(typeof(LootUnit))]
    internal class LootUnit_Inspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var itemId_prop = property.FindPropertyRelative("ItemId");
            var min_prop = property.FindPropertyRelative ("MinAmount");
            var max_prop = property.FindPropertyRelative("MaxAmount");
            var chance_prop = property.FindPropertyRelative("DropChance");
            StyleSheet editorUSS = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/UI/USS/Editor.uss");
            VisualElement root = new();

            root.Add(new PropertyField(itemId_prop, "Item ID"));
            ChildAnnotator amountRow = new();
            amountRow.Add(new PropertyField(min_prop, "Min"));
            amountRow.Add(new PropertyField(max_prop, "Max"));
            amountRow.AddToClassList("row");
            root.Add(amountRow);
            root.Add(new PropertyField(chance_prop));

            root.styleSheets.Add(editorUSS);
            return root;
        }
    }
}
