using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace EGamePlay.Combat.Ability
{
    public class CreateExplosionTaskData
    {
        public Vector3 TargetPoint;
        public GameObject ExplosionPrefab;
    }

    public class CreateExplosionTask : AbilityTask
    {
        public CreateExplosionTaskData CreateExplosionTaskData { get; set; }


        public override void Awake(object paramObject)
        {
            CreateExplosionTaskData = (CreateExplosionTaskData)paramObject;
        }

        public override async ET.ETTask ExecuteTaskAsync()
        {
            var explosion = GameObject.Instantiate(CreateExplosionTaskData.ExplosionPrefab);
            explosion.transform.position = CreateExplosionTaskData.TargetPoint;
            await ET.TimerComponent.Instance.WaitAsync(800);
            GameObject.Destroy(explosion);
            Entity.Destroy(this);
        }
    }
}