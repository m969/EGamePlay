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
        public Vector3 Position;
        public float Direction;
        public GameObject TriggerPrefab;
        public int LifeTime;
        public Action<Collider> OnTriggerEnterCallback;
    }

    public class CreateTriggerTask : AbilityTask
    {
        public GameObject TriggerObj { get; set; }


        public override async ETTask ExecuteTaskAsync()
        {
            var taskData = taskInitData as CreateTriggerTaskData;
            TriggerObj = GameObject.Instantiate(taskData.TriggerPrefab, taskData.Position, Quaternion.identity);
            TriggerObj.GetComponent<Collider>().enabled = false;
            TriggerObj.transform.eulerAngles = new Vector3(0, taskData.Direction, 0);
            TriggerObj.GetComponent<OnTriggerEnterCallback>().OnTriggerEnterCallbackAction = (other) => { taskData.OnTriggerEnterCallback?.Invoke(other); };
            TriggerObj.GetComponent<Collider>().enabled = true;
            await TimerComponent.Instance.WaitAsync(100);
            GameObject.Destroy(TriggerObj);
            Entity.Destroy(this);
        }
    }
}