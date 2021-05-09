using EGamePlay.Combat.Ability;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 普攻执行体
    /// </summary>
    public class AttackExecution : AbilityExecution
    {
        public AttackAction AttackAction { get; set; }
        
        
        public override void Update()
        {

        }

        public override void BeginExecute()
        {
            if (OwnerEntity.DamageActionAbility.TryCreateAction(out var action))
            {
                action.Target = AttackAction.Target;
                action.DamageSource = DamageSource.Attack;
                action.ApplyDamage();
            }

            this.EndExecute();
        }

        public override void EndExecute()
        {
            base.EndExecute();
            AttackAction = null;
        }
    }
}