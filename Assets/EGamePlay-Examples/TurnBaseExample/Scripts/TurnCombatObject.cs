using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EGamePlay;
using EGamePlay.Combat;
using DG.Tweening;
using ET;

public class TurnCombatObject : MonoBehaviour
{
    public CombatEntity CombatEntity { get; set; } 
    public Vector3 SeatPoint { get; set; }
    public CombatObjectData CombatObjectData { get; set; }
    public AnimationComponent AnimationComponent => CombatObjectData.AnimationComponent;


    public void Setup(int seat)
    {
        if (transform.parent.name.Contains("Hero")) CombatEntity = CombatContext.Instance.AddHeroEntity(seat);
        if (transform.parent.name.Contains("Monster")) CombatEntity = CombatContext.Instance.AddMonsterEntity(seat);
        CombatEntity.CurrentHealth.SetMaxValue(999);
        CombatEntity.CurrentHealth.Reset();

        CombatObjectData = GetComponent<CombatObjectData>();
        SeatPoint = transform.position;
        CombatEntity.ModelObject = gameObject;
        CombatEntity.ListenActionPoint(ActionPointType.PreJumpTo, OnPreJumpTo);
        CombatEntity.ListenActionPoint(ActionPointType.PreGiveAttack, OnPreAttack);
        CombatEntity.ListenActionPoint(ActionPointType.PostGiveAttack, OnPostAttack);
        CombatEntity.ListenActionPoint(ActionPointType.PostReceiveDamage, OnReceiveDamage);
        CombatEntity.ListenActionPoint(ActionPointType.PostReceiveCure, OnReceiveCure);
        CombatEntity.ListenActionPoint(ActionPointType.PostReceiveStatus, OnReceiveStatus);
        CombatEntity.Subscribe<RemoveStatusEvent>(OnRemoveStatus);
        CombatEntity.Subscribe<DeadEvent>(OnDead);

        //var config = Resources.Load<StatusConfigObject>("StatusConfigs/Status_Tenacity");
        //var Status = CombatEntity.AttachStatus<StatusTenacity>(config);
        //Status.Caster = CombatEntity;
        //Status.TryActivateAbility();
    }

    private void Update()
    {
        CombatEntity.Position = transform.position;
    }

    public void OnPreJumpTo(ActionExecution action)
    {
        var jumpToAction = action as JumpToAction;
        var targetPoint = jumpToAction.Target.ModelObject.transform.position + jumpToAction.Target.ModelObject.transform.forward * 2;
        jumpToAction.Creator.ModelObject.transform.DOMove(targetPoint, jumpToAction.Creator.JumpToTime / 1000f).SetEase(Ease.Linear);
        var AnimationComponent = jumpToAction.Creator.ModelObject.GetComponent<CombatObjectData>().AnimationComponent;
        AnimationComponent.Speed = 2f;
        AnimationComponent.PlayFade(AnimationComponent.RunAnimation);
    }

    public void OnPreAttack(ActionExecution action)
    {
        AnimationComponent.Speed = 1f;
        AnimationComponent.PlayFade(AnimationComponent.AttackAnimation);
    }

    public async void OnPostAttack(ActionExecution action)
    {
        transform.DOMove(SeatPoint, CombatEntity.JumpToTime / 1000f).SetEase(Ease.Linear);
        var modelTrm = transform.GetChild(0);
        modelTrm.forward = -modelTrm.forward;
        AnimationComponent.Speed = 2f;
        AnimationComponent.PlayFade(AnimationComponent.RunAnimation);
        await TimeHelper.WaitAsync(CombatEntity.JumpToTime);
        AnimationComponent.Speed = 1f;
        AnimationComponent.PlayFade(AnimationComponent.IdleAnimation);
        modelTrm.forward = -modelTrm.forward;
    }

    private void OnReceiveDamage(ActionExecution combatAction)
    {
        AnimationComponent.Speed = 1f;
        if (CombatEntity.CheckDead() == false)
        {
            PlayThenIdleAsync(AnimationComponent.DamageAnimation).Coroutine();
        }

        var damageAction = combatAction as DamageAction;
        CombatObjectData.HealthBarImage.fillAmount = CombatEntity.CurrentHealth.Percent();
        var damageText = GameObject.Instantiate(CombatObjectData.DamageText);
        damageText.transform.SetParent(CombatObjectData.CanvasTrm);
        damageText.transform.localPosition = Vector3.up * 120;
        damageText.transform.localScale = Vector3.one;
        damageText.transform.localEulerAngles = Vector3.zero;
        damageText.text = $"-{damageAction.DamageValue.ToString()}";
        damageText.GetComponent<DOTweenAnimation>().DORestart();
        GameObject.Destroy(damageText.gameObject, 0.5f);
    }

