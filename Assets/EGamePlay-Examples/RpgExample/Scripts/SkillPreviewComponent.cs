using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat.Ability;

/// <summary>
/// 主动技能的预览组件，预览功能不是所有游戏都会有，moba类游戏一般会有技能预览的功能，部分mmorpg游戏也可能有，回合制、卡牌、arpg动作游戏一般没有
/// </summary>
public class SkillPreviewComponent : EGamePlay.Component
{
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
            PreviewingSkill = GetEntity<CombatEntity>().InputAbilitys[KeyCode.Q] as SkillAbility;
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            UnityEngine.Cursor.visible = false;
            PreviewingSkill = GetEntity<CombatEntity>().InputAbilitys[KeyCode.W] as SkillAbility;
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UnityEngine.Cursor.visible = false;
            PreviewingSkill = GetEntity<CombatEntity>().InputAbilitys[KeyCode.E] as SkillAbility;
            EnterPreview();
        }
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
        if (PreviewingSkill is Skill1001Entity)
        {
            TargetSelectManager.Instance.Show(OnSelectedTarget);
        }
        if (PreviewingSkill is Skill1002Entity)
        {
            PointSelectManager.Instance.Show(OnInputPoint);
        }
        if (PreviewingSkill is Skill1004Ability)
        {
            DirectRectSelectManager.Instance.Show(OnInputDirect);
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
        OnInputTarget(combatEntity);
    }

    private void OnInputTarget(CombatEntity combatEntity)
    {
        var action = CombatActionManager.CreateAction<SpellSkillAction>(GetEntity<CombatEntity>());
        action.SkillAbility = PreviewingSkill;
        action.SkillAbilityExecution = PreviewingSkill.CreateAbilityExecution() as SkillAbilityExecution;
        action.SkillAbilityExecution.InputCombatEntity = combatEntity;
        action.SpellSkill();
    }

    private void OnInputPoint(Vector3 point)
    {
        var action = CombatActionManager.CreateAction<SpellSkillAction>(GetEntity<CombatEntity>());
        action.SkillAbility = PreviewingSkill;
        action.SkillAbilityExecution = PreviewingSkill.CreateAbilityExecution() as SkillAbilityExecution;
        action.SkillAbilityExecution.InputPoint = point;
        action.SpellSkill();
    }

    private void OnInputDirect(float direction)
    {
        var action = CombatActionManager.CreateAction<SpellSkillAction>(GetEntity<CombatEntity>());
        action.SkillAbility = PreviewingSkill;
        action.SkillAbilityExecution = PreviewingSkill.CreateAbilityExecution() as SkillAbilityExecution;
        action.SkillAbilityExecution.InputDirection = direction;
        action.SpellSkill();
    }
}
