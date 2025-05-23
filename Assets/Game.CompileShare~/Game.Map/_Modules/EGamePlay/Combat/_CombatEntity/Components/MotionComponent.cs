﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameUtils;
using Sirenix.OdinInspector;
using ECS;
#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 运动组件，在这里管理战斗实体的移动、跳跃、击飞等运动功能
    /// </summary>
    public sealed class MotionComponent : EcsComponent
    {
        //public override bool DefaultEnable { get; set; } = true;
        public Vector3 Position { get => GetEntity<CombatEntity>().Position; set => GetEntity<CombatEntity>().Position = value; }
        public Quaternion Rotation { get => GetEntity<CombatEntity>().Rotation; set => GetEntity<CombatEntity>().Rotation = value; }
        public bool CanMove { get; set; }
        public GameTimer IdleTimer { get; set; }
        public GameTimer MoveTimer { get; set; }
        public Vector3 MoveVector { get; set; }
        public Vector3 originPos;


//        public void RunAI()
//        {
//            IdleTimer = new GameTimer(RandomHelper.RandomNumber(20, 30) / 10f);
//            MoveTimer = new GameTimer(RandomHelper.RandomNumber(20, 40) / 10f);
//            IdleTimer.Reset();
//            originPos = Position;
//        }

//        public override void FixedUpdate()
//        {
//            if (IdleTimer == null)
//            {
//                return;
//            }
//#if !EGAMEPLAY_ET
//            if (IdleTimer.IsRunning)
//            {
//                IdleTimer.UpdateAsFinish(Time.fixedDeltaTime, IdleFinish);
//            }
//            else
//            {
//                if (MoveTimer.IsRunning)
//                {
//                    MoveTimer.UpdateAsFinish(Time.fixedDeltaTime, MoveFinish);
//                    var speed = GetEntity<CombatEntity>().GetComponent<AttributeComponent>().MoveSpeed.Value;
//                    Position += MoveVector * speed * 5;
//                }
//            }
//#endif
//        }

//#if !EGAMEPLAY_ET
//        private void IdleFinish()
//        {
//            var x = RandomHelper.RandomNumber(-20, 20);
//            var z = RandomHelper.RandomNumber(-20, 20);
//            var vec2 = new Vector2(x, z);
//            if (Vector3.Distance(originPos, Position) > 0.1f)
//            {
//                vec2 = -(Position - originPos);
//            }
//            vec2.Normalize();
//            var right = new Vector2(1, 0);
//            var y = VectorAngle(right, vec2);
//            Rotation = Quaternion.Euler(0, y, 0);

//            MoveVector = new Vector3(vec2.x, 0, vec2.y) / 100f;
//            MoveTimer.Reset();
//        }

//        private void MoveFinish()
//        {
//            IdleTimer.Reset();

//            var heroEntity = Entity.GetParent<CombatContext>().HeroEntity;
//            if (Vector3.Distance(heroEntity.Position, Position) < 5)
//            {
//                var combatEntity = GetEntity<CombatEntity>();
//                combatEntity.GetComponent<SpellComponent>().SpellWithTarget(combatEntity.GetComponent<SkillComponent>().IdSkills[1001], heroEntity);
//            }
//        }

//        private float VectorAngle(Vector2 from, Vector2 to)
//        {
//            var angle = 0f;
//            var cross = Vector3.Cross(from, to);
//            angle = Vector2.Angle(from, to);
//            return cross.z > 0 ? -angle : angle;
//        }
//#endif
    }
}