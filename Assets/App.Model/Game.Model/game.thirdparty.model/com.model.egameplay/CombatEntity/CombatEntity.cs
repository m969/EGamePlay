using ECS;
using ECSGame;
using EGamePlay.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗实体
    /// </summary>
    public sealed class CombatEntity : EcsEntity, IPosition
    {
        public Actor Actor { get; set; }

        //效果赋给行动能力
        public EffectAssignAbility EffectAssignAbility { get; set; }
        //施法行动能力
        public SpellAbility SpellAbility { get; set; }
        //移动行动能力
        //public MotionAbility MotionAbility { get; set; }
        //伤害行动能力
        public DamageAbility DamageAbility { get; set; }
        //治疗行动能力
        public CureAbility CureAbility { get; set; }
        //施加状态行动能力
        public AddBuffAbility AddStatusAbility { get; set; }
        //施法普攻行动能力
        public AttackAbility AttackSpellAbility { get; set; }
        ////回合行动能力
        //public RoundActionAbility RoundAbility { get; set; }
        ////起跳行动能力
        //public JumpToActionAbility JumpToAbility { get; set; }
        public CollisionAbility CollisionAbility { get; set; }

        //普攻能力
        //public AttackAbility AttackAbility { get; set; }
        //普攻格挡能力
        //public AttackBlockActionAbility AttackBlockAbility { get; set; }

        //执行中的执行体
        public AbilityExecution SpellingExecution { get; set; }

        public Vector3 Position
        {
            get { return GetComponent<TransformComponent>().Position; }
            set { GetComponent<TransformComponent>().Position = value; }
        }
        public Quaternion Rotation
        {
            get { return GetComponent<TransformComponent>().Rotation; }
            set { GetComponent<TransformComponent>().Rotation = value; }
        }
        /// 行为禁制
        public ActionControlType ActionControlType { get; set; }
        /// 禁制豁免
        public ActionControlType ActionControlImmuneType { get; set; }

        public bool IsHero { get; set; }
        public bool IsMonster => IsHero == false;
    }
}