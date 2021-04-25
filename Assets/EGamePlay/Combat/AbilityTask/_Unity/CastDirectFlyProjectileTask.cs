using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
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
        public Action<Collider> OnCollisionCallback { get; set; }
        public GameObject Projectile { get; set; }
        public Vector3 Direction { get; set; }
        public GameTimer FlyTimer { get; set; } = new GameTimer(3f);


        public override void Awake(object initData)
        {
            CastProjectileData = (CastDirectFlyProjectileTaskData)initData;
            OnCollisionCallback = null;
            Direction = new Vector3(0, CastProjectileData.DirectAngle, 0);
            FlyTimer.Reset();
            TaskState = AbilityTaskState.Ready;
        }

        public override void Update()
        {
            if (TaskState == AbilityTaskState.Executing)
            {
                Projectile.transform.eulerAngles = Direction;
                Projectile.transform.Translate(Projectile.transform.forward * 0.1f, Space.World);
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
            Projectile = GameObject.Instantiate(CastProjectileData.ProjectilePrefab);
            Projectile.GetComponent<Collider>().enabled = false;
            Projectile.transform.position = GetParent<CombatEntity>().Position + Vector3.up;
            Projectile.GetComponent<OnTriggerEnterCallback>().OnTriggerEnterCallbackAction = (other) => { OnCollisionCallback?.Invoke(other); };
            Projectile.GetComponent<Collider>().enabled = true;
            Projectile.SetActive(true);

            TaskState = AbilityTaskState.Executing;
            
            await ETTask.CompletedTask;
        }
    }
}