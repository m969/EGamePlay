using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using ET;
using EGamePlay.Combat.Skill;

namespace EGamePlay.Combat.Ability
{
    public class CastProjectileTaskData
    {
        public CombatEntity TargetEntity;
        public GameObject ProjectilePrefab;
        public float FlyTime;
    }

    public class CastProjectileTask : AbilityTask
    {
        public CastProjectileTaskData CastProjectileData { get; set; }


        public override void Awake(object initData)
        {
            CastProjectileData = (CastProjectileTaskData)initData;
        }

        public override async ETTask ExecuteTaskAsync()
        {
            try
            {
                var projectile = GameObject.Instantiate(CastProjectileData.ProjectilePrefab);
                projectile.transform.position = GetParent<CombatEntity>().Position + Vector3.up;

                while (true)
                {
                    var targetPos = CastProjectileData.TargetEntity.Position + Vector3.up;
                    projectile.transform.DOMove(targetPos, CastProjectileData.FlyTime).SetEase(Ease.Linear);
                    if (Vector3.Distance(projectile.transform.position, targetPos) < 0.1f)
                    {
                        break;
                    }
                    await TimerComponent.Instance.WaitAsync(20);
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