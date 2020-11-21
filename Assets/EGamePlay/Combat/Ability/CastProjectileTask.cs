using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace EGamePlay.Combat.Ability
{
    public class CastProjectileTaskData
    {
        public Vector3 TargetPoint;
        public GameObject ProjectilePrefab;
    }

    public class CastProjectileTask : AbilityTask
    {
        public CastProjectileTaskData CastProjectileData { get; set; }


        public override void Awake(object paramObject)
        {
            CastProjectileData = (CastProjectileTaskData)paramObject;
        }

        public override async ET.ETTask ExecuteTaskAsync()
        {
            try
            {
                var projectile = GameObject.Instantiate(CastProjectileData.ProjectilePrefab);
                projectile.transform.position = GetParent<AbilityExecution>().GetParent<CombatEntity>().Position + Vector3.up;
                projectile.transform.DOMove(GetParent<AbilityExecution>().InputCombatEntity.Position + Vector3.up, 0.8f).SetEase(Ease.Linear);
                await ET.TimerComponent.Instance.WaitAsync(800);
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