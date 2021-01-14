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
        public bool CanMove { get; set; }
        public GameTimer IdleTimer { get; set; } = new GameTimer(2);
        public GameTimer MoveTimer { get; set; } = new GameTimer(2);
        public Vector3 TargetPos { get; set; }


        public override void Setup()
        {
            base.Setup();
            IdleTimer.Reset();
        }

        public override void Update()
        {
            base.Update();
            if (Disable)
            {
                return;
            }

            if (IdleTimer.IsRunning)
            {
                IdleTimer.UpdateAsFinish(Time.deltaTime, IdleFinish);
            }
            else
            {
                if (MoveTimer.IsRunning)
                {
                    MoveTimer.UpdateAsFinish(Time.deltaTime);
                    Position += TargetPos;
                }
                else
                {
                    IdleTimer.Reset();
                }
            }
        }

        private void IdleFinish()
        {
            var x = RandomHelper.RandomNumber(-4, 4) / 1000f;
            var z = RandomHelper.RandomNumber(-4, 4) / 1000f;
            TargetPos = new Vector3(x, 0, z);
            MoveTimer.Reset();
        }
    }
}