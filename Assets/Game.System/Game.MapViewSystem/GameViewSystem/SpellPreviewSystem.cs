using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System.Net;
using System;
using ECSGame;
using System.Threading;
using System.ComponentModel;
using ECSUnity;

namespace EGamePlay
{
    public class SpellPreviewSystem : AComponentSystem<Game, SpellPreviewComponent>,
        IAwake<Game, SpellPreviewComponent>,
        IDestroy<Game, SpellPreviewComponent>
    {
        public void Awake(Game game, SpellPreviewComponent component)
        {

        }

        public void Destroy(Game game, SpellPreviewComponent component)
        {

        }

        public static void Update(Game game, SpellPreviewComponent component)
        {
            if (component.OwnerEntity == null)
            {
                component.OwnerEntity = game.MyActor.CombatEntity;
                return;
            }
            var abilityComp = component.OwnerEntity.GetComponent<SkillComponent>();
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Cursor.visible = false;
                component.PreviewingSkill = abilityComp.InputSkills[KeyCode.Q];
                EnterPreview(game);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Cursor.visible = false;
                component.PreviewingSkill = abilityComp.InputSkills[KeyCode.W];
                EnterPreview(game);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Cursor.visible = false;
                component.PreviewingSkill = abilityComp.InputSkills[KeyCode.E];
                EnterPreview(game);
            }
            //#if !EGAMEPLAY_EXCEL
            if (Input.GetKeyDown(KeyCode.R))
            {
                Cursor.visible = false;
                component.PreviewingSkill = abilityComp.InputSkills[KeyCode.R];
                EnterPreview(game);
            }
            //if (Input.GetKeyDown(KeyCode.T))
            //{
            //    Cursor.visible = false;
            //    component.PreviewingSkill = abilityComp.InputSkills[KeyCode.T];
            //    EnterPreview(game);
            //}
            if (Input.GetKeyDown(KeyCode.Y))
            {
                component.PreviewingSkill = abilityComp.InputSkills[KeyCode.Y];
                //SpellSystem.SpellWithTarget(component.PreviewingSkill.OwnerEntity, component.PreviewingSkill, component.PreviewingSkill.OwnerEntity);
                EnterPreview(game);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Cursor.visible = false;
                component.PreviewingSkill = abilityComp.InputSkills[KeyCode.A];
                EnterPreview(game);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                component.PreviewingSkill = abilityComp.InputSkills[KeyCode.S];
                OnSelectedSelf(game);
            }
            //#endif
            if (Input.GetMouseButtonDown((int)UnityEngine.UIElements.MouseButton.RightMouse))
            {
                CancelPreview(game);
            }
            if (component.Previewing)
            {
            }
        }

        public static void EnterPreview(Game game)
        {
            var component = game.GetComponent<SpellPreviewComponent>();
            CancelPreview(game);
            component.Previewing = true;
            var targetSelectType = SkillTargetSelectType.Custom;
            var affectTargetType = SkillAffectTargetType.EnemyTeam;
            var skillId = component.PreviewingSkill.Config.Id;
            if (component.PreviewingSkill.Config.TargetSelect == "手动指定") targetSelectType = SkillTargetSelectType.PlayerSelect;
            if (component.PreviewingSkill.Config.TargetSelect == "碰撞检测") targetSelectType = SkillTargetSelectType.CollisionSelect;
            if (component.PreviewingSkill.Config.TargetSelect == "条件指定") targetSelectType = SkillTargetSelectType.ConditionSelect;
            if (component.PreviewingSkill.Config.TargetGroup == "自身") affectTargetType = SkillAffectTargetType.Self;
            if (component.PreviewingSkill.Config.TargetGroup == "己方") affectTargetType = SkillAffectTargetType.SelfTeam;
            if (component.PreviewingSkill.Config.TargetGroup == "敌方") affectTargetType = SkillAffectTargetType.EnemyTeam;
            if (targetSelectType == SkillTargetSelectType.PlayerSelect)
            {
                TargetSelectManager.Instance.TargetLimitType = TargetLimitType.EnemyTeam;
                if (affectTargetType == SkillAffectTargetType.SelfTeam) TargetSelectManager.Instance.TargetLimitType = TargetLimitType.SelfTeam;
                TargetSelectManager.Instance.Show(x => OnSelectedTarget(game, x));
            }
            if (targetSelectType == SkillTargetSelectType.CollisionSelect)
            {
                if (skillId == 1004) DirectRectSelectManager.Instance.Show((a, b) => OnInputDirect(game, a, b));
                else if (skillId == 1005) DirectRectSelectManager.Instance.Show((a, b) => OnInputDirect(game, a, b));
                else if (skillId == 1008) DirectRectSelectManager.Instance.Show((a, b) => OnInputDirect(game, a, b));
                else PointSelectManager.Instance.Show(x => OnInputPoint(game, x));
            }
            if (targetSelectType == SkillTargetSelectType.ConditionSelect)
            {
                if (skillId == 1006)
                {
                    SelectTargetsWithDistance(game, component.PreviewingSkill, 5);
                }
            }
        }

        public static void CancelPreview(Game game)
        {
            var component = game.GetComponent<SpellPreviewComponent>();
            component.Previewing = false;
            TargetSelectManager.Instance?.Hide();
            PointSelectManager.Instance?.Hide();
            DirectRectSelectManager.Instance?.Hide();
        }

        private static void OnSelectedSelf(Game game)
        {
            var component = game.GetComponent<SpellPreviewComponent>();
            var combatEntity = StaticClient.Game.MyActor.CombatEntity;
            SpellSystem.SpellWithTarget(component.OwnerEntity, component.PreviewingSkill, combatEntity);
        }

        private static void OnSelectedTarget(Game game, GameObject selectObject)
        {
            var component = game.GetComponent<SpellPreviewComponent>();
            CancelPreview(game);
            var combatEntity = game.Object2Entities[selectObject]; ;
            SpellSystem.SpellWithTarget(component.OwnerEntity, component.PreviewingSkill, combatEntity);
        }

        private static void OnInputPoint(Game game, Vector3 point)
        {
            var component = game.GetComponent<SpellPreviewComponent>();
            SpellSystem.SpellWithPoint(component.OwnerEntity, component.PreviewingSkill, point);
        }

        private static void OnInputDirect(Game game, float direction, Vector3 point)
        {
            OnInputPoint(game, point);
        }

        public static void SelectTargetsWithDistance(Game game, Ability spellSkill, float distance)
        {
            var component = game.GetComponent<SpellPreviewComponent>();
            if (component.OwnerEntity.SpellAbility.TryMakeAction(out var action))
            {
                //foreach (var item in StaticClient.Context.Object2Entities.Values)
                //{
                //    if (item.IsHero)
                //    {
                //        continue;
                //    }
                //    if (Vector3.Distance(TransformSystem.GetPosition(item.Actor), TransformSystem.GetPosition(StaticClient.Game.MyActor)) < distance)
                //    {
                //        action.SkillTargets.Add(item);
                //    }
                //}

                if (action.SkillTargets.Count == 0)
                {
                    SpellActionSystem.FinishAction(action);
                    return;
                }

                action.SkillAbility = spellSkill;
                SpellActionSystem.Execute(action, false);
            }
        }
    }
}