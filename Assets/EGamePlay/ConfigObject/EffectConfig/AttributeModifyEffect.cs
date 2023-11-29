using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    [Effect("属性修饰", 2)]
    public class AttributeModifyEffect : Effect
    {
        public override string Label => "属性修饰";

        [ToggleGroup("Enabled")]
        public AttributeType AttributeType;

        [ToggleGroup("Enabled"), LabelText("数值参数")]
        public string NumericValue;

        [ToggleGroup("Enabled")]
        public ModifyType ModifyType;
    }
}