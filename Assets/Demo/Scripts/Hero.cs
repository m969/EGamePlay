using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using UnityEngine.UIElements;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat.Ability;
using DG.Tweening;

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


    // Start is called before the first frame update
    void Start()
    {
        CombatEntity = EntityFactory.Create<CombatEntity>();
        CombatEntity.Initialize();
        CombatEntity.AddComponent<AbilityPreviewComponent>();

        var config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1001_黑火球术");
        var abilityA = EntityFactory.CreateWithParent<Skill1001Entity>(CombatEntity, config);
        CombatEntity.BindAbilityInput(abilityA, KeyCode.Q);

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

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        if (h != 0f || v != 0f)
        {
            MoveTweener?.Kill();
            h *= MoveSpeed * 0.02f;
            v *= MoveSpeed * 0.02f;
            var p = transform.position;
            transform.position = new Vector3(p.x + h, 0, p.z + v);
        }

        if (Input.GetMouseButtonDown((int)MouseButton.RightMouse))
        {
            if (RaycastHelper.CastMapPoint(out var point))
            {
                var time = Vector3.Distance(transform.position, point) * MoveSpeed * 0.5f;
                MoveTweener?.Kill();
                MoveTweener = transform.DOMove(point, time);
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
        var monster = GameObject.Find("/Monster");

        SpawnLineEffect(AttackPrefab, transform.position, monster.transform.position);
        SpawnHitEffect(transform.position, monster.transform.position);

        var action = CombatActionManager.CreateAction<DamageAction>(CombatEntity);
        action.Target = monster.GetComponent<Monster>().CombatEntity;
        action.DamageSource = DamageSource.Attack;
        CombatEntity.NumericBox.PhysicAttack_I.SetBase(RandomHelper.RandomNumber(600, 999));
        action.ApplyDamage();
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
