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
    /// 浮点型修饰器集
    /// </summary>
    public class FloatModifierCollection
    {
        public float Value { get; private set; }
        private List<FloatModifier> Modifiers { get; } = new List<FloatModifier>();

        public float AppendModifier(FloatModifier modifier)
        {
            Modifiers.Add(modifier);
            Update();
            return Value;
        }

        public float RemoveModifier(FloatModifier modifier)
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
        private FloatModifierCollection AddCollection { get; } = new FloatModifierCollection();
        private IntModifierCollection PctAddCollection { get; } = new IntModifierCollection();
        private FloatModifierCollection FinalAddCollection { get; } = new FloatModifierCollection();
        private IntModifierCollection FinalPctAddCollection { get; } = new IntModifierCollection();


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
        public void AppendAddModifier(FloatModifier modifier)
        {
            add = AddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendPctAddModifier(IntModifier modifier)
        {
            pctAdd = PctAddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendFinalAddModifier(FloatModifier modifier)
        {
            finalAdd = FinalAddCollection.AppendModifier(modifier);
            Update();
        }
        public void AppendFinalPctAddModifier(IntModifier modifier)
        {
            finalPctAdd = FinalPctAddCollection.AppendModifier(modifier);
            Update();
        }
        public void RemoveAddModifier(FloatModifier modifier)
        {
            add = AddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemovePctAddModifier(IntModifier modifier)
        {
            pctAdd = PctAddCollection.RemoveModifier(modifier);
            Update();
        }
        public void RemoveFinalAddModifier(FloatModifier modifier)
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
            Value = value3;
        }
    }
}