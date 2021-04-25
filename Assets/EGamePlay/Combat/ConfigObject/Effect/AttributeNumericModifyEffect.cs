using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    //[Effect("属性数值修饰", 50)]
    public class AttributeNumericModifyEffect : Effect
    {
        public override string Label => "属性数值修饰";

        [ToggleGroup("Enabled")]
        public AttributeType NumericType;

        [ToggleGroup("Enabled"), LabelText("数值参数")]
        public string NumericValue;
    }
}