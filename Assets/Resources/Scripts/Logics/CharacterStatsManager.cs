using Heroicsolo.Logics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Scripts.Logics
{
    public class CharacterStatsManager : MonoBehaviour, ICharacterStatsManager
    {
        [SerializeField] [Min(0f)] private float armorPowerAmplifier = 0.01f;
        [SerializeField] [Min(0f)] private float liftCapacityAmplifier = 1f;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public float GetCapacityFromStrength(float strength)
        {
            return Mathf.Ceil(1f + Mathf.Log10(strength) * liftCapacityAmplifier);
        }

        public float GetDamageAbsorbPercentage(float armorAmount)
        {
            return (1f - 1f / (1f + armorPowerAmplifier * armorAmount));
        }

        public void ModifyCharacterStat(ICharacter character, CharacterStatModifier modifier)
        {
            CharacterStat characterStat = character.GetCharacterStat(modifier.statType);
            
            if (characterStat == null)
            {
                return;
            }

            if (modifier.affectTypes.Contains(ModifierAffectType.MaxValue))
            {
                float valueDiff = modifier.modifierType == ModifierType.Additive 
                    ? modifier.modifierValue 
                    : characterStat.MaxValue * modifier.modifierValue;

                characterStat.ModifyMaxValue(valueDiff);
            }

            if (modifier.affectTypes.Contains(ModifierAffectType.CurrentValue))
            {
                float valueDiff = modifier.modifierType == ModifierType.Additive
                    ? modifier.modifierValue
                    : characterStat.Value * modifier.modifierValue;

                characterStat.Change(valueDiff);
            }

            if (modifier.affectTypes.Contains(ModifierAffectType.RegenRate))
            {
                float valueDiff = modifier.modifierType == ModifierType.Additive
                    ? modifier.modifierValue
                    : characterStat.RegenRate * modifier.modifierValue;

                characterStat.ChangeRegenState(valueDiff);
            }
        }
    }

    [Serializable]
    public struct CharacterStatModifier
    {
        public CharacterStatType statType;
        public ModifierType modifierType;
        public List<ModifierAffectType> affectTypes;
        public float modifierValue;
    }

    public enum ModifierType
    {
        Additive = 0,
        Multiplicative = 10
    }

    public enum ModifierAffectType
    {
        CurrentValue = 0,
        MaxValue = 10,
        RegenRate = 20
    }
}