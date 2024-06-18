using EGamePlay.Combat;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 普攻执行体
    /// </summary>
    public class AttackExecution : Entity, IAbilityExecute
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
        private void PreProcess()
        {
            AttackAction.Creator.TriggerActionPoint(ActionPointType.PreGiveAttackEffect, AttackAction);
            AttackAction.Target.GetComponent<ActionPointComponent>().TriggerActionPoint(ActionPointType.PreReceiveAttackEffect, AttackAction);
        }

        /// <summary>   尝试触发普攻效果   </summary>
        private void TryTriggerAttackEffect()
        {
            HasTriggerDamage = true;

            PreProcess();

            if (BeBlocked)
            {
                Log.Debug("被格挡了");
            }
            else
            {
                var effects = AbilityEntity.GetComponent<AbilityEffectComponent>().AbilityEffects;
                for (int i = 0; i < effects.Count; i++)
                {
                    var effect = effects[i];
                    effect.TriggerObserver.OnTrigger(AttackAction.Target);
                }
            }
        }

        public void EndExecute()
        {
            AttackAction.FinishAction();
            AttackAction = null;
            Entity.Destroy(this);
        }
    }
}