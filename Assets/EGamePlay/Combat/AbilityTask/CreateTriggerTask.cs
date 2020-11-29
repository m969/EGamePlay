using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System;
using ET;

namespace EGamePlay.Combat.Ability
{
    public class CreateTriggerTaskData
    {
        public Vector3 TargetPoint;
        public GameObject TriggerPrefab;
    }

    public class CreateTriggerTask : AbilityTask
    {
        public GameObject TriggerObj { get; set; }
        public Action<Collider> OnTriggerEnterCallbackAction { get; set; }


        public override async ETTask ExecuteTaskAsync()
        {
            var taskData = taskInitData as CreateTriggerTaskData;
            TriggerObj = GameObject.Instantiate(taskData.TriggerPrefab, taskData.TargetPoint, Quaternion.identity);
            TriggerObj.GetComponent<OnTriggerEnterCallback>().OnTriggerEnterCallbackAction = (other) => { OnTriggerEnterCallbackAction?.Invoke(other); };
            TriggerObj.GetComponent<Collider>().enabled = true;
            await TimerComponent.Instance.WaitAsync(100);
            GameObject.Destroy(TriggerObj);
            Entity.Destroy(this);
        }
    }
}