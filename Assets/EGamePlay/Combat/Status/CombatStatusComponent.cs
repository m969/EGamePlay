using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Status
{
    public class CombatStatusComponent : Component
    {
        public List<StatusEntity> Statuses { get; set; } = new List<StatusEntity>();

        public override void Setup()
        {

        }
    }
}