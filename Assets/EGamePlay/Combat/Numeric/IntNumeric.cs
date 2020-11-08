using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 整形修饰器
    /// </summary>
    public class IntModifier
    {
        public int Value;
    }
    /// <summary>
    /// 整形修饰器收集器
    /// </summary>
    public class IntModifierCollector
    {
        public int TotalValue { get; private set; }
        private List<IntModifier> Modifiers { get; } = new List<IntModifier>();

        public int AddModifier(IntModifier modifier)
        {
            Modifiers.Add(modifier);
            Update();
            return TotalValue;
        }

        public int RemoveModifier(IntModifier modifier)
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
    /// 整形数值
    /// </summary>
    public class IntNumeric
    {
        public int Value { get; private set; }
        public int baseValue { get; private set; }
        public int add { get; private set; }
        public int pctAdd { get; private set; }
        public int finalAdd { get; private set; }
        public int finalPctAdd { get; private set; }
        private IntModifierCollector AddCollector { get; } = new IntModifierCollector();
        private IntModifierCollector PctAddCollector { get; } = new IntModifierCollector();
        private IntModifierCollector FinalAddCollector { get; } = new IntModifierCollector();
        private IntModifierCollector FinalPctAddCollector { get; } = new IntModifierCollector();


        public void Initialize()
        {
            baseValue = add = pctAdd = finalAdd = finalPctAdd = 0;
        }
        public int SetBase(int value)
        {
            baseValue = value;
            Update();
            return baseValue;
        }
        public void AddAddModifier(IntModifier modifier)
        {
            add = AddCollector.AddModifier(modifier);
            Update();
        }
        public void AddPctAddModifier(IntModifier modifier)
        {
            pctAdd = PctAddCollector.AddModifier(modifier);
            Update();
        }
        public void AddFinalAddModifier(IntModifier modifier)
        {
            finalAdd = FinalAddCollector.AddModifier(modifier);
            Update();
        }
        public void AddFinalPctAddModifier(IntModifier modifier)
        {
            finalPctAdd = FinalPctAddCollector.AddModifier(modifier);
            Update();
        }
        public void RemoveAddModifier(IntModifier modifier)
        {
            add = AddCollector.RemoveModifier(modifier);
            Update();
        }
        public void RemovePctAddModifier(IntModifier modifier)
        {
            pctAdd = PctAddCollector.RemoveModifier(modifier);
            Update();
        }
        public void RemoveFinalAddModifier(IntModifier modifier)
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
            Value = (int)value3;
        }
    }
}