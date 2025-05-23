﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EGamePlay;
using EGamePlay.Combat;
using DG.Tweening;
using ET;
using ECS;
using ECSUnity;

public class TurnCombatObject : MonoBehaviour
{
    public CombatEntity CombatEntity { get; set; }
    public Vector3 SeatPoint { get; set; }
    public CombatObjectData CombatObjectData { get; set; }
    public AnimationComponent AnimationComponent => CombatObjectData.AnimationComponent;


    //public void Setup(int seat)
    //{
    //    if (transform.parent.name.Contains("Hero")) CombatEntity = StaticObject.CombatContext.AddHeroEntity(seat);
    //    if (transform.parent.name.Contains("Monster")) CombatEntity = StaticObject.CombatContext.AddMonsterEntity(seat);
    //    HealthPointSystem.SetMaxValue(CombatEntity, 1200);
    //    HealthPointSystem.Reset(CombatEntity);

    //    CombatObjectData = GetComponent<CombatObjectData>();
    //    SeatPoint = transform.position;
    //    CombatEntity.HeroObject = gameObject;
    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PreExecuteJumpTo, OnPreJumpTo);
    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PreExecuteAttack, OnPreAttack);
    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PostExecuteAttack, OnPostAttack);
    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PostSufferDamage, OnReceiveDamage);
    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PostSufferCure, OnReceiveCure);
    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PostSufferStatus, OnReceiveStatus);
    //    //CombatEntity.Subscribe<RemoveStatusEvent>(OnRemoveStatus);
    //    //CombatEntity.Subscribe<EntityDeadEvent>(OnDead);

    //    //var config = Resources.Load<StatusConfigObject>("StatusConfigs/Status_Tenacity");
    //    //var Status = CombatEntity.AttachStatus<StatusTenacity>(config);
    //    //Status.Caster = CombatEntity;
    //    //Status.TryActivateAbility();
    //}

    //private void Update()
    //{
    //    CombatEntity.Position = transform.position;
    //}

    //public void OnPreJumpTo(EcsEntity action)
    //{
    //    //var jumpToAction = action as JumpToAction;
    //    //var target = jumpToAction.Target as CombatEntity;
    //    //var targetPoint = target.HeroObject.transform.position + target.HeroObject.transform.forward * 1.7f;
    //    //jumpToAction.Creator.HeroObject.transform.DOMove(targetPoint, jumpToAction.Creator.JumpToTime / 1000f).SetEase(Ease.Linear);
    //    //var AnimationComponent = jumpToAction.Creator.HeroObject.GetComponent<CombatObjectData>().AnimationComponent;
    //    //AnimationComponent.Speed = 2f;
    //    //AnimationComponent.PlayFade(AnimationComponent.RunAnimation);
    //}

    //public void OnPreAttack(EcsEntity action)
    //{
    //    AnimationComponent.Speed = 1f;
    //    AnimationComponent.PlayFade(AnimationComponent.AttackAnimation);
    //}

    //public async void OnPostAttack(EcsEntity action)
    //{
    //    //transform.DOMove(SeatPoint, CombatEntity.JumpToTime / 1000f).SetEase(Ease.Linear);
    //    //var modelTrm = transform.GetChild(0);
    //    //modelTrm.forward = -modelTrm.forward;
    //    //AnimationComponent.Speed = 2f;
    //    //AnimationComponent.PlayFade(AnimationComponent.RunAnimation);
    //    //await TimeHelper.WaitAsync(CombatEntity.JumpToTime);
    //    //AnimationComponent.Speed = 1f;
    //    //AnimationComponent.PlayFade(AnimationComponent.IdleAnimation);
    //    //modelTrm.forward = -modelTrm.forward;
    //}

    //private void OnReceiveDamage(EcsEntity combatAction)
    //{
    //    AnimationComponent.Speed = 1f;
    //    if (HealthPointSystem.CheckDead(CombatEntity) == false)
    //    {
    //        PlayThenIdleAsync(AnimationComponent.DamageAnimation).Coroutine();
    //    }

    //    var damageAction = combatAction as DamageAction;
    //    CombatObjectData.HealthBarImage.fillAmount = HealthPointSystem.ToPercent(CombatEntity);
    //    var damageText = GameObject.Instantiate(CombatObjectData.DamageText);
    //    damageText.transform.SetParent(CombatObjectData.CanvasTrm);
    //    damageText.transform.localPosition = Vector3.up * 120;
    //    damageText.transform.localScale = Vector3.one;
    //    damageText.transform.localEulerAngles = Vector3.zero;
    //    damageText.text = $"-{damageAction.DamageValue.ToString()}";
    //    damageText.GetComponent<DOTweenAnimation>().DORestart();
    //    GameObject.Destroy(damageText.gameObject, 0.5f);
    //}

    //private async void OnDead(EntityDeadEvent deadEvent)
    //{
    //    AnimationComponent.PlayFade(AnimationComponent.DeadAnimation);
    //    await TimeHelper.WaitAsync(2000);
    //    GameObject.Destroy(gameObject);
    //}

    //private void OnReceiveCure(EcsEntity combatAction)
    //{
    //    var action = combatAction as CureAction;
    //    CombatObjectData.HealthBarImage.fillAmount = HealthPointSystem.ToPercent(CombatEntity);

    //    var cureText = GameObject.Instantiate(CombatObjectData.CureText);
    //    cureText.transform.SetParent(CombatObjectData.CanvasTrm);
    //    cureText.transform.localPosition = Vector3.up * 120;
    //    cureText.transform.localScale = Vector3.one;
    //    cureText.transform.localEulerAngles = Vector3.zero;
    //    cureText.text = $"+{action.CureValue.ToString()}";
    //    cureText.GetComponent<DOTweenAnimation>().DORestart();
    //    GameObject.Destroy(cureText.gameObject, 0.5f);
    //}

    //private void OnReceiveStatus(EcsEntity combatAction)
    //{
    //    var action = combatAction as AddBuffAction;
    //    var addStatusEffect = action.AddStatusEffect;
    //    var statusConfig = addStatusEffect.AddStatus;
    //    var abilityConfig = ConfigHelper.Get<AbilityConfig>(combatAction.EcsNode, statusConfig.Id);
    //    var keyName = abilityConfig.KeyName;
    //    if (name == "Monster")
    //    {
    //        var obj = GameObject.Instantiate(CombatObjectData.StatusIconPrefab);
    //        obj.transform.SetParent(CombatObjectData.StatusSlotsTrm);
    //        obj.GetComponentInChildren<Text>().text = abilityConfig.Name;
    //        obj.name = action.BuffAbility.Id.ToString();
    //    }

    //    if (keyName == "Vertigo")
    //    {
    //        CombatEntity.GetComponent<MotionComponent>().Enable = false;
    //        CombatObjectData.AnimationComponent.Play(CombatObjectData.AnimationComponent.StunAnimation);
    //        var vertigoParticle = CombatObjectData.vertigoParticle;
    //        if (vertigoParticle == null)
    //        {
    //            //vertigoParticle = GameObject.Instantiate(statusConfig.ParticleEffect);
    //            //vertigoParticle.transform.parent = transform;
    //            //vertigoParticle.transform.localPosition = new Vector3(0, 2, 0);
    //        }
    //    }
    //    if (keyName == "Weak")
    //    {
    //        var weakParticle = CombatObjectData.weakParticle;
    //        if (weakParticle == null)
    //        {
    //            //weakParticle = GameObject.Instantiate(statusConfig.ParticleEffect);
    //            //weakParticle.transform.parent = transform;
    //            //weakParticle.transform.localPosition = new Vector3(0, 0, 0);
    //        }
    //    }
    //}

    //private void OnRemoveStatus(RemoveStatusEvent eventData)
    //{
    //    if (name == "Monster")
    //    {
    //        var trm = CombatObjectData.StatusSlotsTrm.Find(eventData.StatusId.ToString());
    //        if (trm != null)
    //        {
    //            GameObject.Destroy(trm.gameObject);
    //        }
    //    }

    //    var statusConfig = eventData.Status.Config;
    //    if (statusConfig.KeyName == "Vertigo")
    //    {
    //        CombatEntity.GetComponent<MotionComponent>().Enable = true;
    //        CombatObjectData.AnimationComponent.Play(CombatObjectData.AnimationComponent.IdleAnimation);
    //        if (CombatObjectData.vertigoParticle != null)
    //        {
    //            GameObject.Destroy(CombatObjectData.vertigoParticle);
    //        }
    //    }
    //    if (statusConfig.KeyName == "Weak")
    //    {
    //        if (CombatObjectData.weakParticle != null)
    //        {
    //            GameObject.Destroy(CombatObjectData.weakParticle);
    //        }
    //    }
    //}

    //private ETCancellationToken token;
    //public async ETTask PlayThenIdleAsync(AnimationClip animation)
    //{
    //    AnimationComponent.Play(AnimationComponent.IdleAnimation);
    //    AnimationComponent.PlayFade(animation);
    //    if (token != null)
    //    {
    //        token.Cancel();
    //    }
    //    token = new ETCancellationToken();
    //    //var isTimeout = 
    //        await TimerSystem.WaitAsync(StaticObject.EcsNode, (int)(animation.length * 1000));
    //    //if (isTimeout)
    //    //{
    //    //    AnimationComponent.PlayFade(AnimationComponent.IdleAnimation);
    //    //}
    //}
}
