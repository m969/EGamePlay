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
public class SpellPreviewComponent : ECS.EcsComponent
{
    public CombatEntity OwnerEntity { get; set; }
    public bool Previewing { get; set; }
    public Ability PreviewingSkill { get; set; }
}
