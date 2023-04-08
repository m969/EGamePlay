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
        private void PreProcess()
        {
            AttackAction.Creator.TriggerActionPoint(ActionPointType.PreGiveAttackEffect, AttackAction);
            AttackAction.Target.TriggerActionPoint(ActionPointType.PreReceiveAttackEffect, AttackAction);
        }

        /// <summary>   尝试触发普攻效果   </summary>
        private void TryTriggerAttackEffect()
        {
            //Log.Debug("AttackExecution TryTriggerAttackEffect");
            HasTriggerDamage = true;

            PreProcess();

            if (BeBlocked)
            {
                Log.Debug("被格挡了");
            }
            else
            {
                //AbilityEntity.Get<AbilityEffectComponent>().TryAssignAllEffectsToTargetWithExecution(AttackAction.Target, this);
                var effectAssigns = AbilityEntity.GetComponent<AbilityEffectComponent>().CreateAssignActions(AttackAction.Target);
                foreach (var item in effectAssigns)
                {
                    item.AssignEffect();
                }
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