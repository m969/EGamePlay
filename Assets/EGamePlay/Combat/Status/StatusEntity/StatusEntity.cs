using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

namespace EGamePlay.Combat.Status
{
    //public partial class StatusEntity
    //{
    //    public static StatusEntity Create()
    //    {
    //        var entity = EntityFactory.Create<StatusEntity>();
    //        return entity;
    //    }
    //}

    public partial class StatusEntity : Entity
    {
        public bool Enabled { get; private set; }
        public CombatEntity Caster { get; set; }
        public StatusConfigObject StatusConfigObject { get; set; }
        public StatusListen StatusListen { get; set; }
        public StatusRun StatusRun { get; set; }


        public override void Awake(object paramObject)
        {
            StatusConfigObject = paramObject as StatusConfigObject;
            foreach (var item in StatusConfigObject.Effects)
            {
                var effectEntity = EntityFactory.CreateWithParent<EffectEntity>(this, item);
                if (item is DamageEffect damageEffect)
                {
                    if (damageEffect.EffectTriggerType == EffectTriggerType.Interval)
                    {
                        effectEntity.AddComponent<EffectIntervalTriggerComponent>();
                    }
                }
                if (item is AddStatusEffect addStatusEffect)
                {

                }
            }
        }

        public void Enable()
        {
            Enabled = true;
        }

        public void Disable()
        {
            Enabled = false;
        }
    }
}