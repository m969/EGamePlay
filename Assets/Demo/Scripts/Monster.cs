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
    public Image HealthBarImage;
    public Transform CanvasTrm;


    // Start is called before the first frame update
    void Start()
    {
        CombatEntity = EntityFactory.Create<CombatEntity>();
        CombatEntity.Initialize();
        CombatEntity.AddListener(ActionPointType.PostReceiveDamage, OnReceiveDamage);
    }

    // Update is called once per frame
    void Update()
    {

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
}
