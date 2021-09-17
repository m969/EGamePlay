using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay.Combat;
using ET;
using Log = EGamePlay.Log;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace EGamePlay.Combat
{
    public class AnimationData
    {
        public bool HasStart;
        public bool HasEnded;
        public float StartTime;
        public float EndTime;
        public float Duration;
        public AnimationClip AnimationClip;
    }

    public class ColliderSpawnData
    {
        public bool HasStart;
        public ColliderSpawnEmitter ColliderSpawnEmitter;
    }

    /// <summary>
    /// 技能执行体
    /// </summary>
    public partial class SkillExecution : AbilityExecution
    {
        public List<AnimationData> AnimationDatas { get; set; } = new List<AnimationData>();
        public List<ColliderSpawnData> ColliderSpawnDatas { get; set; } = new List<ColliderSpawnData>();
        
        
        public override void Awake(object initData)
        {
            base.Awake(initData);

            SkillExecutionAsset = Resources.Load<GameObject>($"SkillExecution_{this.SkillAbility.SkillConfig.Id}");

            if (SkillExecutionAsset == null)
                return;
            var timelineAsset = SkillExecutionAsset.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
            if (timelineAsset == null)
                return;

            var markers = timelineAsset.markerTrack.GetMarkers();
            foreach (var item in markers)
            {
                if (item is ColliderSpawnEmitter colliderSpawnEmitter)
                {
                    ColliderSpawnDatas.Add(new ColliderSpawnData() { ColliderSpawnEmitter = colliderSpawnEmitter });
                }
            }

            var rootTracks = timelineAsset.GetRootTracks();
            foreach (var item in rootTracks)
            {
                if (item.hasClips)
                {
                    var clips = item.GetClips();
                    foreach (var clip in clips)
                    {
                        if (clip.animationClip != null)
                        {
                            var animationData = new AnimationData();
                            animationData.StartTime = (float)clip.clipIn;
                            animationData.Duration = (float)clip.duration;
                            animationData.EndTime = animationData.StartTime + animationData.Duration;
                            animationData.AnimationClip = clip.animationClip;
                            AnimationDatas.Add(animationData);
                        }
                    }
                }
            }

            OriginTime = ET.TimeHelper.Now();
        }

        public override void Update()
        {
            if (SkillAbility.Spelling == false)
            {
                return;
            }

            var nowSeconds = (double)(ET.TimeHelper.Now() - OriginTime) / 1000;
            foreach (var item in ColliderSpawnDatas)
            {
                if (item.HasStart == false)
                {
                    if (nowSeconds >= item.ColliderSpawnEmitter.time)
                    {
                        item.HasStart = true;
                        SpawnCollisionItem(item.ColliderSpawnEmitter);
                    }
                }
            }

            var allAnimationEnded = true;
            foreach (var item in AnimationDatas)
            {
                if (item.HasStart == false)
                {
                    allAnimationEnded = false;
                    if (nowSeconds >= item.StartTime)
                    {
                        item.HasStart = true;
                        OwnerEntity.Publish(item.AnimationClip);
                    }
                }
                else
                {
                    if (item.HasEnded == false)
                    {
                        allAnimationEnded = false;
                        if (nowSeconds >= item.EndTime)
                        {
                            item.HasEnded = true;
                        }
                    }
                }
            }
            if (allAnimationEnded)
            {
                EndExecute();
            }
        }

        public override void BeginExecute()
        {
            GetParent<CombatEntity>().CurrentSkillExecution = this;
            SkillAbility.Spelling = true;

            if (SkillExecutionAsset == null)
                return;
            var timelineAsset = SkillExecutionAsset.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
            if (timelineAsset == null)
                return;
            var skillExecutionObj = GameObject.Instantiate(SkillExecutionAsset, OwnerEntity.Position, Quaternion.Euler(0, OwnerEntity.Direction, 0));
            GameObject.Destroy(skillExecutionObj, (float)timelineAsset.duration);
            base.BeginExecute();
        }

        public override void EndExecute()
        {
            GetParent<CombatEntity>().CurrentSkillExecution = null;
            SkillAbility.Spelling = false;
            SkillTargets.Clear();
            base.EndExecute();
        }

        private void SpawnCollisionItem(ColliderSpawnEmitter colliderSpawnEmitter)
        {
            if (colliderSpawnEmitter.ColliderType == ColliderType.TargetFly) TargetFlyProccess(colliderSpawnEmitter);
            if (colliderSpawnEmitter.ColliderType == ColliderType.ForwardFly) ForwardFlyProccess(colliderSpawnEmitter);
            if (colliderSpawnEmitter.ColliderType == ColliderType.FixedPosition) FixedPositionProccess(colliderSpawnEmitter);
            if (colliderSpawnEmitter.ColliderType == ColliderType.FixedDirection) FixedDirectionProccess(colliderSpawnEmitter);
        }

        private void TargetFlyProccess(ColliderSpawnEmitter colliderSpawnEmitter)
        {
            var abilityItem = Entity.Create<AbilityItem>(colliderSpawnEmitter.ColliderName);
            abilityItem.AbilityEntity = SkillAbility;
            abilityItem.TargetEntity = InputTarget;
            abilityItem.Position = OwnerEntity.Position;
            CreateAbilityItemObj(abilityItem);
            abilityItem.AddComponent<MoveWithDotweenComponent>().DoMoveTo(InputTarget);
        }

        private void ForwardFlyProccess(ColliderSpawnEmitter colliderSpawnEmitter)
        {
            var abilityItem = Entity.Create<AbilityItem>(colliderSpawnEmitter.ColliderName);
            abilityItem.AbilityEntity = SkillAbility;
            abilityItem.Position = OwnerEntity.Position;
            CreateAbilityItemObj(abilityItem);
            var x = Mathf.Sin(Mathf.Deg2Rad * InputDirection);
            var z = Mathf.Cos(Mathf.Deg2Rad * InputDirection);
            var destination = abilityItem.Position + new Vector3(x, 0, z) * 30;
            abilityItem.AddComponent<MoveWithDotweenComponent>().DoMoveTo(destination, 1f).OnMoveFinish(()=> { Entity.Destroy(abilityItem); });
        }

        private void FixedPositionProccess(ColliderSpawnEmitter colliderSpawnEmitter)
        {
            var abilityItem = Entity.Create<AbilityItem>(colliderSpawnEmitter.ColliderName);
            abilityItem.AbilityEntity = SkillAbility;
            abilityItem.Position = InputPoint;
            CreateAbilityItemObj(abilityItem);
            abilityItem.AddComponent<LifeTimeComponent>(colliderSpawnEmitter.ExistTime);
        }

        private void FixedDirectionProccess(ColliderSpawnEmitter colliderSpawnEmitter)
        {
            var abilityItem = Entity.Create<AbilityItem>(colliderSpawnEmitter.ColliderName);
            abilityItem.AbilityEntity = SkillAbility;
            abilityItem.Position = OwnerEntity.Position;
            abilityItem.Direction = OwnerEntity.Direction;
            CreateAbilityItemObj(abilityItem);
            abilityItem.AddComponent<LifeTimeComponent>(colliderSpawnEmitter.ExistTime);
        }

        public void CreateAbilityItemObj(AbilityItem abilityItem)
        {
            var abilityItemObj = GameObject.Instantiate(Resources.Load<GameObject>($"AbilityItems/{abilityItem.Name}"), abilityItem.Position, Quaternion.Euler(0, abilityItem.Direction, 0));
            abilityItemObj.GetComponent<AbilityItemProxyObj>().AbilityItem = abilityItem;
            abilityItemObj.GetComponent<Collider>().enabled = false;
            abilityItemObj.GetComponent<OnTriggerEnterCallback>().OnTriggerEnterCallbackAction = (other) => {
                var combatEntity = CombatContext.Instance.Object2Entities[other.gameObject];
                abilityItem.OnCollision(combatEntity);
            };
            abilityItemObj.GetComponent<Collider>().enabled = true;
        }
    }
}

