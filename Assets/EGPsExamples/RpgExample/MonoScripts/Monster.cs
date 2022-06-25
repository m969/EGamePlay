using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using UnityEngine.UI;
using DG.Tweening;

public sealed class Monster : MonoBehaviour
{
    public CombatEntity CombatEntity;
    public AnimationComponent AnimationComponent;
    public float MoveSpeed = 0.2f;
    public Text DamageText;
    public Text CureText;
    public Image HealthBarImage;
    public Transform CanvasTrm;
    public Transform StatusSlotsTrm;
    public GameObject StatusIconPrefab;
    private GameObject vertigoParticle;
    private GameObject weakParticle;
    public MotionComponent MotionComponent { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        CombatEntity = CombatContext.Instance.AddChild<CombatEntity>();
        CombatContext.Instance.Object2Entities.Add(gameObject, CombatEntity);
        CombatEntity.Position = transform.position;
        MotionComponent = CombatEntity.AddComponent<MotionComponent>();
        CombatEntity.ListenActionPoint(ActionPointType.PostReceiveDamage, OnReceiveDamage);
        CombatEntity.ListenActionPoint(ActionPointType.PostReceiveCure, OnReceiveCure);
        CombatEntity.ListenActionPoint(ActionPointType.PostReceiveStatus, OnReceiveStatus);
        CombatEntity.Subscribe<RemoveStatusEvent>(OnRemoveStatus);

#if EGAMEPLAY_EXCEL
        var config = ET.StatusConfigCategory.Instance.GetByName("Tenacity");
#else
        var config = Resources.Load<StatusConfigObject>("StatusConfigs/Status_Tenacity");
#endif
        var Status = CombatEntity.AttachStatus<StatusAbility>(config);
        Status.AddComponent<StatusTenacityComponent>();
        Status.OwnerEntity = CombatEntity;
        Status.TryActivateAbility();
        if (name == "Monster")
        {
#if !EGAMEPLAY_EXCEL
            config = Resources.Load<StatusConfigObject>("StatusConfigs/Status_GeDang");
            Status = CombatEntity.AttachStatus<StatusAbility>(config);
            Status.AddComponent<StatusTenacityComponent>();
            Status.OwnerEntity = CombatEntity;
            Status.TryActivateAbility();
#endif
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (MotionComponent.Enable)
        {
            if (MotionComponent.MoveTimer.IsRunning)
            {
                AnimationComponent.Speed = CombatEntity.GetComponent<AttributeComponent>().MoveSpeed.Value;
                AnimationComponent.TryPlayFade(AnimationComponent.RunAnimation);
            }
            else
            {
                AnimationComponent.Speed = 1;
                AnimationComponent.TryPlayFade(AnimationComponent.IdleAnimation);
            }
            transform.position = CombatEntity.Position;
            transform.GetChild(0).localEulerAngles = new Vector3(0, CombatEntity.Direction + 90, 0);
        }
    }

    private void OnReceiveDamage(IActionExecution combatAction)
    {
        var damageAction = combatAction as DamageAction;
        HealthBarImage.fillAmount = CombatEntity.CurrentHealth.Percent();
        var damageText = GameObject.Instantiate(DamageText);
        damageText.transform.SetParent(CanvasTrm);
        damageText.transform.localPosition = Vector3.up * 120;
        damageText.transform.localScale = Vector3.one;
        damageText.transform.localEulerAngles = Vector3.zero;
        damageText.text = $"-{damageAction.DamageValue.ToString()}";
        damageText.GetComponent<DOTweenAnimation>().DORestart();
        GameObject.Destroy(damageText.gameObject, 0.5f);
    }

    private void OnReceiveCure(IActionExecution combatAction)
    {
        var action = combatAction as CureAction;
        HealthBarImage.fillAmount = CombatEntity.CurrentHealth.Percent();
        var cureText = GameObject.Instantiate(CureText);
        cureText.transform.SetParent(CanvasTrm);
        cureText.transform.localPosition = Vector3.up * 120;
        cureText.transform.localScale = Vector3.one;
        cureText.transform.localEulerAngles = Vector3.zero;
        cureText.text = $"+{action.CureValue.ToString()}";
        cureText.GetComponent<DOTweenAnimation>().DORestart();
        GameObject.Destroy(cureText.gameObject, 0.5f);
    }

    private void OnReceiveStatus(IActionExecution combatAction)
    {
        var action = combatAction as AddStatusAction;
        var addStatusEffect = action.AddStatusEffect;
#if EGAMEPLAY_EXCEL
        var statusConfig = addStatusEffect.AddStatusConfig;
#else
        var statusConfig = addStatusEffect.AddStatus;
#endif
        if (name == "Monster")
        {
            var obj = GameObject.Instantiate(StatusIconPrefab);
            obj.transform.SetParent(StatusSlotsTrm);
            obj.GetComponentInChildren<Text>().text = statusConfig.Name;
            obj.name = action.Status.Id.ToString();
        }

        if (statusConfig.ID == "Vertigo")
        {
            AnimationComponent.AnimancerComponent.Play(AnimationComponent.StunAnimation);
            if (vertigoParticle == null)
            {
                vertigoParticle = GameObject.Instantiate(statusConfig.GetParticleEffect());
                vertigoParticle.transform.parent = transform;
                vertigoParticle.transform.localPosition = new Vector3(0, 2, 0);
            }
        }
        if (statusConfig.ID == "Weak")
        {
            if (weakParticle == null)
            {
                weakParticle = GameObject.Instantiate(statusConfig.GetParticleEffect());
                weakParticle.transform.parent = transform;
                weakParticle.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }

    private void OnRemoveStatus(RemoveStatusEvent eventData)
    {
        if (name == "Monster")
        {
            var trm = StatusSlotsTrm.Find(eventData.StatusId.ToString());
            if (trm != null)
            {
                GameObject.Destroy(trm.gameObject);
            }
        }
        
        var statusConfig = eventData.Status.StatusConfig;
        if (statusConfig.ID == "Vertigo")
        {
            AnimationComponent.AnimancerComponent.Play(AnimationComponent.IdleAnimation);
            if (vertigoParticle != null)
            {
                GameObject.Destroy(vertigoParticle);
            }
        }
        if (statusConfig.ID == "Weak")
        {
            if (weakParticle != null)
            {
                GameObject.Destroy(weakParticle);
            }
        }

        //EEvent.Run<BossDeadEventExecution>(eventData.CombatEntity);
    }
}
