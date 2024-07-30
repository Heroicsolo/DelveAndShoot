using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Dungeonizer;
using UnityEngine;
using Heroicsolo.Utils;
using Heroicsolo.UI.UElements;

namespace Heroicsolo.Scripts.Editor.CustomInspectors
{
    [CustomPropertyDrawer(typeof(SpawnOption))]
    internal class SpawnOption_Inspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var goProp = property.FindPropertyRelative("gameObject");
            var minCntProp = property.FindPropertyRelative("minSpawnCount");
            var maxCntProp = property.FindPropertyRelative("maxSpawnCount");
            var byWallProp = property.FindPropertyRelative("spawnByWall");
            var inTheMiddleProp = property.FindPropertyRelative("spawmInTheMiddle");
            var rotatedProp = property.FindPropertyRelative("spawnRotated");
            var heightProp = property.FindPropertyRelative("heightFix");
            var offsetProp = property.FindPropertyRelative("offsetFix");
            var spawnRoomProp = property.FindPropertyRelative("spawnRoom");

            StyleSheet editorUSS = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/UI/USS/Editor.uss");
            ChildAnnotator root = new();
            root.AddToClassList("row");
            Image preview = new();
            Sprite previwSprite = SpriteUtils.Create(AssetPreview.GetAssetPreview((GameObject)goProp.objectReferenceValue));
            preview.style.flexGrow = .1f;
            if (previwSprite != null)
                preview.sprite = previwSprite;
            //else placeholder
            VisualElement props = new();
            root.Add(preview);
            root.Add(props);
            props.AddToClassList("compact"); 
            props.Add(new PropertyField(goProp));
            Row cntRow = new(
                new PropertyField(minCntProp, "Min"),
                new PropertyField(maxCntProp, "Max")
            );
            props.Add(cntRow);
            props.Add(new PropertyField(byWallProp));
            props.Add(new PropertyField(inTheMiddleProp));
            props.Add(new PropertyField(rotatedProp));
            props.Add(new PropertyField(heightProp));
            props.Add(new PropertyField(offsetProp));
            props.Add(new PropertyField(spawnRoomProp));

            root.styleSheets.Add(editorUSS);
            return root;
        }


    }
}