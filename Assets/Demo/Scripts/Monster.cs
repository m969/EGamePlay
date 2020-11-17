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
    public float MoveSpeed = 0.2f;
    public Text DamageText;
    public Text CureText;
    public Image HealthBarImage;
    public Transform CanvasTrm;


    // Start is called before the first frame update
    void Start()
    {
        CombatEntity = EntityFactory.Create<CombatEntity>();
        CombatEntity.Initialize();
        CombatEntity.AddListener(ActionPointType.PostReceiveDamage, OnReceiveDamage);
        CombatEntity.AddListener(ActionPointType.PostReceiveCure, OnReceiveCure);

        var config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1004_坚韧");
        var abilityA = CombatEntity.AttachSkill<PassiveSkill1004Entity>(config);
    }

    // Update is called once per frame
    void Update()
    {
        CombatEntity.Position = transform.position;
    }

    private void OnReceiveDamage(CombatAction combatAction)
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

    private void OnReceiveCure(CombatAction combatAction)
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
}
