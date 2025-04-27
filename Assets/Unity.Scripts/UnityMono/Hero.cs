using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EGamePlay;
using EGamePlay.Combat;
using UnityEngine.UIElements;
using DG.Tweening;

public sealed class Hero : MonoBehaviour
{
    //public CombatEntity CombatEntity;
    //public ECSGame.AnimationComponent AnimationComponent {  get; private set; }
    public float MoveSpeed = 1f;
    public float AnimTime = 0.05f;
    //public GameTimer AnimTimer = new GameTimer(0.1f);
    public GameObject AttackPrefab;
    public GameObject SkillEffectPrefab;
    public GameObject HitEffectPrefab;
    public Transform SkillSlotsTrm;
    //public Text DamageText;
    //public Text CureText;
    //public UnityEngine.UI.Image HealthBarImage;
    //public Transform CanvasTrm;

    //private Tweener MoveTweener { get; set; }
    //private Tweener LookAtTweener { get; set; }


    //// Start is called before the first frame update
    //void Start()
    //{
    //    CombatEntity = CombatEntitySystem.Create(StaticClient.Context);
    //    CombatEntity.Init();
    //    GetComponent<AnimationComponent>().SetEntity(CombatEntity);
    //    CombatEntity.GetComponent<EntityViewComponent>().ViewObj = gameObject;
    //    AnimationComponent = CombatEntity.GetComponent<ECSGame.AnimationComponent>();
    //    StaticClient.Context.Object2Entities.Add(gameObject, CombatEntity);
    //    StaticClient.Context.HeroEntity = CombatEntity;
    //    CombatEntity.HeroObject = gameObject;
    //    CombatEntity.ModelTrans = gameObject.transform.GetChild(0);
    //    CombatEntity.IsHero = true;

    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PreExecuteSpell, OnPreSpell);
    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PostExecuteSpell, OnPostSpell);
    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PostSufferDamage, OnReceiveDamage);
    //    CombatEntitySystem.ListenActionPoint(CombatEntity, ActionPointType.PostSufferCure, OnReceiveCure);

    //    HealthSystem.Minus(CombatEntity, 30000);

    //    var allConfigs = StaticConfig.Config.GetAll<AbilityConfig>().Values.ToArray();
    //    for (int i = 0; i < allConfigs.Length; i++)
    //    {
    //        var config = allConfigs[i];
    //        if (config.Type != "Skill")
    //        {
    //            continue;
    //        }
    //        var skilld = config.Id;
    //        if (skilld == 3001)
    //        {
    //            continue;
    //        }
    //        var configObj = GameUtils.AssetUtils.LoadObject<AbilityConfigObject>($"{AbilityManagerObject.SkillResFolder}/Skill_{skilld}");
    //        var ability = SkillSystem.Attach(CombatEntity, configObj);
    //        if (skilld == 1001) CombatEntitySystem.BindSkillInput(CombatEntity, ability, KeyCode.Q);
    //        if (skilld == 1002) CombatEntitySystem.BindSkillInput(CombatEntity, ability, KeyCode.W);
    //        if (skilld == 1003) CombatEntitySystem.BindSkillInput(CombatEntity, ability, KeyCode.Y);
    //        if (skilld == 1004) CombatEntitySystem.BindSkillInput(CombatEntity, ability, KeyCode.E);
    //        if (skilld == 1005) CombatEntitySystem.BindSkillInput(CombatEntity, ability, KeyCode.R);
    //        //if (skilld == 1006)
    //        //{
    //        //    CombatEntity.BindSkillInput(ability, KeyCode.T);
    //        //    ability.AddComponent<Skill1006Component>();
    //        //}
    //        if (skilld == 1008) CombatEntitySystem.BindSkillInput(CombatEntity, ability, KeyCode.A);
    //        //if (skilld == 2001) CombatEntitySystem.BindSkillInput(CombatEntity, ability, KeyCode.S);
    //    }

    //    SpellSystem.LoadExecutionObjects(CombatEntity);

    //    HealthBarImage.fillAmount = HealthSystem.ToPercent(CombatEntity);
    //    AnimTimer.MaxTime = AnimTime;
    //    //InitInventory();

    //    var ExecutionLinkPanelObj = GameObject.Find("ExecutionLinkPanel");
    //    if (ExecutionLinkPanelObj != null)
    //    {
    //        ExecutionLinkPanelObj.GetComponent<ExecutionLinkPanel>().HeroEntity = CombatEntity;
    //    }
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    CombatEntity.Position = transform.position;
    //    CombatEntity.Rotation = transform.GetChild(0).localRotation;

    //    if (CombatEntity.SpellingExecution != null && CombatEntity.SpellingExecution.ActionOccupy)
    //        return;

    //    if (Input.GetMouseButtonDown((int)MouseButton.RightMouse))
    //    {
    //        if (RaycastHelper.CastMapPoint(out var point))
    //        {
    //            var time = Vector3.Distance(transform.position, point) * MoveSpeed * 0.5f;
    //            StopMove();
    //            MoveTweener = transform.DOMove(point, time).SetEase(Ease.Linear).OnComplete(() => { AnimationSystem.PlayFade(CombatEntity, AnimationComponent.IdleAnimation); });
    //            LookAtTweener = transform.GetChild(0).DOLookAt(point, 0.2f);
    //            AnimationSystem.PlayFade(CombatEntity, AnimationComponent.RunAnimation);
    //        }
    //    }
    //}

    //private void OnPreSpell(EcsEntity combatAction)
    //{
    //    var spellAction = combatAction as SpellAction;
    //    if (spellAction.InputTarget != null)
    //    {
    //        CombatEntity.ModelTrans.localRotation = Quaternion.LookRotation(spellAction.InputTarget.Position - CombatEntity.ModelTrans.position);
    //    }
    //    else
    //    {
    //        CombatEntity.ModelTrans.localRotation = Quaternion.LookRotation(spellAction.InputPoint - CombatEntity.ModelTrans.position);
    //    }
    //    DisableMove();

    //    if (spellAction.SkillExecution != null)
    //    {
    //        //if (spellAction.SkillAbility.HasComponent<Skill1006Component>())
    //        //{
    //        //    return;
    //        //}

    //        if (spellAction.SkillExecution.InputTarget != null)
    //            transform.GetChild(0).LookAt(spellAction.SkillExecution.InputTarget.Position);
    //        else if (spellAction.SkillExecution.InputPoint != null)
    //            transform.GetChild(0).LookAt(spellAction.SkillExecution.InputPoint);
    //        else
    //            transform.GetChild(0).localEulerAngles = new Vector3(0, spellAction.SkillExecution.InputRadian, 0);

    //        CombatEntity.Position = transform.position;
    //        CombatEntity.Rotation = transform.GetChild(0).localRotation;
    //    }
    //}

    //private void OnPostSpell(EcsEntity combatAction)
    //{
    //    var spellAction = combatAction as SpellAction;
    //    if (spellAction.SkillExecution != null)
    //    {
    //        AnimationSystem.PlayFade(CombatEntity, AnimationComponent.IdleAnimation);
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
    //    damageText.text = $"-{damageAction.DamageValue}";
    //    damageText.GetComponent<DOTweenAnimation>().DORestart();
    //    GameObject.Destroy(damageText.gameObject, 0.5f);
    //}

    //private void OnReceiveCure(EcsEntity combatAction)
    //{
    //    var cureAction = combatAction as CureAction;
    //    HealthBarImage.fillAmount = HealthSystem.ToPercent(CombatEntity);
    //    var cureText = GameObject.Instantiate(CureText);
    //    cureText.transform.SetParent(CanvasTrm);
    //    cureText.transform.localPosition = Vector3.up * 120;
    //    cureText.transform.localScale = Vector3.one;
    //    cureText.transform.localEulerAngles = Vector3.zero;
    //    cureText.text = $"+{cureAction.CureValue}";
    //    cureText.GetComponent<DOTweenAnimation>().DORestart();
    //    GameObject.Destroy(cureText.gameObject, 0.5f);
    //}

    //public void StopMove()
    //{
    //    MoveTweener?.Kill();
    //    LookAtTweener?.Kill();
    //}

    //public void DisableMove()
    //{
    //    MoveTweener?.Kill();
    //    LookAtTweener?.Kill();
    //    CombatEntity.GetComponent<MotionComponent>().Enable = false;
    //}

    //private void SpawnLineEffect(GameObject lineEffectPrefab, Vector3 p1, Vector3 p2)
    //{
    //    var attackEffect = GameObject.Instantiate(lineEffectPrefab);
    //    attackEffect.transform.position = Vector3.up;
    //    attackEffect.GetComponent<LineRenderer>().SetPosition(0, p1);
    //    attackEffect.GetComponent<LineRenderer>().SetPosition(1, p2);
    //    GameObject.Destroy(attackEffect, 0.05f);
    //}

    //private void SpawnHitEffect(Vector3 p1, Vector3 p2)
    //{
    //    var vec = p1 - p2;
    //    var hitPoint = p2 + vec.normalized * .6f;
    //    hitPoint += Vector3.up;
    //    var hitEffect = GameObject.Instantiate(HitEffectPrefab);
    //    hitEffect.transform.position = hitPoint;
    //    GameObject.Destroy(hitEffect, 0.2f);
    //}

    //public void Attack()
    //{
    //    //PlayThenIdleAsync(AnimationComponent.AttackAnimation).Coroutine();

    //    if (CombatEntity.AttackSpellAbility.TryMakeAction(out var action))
    //    {
    //        var monster = GameObject.Find("Monster");
    //        SpawnLineEffect(AttackPrefab, transform.position, monster.transform.position);
    //        SpawnHitEffect(transform.position, monster.transform.position);

    //        NumericSystem.SetBase(CombatEntity.GetComponent<AttributeComponent>().Attack, ET.RandomHelper.RandomNumber(600, 999));

    //        //action.Target = monster.GetComponent<Monster>().CombatEntity;
    //        //AttakcActionSystem.ApplyAttack();
    //    }
    //}

    //private ETCancellationToken token;
    //public async ETTask PlayThenIdleAsync(AnimationClip animation)
    //{
    //    AnimationSystem.Play(CombatEntity, AnimationComponent.IdleAnimation);
    //    AnimationSystem.PlayFade(CombatEntity, animation);
    //    //if (token != null)
    //    //{
    //    //    token.Cancel();
    //    //}
    //    //token = new ETCancellationToken();
    //    //var isTimeout = 
    //        await TimerSystem.WaitAsync(StaticClient.EcsNode, (int)(animation.length * 1000));
    //    //if (isTimeout)
    //    //{
    //    //    token = null;
    //    //    AnimationComponent.PlayFade(AnimationComponent.IdleAnimation);
    //    //}
    //}
}
