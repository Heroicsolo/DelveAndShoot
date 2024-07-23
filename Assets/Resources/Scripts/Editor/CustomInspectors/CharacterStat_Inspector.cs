using Heroicsolo.Scripts.Logics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using Heroicsolo.Utils;
using System.Xml.Schema;

namespace Heroicsolo.Scripts.Editor.CustomInspectors
{
    [CustomPropertyDrawer(typeof(CharacterStat))]
    internal class CharacterStat_Inspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            //Gather properties
            var isRegen_prop = property.FindPropertyRelative("isRegenEnabled");
            var regenRate_prop = property.FindPropertyRelative("regenRate");
            var type_prop = property.FindPropertyRelative("statType");
            var baseValue_prop = property.FindPropertyRelative("baseValue");
            var levelBonus_prop = property.FindPropertyRelative("bonusPerLevel");
            //Build UI
            VisualElement root = new();
            var card = new VisualElement();
            card.Add(new PropertyField(type_prop, "Type"));
            card.Add(new PropertyField(baseValue_prop, "Base Value"));
            card.Add(new PropertyField(levelBonus_prop, "Bonus per Level"));
            //Build regen row
            var regen_checkbox = new PropertyField(isRegen_prop, "Regen");
            var regenRate_field = new PropertyField(regenRate_prop, "");
            var regen_box = new VisualElement();
            regen_box.style.flexDirection = FlexDirection.Row;
            regen_box.style.alignItems = Align.FlexStart;
            regenRate_field.style.flexGrow = 1;
            regen_box.Add(regen_checkbox);
            regen_box.Add(regenRate_field);
            card.Add(regen_box);
            root.Add(card);
            //Hide regen rate if unused
            regenRate_field.SetEnabled(isRegen_prop.boolValue);
            return root;
        }
    }
}
