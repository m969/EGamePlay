using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;

namespace EGamePlay.Combat.Ability
{
    public class CreateEffectTaskData
    {
        public Vector3 Position;
        public float Direction;
        public GameObject EffectPrefab;
        public int LifeTime;
    }

    public class CreateEffectTask : AbilityTask
    {
        public CreateEffectTaskData TaskData { get; set; }


        public override void Awake(object initData)
        {
            TaskData = (CreateEffectTaskData)initData;
        }

        public override async ETTask ExecuteTaskAsync()
        {
            var explosion = GameObject.Instantiate(TaskData.EffectPrefab);
            explosion.transform.position = TaskData.Position;
            explosion.transform.eulerAngles = new Vector3(0, TaskData.Direction, 0);
            await TimerComponent.Instance.WaitAsync(TaskData.LifeTime);
            GameObject.Destroy(explosion);
            Entity.Destroy(this);
        }
    }
}