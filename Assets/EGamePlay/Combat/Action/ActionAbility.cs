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
    public interface IActionAbility
    {
        public CombatEntity OwnerEntity { get; set; }
        public CombatEntity ParentEntity { get; }
        public bool Enable { get; set; }

        //public AbilityExecution MakeAction()
        //{
        //    return CreateExecution();
        //}

        //public AbilityExecution TryMakeAction()
        //{
        //    if (Enable == false)
        //    {
        //        return null;
        //    }
        //    return CreateExecution();
        //}

        //public bool TryMakeAction(out AbilityExecution abilityExecution)
        //{
        //    if (Enable == false)
        //    {
        //        abilityExecution = null;
        //    }
        //    else
        //    {
        //        abilityExecution = CreateExecution();
        //    }
        //    return Enable;
        //}
    }

    public interface IActionAbility<T> : IActionAbility, IAbilityEntity where T : IActionExecution
    {
        //public new T CreateExecution();
        //{
        //    var execution = OwnerEntity.MakeAction<T>();
        //    execution.ActionAbility = this;
        //    return execution;
        //}

        /// <summary>
        /// 发动行动
        /// </summary>
        /// <returns></returns>
        //public T MakeAction();
        //{
        //    return CreateExecution() as T;
        //}

        /// <summary>
        /// 尝试发动行动
        /// </summary>
        /// <returns></returns>
        //public T TryMakeAction();
        //{
        //    if (Enable == false)
        //    {
        //        return null;
        //    }
        //    return CreateExecution() as T;
        //}

        /// <summary>
        /// 尝试发动行动
        /// </summary>
        /// <param name="abilityExecution"></param>
        /// <returns></returns>
        //public bool TryMakeAction(out T abilityExecution);
        //{
        //    if (Enable == false)
        //    {
        //        abilityExecution = null;
        //    }
        //    else
        //    {
        //        abilityExecution = CreateExecution() as T;
        //    }
        //    return Enable;
        //}
    }
}