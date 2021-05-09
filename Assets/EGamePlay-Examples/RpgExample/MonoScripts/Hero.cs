using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EGamePlay;
using EGamePlay.Combat;
using UnityEngine.UIElements;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat.Ability;
using DG.Tweening;
using ET;
using GameUtils;

public sealed class Hero : MonoBehaviour
{
    public CombatEntity CombatEntity;
    public AnimationComponent AnimationComponent;
    public float MoveSpeed = 1f;
    public float AnimTime = 0.05f;
    public GameTimer AnimTimer = new GameTimer(0.1f);
    public GameObject AttackPrefab;
    public GameObject SkillEffectPrefab;
    public GameObject HitEffectPrefab;
    public Transform InventoryPanelTrm;
    public Transform EquipmentPanelTrm;
    public Transform SkillSlotsTrm;
    public GameObject ItemPrefab;
    public Text DamageText;
    public Text CureText;
    public UnityEngine.UI.Image HealthBarImage;
    public Transform CanvasTrm;

    private Tweener MoveTweener { get; set; }
    private Tweener LookAtTweener { get; set; }

    public static Hero Instance { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public bool SkillPlaying { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        CombatEntity = Entity.CreateWithParent<CombatEntity>(CombatContext.Instance);
        CombatEntity.AddComponent<SkillPreviewComponent>();
        CombatEntity.AddComponent<EquipmentComponent>();
        //CombatEntity.GetComponent<MotionComponent>().Enable = false;
        CombatEntity.ListenActionPoint(ActionPointType.PreSpell, OnPreSpell);
        CombatEntity.ListenActionPoint(ActionPointType.PostSpell, OnPostSpell);
        CombatEntity.Subscribe<AnimationClip>(OnPlayAnimation);

#if EGAMEPLAY_EXCEL
        var config = ConfigHelper.Get<SkillConfig>(1001);
        SkillAbility ability = CombatEntity.AttachSkill<SkillAbility>(config);
        CombatEntity.BindSkillInput(ability, KeyCode.Q);

        config = ConfigHelper.Get<SkillConfig>(1002);
        ability = CombatEntity.AttachSkill<SkillAbility>(config);
        CombatEntity.BindSkillInput(ability, KeyCode.W);

        config = ConfigHelper.Get<SkillConfig>(1004);
        ability = CombatEntity.AttachSkill<SkillAbility>(config);
        CombatEntity.BindSkillInput(ability, KeyCode.E);

        SkillSlotsTrm.Find("SkillButtonD").gameObject.SetActive(false);
        SkillSlotsTrm.Find("SkillButtonE").gameObject.SetActive(false);
#else
        SkillConfigObject config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1001_黑火球术");
        SkillAbility ability = CombatEntity.AttachSkill<SkillAbility>(config);
        CombatEntity.BindSkillInput(ability, KeyCode.Q);

        config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1002_炎爆");
        ability = CombatEntity.AttachSkill<SkillAbility>(config);
        CombatEntity.BindSkillInput(ability, KeyCode.W);

        config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1004_血红激光炮");
        ability = CombatEntity.AttachSkill<SkillAbility>(config);
        CombatEntity.BindSkillInput(ability, KeyCode.E);

        config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1005_火弹");
        ability = CombatEntity.AttachSkill<SkillAbility>(config);
        CombatEntity.BindSkillInput(ability, KeyCode.R);

        config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1006_灵魂镣铐");
        ability = CombatEntity.AttachSkill<SkillAbility_1006>(config);
        CombatEntity.BindSkillInput(ability, KeyCode.T);

        config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1003_治愈");
        ability = CombatEntity.AttachSkill<SkillAbility>(config);
        CombatEntity.BindSkillInput(ability, KeyCode.Y);
#endif
        for (int i = InventoryPanelTrm.childCount; i > 0; i--)
        {
            GameObject.Destroy(InventoryPanelTrm.GetChild(i - 1).gameObject);
        }
        var allItemConfigs = ConfigHelper.GetAll<EquipmentConfig>();
        foreach (var item in allItemConfigs)
        {
            var itemObj = GameObject.Instantiate(ItemPrefab);
            itemObj.transform.parent = InventoryPanelTrm;
            itemObj.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>($"Icons/{item.Value.Name}");
            var text = itemObj.transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
            text.text = $"+{item.Value.Value}";
            if (item.Value.Attribute == "AttackPower")
            {
                text.color = Color.red;
            }
            itemObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
                if (EquipmentPanelTrm.childCount >= 4)
                {
                    return;
                }
                var equipObj = GameObject.Instantiate(ItemPrefab);
                equipObj.transform.parent = EquipmentPanelTrm;
                equipObj.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>($"Icons/{item.Value.Name}");
                var equipText = equipObj.transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
                equipText.text = $"+{item.Value.Value}";
                if (item.Value.Attribute == "AttackPower")
                {
                    equipText.color = Color.red;
                }
                var itemData = Entity.CreateWithParent<ItemData>(CombatEntity);
                equipObj.name = $"{itemData.Id}";
                itemData.ConfigId = item.Value.Id;
                CombatEntity.GetComponent<EquipmentComponent>().AddItemData(itemData);
                equipObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
                    var id = long.Parse(equipObj.name);
                    CombatEntity.GetComponent<EquipmentComponent>().RemoveItemData(id);
                    GameObject.Destroy(equipObj);
                });
            });
        }

        AnimTimer.MaxTime = AnimTime;
    }

    // Update is called once per frame
    void Update()
    {
        CombatEntity.Position = transform.position;
        CombatEntity.Direction = transform.GetChild(0).localEulerAngles.y;

        if (CombatEntity.CurrentSkillExecution != null)
            return;

        if (Input.GetMouseButtonDown((int)MouseButton.RightMouse))
        {
            if (RaycastHelper.CastMapPoint(out var point))
            {
                var time = Vector3.Distance(transform.position, point) * MoveSpeed * 0.5f;
                StopMove();
                MoveTweener = transform.DOMove(point, time).SetEase(Ease.Linear).OnComplete(()=>{ AnimationComponent.PlayFade(AnimationComponent.IdleAnimation); });
                LookAtTweener = transform.GetChild(0).DOLookAt(point, 0.2f);
                AnimationComponent.PlayFade(AnimationComponent.RunAnimation);
            }
        }
    }

    private void OnPreSpell(ActionExecution combatAction)
    {
        var spellAction = combatAction as SpellAction;
        if (spellAction.SkillExecution != null)
        {
            StopMove();

            if (spellAction.SkillAbility is SkillAbility_1006)
            {
                return;
            }

            if (spellAction.SkillExecution.InputTarget != null)
                transform.GetChild(0).LookAt(spellAction.SkillExecution.InputTarget.Position);
            else if (spellAction.SkillExecution.InputPoint != null)
                transform.GetChild(0).LookAt(spellAction.SkillExecution.InputPoint);
            else
                transform.GetChild(0).localEulerAngles = new Vector3(0, spellAction.SkillExecution.InputDirection, 0);

            CombatEntity.Position = transform.position;
            CombatEntity.Direction = transform.GetChild(0).localEulerAngles.y;
        }
    }

    private void OnPostSpell(ActionExecution combatAction)
    {
        var spellAction = combatAction as SpellAction;
        if (spellAction.SkillExecution != null)
        {
            AnimationComponent.PlayFade(AnimationComponent.IdleAnimation);
        }
    }

    private void OnPlayAnimation(AnimationClip animationClip)
    {
        AnimationComponent.PlayFade(animationClip);
    }

    public void StopMove()
    {
        MoveTweener?.Kill();
        LookAtTweener?.Kill();
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
        //PlayThenIdleAsync(AnimationComponent.AttackAnimation).Coroutine();

        if (CombatEntity.AttackActionAbility.TryCreateAction(out var action))
        {
            var monster = GameObject.Find("Monster");
            SpawnLineEffect(AttackPrefab, transform.position, monster.transform.position);
            SpawnHitEffect(transform.position, monster.transform.position);

            CombatEntity.GetComponent<AttributeComponent>().AttackPower.SetBase(ET.RandomHelper.RandomNumber(600, 999));

            action.Target = monster.GetComponent<Monster>().CombatEntity;
            action.ApplyAttack();
            Entity.Destroy(action);
        }
    }

    private ETCancellationToken token;
    public async ETVoid PlayThenIdleAsync(AnimationClip animation)
    {
        AnimationComponent.Play(AnimationComponent.IdleAnimation);
        AnimationComponent.PlayFade(animation);
        if (token != null)
        {
            token.Cancel();
        }
        token = new ETCancellationToken();
        var isTimeout = await TimerComponent.Instance.WaitAsync((int)(animation.length * 1000), token);
        if (isTimeout)
        {
            AnimationComponent.PlayFade(AnimationComponent.IdleAnimation);
        }
    }
}
