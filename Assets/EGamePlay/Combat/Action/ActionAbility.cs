using EGamePlay.Combat;
using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗行动能力
    /// </summary>
    public class ActionAbility : AbilityEntity
    {
        public AbilityExecution MakeAction()
        {
            return CreateExecution();
        }

        public AbilityExecution TryMakeAction()
        {
            if (Enable == false)
            {
                return null;
            }
            return CreateExecution();
        }

        public bool TryMakeAction(out AbilityExecution abilityExecution)
        {
            if (Enable == false)
            {
                abilityExecution = null;
            }
            else
            {
                abilityExecution = CreateExecution();
            }
            return Enable;
        }
    }

    public class ActionAbility<T> : ActionAbility where T : ActionExecution
    {
        public override AbilityExecution CreateExecution()
        {
            var execution = OwnerEntity.MakeAction<T>();
            execution.ActionAbility = this;
            return execution;
        }

        /// <summary>
        /// 发动行动
        /// </summary>
        /// <returns></returns>
        public new T MakeAction()
        {
            return CreateExecution() as T;
        }

        /// <summary>
        /// 尝试发动行动
        /// </summary>
        /// <returns></returns>
        public new T TryMakeAction()
        {
            if (Enable == false)
            {
                return null;
            }
            return CreateExecution() as T;
        }

        /// <summary>
        /// 尝试发动行动
        /// </summary>
        /// <param name="abilityExecution"></param>
        /// <returns></returns>
        public bool TryMakeAction(out T abilityExecution)
        {
            if (Enable == false)
            {
                abilityExecution = null;
            }
            else
            {
                abilityExecution = CreateExecution() as T;
            }
            return Enable;
        }
    }
}