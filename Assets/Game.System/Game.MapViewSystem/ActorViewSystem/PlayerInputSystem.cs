using DG.Tweening;
using ECS;
using EGamePlay.Combat;
using GameUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace ECSGame
{
    public class PlayerInputSystem : AComponentSystem<Game, PlayerInputComponent>,
IAwake<Game, PlayerInputComponent>,
IInit<Game, PlayerInputComponent>
    {
        public void Awake(Game game, PlayerInputComponent component)
        {
        }

        public void Init(Game game, PlayerInputComponent component)
        {
        }

        public static void Update(Game game, PlayerInputComponent component)
        {
            var actor = game.MyActor;
            var combatEntity = actor.CombatEntity;
            //combatEntity.Position = transform.position;
            //combatEntity.Rotation = transform.GetChild(0).localRotation;

            if (combatEntity.SpellingExecution != null && combatEntity.SpellingExecution.ActionOccupy)
                return;

            if (Input.GetMouseButtonDown((int)MouseButton.RightMouse))
            {
                if (RaycastHelper.CastMapPoint(out var point))
                {
                    var transComp = actor.GetComponent<TransformComponent>();
                    var moveComp = actor.GetComponent<MoveComponent>();
                    var animComp = actor.GetComponent<AnimationComponent>();
                    var time = Vector3.Distance(transComp.Position, point) / moveComp.Speed;
                    StopMove(game);
                    component.MoveTweener = DOTween.To(() => TransformSystem.GetPosition(actor), x => TransformSystem.ChangePosition(actor, x), point, time).SetEase(Ease.Linear).OnComplete(() => { AnimationSystem.PlayFade(actor, animComp.IdleAnimation); });
                    TransformSystem.ChangeForward(actor, point - transComp.Position);
                    //component.LookAtTweener = transform.GetChild(0).DOLookAt(point, 0.2f);
                    AnimationSystem.PlayFade(actor, animComp.RunAnimation);
                }
            }
        }

        public static void StopMove(Game game)
        {
            var component = game.GetComponent<PlayerInputComponent>();
            component.MoveTweener?.Kill();
            //component.LookAtTweener?.Kill();
        }

        public static void DisableMove(Game game)
        {
            var actor = game.MyActor;
            var combatEntity = actor.CombatEntity;
            var component = game.GetComponent<PlayerInputComponent>();
            component.MoveTweener?.Kill();
            //component.LookAtTweener?.Kill();
            combatEntity.GetComponent<MotionComponent>().Enable = false;
        }
    }
}
