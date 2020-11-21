using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using EGamePlay.Combat.Ability;

/// <summary>
/// 主动技能的预览组件，预览功能不是所有游戏都会有，moba类游戏一般会有技能预览的功能，部分mmorpg游戏也可能有，回合制、卡牌、arpg动作游戏一般没有
/// </summary>
public class AbilityPreviewComponent : EGamePlay.Component
{
    private bool Previewing { get; set; }
    private AbilityEntity PreviewingAbility { get; set; }


    public override void Setup()
    {

    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnityEngine.Cursor.visible = false;
            PreviewingAbility = GetEntity<CombatEntity>().InputAbilitys[KeyCode.Q];
            EnterPreview();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            UnityEngine.Cursor.visible = false;
            PreviewingAbility = GetEntity<CombatEntity>().InputAbilitys[KeyCode.W];
            EnterPreview();
        }
        if (Input.GetMouseButtonDown((int)UnityEngine.UIElements.MouseButton.RightMouse))
        {
            CancelPreview();
        }
        if (Previewing)
        {
            TargetSelectManager.Instance.transform.position = Input.mousePosition;
        }
    }

    public void EnterPreview()
    {
        Previewing = true;
        TargetSelectManager.Instance.Show(OnSelectedTarget);
    }

    public void CancelPreview()
    {
        Previewing = false;
        TargetSelectManager.Instance.Hide();
    }

    private void OnSelectedTarget(GameObject selectObject)
    {
        CancelPreview();
        var combatEntity = selectObject.transform.parent.GetComponent<Monster>().CombatEntity;
        OnInputTarget(combatEntity);
    }

    private void OnInputTarget(CombatEntity combatEntity)
    {
        var abilityExecution = PreviewingAbility.CreateAbilityExecution();
        abilityExecution.InputCombatEntity = combatEntity;
        abilityExecution.BeginExecute();
    }
}
