using System;
using UnityEngine;

namespace Heroicsolo.Logics
{
    [Serializable]
    public class CharacterStat
    {
        [SerializeField] protected CharacterStatType statType;
        [SerializeField] protected float baseValue;
        [SerializeField] protected float regenRate;
        [SerializeField] protected float regenDelay;
        [SerializeField][Min(0f)] protected float bonusPerLevel = 0f;

        public Action<float, float, float> OnChange;

        protected float currValue;
        protected float currMaxValue;

        [SerializeField]
        private bool isRegenEnabled;
        private float timeToRegen;

        public CharacterStatType StatType => statType;
        public float Value => currValue;
        public float MaxValue => currMaxValue;

        public float Percent => Value / MaxValue;
        public float RegenRate => regenRate;

        public virtual void Change(float change)
        {
            float oldValue = currValue;

            currValue += change;
            currValue = Mathf.Clamp(currValue, 0f, currMaxValue);

            if (change < 0f)
            {
                timeToRegen = regenDelay;
            }

            OnChange?.Invoke(oldValue, currValue, currMaxValue);
        }

        public virtual void Set(float value)
        {
            currValue = value;

            OnChange?.Invoke(currValue, currValue, currMaxValue);
        }

        public virtual void ModifyMaxValue(float change)
        {
            currMaxValue += change;

            Change(change);
        }

        public virtual void ScaleByLevel(int level)
        {
            Reset();

            ModifyMaxValue(bonusPerLevel * level);
        }

        public void SetRegenState(bool enabled)
        {
            isRegenEnabled = enabled;
        }

        public void ChangeRegenState(float amount)
        {
            regenRate += amount;
        }

        public virtual void Reset()
        {
            currMaxValue = baseValue;
            currValue = baseValue;

            OnChange?.Invoke(currValue, currValue, currMaxValue);
        }

        public void Init(Action<float, float, float> onChangeCallback)
        {
            OnChange = onChangeCallback;
            Reset();
        }

        public void Update(float deltaTime)
        {
            if (isRegenEnabled)
            {
                timeToRegen -= deltaTime;

                if (timeToRegen <= 0f)
                {
                    Change(regenRate * deltaTime);
                }
            }
        }
    }

    public enum CharacterStatType
    {
        Health = 0,
        HealthRegen = 1,
        Stamina = 10,
        StaminaRegen = 11,
        MoveSpeed = 20,
        Armor = 30,
        Strength = 40,
        MagicResist = 50,
        FireResist = 60,
        FrostResist = 70
    }
}