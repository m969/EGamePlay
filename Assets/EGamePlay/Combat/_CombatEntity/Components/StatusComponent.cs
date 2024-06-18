using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class StatusComponent : Component
    {
        //public CombatEntity CombatEntity => GetEntity<CombatEntity>();
        public List<StatusAbility> Statuses { get; set; } = new List<StatusAbility>();
        public Dictionary<string, List<StatusAbility>> TypeIdStatuses { get; set; } = new Dictionary<string, List<StatusAbility>>();


        public StatusAbility AttachStatus(object configObject)
        {
            var statusAbility = Entity.GetComponent<AbilityComponent>().AttachAbility<StatusAbility>(configObject);
            var statusConfigID = statusAbility.StatusConfig.ID;
            if (!TypeIdStatuses.ContainsKey(statusConfigID))
            {
                TypeIdStatuses.Add(statusConfigID, new List<StatusAbility>());
            }
            TypeIdStatuses[statusConfigID].Add(statusAbility);
            Statuses.Add(statusAbility);
            return statusAbility;
        }

        public bool HasStatus(string statusTypeId)
        {
            return TypeIdStatuses.ContainsKey(statusTypeId);
        }

        public StatusAbility GetStatus(string statusTypeId)
        {
            return TypeIdStatuses[statusTypeId][0];
        }

        public void OnStatusRemove(StatusAbility statusAbility)
        {
            var statusConfigID = statusAbility.StatusConfig.ID;
            TypeIdStatuses[statusConfigID].Remove(statusAbility);
            if (TypeIdStatuses[statusConfigID].Count == 0)
            {
                TypeIdStatuses.Remove(statusConfigID);
            }
            Statuses.Remove(statusAbility);
            this.Publish(new RemoveStatusEvent() { Entity = this.Entity, Status = statusAbility, StatusId = statusAbility.Id });
        }

        public void OnAddStatus(StatusAbility statusAbility)
        {

        }

        public void OnRemoveStatus(StatusAbility statusAbility)
        {

        }

        public void OnStatusesChanged(StatusAbility statusAbility)
        {
            var parentEntity = statusAbility.ParentEntity;

            var tempActionControl = ActionControlType.None;
            //var statuses = CombatEntity.GetTypeChildren<StatusAbility>();
            //Log.Debug($"OnStatusesChanged {statuses.Length}");
            foreach (var item in parentEntity.GetTypeChildren<StatusAbility>())
            {
                if (!item.Enable)
                {
                    continue;
                }
                foreach (var effect in item.GetComponent<AbilityEffectComponent>().AbilityEffects)
                {
                    if (effect.Enable && effect.TryGet(out EffectActionControlComponent actionControlComponent))
                    {
                        //Log.Debug($"{tempActionControl} {actionControlComponent.ActionControlEffect.ActionControlType}");
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
