using System;

namespace EECustom.Utils.JsonElements
{
    public struct ValueBase
    {
        public readonly static ValueBase Unchanged = new(1.0f, ValueMode.Rel, true);
        public readonly static ValueBase Zero = new(0.0f, ValueMode.Abs, false);

        public float Value;
        public ValueMode Mode;
        public bool FromDefault;

        public ValueBase(float value = 1.0f, ValueMode mode = ValueMode.Rel, bool fromDefault = true)
        {
            Value = value;
            Mode = mode;
            FromDefault = fromDefault;
        }

        public float GetAbsValue(float maxValue, float currentValue)
        {
            if (Mode == ValueMode.Rel)
            {
                if (FromDefault)
                    return currentValue * Value;
                else
                    return maxValue * Value;
            }
            else
                return Value;
        }

        public float GetAbsValue(float baseValue)
        {
            if (Mode == ValueMode.Rel)
                return baseValue * Value;
            else
                return Value;
        }

        public int GetAbsValue(int maxValue, int currentValue)
        {
            return (int)Math.Round(GetAbsValue((float)maxValue, currentValue));
        }

        public int GetAbsValue(int baseValue)
        {
            return (int)Math.Round(GetAbsValue((float)baseValue));
        }

        public override string ToString()
        {
            return $"[Mode: {Mode}, Value: {Value}]";
        }
    }

    public enum ValueMode
    {
        Rel,
        Abs
    }
}