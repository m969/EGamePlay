using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EGamePlay.Combat;
using DG.Tweening;
using ET;

public class TurnCombatObject : MonoBehaviour
{
    public CombatEntity CombatEntity { get; set; } 
    public Vector3 SeatPoint { get; set; }
    public MonsterObjectData MonsterObjectData { get; set; }
    public AnimationComponent AnimationComponent => MonsterObjectData.AnimationComponent;


    public virtual void Setup()
    {
        MonsterObjectData = GetComponent<MonsterObjectData>();
        SeatPoint = transform.position;
        CombatEntity.gameObject = gameObject;
        CombatEntity.ListenActionPoint(ActionPointType.PreJumpTo, OnPreJumpTo);
        CombatEntity.ListenActionPoint(ActionPointType.PreGiveAttack, OnPreAttack);
        CombatEntity.ListenActionPoint(ActionPointType.PostGiveAttack, OnPostAttack);
        CombatEntity.ListenActionPoint(ActionPointType.PostReceiveDamage, OnReceiveDamage);
        CombatEntity.ListenActionPoint(ActionPointType.PostReceiveCure, OnReceiveCure);
        CombatEntity.ListenActionPoint(ActionPointType.PostReceiveStatus, OnReceiveStatus);
        CombatEntity.Subscribe<RemoveStatusEvent>(OnRemoveStatus).AsCoroutine();

        var config = Resources.Load<StatusConfigObject>("StatusConfigs/Status_Tenacity");
        var Status = CombatEntity.AttachStatus<StatusTenacity>(config);
        Status.Caster = CombatEntity;
        Status.TryActivateAbility();
    }

    private void Update()
    {
        CombatEntity.Position = transform.position;
    }

    public void OnTurnAction()
    {

    }

    public void OnPreJumpTo(CombatAction action)
    {
        var targetPoint = action.Target.gameObject.transform.position + action.Target.gameObject.transform.forward * 2;
        transform.DOMove(targetPoint, CombatEntity.JumpToTime / 1000f).SetEase(Ease.Linear);
        AnimationComponent.Speed = 2f;
        AnimationComponent.PlayFade(AnimationComponent.RunAnimation);
    }

    public void OnPreAttack(CombatAction action)
    {
        AnimationComponent.Speed = 1f;
        AnimationComponent.PlayFade(AnimationComponent.AttackAnimation);
    }

    public void OnPostAttack(CombatAction action)
    {
        transform.DOMove(SeatPoint, CombatEntity.JumpToTime / 1000f).SetEase(Ease.Linear);
        AnimationComponent.Speed = 1f;
        AnimationComponent.PlayFade(AnimationComponent.IdleAnimation);
    }

    private void OnReceiveDamage(CombatAction combatAction)
    {
        AnimationComponent.Speed = 1f;
        PlayThenIdleAsync(AnimationComponent.DamageAnimation).Coroutine();

        var damageAction = combatAction as DamageAction;
        MonsterObjectData.HealthBarImage.fillAmount = CombatEntity.CurrentHealth.Percent();
        var damageText = GameObject.Instantiate(MonsterObjectData.DamageText);
        damageText.transform.SetParent(MonsterObjectData.CanvasTrm);
        damageText.transform.localPosition = Vector3.up * 120;
        damageText.transform.localScale = Vector3.one;
        damageText.transform.localEulerAngles = Vector3.zero;
        damageText.text = $"-{damageAction.DamageValue.ToString()}";
        damageText.GetComponent<DOTweenAnimation>().DORestart();
        GameObject.Destroy(damageText.gameObject, 0.5f);
    }

    private void OnReceiveCure(CombatAction combatAction)
    {
        var action = combatAction as CureAction;
        MonsterObjectData.HealthBarImage.fillAmount = CombatEntity.CurrentHealth.Percent();

        var cureText = GameObject.Instantiate(MonsterObjectData.CureText);
        cureText.transform.SetParent(MonsterObjectData.CanvasTrm);
        cureText.transform.localPosition = Vector3.up * 120;
        cureText.transform.localScale = Vector3.one;
        cureText.transform.localEulerAngles = Vector3.zero;
        cureText.text = $"+{action.CureValue.ToString()}";
        cureText.GetComponent<DOTweenAnimation>().DORestart();
        GameObject.Destroy(cureText.gameObject, 0.5f);
    }

    private void OnReceiveStatus(CombatAction combatAction)
    {
        var action = combatAction as AssignEffectAction;
        if (action.Effect is AddStatusEffect addStatusEffect)
        {
            var statusConfig = addStatusEffect.AddStatus;
            if (name == "Monster")
            {
                var obj = GameObject.Instantiate(MonsterObjectData.StatusIconPrefab);
                obj.transform.SetParent(MonsterObjectData.StatusSlotsTrm);
                obj.GetComponentInChildren<Text>().text = statusConfig.Name;
                obj.name = action.Status.Id.ToString();
            }

            if (statusConfig.ID == "Vertigo")
            {
                CombatEntity.GetComponent<MotionComponent>().Enable = false;
                MonsterObjectData.AnimationComponent.AnimancerComponent.Play(MonsterObjectData.AnimationComponent.StunAnimation);
                var vertigoParticle = MonsterObjectData.vertigoParticle;
                if (vertigoParticle == null)
                {
                    vertigoParticle = GameObject.Instantiate(statusConfig.ParticleEffect);
                    vertigoParticle.transform.parent = transform;
                    vertigoParticle.transform.localPosition = new Vector3(0, 2, 0);
                }
            }
            if (statusConfig.ID == "Weak")
            {
                var weakParticle = MonsterObjectData.weakParticle;
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
            var trm = MonsterObjectData.StatusSlotsTrm.Find(eventData.StatusId.ToString());
            if (trm != null)
            {
                GameObject.Destroy(trm.gameObject);
            }
        }

        var statusConfig = eventData.Status.StatusConfigObject;
        if (statusConfig.ID == "Vertigo")
        {
            CombatEntity.GetComponent<MotionComponent>().Enable = true;
            MonsterObjectData.AnimationComponent.AnimancerComponent.Play(MonsterObjectData.AnimationComponent.IdleAnimation);
            if (MonsterObjectData.vertigoParticle != null)
            {
                GameObject.Destroy(MonsterObjectData.vertigoParticle);
            }
        }
        if (statusConfig.ID == "Weak")
        {
            if (MonsterObjectData.weakParticle != null)
            {
                GameObject.Destroy(MonsterObjectData.weakParticle);
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
