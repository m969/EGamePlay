using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using DG.Tweening;

/// <summary>
/// 主动技能的预览组件，预览功能不是所有游戏都会有，moba类游戏一般会有技能预览的功能，部分mmorpg游戏也可能有，回合制、卡牌、arpg动作游戏一般没有
/// </summary>
public class SpellPreviewComponent : EGamePlay.Component
{
    private CombatEntity OwnerEntity => GetEntity<CombatEntity>();
    private SpellComponent SpellComponent => Entity.GetComponent<SpellComponent>();
    public override bool DefaultEnable { get; set; } = true;
    private bool Previewing { get; set; }
    private Ability PreviewingSkill { get; set; }


    public override void Update()
    {
        var abilityComp = OwnerEntity.GetComponent<SkillComponent>();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Cursor.visible = false;
            PreviewingSkill = abilityComp.InputSkills[KeyCode.Q];
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Cursor.visible = false;
            PreviewingSkill = abilityComp.InputSkills[KeyCode.W];
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Cursor.visible = false;
            PreviewingSkill = abilityComp.InputSkills[KeyCode.E];
            EnterPreview();
        }
//#if !EGAMEPLAY_EXCEL
        if (Input.GetKeyDown(KeyCode.R))
        {
            Cursor.visible = false;
            PreviewingSkill = abilityComp.InputSkills[KeyCode.R];
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Cursor.visible = false;
            PreviewingSkill = abilityComp.InputSkills[KeyCode.T];
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            PreviewingSkill = abilityComp.InputSkills[KeyCode.Y];
            //SpellComp.SpellWithTarget(PreviewingSkill, PreviewingSkill.OwnerEntity);
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Cursor.visible = false;
            PreviewingSkill = abilityComp.InputSkills[KeyCode.A];
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PreviewingSkill = abilityComp.InputSkills[KeyCode.S];
            OnSelectedSelf();
        }
//#endif
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
        var targetSelectType = SkillTargetSelectType.Custom;
        var affectTargetType = SkillAffectTargetType.EnemyTeam;
        var skillId = PreviewingSkill.Config.Id;
        if (PreviewingSkill.Config.TargetSelect == "手动指定") targetSelectType = SkillTargetSelectType.PlayerSelect;
        if (PreviewingSkill.Config.TargetSelect == "碰撞检测") targetSelectType = SkillTargetSelectType.CollisionSelect;
        if (PreviewingSkill.Config.TargetSelect == "条件指定") targetSelectType = SkillTargetSelectType.ConditionSelect;
        if (PreviewingSkill.Config.TargetGroup == "自身") affectTargetType = SkillAffectTargetType.Self;
        if (PreviewingSkill.Config.TargetGroup == "己方") affectTargetType = SkillAffectTargetType.SelfTeam;
        if (PreviewingSkill.Config.TargetGroup == "敌方") affectTargetType = SkillAffectTargetType.EnemyTeam;
        if (targetSelectType == SkillTargetSelectType.PlayerSelect)
        {
            TargetSelectManager.Instance.TargetLimitType = TargetLimitType.EnemyTeam;
            if (affectTargetType == SkillAffectTargetType.SelfTeam) TargetSelectManager.Instance.TargetLimitType = TargetLimitType.SelfTeam;
            TargetSelectManager.Instance.Show(OnSelectedTarget);
        }
        if (targetSelectType == SkillTargetSelectType.CollisionSelect)
        {
            if (skillId == 1004) DirectRectSelectManager.Instance.Show(OnInputDirect);
            else if (skillId == 1005) DirectRectSelectManager.Instance.Show(OnInputDirect);
            else if (skillId == 1008) DirectRectSelectManager.Instance.Show(OnInputDirect);
            else PointSelectManager.Instance.Show(OnInputPoint);
        }
        if (targetSelectType == SkillTargetSelectType.ConditionSelect)
        {
            if (skillId == 1006)
            {
                SelectTargetsWithDistance(PreviewingSkill, 5);
            }
        }
    }

    public void CancelPreview()
    {
        Previewing = false;
        TargetSelectManager.Instance?.Hide();
        PointSelectManager.Instance?.Hide();
        DirectRectSelectManager.Instance?.Hide();
    }

    private void OnSelectedSelf()
    {
        CombatEntity combatEntity = Hero.Instance.CombatEntity; ;
        SpellComponent.SpellWithTarget(PreviewingSkill, combatEntity);
    }

    private void OnSelectedTarget(GameObject selectObject)
    {
        CancelPreview();
        CombatEntity combatEntity = null;
        if (selectObject.GetComponent<Monster>() != null) combatEntity = selectObject.GetComponent<Monster>().CombatEntity;
        if (selectObject.GetComponent<Hero>() != null) combatEntity = selectObject.GetComponent<Hero>().CombatEntity;
        //OwnerEntity.ModelTrans.LookAt(selectObject.transform);
        //Hero.Instance.DisableMove();
        SpellComponent.SpellWithTarget(PreviewingSkill, combatEntity);
    }

    private void OnInputPoint(Vector3 point)
    {
        //OwnerEntity.ModelTrans.localRotation = Quaternion.LookRotation(point - OwnerEntity.ModelTrans.position);
        //Hero.Instance.DisableMove();
        SpellComponent.SpellWithPoint(PreviewingSkill, point);
    }

    private void OnInputDirect(float direction, Vector3 point)
    {
        OnInputPoint(point);
    }

    public void SelectTargetsWithDistance(Ability SpellSkill, float distance)
    {
        if (OwnerEntity.SpellAbility.TryMakeAction(out var action))
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
                action.FinishAction();
                return;
            }

            //OwnerEntity.ModelTrans.localRotation = Quaternion.LookRotation(point - OwnerEntity.ModelTrans.position);
            //Hero.Instance.DisableMove();
            action.SkillAbility = SpellSkill;
            //action.SkillExecution = SpellSkill.CreateExecution() as SkillExecution;
            action.SpellSkill(false);
        }
    }
}
