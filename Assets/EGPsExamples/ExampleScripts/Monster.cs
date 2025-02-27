using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using UnityEngine.UI;
using DG.Tweening;
using ET;
using System.Linq;

public sealed class Monster : MonoBehaviour
{
    public static Monster Boss { get; set; }
    public CombatEntity CombatEntity;
    public AnimationComponent AnimationComponent;
    public float MoveSpeed = 0.2f;
    public Text DamageText;
    public Text CureText;
    public Image HealthBarImage;
    public Transform CanvasTrm;
    public Transform StatusSlotsTrm;
    public GameObject StatusIconPrefab;
    //public GameObject VertigoParticlePrefab;
    //public GameObject WeakParticlePrefab;

    private GameObject vertigoParticle;
    private GameObject weakParticle;
    public MotionComponent MotionComponent { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        CombatEntity = CombatContext.Instance.AddChild<CombatEntity>();
        CombatContext.Instance.Object2Entities.Add(gameObject, CombatEntity);
        CombatEntity.Position = transform.position;
        MotionComponent = CombatEntity.GetComponent<MotionComponent>();
        MotionComponent.RunAI();
        CombatEntity.ListenActionPoint(ActionPointType.PostSufferDamage, OnReceiveDamage);
        CombatEntity.ListenActionPoint(ActionPointType.PostSufferCure, OnReceiveCure);
        CombatEntity.ListenActionPoint(ActionPointType.PostSufferStatus, OnReceiveStatus);
        CombatEntity.Subscribe<RemoveStatusEvent>(OnRemoveStatus);

//#if EGAMEPLAY_EXCEL
//        var config = ET.StatusConfigCategory.Instance.GetByName("Tenacity");
//#else
//        var config = Resources.Load<StatusConfigObject>("StatusConfigs/Status_Tenacity");
//#endif
//        var Status = CombatEntity.GetComponent<StatusComponent>().AttachStatus(config);
//        Status.AddComponent<StatusTenacityComponent>();
//        Status.OwnerEntity = CombatEntity;
//        Status.TryActivateAbility();

        var allConfigs = ConfigHelper.GetAll<AbilityConfig>().Values.ToArray();
        for (int i = 0; i < allConfigs.Length; i++)
        {
            var skillConfig = allConfigs[i];
            var skilld = skillConfig.Id;
            if (skilld == 1001)
            {
                var skillConfigObj = GameUtils.AssetUtils.LoadObject<AbilityConfigObject>($"{AbilityManagerObject.SkillResFolder}/Skill_{skilld}");
                var ability = CombatEntity.GetComponent<SkillComponent>().AttachSkill(skillConfigObj);
                CombatEntity.BindSkillInput(ability, KeyCode.Q);
            }
        }

        if (name == "Monster")// Boss
        {
            Boss = this;
            var ExecutionLinkPanelObj = GameObject.Find("ExecutionLinkPanel");
            if (ExecutionLinkPanelObj != null)
            {
                ExecutionLinkPanelObj.GetComponent<ExecutionLinkPanel>().BossEntity = Boss.CombatEntity;
            }
//#if !EGAMEPLAY_EXCEL
//            config = Resources.Load<StatusConfigObject>("StatusConfigs/Status_QiangTi");
//            Status = CombatEntity.AttachStatus(config);
//            Status.OwnerEntity = CombatEntity;
//            Status.TryActivateAbility();
//#endif
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
            transform.GetChild(0).localEulerAngles = new Vector3(0, CombatEntity.Rotation.eulerAngles.y + 90, 0);
        }
        else
        {
            AnimationComponent.Speed = 1;
            AnimationComponent.TryPlayFade(AnimationComponent.IdleAnimation);
        }
    }

    private void OnReceiveDamage(Entity combatAction)
    {
        var damageAction = combatAction as DamageAction;
        HealthBarImage.fillAmount = CombatEntity.CurrentHealth.ToPercent();
        var damageText = GameObject.Instantiate(DamageText);
        damageText.transform.SetParent(CanvasTrm);
        damageText.transform.localPosition = Vector3.up * 120;
        damageText.transform.localScale = Vector3.one;
        damageText.transform.localEulerAngles = Vector3.zero;
        damageText.text = $"-{damageAction.DamageValue.ToString()}";
        damageText.GetComponent<DOTweenAnimation>().DORestart();
        GameObject.Destroy(damageText.gameObject, 0.5f);
    }

    private void OnReceiveCure(Entity combatAction)
    {
        var action = combatAction as CureAction;
        HealthBarImage.fillAmount = CombatEntity.CurrentHealth.ToPercent();
        var cureText = GameObject.Instantiate(CureText);
        cureText.transform.SetParent(CanvasTrm);
        cureText.transform.localPosition = Vector3.up * 120;
        cureText.transform.localScale = Vector3.one;
        cureText.transform.localEulerAngles = Vector3.zero;
        cureText.text = $"+{action.CureValue.ToString()}";
        cureText.GetComponent<DOTweenAnimation>().DORestart();
        GameObject.Destroy(cureText.gameObject, 0.5f);
    }

    private void OnReceiveStatus(Entity combatAction)
    {
        var action = combatAction as AddStatusAction;
        var addStatusEffect = action.AddStatusEffect;
//#if EGAMEPLAY_EXCEL
//        var statusConfig = addStatusEffect.AddStatusConfig;
//#else
        var statusConfig = addStatusEffect.AddStatus;
//#endif
        var abilityConfig = ConfigHelper.Get<AbilityConfig>(statusConfig.Id);
        var keyName = abilityConfig.KeyName;
        if (name == "Monster")
        {
            if (StatusSlotsTrm != null)
            {
                var obj = GameObject.Instantiate(StatusIconPrefab);
                obj.transform.SetParent(StatusSlotsTrm);
                obj.GetComponentInChildren<Text>().text = abilityConfig.Name;
                obj.name = action.BuffAbility.Id.ToString();
            }
        }

        if (keyName == "Vertigo")
        {
            AnimationComponent.Play(AnimationComponent.StunAnimation);
            if (vertigoParticle == null)
            {
                vertigoParticle = GameObject.Instantiate(Resources.Load<GameObject>("Status_Vertigo"));
                vertigoParticle.transform.parent = transform;
                vertigoParticle.transform.localPosition = new Vector3(0, 2, 0);
            }
        }
        if (keyName == "Weak")
        {
            if (weakParticle == null)
            {
                weakParticle = GameObject.Instantiate(Resources.Load<GameObject>("Status_Weak"));
                weakParticle.transform.parent = transform;
                weakParticle.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }

    private void OnRemoveStatus(RemoveStatusEvent eventData)
    {
        if (name == "Monster")
        {
            if (StatusSlotsTrm != null)
            {
                var trm = StatusSlotsTrm.Find(eventData.StatusId.ToString());
                if (trm != null)
                {
                    GameObject.Destroy(trm.gameObject);
                }
            }
        }

        var statusConfig = eventData.Status.Config;
        if (statusConfig.KeyName == "Vertigo")
        {
            AnimationComponent.Play(AnimationComponent.IdleAnimation);
            if (vertigoParticle != null)
            {
                GameObject.Destroy(vertigoParticle);
            }
        }
        if (statusConfig.KeyName == "Weak")
        {
            if (weakParticle != null)
            {
                GameObject.Destroy(weakParticle);
            }
        }
    }
}
