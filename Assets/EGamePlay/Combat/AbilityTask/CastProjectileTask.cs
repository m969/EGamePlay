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
        public Vector3 TargetPoint;
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
                projectile.transform.position = GetParent<SkillAbilityExecution>().GetParent<CombatEntity>().Position + Vector3.up;
                projectile.transform.DOMove(GetParent<SkillAbilityExecution>().InputCombatEntity.Position + Vector3.up, CastProjectileData.FlyTime).SetEase(Ease.Linear);
                await TimerComponent.Instance.WaitAsync((int)(CastProjectileData.FlyTime * 1000));
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