using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 浮点型数值修饰器
    /// </summary>
    public class FloatNumericModifier
    {
        public float Value;
    }
    /// <summary>
    /// 浮点型数值修饰器集合
    /// </summary>
    public class FloatNumericModifierCollection
    {
        public float Value { get; private set; }
        private List<FloatNumericModifier> Modifiers { get; } = new List<FloatNumericModifier>();

        public float AppendModifier(FloatNumericModifier modifier)
        {
            Modifiers.Add(modifier);
            Update();
            return Value;
        }

        public float RemoveModifier(FloatNumericModifier modifier)
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
        private FloatNumericModifierCollection AddCollection { get; } = new FloatNumericModifierCollection();
        private IntNumericModifierCollection PctAddCollection { get; } = new IntNumericModifierCollection();
        private FloatNumericModifierCollection FinalAddCollection { get; } = new FloatNumericModifierCollection();
        private IntNumericModifierCollection FinalPctAddCollection { get; } = new IntNumericModifierCollection();


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
        public void AppendAddModifier(FloatNumericModifier modifier)
        {
            add = AddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendPctAddModifier(IntNumericModifier modifier)
        {
            pctAdd = PctAddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendFinalAddModifier(FloatNumericModifier modifier)
        {
            finalAdd = FinalAddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendFinalPctAddModifier(IntNumericModifier modifier)
        {
            finalPctAdd = FinalPctAddCollection.AppendModifier(modifier);
            Update();
        }
        public void RemoveAddModifier(FloatNumericModifier modifier)
        {
            add = AddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemovePctAddModifier(IntNumericModifier modifier)
        {
            pctAdd = PctAddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemoveFinalAddModifier(FloatNumericModifier modifier)
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
            Value = value3;
        }
    }
}