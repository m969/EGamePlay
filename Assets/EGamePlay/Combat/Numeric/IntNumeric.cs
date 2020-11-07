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
    /// 整形修饰器集
    /// </summary>
    public class IntModifierCollection
    {
        public int Value { get; private set; }
        private List<IntModifier> Modifiers { get; } = new List<IntModifier>();

        public int AppendModifier(IntModifier modifier)
        {
            Modifiers.Add(modifier);
            Update();
            return Value;
        }

        public int RemoveModifier(IntModifier modifier)
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
        private IntModifierCollection AddCollection { get; } = new IntModifierCollection();
        private IntModifierCollection PctAddCollection { get; } = new IntModifierCollection();
        private IntModifierCollection FinalAddCollection { get; } = new IntModifierCollection();
        private IntModifierCollection FinalPctAddCollection { get; } = new IntModifierCollection();


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
        public void AppendAddModifier(IntModifier modifier)
        {
            add = AddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendPctAddModifier(IntModifier modifier)
        {
            pctAdd = PctAddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendFinalAddModifier(IntModifier modifier)
        {
            finalAdd = FinalAddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendFinalPctAddModifier(IntModifier modifier)
        {
            finalPctAdd = FinalPctAddCollection.AppendModifier(modifier);
            Update();
        }
        public void RemoveAddModifier(IntModifier modifier)
        {
            add = AddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemovePctAddModifier(IntModifier modifier)
        {
            pctAdd = PctAddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemoveFinalAddModifier(IntModifier modifier)
        {
            finalAdd = FinalAddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemoveFinalPctAddModifier(IntModifier modifier)
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