using System;

namespace Heroicsolo.Logics
{
    [Serializable]
    public class CharacterStatInt : CharacterStat
    {
        public new Action<int, int, int> OnChange;
        public new int Value => (int)currValue;
        public new int MaxValue => (int)currMaxValue;

        public new float Percent => Value / MaxValue;

        public void Change(int change)
        {
            base.Change(change);
        }

        public void Set(int value)
        {
            base.Change(value);
        }

        public void ModifyMaxValue(int change)
        {
            base.ModifyMaxValue(change);
        }

        public override void Reset()
        {
            currMaxValue = baseValue;
            currValue = baseValue;

            OnChange?.Invoke(Value, Value, MaxValue);
        }

        public void Init(Action<int, int, int> onChangeCallback)
        {
            OnChange = onChangeCallback;
            Reset();
        }
    }
}