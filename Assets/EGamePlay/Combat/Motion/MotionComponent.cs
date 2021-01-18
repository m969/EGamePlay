using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameUtils;
using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 运动组件，在这里管理战斗实体的移动、跳跃、击飞等运动功能
    /// </summary>
    public sealed class MotionComponent : Component
    {
        public Vector3 Position { get=> GetEntity<CombatEntity>().Position; set=> GetEntity<CombatEntity>().Position = value; }
        public float Direction { get=> GetEntity<CombatEntity>().Direction; set=> GetEntity<CombatEntity>().Direction = value; }
        public bool CanMove { get; set; }
        public GameTimer IdleTimer { get; set; } = new GameTimer(2);
        public GameTimer MoveTimer { get; set; } = new GameTimer(2);
        public Vector3 MoveVector { get; set; }


        public override void Setup()
        {
            base.Setup();
            IdleTimer.Reset();
        }

        public override void Update()
        {
            if (IdleTimer.IsRunning)
            {
                IdleTimer.UpdateAsFinish(Time.deltaTime, IdleFinish);
            }
            else
            {
                if (MoveTimer.IsRunning)
                {
                    MoveTimer.UpdateAsFinish(Time.deltaTime, MoveFinish);
                    Position += MoveVector;
                }
            }
        }

        private void IdleFinish()
        {
            //Direction = RandomHelper.RandomNumber(0, 360);
            var x = RandomHelper.RandomNumber(-20, 20);
            var z = RandomHelper.RandomNumber(-20, 20);
            x = z = 10;
            //x = 0;z = 1;
            var vec2 = new Vector2(x, z);
            var right = Vector2.right;
            var dotV = Vector2.Dot(vec2.normalized, right.normalized);
            Log.Debug(dotV.ToString());
            if (vec2.y > 0)
            {
                Direction = -(dotV - 1) * 90;
            }
            else
            {
                Direction = -(dotV - 1) * -90;
            }
            Log.Debug(Direction.ToString());
            MoveVector = new Vector3(x, 0, z) / 1000f;
            MoveTimer.Reset();
        }

        private void MoveFinish()
        {
            IdleTimer.Reset();
        }
    }
}