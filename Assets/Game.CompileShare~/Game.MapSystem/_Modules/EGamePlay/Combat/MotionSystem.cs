using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;
using System.ComponentModel;
using ECSGame;
using UnityEngine.UIElements;

namespace EGamePlay
{
    public class MotionSystem : AComponentSystem<CombatEntity, MotionComponent>,
        IAwake<CombatEntity, MotionComponent>
    {
        public void Awake(CombatEntity entity, MotionComponent component)
        {

        }

        public static void RunAI(CombatEntity entity)
        {
            var component = entity.GetComponent<MotionComponent>();
            component.IdleTimer = new GameTimer(ET.RandomHelper.RandomNumber(20, 30) / 10f);
            component.MoveTimer = new GameTimer(ET.RandomHelper.RandomNumber(20, 40) / 10f);
            component.IdleTimer.Reset();
            component.originPos = component.Position;
        }

        public static void FixedUpdate(CombatEntity entity, MotionComponent component)
        {
            if (component.IdleTimer == null)
            {
                return;
            }
#if !EGAMEPLAY_ET
            if (component.IdleTimer.IsRunning)
            {
                component.IdleTimer.UpdateAsFinish(Time.fixedDeltaTime, () => IdleFinish(entity));
            }
            else
            {
                if (component.MoveTimer.IsRunning)
                {
                    component.MoveTimer.UpdateAsFinish(Time.fixedDeltaTime, () => MoveFinish(entity));
                    var speed = entity.GetComponent<AttributeComponent>().MoveSpeed.Value;
                    component.Position += component.MoveVector * speed * 5;
                    //ConsoleLog.Debug($"MoveTimer {component.Position}");
                }
            }
#endif
        }

#if !EGAMEPLAY_ET
        private static void IdleFinish(CombatEntity entity)
        {
            //ConsoleLog.Debug($"IdleFinish ");
            var component = entity.GetComponent<MotionComponent>();
            var x = ET.RandomHelper.RandomNumber(-20, 20);
            var z = ET.RandomHelper.RandomNumber(-20, 20);
            var vec2 = new Vector2(x, z);
            if (Vector3.Distance(component.originPos, component.Position) > 0.1f)
            {
                vec2 = -(component.Position - component.originPos);
            }
            vec2.Normalize();
            var right = new Vector2(1, 0);
            var y = VectorAngle(entity, right, vec2);
            component.Rotation = Quaternion.Euler(0, y, 0);

            component.MoveVector = new Vector3(vec2.x, 0, vec2.y) / 100f;
            component.MoveTimer.Reset();
        }

        private static void MoveFinish(CombatEntity entity)
        {
            //ConsoleLog.Debug($"MoveFinish ");
            var component = entity.GetComponent<MotionComponent>();
            component.IdleTimer.Reset();

            //var heroEntity = entity.GetParent<CombatContext>().HeroEntity;
            //if (Vector3.Distance(heroEntity.Position, component.Position) < 5)
            //{
            //    SpellSystem.SpellWithTarget(entity, entity.GetComponent<SkillComponent>().IdSkills[1001], heroEntity);
            //}
        }

        private static float VectorAngle(CombatEntity entity, Vector2 from, Vector2 to)
        {
            var component = entity.GetComponent<MotionComponent>();
            var angle = 0f;
            var cross = Vector3.Cross(from, to);
            angle = Vector2.Angle(from, to);
            return cross.z > 0 ? -angle : angle;
        }
#endif
    }
}