//var taskData = new CastTargetFlyProjectileTaskData();
//taskData.FlyTime = 0.3f;
//taskData.TargetEntity = InputTarget;
//var prefab = SkillExecutionAsset.transform.Find(colliderSpawnEmitter.ColliderName);
//taskData.ProjectilePrefab = prefab.gameObject;
//var task = Entity.CreateWithParent<CastTargetFlyProjectileTask>(OwnerEntity, taskData);
//task.OnEnterCallback = () => { AbilityEntity.ApplyAbilityEffectsTo(InputTarget); };
//task.ExecuteTaskAsync().Coroutine();

//var taskData = new CastDirectFlyProjectileTaskData();
//var prefab = SkillExecutionAsset.transform.Find(colliderSpawnEmitter.ColliderName);
//taskData.ProjectilePrefab = prefab.gameObject;
//taskData.DirectAngle = InputDirection;
//var task = Entity.CreateWithParent<CastDirectFlyProjectileTask>(OwnerEntity, taskData);
//task.OnCollisionCallback = (other) => {
//    var combatEntity = CombatContext.Instance.GameObject2Entitys[other.gameObject];
//    AbilityEntity.ApplyAbilityEffectsTo(combatEntity);
//    GameObject.Destroy(task.Projectile);
//    Entity.Destroy(task);
//};
//task.AddComponent<UpdateComponent>();
//task.ExecuteTaskAsync().Coroutine();

//var taskData = new CreateColliderTaskData();
//taskData.Position = InputPoint;
//var prefab = SkillExecutionAsset.transform.Find(colliderSpawnEmitter.ColliderName);
//taskData.ColliderPrefab = prefab.gameObject;
//taskData.LifeTime = (int)(colliderSpawnEmitter.ExistTime * 1000);
//taskData.OnTriggerEnterCallback = (other) => {
//    var combatEntity = CombatContext.Instance.GameObject2Entitys[other.gameObject];
//    AbilityEntity.ApplyAbilityEffectsTo(combatEntity);
//};
//var task = Entity.CreateWithParent<CreateColliderTask>(this, taskData);
//task.ExecuteTaskAsync().Coroutine();

//var taskData = new CreateColliderTaskData();
//taskData.Position = OwnerEntity.Position;
//taskData.Direction = OwnerEntity.Direction;
//var prefab = SkillExecutionAsset.transform.Find(colliderSpawnEmitter.ColliderName);
//taskData.ColliderPrefab = prefab.gameObject;
//taskData.LifeTime = (int)(colliderSpawnEmitter.ExistTime * 1000);
//taskData.OnTriggerEnterCallback = (other) => {
//    var combatEntity = CombatContext.Instance.GameObject2Entitys[other.gameObject];
//    AbilityEntity.ApplyAbilityEffectsTo(combatEntity);
//};
//var task = Entity.CreateWithParent<CreateColliderTask>(this, taskData);
//task.ExecuteTaskAsync().Coroutine();