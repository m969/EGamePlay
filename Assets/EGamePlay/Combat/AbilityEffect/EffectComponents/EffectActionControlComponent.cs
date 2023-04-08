using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动禁制效果组件
    /// </summary>
    public class EffectActionControlComponent : Component, IEffectTriggerSystem
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
        }

        public override void OnDisable()
        {
            Entity.Parent.Parent.GetComponent<StatusComponent>().OnStatusesChanged(Entity.GetParent<StatusAbility>());
        }

        public void OnTriggerApplyEffect(Entity effectAssign)
        {

        }
    }
}