using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using UnityEngine.UI;
using DG.Tweening;

public sealed class Monster : MonoBehaviour
{
    public static Monster Boss { get; set; }
    //public CombatEntity CombatEntity;
    //public ECSGame.AnimationComponent AnimationComponent { get; private set; }
    public float MoveSpeed = 0.2f;
    //public Text DamageText;
    //public Text CureText;
    //public Image HealthBarImage { get; set; }
    //public Transform CanvasTrm { get; set; }
    //public MotionComponent MotionComponent { get; set; }


    //private void Awake()
    //{
    //    CanvasTrm = transform.Find("Canvas");
    //    HealthBarImage = CanvasTrm.Find("Image").GetComponent<Image>();
    //}

    //// Start is called before the first frame update
    //void Start()
    //{
    //    CombatEntity = CombatEntitySystem.Create(StaticClient.Game);
    //    CombatEntity.Init();
    //    GetComponent<AnimationComponent>().SetEntity(CombatEntity);
    //    CombatEntity.GetComponent<EntityViewComponent>().ViewObj = gameObject;
    //    AnimationComponent = CombatEntity.GetComponent<ECSGame.AnimationComponent>();
    //    StaticClient.Context.Object2Entities.Add(gameObject, CombatEntity);
    //    CombatEntity.Position = transform.position;
    //    CombatEntity.IsHero = false;
    //    MotionComponent = CombatEntity.GetComponent<MotionComponent>();
    //    MotionSystem.RunAI(CombatEntity);
    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PostSufferDamage, OnReceiveDamage);
    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PostSufferCure, OnReceiveCure);

    //    var allConfigs = StaticConfig.Config.GetAll<AbilityConfig>().Values.ToArray();
    //    for (int i = 0; i < allConfigs.Length; i++)
    //    {
    //        var skillConfig = allConfigs[i];
    //        var skilld = skillConfig.Id;
    //        if (skilld == 1001)
    //        {
    //            var skillConfigObj = GameUtils.AssetUtils.LoadObject<AbilityConfigObject>($"{AbilityManagerObject.SkillResFolder}/Skill_{skilld}");
    //            var ability = SkillSystem.Attach(CombatEntity, skillConfigObj);
    //            CombatEntitySystem.BindSkillInput(CombatEntity, ability, KeyCode.Q);
    //        }
    //    }

    //    if (name == "Monster")// Boss
    //    {
    //        Boss = this;
    //        var ExecutionLinkPanelObj = GameObject.Find("ExecutionLinkPanel");
    //        if (ExecutionLinkPanelObj != null)
    //        {
    //            ExecutionLinkPanelObj.GetComponent<ExecutionLinkPanel>().BossEntity = Boss.CombatEntity;
    //        }
    //    }
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (MotionComponent.Enable)
    //    {
    //        if (MotionComponent.MoveTimer.IsRunning)
    //        {
    //            AnimationComponent.Speed = CombatEntity.GetComponent<AttributeComponent>().MoveSpeed.Value;
    //            AnimationSystem.TryPlayFade(CombatEntity, AnimationComponent.RunAnimation);
    //        }
    //        else
    //        {
    //            AnimationComponent.Speed = 1;
    //            AnimationSystem.TryPlayFade(CombatEntity, AnimationComponent.IdleAnimation);
    //        }
    //        transform.position = CombatEntity.Position;
    //        transform.GetChild(0).localEulerAngles = new Vector3(0, CombatEntity.Rotation.eulerAngles.y + 90, 0);
    //    }
    //    else
    //    {
    //        AnimationComponent.Speed = 1;
    //        AnimationSystem.TryPlayFade(CombatEntity, AnimationComponent.IdleAnimation);
    //    }
    //}

    //private void OnReceiveDamage(EcsEntity combatAction)
    //{
    //    var damageAction = combatAction as DamageAction;
    //    HealthBarImage.fillAmount = HealthSystem.ToPercent(CombatEntity);
    //    var damageText = GameObject.Instantiate(DamageText);
    //    damageText.transform.SetParent(CanvasTrm);
    //    damageText.transform.localPosition = Vector3.up * 120;
    //    damageText.transform.localScale = Vector3.one;
    //    damageText.transform.localEulerAngles = Vector3.zero;
    //    damageText.text = $"-{damageAction.DamageValue.ToString()}";
    //    damageText.GetComponent<DOTweenAnimation>().DORestart();
    //    GameObject.Destroy(damageText.gameObject, 0.5f);
    //}

    //private void OnReceiveCure(EcsEntity combatAction)
    //{
    //    var action = combatAction as CureAction;
    //    HealthBarImage.fillAmount = HealthSystem.ToPercent(CombatEntity);
    //    var cureText = GameObject.Instantiate(CureText);
    //    cureText.transform.SetParent(CanvasTrm);
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
    //    var abilityConfig = StaticConfig.Config.Get<AbilityConfig>(statusConfig.Id);
    //    var keyName = abilityConfig.KeyName;
    //    if (name == "Monster")
    //    {
    //        if (StatusSlotsTrm != null)
    //        {
    //            var obj = GameObject.Instantiate(StatusIconPrefab);
    //            obj.transform.SetParent(StatusSlotsTrm);
    //            obj.GetComponentInChildren<Text>().text = abilityConfig.Name;
    //            obj.name = action.BuffAbility.Id.ToString();
    //        }
    //    }

    //    if (keyName == "Vertigo")
    //    {
    //        AnimationSystem.Play(CombatEntity, AnimationComponent.StunAnimation);
    //        if (vertigoParticle == null)
    //        {
    //            vertigoParticle = GameObject.Instantiate(Resources.Load<GameObject>("Status_Vertigo"));
    //            vertigoParticle.transform.parent = transform;
    //            vertigoParticle.transform.localPosition = new Vector3(0, 2, 0);
    //        }
    //    }
    //    if (keyName == "Weak")
    //    {
    //        if (weakParticle == null)
    //        {
    //            weakParticle = GameObject.Instantiate(Resources.Load<GameObject>("Status_Weak"));
    //            weakParticle.transform.parent = transform;
    //            weakParticle.transform.localPosition = new Vector3(0, 0, 0);
    //        }
    //    }
    //}

    //private void OnRemoveStatus(RemoveStatusEvent eventData)
    //{
    //    if (name == "Monster")
    //    {
    //        if (StatusSlotsTrm != null)
    //        {
    //            var trm = StatusSlotsTrm.Find(eventData.StatusId.ToString());
    //            if (trm != null)
    //            {
    //                GameObject.Destroy(trm.gameObject);
    //            }
    //        }
    //    }

    //    var statusConfig = eventData.Status.Config;
    //    if (statusConfig.KeyName == "Vertigo")
    //    {
    //        AnimationSystem.Play(CombatEntity, AnimationComponent.IdleAnimation);
    //        if (vertigoParticle != null)
    //        {
    //            GameObject.Destroy(vertigoParticle);
    //        }
    //    }
    //    if (statusConfig.KeyName == "Weak")
    //    {
    //        if (weakParticle != null)
    //        {
    //            GameObject.Destroy(weakParticle);
    //        }
    //    }
    //}
}
