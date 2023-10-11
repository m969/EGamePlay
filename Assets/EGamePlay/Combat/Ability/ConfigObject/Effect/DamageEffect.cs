using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace EGamePlay.Combat
{
    [Effect("造成伤害", 10)]
    public class DamageEffect : Effect
    {
        public override string Label => "造成伤害";

        [ToggleGroup("Enabled")]
        public DamageType DamageType;

        [ToggleGroup("Enabled"), LabelText("伤害取值")]
        public string DamageValueFormula;

        [ToggleGroup("Enabled"), LabelText("能否暴击")]
        public bool CanCrit;

        //public string DamageValueProperty { get; set; }

        //[ToggleGroup("Enabled"), LabelText("效果修饰")]
        //[HideReferenceObjectPicker, ListDrawerSettings(DraggableItems = false)]
        //[ShowInInspector]
        ///// Effect是直接效果，Effect的组件是基于直接效果的辅助效果
        //public override List<Component> Components { get; set; } = new List<Component>();
    }
}