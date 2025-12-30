using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;
using System.ComponentModel;
using ECSGame;
using System.Security.Cryptography;

namespace EGamePlay
{
    public interface IBeforeExecuteAction:IDispatch
    {
        void BeforeExecuteAction(CombatEntity entity, EcsEntity combatAction);
    }

    public interface IBeforeSufferAction:IDispatch
    {
        void BeforeSufferAction(EcsEntity entity, EcsEntity combatAction);
    }

    public interface IAfterExecuteAction:IDispatch
    {
        void AfterExecuteAction(CombatEntity entity, EcsEntity combatAction);
    }

    public interface IAfterSufferAction:IDispatch
    {
        void AfterSufferAction(EcsEntity entity, EcsEntity combatAction);
    }

    public interface IBeforeCauseApply:IDispatch
    {
        void BeforeCauseApply(CombatEntity entity, EcsEntity combatAction);
    }

    public interface IBeforeReceiveApply:IDispatch
    {
        void BeforeReceiveApply(EcsEntity entity, EcsEntity combatAction);
    }

    public interface IAfterCauseApply:IDispatch
    {
        void AfterCauseApply(CombatEntity entity, EcsEntity combatAction);
    }

    public interface IAfterReceiveApply:IDispatch
    {
        void AfterReceiveApply(EcsEntity entity, EcsEntity combatAction);
    }

    public class ActionSystem
    {
        public static void ExecuteAction<T>(T entity, Func<T, bool> actionProcess, Func<T, bool> actionApply) where T : EcsEntity, IActionExecute
        {
            ActionSystem.BeforeExecuteAction(entity.Creator, x => x.BeforeExecuteAction(entity.Creator, entity));
            ActionSystem.BeforeSufferAction(entity.Target, x => x.BeforeSufferAction(entity.Target, entity));

            var result = actionProcess.Invoke(entity);

            if (result)
            {
                ActionSystem.BeforeCauseApply(entity.Creator, x => x.BeforeCauseApply(entity.Creator, entity));
                ActionSystem.BeforeReceiveApply(entity.Target, x => x.BeforeReceiveApply(entity.Target, entity));

                var applyResult = actionApply.Invoke(entity);

                if (applyResult)
                {
                    ActionSystem.AfterCauseApply(entity.Creator, x => x.AfterCauseApply(entity.Creator, entity));
                    ActionSystem.AfterReceiveApply(entity.Target, x => x.AfterReceiveApply(entity.Target, entity));
                }
            }

            ActionSystem.AfterExecuteAction(entity.Creator, x => x.AfterExecuteAction(entity.Creator, entity));
            ActionSystem.AfterSufferAction(entity.Target, x => x.AfterSufferAction(entity.Target, entity));
        }

        public static void BeforeExecuteAction(CombatEntity entity, Action<IBeforeExecuteAction> action)
        {
            entity?.Dispatch(action);
        }

        public static void BeforeSufferAction(EcsEntity entity, Action<IBeforeSufferAction> action)
        {
            entity?.Dispatch(action);
        }

        public static void AfterExecuteAction(CombatEntity entity, Action<IAfterExecuteAction> action)
        {
            entity?.Dispatch(action);
        }

        public static void AfterSufferAction(EcsEntity entity, Action<IAfterSufferAction> action)
        {
            entity?.Dispatch(action);
        }

        public static void BeforeCauseApply(CombatEntity entity, Action<IBeforeCauseApply> action)
        {
            entity?.Dispatch(action);
        }

        public static void BeforeReceiveApply(EcsEntity entity, Action<IBeforeReceiveApply> action)
        {
            entity?.Dispatch(action);
        }

        public static void AfterCauseApply(CombatEntity entity, Action<IAfterCauseApply> action)
        {
            entity?.Dispatch(action);
        }

        public static void AfterReceiveApply(EcsEntity entity, Action<IAfterReceiveApply> action)
        {
            entity?.Dispatch(action);
        }
    }
}