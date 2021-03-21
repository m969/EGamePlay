using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;

namespace EGamePlay.Combat.Ability
{
    public class CreateExplosionTaskData
    {
        public Vector3 TargetPoint;
        public GameObject ExplosionPrefab;
        public int LifeTime;
    }

    public class CreateExplosionTask : AbilityTask
    {
        public CreateExplosionTaskData CreateExplosionTaskData { get; set; }


        public override void Awake(object initData)
        {
            CreateExplosionTaskData = (CreateExplosionTaskData)initData;
        }

        public override async ETTask ExecuteTaskAsync()
        {
            var explosion = GameObject.Instantiate(CreateExplosionTaskData.ExplosionPrefab);
            explosion.transform.position = CreateExplosionTaskData.TargetPoint;
            await TimerComponent.Instance.WaitAsync(800);
            GameObject.Destroy(explosion);
            Entity.Destroy(this);
        }
    }
}