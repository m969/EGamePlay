using EGamePlay.Combat;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 普攻执行体
    /// </summary>
    public class AttackExecution : Entity, IAbilityExecution
    {
        public AttackAction AttackAction { get; set; }
        public Entity AbilityEntity { get; set; }
        public CombatEntity OwnerEntity { get; set; }

        private bool BeBlocked;/// 是否被格挡
        private bool HasTriggerDamage;/// 是否触发了伤害


        public override void Update()
        {
            if (!IsDisposed)
            {
                if (!HasTriggerDamage)
                {
                    TryTriggerAttackEffect();
                }
                else
                {
                    this.EndExecute();
                }
            }
        }

        public void SetBlocked()
        {
            BeBlocked = true;
        }

        public void BeginExecute()
        {
            AddComponent<UpdateComponent>();
            //Log.Debug("AttackExecution BeginExecute");
        }

        /// 前置处理
        public void TriggerAttackPreProcess()
        {
            AbilityEntity.FireEvent(nameof(TriggerAttackPreProcess), this);
        }

        /// <summary>   尝试触发普攻效果   </summary>
        private void TryTriggerAttackEffect()
        {
            Log.Debug("AttackExecution TryTriggerAttackEffect");
            HasTriggerDamage = true;

            TriggerAttackPreProcess();

            if (BeBlocked)
            {
                Log.Debug("被格挡了");
            }
            else
            {
                AbilityEntity.Get<AbilityEffectComponent>().TryAssignAllEffectsToTargetWithExecution(AttackAction.Target, this);
                //if (OwnerEntity.DamageAbility.TryMakeAction(out var action))
                //{
                //    action.Target = AttackAction.Target;
                //    action.DamageSource = DamageSource.Attack;
                //    action.ApplyDamage();
                //}
            }
        }

        public void EndExecute()
        {
            //Log.Debug("AttackExecution EndExecute");
            //base.EndExecute();
            AttackAction.FinishAction();
            AttackAction = null;
            Entity.Destroy(this);
        }
    }
}