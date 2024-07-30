using Heroicsolo.Inventory;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Heroicsolo.UI.UElements;

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
            Row amountRow = new(
                new PropertyField(min_prop, "Min"),
                new PropertyField(max_prop, "Max")
                );
            root.Add(amountRow);
            root.Add(new PropertyField(chance_prop));

            root.styleSheets.Add(editorUSS);
            return root;
        }
    }
}
