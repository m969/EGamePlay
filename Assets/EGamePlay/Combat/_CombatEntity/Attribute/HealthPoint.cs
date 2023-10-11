using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class HealthPoint : Entity
    {
        public FloatNumeric HealthPointNumeric;
        public FloatNumeric HealthPointMaxNumeric;
        public int Value { get => (int)HealthPointNumeric.Value; }
        public int MaxValue { get => (int)HealthPointMaxNumeric.Value; }


        public void Reset()
        {
            HealthPointNumeric.SetBase(HealthPointMaxNumeric.Value);
        }

        public void SetMaxValue(int value)
        {
            HealthPointMaxNumeric.SetBase(value);
        }

        public void Minus(int value)
        {
            HealthPointNumeric.MinusBase(value);
        }

        public void Add(int value)
        {
            if (value + Value > MaxValue)
            {
                HealthPointNumeric.SetBase(MaxValue);
                return;
            }
            HealthPointNumeric.AddBase(value);
        }

        public float Percent()
        {
            return (float)Value / MaxValue;
        }

        public int PercentHealth(float pct)
        {
            return (int)(MaxValue * pct);
        }

        public bool IsFull()
        {
            return Value == MaxValue;
        }
    }
}