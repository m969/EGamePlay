using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using UnityEngine.UIElements;
using EGamePlay.Combat.Skill;

public sealed class Hero : MonoBehaviour
{
    public CombatEntity CombatEntity;
    public float MoveSpeed = 0.2f;
    public float AnimTime = 0.05f;
    public GameTimer AnimTimer = new GameTimer(0.1f);
    public GameObject AttackPrefab;
    public GameObject SkillEffectPrefab;
    public GameObject HitEffectPrefab;


    // Start is called before the first frame update
    void Start()
    {
        CombatEntity = Entity.Create<CombatEntity>();
        CombatEntity.Initialize();
        CombatEntity.AddComponent<CombatSkillComponent>().Setup();
        AnimTimer.MaxTime = AnimTime;
    }

    // Update is called once per frame
    void Update()
    {
        AnimTimer.UpdateAsFinish(Time.deltaTime);
        if (!AnimTimer.IsFinished)
        {
            //return;
        }

        var h = Input.GetAxis("Horizontal") * MoveSpeed;
        var v = Input.GetAxis("Vertical") * MoveSpeed;
        var p = transform.position;
        transform.position = new Vector3(p.x + h, 0, p.z + v);

        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
        {
            AnimTimer.Reset();
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
        var monster = GameObject.Find("/Monster");

        SpawnLineEffect(SkillEffectPrefab, transform.position, monster.transform.position);
        SpawnHitEffect(transform.position, monster.transform.position);

        var action = CombatActionManager.CreateAction<SpellSkillAction>(this.CombatEntity);
        action.Target = monster.GetComponent<Monster>().CombatEntity;
        var skill = CombatSkillManager.CreateSkill<Skill_1001>();
        skill.SpellCaster = this.CombatEntity;
        skill.SkillConfigObject = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1001_黑火球术");
        action.SkillEntity = skill;
        action.SpellSkill();
    }

    public void SpellSkillB()
    {
        var monster = GameObject.Find("/Monster");

        SpawnLineEffect(SkillEffectPrefab, transform.position, monster.transform.position);
        SpawnHitEffect(transform.position, monster.transform.position);

        var action = CombatActionManager.CreateAction<SpellSkillAction>(this.CombatEntity);
        action.Target = monster.GetComponent<Monster>().CombatEntity;
        var skill = CombatSkillManager.CreateSkill<Skill_1002>();
        skill.SpellCaster = this.CombatEntity;
        skill.SkillConfigObject = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1002_炎爆");
        action.SkillEntity = skill;
        action.SpellSkill();
    }
}
