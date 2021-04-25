using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using ET;

namespace EGamePlay.Combat.Ability
{
    public class CreateColliderTaskData
    {
        public Vector3 Position;
        public float Direction;
        public GameObject ColliderPrefab;
        public int LifeTime;
        public Action<Collider> OnTriggerEnterCallback;
    }

    public class CreateColliderTask : AbilityTask
    {
        public GameObject ColliderObj { get; set; }


        public override async ETTask ExecuteTaskAsync()
        {
            var taskData = taskInitData as CreateColliderTaskData;
            ColliderObj = GameObject.Instantiate(taskData.ColliderPrefab, taskData.Position, Quaternion.identity);
            ColliderObj.SetActive(true);
            ColliderObj.GetComponent<Collider>().enabled = false;
            ColliderObj.transform.eulerAngles = new Vector3(0, taskData.Direction, 0);
            ColliderObj.GetComponent<OnTriggerEnterCallback>().OnTriggerEnterCallbackAction = (other) => {
                taskData.OnTriggerEnterCallback?.Invoke(other);
            };
            ColliderObj.GetComponent<Collider>().enabled = true;
            await TimerComponent.Instance.WaitAsync(taskData.LifeTime);
            GameObject.Destroy(ColliderObj);
            Entity.Destroy(this);
        }
    }
}