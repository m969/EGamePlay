using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class HealthPoint
    {
        public int Value { get; private set; }
        public int MaxValue { get; private set; }


        public void Reset()
        {
            Value = MaxValue;
        }

        public void SetMaxValue(int value)
        {
            MaxValue = value;
        }

        public void Minus(int value)
        {
            Value = Mathf.Max(0, Value - value);
        }

        public void Add(int value)
        {
            Value = Mathf.Min(MaxValue, Value + value);
        }

        public float Percent()
        {
            return (float)Value / MaxValue;
        }

        public int PercentHealth(int pct)
        {
            return (int)(MaxValue * (pct / 100f));
        }

        public bool IsFull()
        {
            return Value == MaxValue;
        }
    }
}