using Sirenix.OdinInspector;

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

        [ToggleGroup("Enabled"), LabelText("是否可以暴击")]
        public bool CanCrit;
    }
}