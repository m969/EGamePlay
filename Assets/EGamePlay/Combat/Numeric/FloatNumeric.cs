using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 浮点型修饰器
    /// </summary>
    public class FloatModifier
    {
        public float Value;
    }
    /// <summary>
    /// 浮点型修饰器收集器
    /// </summary>
    public class FloatModifierCollector
    {
        public float TotalValue { get; private set; }
        private List<FloatModifier> Modifiers { get; } = new List<FloatModifier>();

        public float AddModifier(FloatModifier modifier)
        {
            Modifiers.Add(modifier);
            Update();
            return TotalValue;
        }

        public float RemoveModifier(FloatModifier modifier)
        {
            Modifiers.Remove(modifier);
            Update();
            return TotalValue;
        }

        public void Update()
        {
            TotalValue = 0;
            foreach (var item in Modifiers)
            {
                TotalValue += item.Value;
            }
        }
    }
    /// <summary>
    /// 浮点型数值
    /// </summary>
    public class FloatNumeric
    {
        public float Value { get; private set; }
        public float baseValue { get; private set; }
        public float add { get; private set; }
        public int pctAdd { get; private set; }
        public float finalAdd { get; private set; }
        public int finalPctAdd { get; private set; }
        private FloatModifierCollector AddCollector { get; } = new FloatModifierCollector();
        private IntModifierCollector PctAddCollector { get; } = new IntModifierCollector();
        private FloatModifierCollector FinalAddCollector { get; } = new FloatModifierCollector();
        private IntModifierCollector FinalPctAddCollector { get; } = new IntModifierCollector();


        public void Initialize()
        {
            //baseValue = add = pctAdd = finalAdd = finalPctAdd = 0;
        }
        public float SetBase(float value)
        {
            baseValue = value;
            Update();
            return baseValue;
        }
        public void AddAddModifier(FloatModifier modifier)
        {
            add = AddCollector.AddModifier(modifier);
            Update();
        }
        public void AddPctAddModifier(IntModifier modifier)
        {
            pctAdd = PctAddCollector.AddModifier(modifier);
            Update();
        }
        public void AddFinalAddModifier(FloatModifier modifier)
        {
            finalAdd = FinalAddCollector.AddModifier(modifier);
            Update();
        }
        public void AddFinalPctAddModifier(IntModifier modifier)
        {
            finalPctAdd = FinalPctAddCollector.AddModifier(modifier);
            Update();
        }
        public void RemoveAddModifier(FloatModifier modifier)
        {
            add = AddCollector.RemoveModifier(modifier);
            Update();
        }
        public void RemovePctAddModifier(IntModifier modifier)
        {
            pctAdd = PctAddCollector.RemoveModifier(modifier);
            Update();
        }
        public void RemoveFinalAddModifier(FloatModifier modifier)
        {
            finalAdd = FinalAddCollector.RemoveModifier(modifier);
            Update();
        }
        public void RemoveFinalPctAddModifier(IntModifier modifier)
        {
            finalPctAdd = FinalPctAddCollector.RemoveModifier(modifier);
            Update();
        }

        public void Update()
        {
            var value1 = baseValue;
            var value2 = (value1 + add) * (100 + pctAdd) / 100f;
            var value3 = (value2 + finalAdd) * (100 + finalPctAdd) / 100f;
            Value = value3;
        }
    }
}