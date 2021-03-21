using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;
using System;
using EGamePlay.Combat.Skill;

namespace EGamePlay.Combat.Ability
{
    public class CastTargetFlyProjectileTaskData
    {
        public CombatEntity TargetEntity;
        public GameObject ProjectilePrefab;
        public float FlyTime;
    }

    public class CastTargetFlyProjectileTask : AbilityTask
    {
        public CastTargetFlyProjectileTaskData CastProjectileData { get; set; }
        public Action OnEnterCallback { get; set; }


        public override void Awake(object initData)
        {
            CastProjectileData = (CastTargetFlyProjectileTaskData)initData;
            OnEnterCallback = null;
        }

        public override async ETTask ExecuteTaskAsync()
        {
            try
            {
                var projectile = GameObject.Instantiate(CastProjectileData.ProjectilePrefab);
                projectile.SetActive(true);
                projectile.transform.position = GetParent<CombatEntity>().Position + Vector3.up;

                while (true)
                {
                    var targetPos = CastProjectileData.TargetEntity.Position + Vector3.up;
                    projectile.transform.LookAt(targetPos);
                    var moveVector = targetPos - projectile.transform.position;
                    projectile.transform.Translate(moveVector.normalized * 0.2f, Space.World);
                    //projectile.transform.DOMove(targetPos, CastProjectileData.FlyTime).SetEase(Ease.Linear);
                    if (Vector3.Distance(projectile.transform.position, targetPos) < 0.4f)
                    {
                        OnEnterCallback?.Invoke();
                        break;
                    }
                    await TimerComponent.Instance.WaitAsync(10);
                }

                //await TimerComponent.Instance.WaitAsync((int)(CastProjectileData.FlyTime * 1000));
                GameObject.Destroy(projectile);
                Entity.Destroy(this);
            }
            catch (System.Exception e)
            {
                Log.Error(e);
                throw;
            }
        }
    }
}