using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;

/// <summary>
/// 主动技能的预览组件，预览功能不是所有游戏都会有，moba类游戏一般会有技能预览的功能，部分mmorpg游戏也可能有，回合制、卡牌、arpg动作游戏一般没有
/// </summary>
public class SpellPreviewComponent : EGamePlay.Component
{
    private CombatEntity CombatEntity => GetEntity<CombatEntity>();
    private SpellComponent SpellComp => Entity.GetComponent<SpellComponent>();
    public override bool Enable { get; set; } = true;
    private bool Previewing { get; set; }
    private SkillAbility PreviewingSkill { get; set; }


    public override void Setup()
    {

    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnityEngine.Cursor.visible = false;
            PreviewingSkill = CombatEntity.InputSkills[KeyCode.Q];
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            UnityEngine.Cursor.visible = false;
            PreviewingSkill = CombatEntity.InputSkills[KeyCode.W];
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UnityEngine.Cursor.visible = false;
            PreviewingSkill = CombatEntity.InputSkills[KeyCode.E];
            EnterPreview();
        }
#if !EGAMEPLAY_EXCEL
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.Cursor.visible = false;
            PreviewingSkill = CombatEntity.InputSkills[KeyCode.R];
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            UnityEngine.Cursor.visible = false;
            PreviewingSkill = CombatEntity.InputSkills[KeyCode.T];
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            PreviewingSkill = CombatEntity.InputSkills[KeyCode.Y];
            SpellComp.SpellWithTarget(PreviewingSkill, PreviewingSkill.OwnerEntity);
            //EnterPreview();
        }
#endif
        if (Input.GetMouseButtonDown((int)UnityEngine.UIElements.MouseButton.RightMouse))
        {
            CancelPreview();
        }
        if (Previewing)
        {
        }
    }

    public void EnterPreview()
    {
        CancelPreview();
        Previewing = true;
        SkillTargetSelectType targetSelectType = SkillTargetSelectType.Auto;
#if EGAMEPLAY_EXCEL
        if (PreviewingSkill.SkillConfig.TargetSelect == "手动指定") targetSelectType = SkillTargetSelectType.PlayerSelect;
        if (PreviewingSkill.SkillConfig.TargetSelect == "固定区域场检测") targetSelectType = SkillTargetSelectType.AreaSelect;
#else
        targetSelectType = PreviewingSkill.SkillConfig.TargetSelectType;
#endif
        if (targetSelectType == SkillTargetSelectType.PlayerSelect)
        {
            TargetSelectManager.Instance.Show(OnSelectedTarget);
        }
        if (targetSelectType == SkillTargetSelectType.AreaSelect)
        {
            if (PreviewingSkill.SkillConfig.Id == 1002)
            {
                PointSelectManager.Instance.Show(OnInputPoint);
            }
            if (PreviewingSkill.SkillConfig.Id == 1004)
            {
                DirectRectSelectManager.Instance.Show(OnInputDirect);
            }
        }
        if (targetSelectType == SkillTargetSelectType.BodyCollideSelect)
        {
            DirectRectSelectManager.Instance.Show(OnInputDirect);
        }
        if (targetSelectType == SkillTargetSelectType.ConditionSelect)
        {
            SelectTargetsWithDistance(PreviewingSkill, 5);
        }
    }

    public void CancelPreview()
    {
        Previewing = false;
        TargetSelectManager.Instance.Hide();
        PointSelectManager.Instance.Hide();
        DirectRectSelectManager.Instance.Hide();
    }

    private void OnSelectedTarget(GameObject selectObject)
    {
        CancelPreview();
        var combatEntity = selectObject.transform.GetComponent<Monster>().CombatEntity;
        SpellComp.SpellWithTarget(PreviewingSkill, combatEntity);
    }
    
    private void OnInputPoint(Vector3 point)
    {
        SpellComp.SpellWithPoint(PreviewingSkill, point);
    }

    private void OnInputDirect(float direction, Vector3 point)
    {
        SpellComp.SpellWithDirect(PreviewingSkill, direction, point);
    }

    public void SelectTargetsWithDistance(SkillAbility SpellSkill, float distance)
    {
        if (CombatEntity.SpellActionAbility.TryCreateAction(out var action))
        {
            var enemiesRoot = GameObject.Find("Enemies");
            foreach (Transform item in enemiesRoot.transform)
            {
                if (Vector3.Distance(item.position, Hero.Instance.transform.position) < distance)
                {
                    action.SkillTargets.Add(item.GetComponent<Monster>().CombatEntity);
                }
            }

            if (action.SkillTargets.Count == 0)
            {
                action.ApplyAction();
                return;
            }

            action.SkillAbility = SpellSkill;
            action.SkillExecution = SpellSkill.CreateExecution() as SkillExecution;
            action.SpellSkill();
        }
    }
}
