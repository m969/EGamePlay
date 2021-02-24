using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using ET;
using System;
using EGamePlay.Combat.Skill;
using GameUtils;

namespace EGamePlay.Combat.Ability
{
    public class CastDirectFlyProjectileTaskData
    {
        public GameObject ProjectilePrefab;
        public float FlyTime;
        public float DirectAngle;
    }

    public class CastDirectFlyProjectileTask : AbilityTask
    {
        public CastDirectFlyProjectileTaskData CastProjectileData { get; set; }
        public Action OnCollisionCallback { get; set; }
        public GameObject Projectile { get; set; }
        public Vector3 Direction { get; set; }
        public GameTimer FlyTimer { get; set; } = new GameTimer(3f);


        public override void Awake(object initData)
        {
            CastProjectileData = (CastDirectFlyProjectileTaskData)initData;
            OnCollisionCallback = null;
            Projectile = GameObject.Instantiate(CastProjectileData.ProjectilePrefab);
            Projectile.SetActive(true);
            Projectile.transform.position = GetParent<CombatEntity>().Position + Vector3.up;
            FlyTimer.Reset();
            TaskState = AbilityTaskState.Ready;
        }

        public override void Update()
        {
            if (TaskState == AbilityTaskState.Executing)
            {
                Projectile.transform.Translate(Direction * 0.01f, Space.World);
                FlyTimer.UpdateAsFinish(Time.deltaTime, OnFlyFinish);
            }
        }

        private void OnFlyFinish()
        {
            TaskState = AbilityTaskState.Ended;
            GameObject.Destroy(Projectile);
            Entity.Destroy(this);
        }

        public override async ETTask ExecuteTaskAsync()
        {
            TaskState = AbilityTaskState.Executing;
        }
    }
}