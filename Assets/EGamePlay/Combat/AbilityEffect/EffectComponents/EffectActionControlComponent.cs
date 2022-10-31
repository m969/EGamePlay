using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动禁制效果组件
    /// </summary>
    public class EffectActionControlComponent : Component
    {
        public override bool DefaultEnable => false;
        public ActionControlEffect ActionControlEffect { get; set; }
        public ActionControlType ActionControlType { get; set; }


        public override void Awake()
        {
            ActionControlEffect = GetEntity<AbilityEffect>().EffectConfig as ActionControlEffect;
        }

        public override void OnEnable()
        {
            Entity.Parent.Parent.GetComponent<StatusComponent>().OnStatusesChanged(Entity.GetParent<StatusAbility>());

            //var prohibitEffect = ActionControlEffect;
            //ActionControlType = ActionControlType | prohibitEffect.ActionControlType;
            //var parentEntity = Entity.GetParent<StatusAbility>().GetParent<CombatEntity>();
            //parentEntity.ActionControlType = parentEntity.ActionControlType | prohibitEffect.ActionControlType;
            //var moveForbid = parentEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid);
            //if (moveForbid)
            //{
            //    parentEntity.GetComponent<MotionComponent>().Enable = false;
            //}
        }

        public override void OnDisable()
        {
            Entity.Parent.Parent.GetComponent<StatusComponent>().OnStatusesChanged(Entity.GetParent<StatusAbility>());

            //var parentEntity = Entity.GetParent<StatusAbility>().GetParent<CombatEntity>();
            //parentEntity.ActionControlType = parentEntity.ActionControlType & (~ActionControlType);
            //var moveForbid = parentEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid);
            //if (!moveForbid)
            //{
            //    parentEntity.GetComponent<MotionComponent>().Enable = true;
            //}
        }
    }
}