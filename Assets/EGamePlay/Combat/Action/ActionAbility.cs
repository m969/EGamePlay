using EGamePlay.Combat.Ability;
using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动能力
    /// </summary>
    public class ActionAbility : AbilityEntity
    {
        public AbilityExecution CreateAction()
        {
            return CreateExecution();
        }

        public AbilityExecution TryCreateAction()
        {
            if (Enable == false)
            {
                return null;
            }
            return CreateExecution();
        }

        public bool TryCreateAction(out AbilityExecution abilityExecution)
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
            return OwnerEntity.CreateAction<T>();
        }

        public new T CreateAction()
        {
            return CreateExecution() as T;
        }

        public new T TryCreateAction()
        {
            if (Enable == false)
            {
                return null;
            }
            return CreateExecution() as T;
        }

        public bool TryCreateAction(out T abilityExecution)
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