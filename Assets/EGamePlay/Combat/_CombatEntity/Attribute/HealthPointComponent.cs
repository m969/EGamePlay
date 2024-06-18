using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class HealthPointComponent : Component
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

        public void SetDie()
        {
            HealthPointNumeric.MinusBase(Value);
        }

        public float ToPercent()
        {
            return (float)Value / MaxValue;
        }

        public int GetPercentHealth(float pct)
        {
            return (int)(MaxValue * pct);
        }

        public bool IsFull()
        {
            return Value == MaxValue;
        }

        public void ReceiveDamage(IActionExecute combatAction)
        {
            var damageAction = combatAction as DamageAction;
            Minus(damageAction.DamageValue);
        }

        public void ReceiveCure(IActionExecute combatAction)
        {
            var cureAction = combatAction as CureAction;
            Add(cureAction.CureValue);
        }

        public bool CheckDead()
        {
            return Value <= 0;
        }
    }
}