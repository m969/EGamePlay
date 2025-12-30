using ECS;
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
    /// 浮点型修饰器集合
    /// </summary>
    public class FloatModifierCollection
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

    public enum ModifierType : int
    {
        Add,
        PctAdd,
        FinalAdd,
        FinalPctAdd,
    }

    /// <summary>
    /// 浮点型数值
    /// </summary>
    public class FloatNumeric : EcsEntity
    {
        public string Name {  get; set; }
        public float Value { get; set; }
        public float baseValue { get; set; }
        public float add { get; set; }
        public float pctAdd { get; set; }
        public float finalAdd { get; set; }
        public float finalPctAdd { get; set; }
        public Dictionary<int, FloatModifierCollection> TypeModifierCollections { get; set; } = new Dictionary<int, FloatModifierCollection>();
        public AttributeType AttributeType { get; set; }
    }
}