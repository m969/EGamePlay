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
            var monster = GameObject.Find("/Monster");
            var attackEffect = GameObject.Instantiate(AttackPrefab);
            attackEffect.transform.position = Vector3.up;
            attackEffect.GetComponent<LineRenderer>().SetPosition(0, transform.position);
            attackEffect.GetComponent<LineRenderer>().SetPosition(1, monster.transform.position);
            GameObject.Destroy(attackEffect, 0.05f);

            var vec = transform.position - monster.transform.position;
            var hitPoint = monster.transform.position + vec.normalized * .6f;
            hitPoint += Vector3.up;
            var hitEffect = GameObject.Instantiate(HitEffectPrefab);
            hitEffect.transform.position = hitPoint;
            GameObject.Destroy(hitEffect, 0.2f);


            var DamageOperation = CombatOperationManager.CreateOperation<DamageOperation>(CombatEntity);
            DamageOperation.Target = monster.GetComponent<Monster>().CombatEntity;
            CombatEntity.NumericBox.PhysicAttack_I.SetBase(RandomHelper.RandomNumber(600, 999));
            DamageOperation.ApplyDamage();
        }
    }

    public void SpellSkillA()
    {
        var monster = GameObject.Find("/Monster");
        var operation = CombatOperationManager.CreateOperation<SpellSkillOperation>(this.CombatEntity);
        operation.SkillEntity = this.CombatEntity.CreateSkill<Skill_1002>();
        operation.Target = monster.GetComponent<Monster>().CombatEntity;
        operation.SpellSkill();
    }
}
