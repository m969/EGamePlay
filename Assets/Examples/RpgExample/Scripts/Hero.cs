using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using UnityEngine.UIElements;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat.Ability;
using DG.Tweening;
using ET;

public sealed class Hero : MonoBehaviour
{
    public CombatEntity CombatEntity;
    public float MoveSpeed = 1f;
    public float AnimTime = 0.05f;
    public GameTimer AnimTimer = new GameTimer(0.1f);
    public GameObject AttackPrefab;
    public GameObject SkillEffectPrefab;
    public GameObject HitEffectPrefab;
    private Tweener MoveTweener { get; set; }
    [Space(10)]
    public Animancer.AnimancerComponent AnimancerComponent;
    public AnimationClip IdleAnimation;
    public AnimationClip RunAnimation;
    public AnimationClip JumpAnimation;
    public AnimationClip AttackAnimation;
    public AnimationClip SkillAnimation;


    // Start is called before the first frame update
    void Start()
    {
        AnimancerComponent.Animator.fireEvents = false;
        AnimancerComponent.States.CreateIfNew(IdleAnimation);
        AnimancerComponent.States.CreateIfNew(RunAnimation);
        AnimancerComponent.States.CreateIfNew(JumpAnimation);
        AnimancerComponent.States.CreateIfNew(AttackAnimation);
        AnimancerComponent.States.CreateIfNew(SkillAnimation);

        CombatEntity = EntityFactory.Create<CombatEntity>();
        CombatEntity.AddComponent<SkillPreviewComponent>();

        SkillConfigObject config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1001_黑火球术");
        AbilityEntity abilityA = CombatEntity.AttachSkill<Skill1001Entity>(config);
        CombatEntity.BindAbilityInput(abilityA, KeyCode.Q);

        config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1002_炎爆");
        abilityA = CombatEntity.AttachSkill<Skill1002Entity>(config);
        CombatEntity.BindAbilityInput(abilityA, KeyCode.W);

        config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1004_血红激光炮");
        abilityA = CombatEntity.AttachSkill<Skill1004Entity>(config);
        CombatEntity.BindAbilityInput(abilityA, KeyCode.E);

        AnimTimer.MaxTime = AnimTime;
    }

    // Update is called once per frame
    void Update()
    {
        CombatEntity.Position = transform.position;

        AnimTimer.UpdateAsFinish(Time.deltaTime);
        if (!AnimTimer.IsFinished)
        {
            //return;
        }

        //var h = Input.GetAxis("Horizontal");
        //var v = Input.GetAxis("Vertical");
        //if (h != 0f || v != 0f)
        //{
        //    MoveTweener?.Kill();
        //    h *= MoveSpeed * 0.02f;
        //    v *= MoveSpeed * 0.02f;
        //    var p = transform.position;
        //    transform.position = new Vector3(p.x + h, 0, p.z + v);
        //}

        if (Input.GetMouseButtonDown((int)MouseButton.RightMouse))
        {
            if (RaycastHelper.CastMapPoint(out var point))
            {
                var time = Vector3.Distance(transform.position, point) * MoveSpeed * 0.5f;
                MoveTweener?.Kill();
                MoveTweener = transform.DOMove(point, time).SetEase(Ease.Linear).OnUpdate(()=> { transform.GetChild(0).DOLookAt(point, 0.2f); }).OnComplete(()=>{ AnimancerComponent.Play(IdleAnimation, 0.25f); });
                AnimancerComponent.Play(RunAnimation, 0.25f);
            }
        }
    }

    private void SpawnLineEffect(GameObject lineEffectPrefab, Vector3 p1, Vector3 p2)
    {
        var attackEffect = GameObject.Instantiate(lineEffectPrefab);
        attackEffect.transform.position = Vector3.up;
        attackEffect.GetComponent<LineRenderer>().SetPosition(0, p1);
        attackEffect.GetComponent<LineRenderer>().SetPosition(1, p2);
        GameObject.Destroy(attackEffect, 0.05f);
    }

    private void SpawnHitEffect(Vector3 p1, Vector3 p2)
    {
        var vec = p1 - p2;
        var hitPoint = p2 + vec.normalized * .6f;
        hitPoint += Vector3.up;
        var hitEffect = GameObject.Instantiate(HitEffectPrefab);
        hitEffect.transform.position = hitPoint;
        GameObject.Destroy(hitEffect, 0.2f);
    }

    public void Attack()
    {
        PlayThenIdleAsync(AttackAnimation).Coroutine();

        var monster = GameObject.Find("/Monster");

        SpawnLineEffect(AttackPrefab, transform.position, monster.transform.position);
        SpawnHitEffect(transform.position, monster.transform.position);

        var action = CombatActionManager.CreateAction<DamageAction>(CombatEntity);
        action.Target = monster.GetComponent<Monster>().CombatEntity;
        action.DamageSource = DamageSource.Attack;
        CombatEntity.AttributeComponent.AttackPower.SetBase(ET.RandomHelper.RandomNumber(600, 999));
        action.ApplyDamage();
    }

    public async ETVoid PlayThenIdleAsync(AnimationClip animation)
    {
        AnimancerComponent.Play(animation, 0.25f);
        await TimerComponent.Instance.WaitAsync((int)(animation.length * 1000));
        AnimancerComponent.Play(IdleAnimation, 0.25f);
    }

    public void SpellSkillA()
    {
        //var abilityA = CombatEntity.IndexAbilitys[1];
        //var monster = GameObject.Find("/Monster");
        //CombatEntity.Position = transform.position;

        //var abilityExecution = EntityFactory.CreateWithParent<Skill1001Execution>(CombatEntity, abilityA);
        //abilityExecution.AbilityExecutionTarget = monster.GetComponent<Monster>().CombatEntity;
        //abilityExecution.InputPoint = monster.transform.position;
        //abilityExecution.BeginExecute();
    }

    public void SpellSkillB()
    {

    }
}
