using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class StatusComponent : Component
    {
        public List<Ability> Statuses { get; set; } = new List<Ability>();
        public Dictionary<string, List<Ability>> TypeIdStatuses { get; set; } = new Dictionary<string, List<Ability>>();


        public Ability AttachStatus(object configObject)
        {
            var statusAbility = Entity.GetComponent<AbilityComponent>().AttachAbility(configObject);
            var statusConfigID = statusAbility.Config.KeyName;
            if (!TypeIdStatuses.ContainsKey(statusConfigID))
            {
                TypeIdStatuses.Add(statusConfigID, new List<Ability>());
            }
            TypeIdStatuses[statusConfigID].Add(statusAbility);
            Statuses.Add(statusAbility);
            return statusAbility;
        }

        public void RemoveBuff(Ability buff)
        {
            this.Publish(new RemoveStatusEvent() { Entity = this.Entity, Status = buff, StatusId = buff.Id });
            var keyName = buff.Config.KeyName;
            TypeIdStatuses[keyName].Remove(buff);
            if (TypeIdStatuses[keyName].Count == 0)
            {
                TypeIdStatuses.Remove(keyName);
            }
            Statuses.Remove(buff);
            Entity.GetComponent<AbilityComponent>().RemoveAbility(buff);
        }

        public bool HasStatus(string statusTypeId)
        {
            return TypeIdStatuses.ContainsKey(statusTypeId);
        }

        public Ability GetStatus(string statusTypeId)
        {
            return TypeIdStatuses[statusTypeId][0];
        }

        public void OnStatusesChanged(Ability statusAbility)
        {
            var parentEntity = statusAbility.ParentEntity;

            var tempActionControl = ActionControlType.None;
            foreach (var item in Statuses)
            {
                if (!item.Enable)
                {
                    continue;
                }
                foreach (var effect in item.GetComponent<AbilityEffectComponent>().AbilityEffects)
                {
                    if (effect.Enable && effect.TryGet(out EffectActionControlComponent actionControlComponent))
                    {
                        tempActionControl = tempActionControl | actionControlComponent.ActionControlEffect.ActionControlType;
                    }
                }
            }

            if (parentEntity is CombatEntity combatEntity)
            {
                combatEntity.ActionControlType = tempActionControl;
                var moveForbid = combatEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid);
                combatEntity.GetComponent<MotionComponent>().Enable = !moveForbid;
            }
        }
    }
}