    private async void OnDead(DeadEvent deadEvent)
    {
        AnimationComponent.PlayFade(AnimationComponent.DeadAnimation);
        await TimeHelper.WaitAsync(2000);
        GameObject.Destroy(gameObject);
    }

    private void OnReceiveCure(ActionExecution combatAction)
    {
        var action = combatAction as CureAction;
        CombatObjectData.HealthBarImage.fillAmount = CombatEntity.CurrentHealth.Percent();

        var cureText = GameObject.Instantiate(CombatObjectData.CureText);
        cureText.transform.SetParent(CombatObjectData.CanvasTrm);
        cureText.transform.localPosition = Vector3.up * 120;
        cureText.transform.localScale = Vector3.one;
        cureText.transform.localEulerAngles = Vector3.zero;
        cureText.text = $"+{action.CureValue.ToString()}";
        cureText.GetComponent<DOTweenAnimation>().DORestart();
        GameObject.Destroy(cureText.gameObject, 0.5f);
    }

    private void OnReceiveStatus(ActionExecution combatAction)
    {
        var action = combatAction as AssignEffectAction;
        if (action.Effect is AddStatusEffect addStatusEffect)
        {
            var statusConfig = addStatusEffect.AddStatus;
            if (name == "Monster")
            {
                var obj = GameObject.Instantiate(CombatObjectData.StatusIconPrefab);
                obj.transform.SetParent(CombatObjectData.StatusSlotsTrm);
                obj.GetComponentInChildren<Text>().text = statusConfig.Name;
                obj.name = action.Status.Id.ToString();
            }

            if (statusConfig.ID == "Vertigo")
            {
                CombatEntity.GetComponent<MotionComponent>().Enable = false;
                CombatObjectData.AnimationComponent.AnimancerComponent.Play(CombatObjectData.AnimationComponent.StunAnimation);
                var vertigoParticle = CombatObjectData.vertigoParticle;
                if (vertigoParticle == null)
                {
                    vertigoParticle = GameObject.Instantiate(statusConfig.ParticleEffect);
                    vertigoParticle.transform.parent = transform;
                    vertigoParticle.transform.localPosition = new Vector3(0, 2, 0);
                }
            }
            if (statusConfig.ID == "Weak")
            {
                var weakParticle = CombatObjectData.weakParticle;
                if (weakParticle == null)
                {
                    weakParticle = GameObject.Instantiate(statusConfig.ParticleEffect);
                    weakParticle.transform.parent = transform;
                    weakParticle.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
        }
    }

    private void OnRemoveStatus(RemoveStatusEvent eventData)
    {
        if (name == "Monster")
        {
            var trm = CombatObjectData.StatusSlotsTrm.Find(eventData.StatusId.ToString());
            if (trm != null)
            {
                GameObject.Destroy(trm.gameObject);
            }
        }

        var statusConfig = eventData.Status.StatusConfigObject;
        if (statusConfig.ID == "Vertigo")
        {
            CombatEntity.GetComponent<MotionComponent>().Enable = true;
            CombatObjectData.AnimationComponent.AnimancerComponent.Play(CombatObjectData.AnimationComponent.IdleAnimation);
            if (CombatObjectData.vertigoParticle != null)
            {
                GameObject.Destroy(CombatObjectData.vertigoParticle);
            }
        }
        if (statusConfig.ID == "Weak")
        {
            if (CombatObjectData.weakParticle != null)
            {
                GameObject.Destroy(CombatObjectData.weakParticle);
            }
        }
    }

    private ETCancellationToken token;
    public async ETVoid PlayThenIdleAsync(AnimationClip animation)
    {
        AnimationComponent.Play(AnimationComponent.IdleAnimation);
        AnimationComponent.PlayFade(animation);
        if (token != null)
        {
            token.Cancel();
        }
        token = new ETCancellationToken();
        var isTimeout = await TimerComponent.Instance.WaitAsync((int)(animation.length * 1000), token);
        if (isTimeout)
        {
            AnimationComponent.PlayFade(AnimationComponent.IdleAnimation);
        }
    }
}
