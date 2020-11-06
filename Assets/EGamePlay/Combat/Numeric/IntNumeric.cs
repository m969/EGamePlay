using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 整形数值修饰器
    /// </summary>
    public class IntNumericModifier
    {
        public int Value;
    }
    /// <summary>
    /// 整形数值修饰器集合
    /// </summary>
    public class IntNumericModifierCollection
    {
        public int Value { get; private set; }
        private List<IntNumericModifier> Modifiers { get; } = new List<IntNumericModifier>();

        public int AppendModifier(IntNumericModifier modifier)
        {
            Modifiers.Add(modifier);
            Update();
            return Value;
        }

        public int RemoveModifier(IntNumericModifier modifier)
        {
            Modifiers.Remove(modifier);
            Update();
            return Value;
        }

        public void Update()
        {
            Value = 0;
            foreach (var item in Modifiers)
            {
                Value += item.Value;
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
        private IntNumericModifierCollection AddCollection { get; } = new IntNumericModifierCollection();
        private IntNumericModifierCollection PctAddCollection { get; } = new IntNumericModifierCollection();
        private IntNumericModifierCollection FinalAddCollection { get; } = new IntNumericModifierCollection();
        private IntNumericModifierCollection FinalPctAddCollection { get; } = new IntNumericModifierCollection();


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
        public void AppendAddModifier(IntNumericModifier modifier)
        {
            add = AddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendPctAddModifier(IntNumericModifier modifier)
        {
            pctAdd = PctAddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendFinalAddModifier(IntNumericModifier modifier)
        {
            finalAdd = FinalAddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendFinalPctAddModifier(IntNumericModifier modifier)
        {
            finalPctAdd = FinalPctAddCollection.AppendModifier(modifier);
            Update();
        }
        public void RemoveAddModifier(IntNumericModifier modifier)
        {
            add = AddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemovePctAddModifier(IntNumericModifier modifier)
        {
            pctAdd = PctAddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemoveFinalAddModifier(IntNumericModifier modifier)
        {
            finalAdd = FinalAddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemoveFinalPctAddModifier(IntNumericModifier modifier)
        {
            finalPctAdd = FinalPctAddCollection.RemoveModifier(modifier);
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