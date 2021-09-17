using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay
{
    public partial class AbilityData : Entity
    {
        public long UniqueId { get; set; }
        public short ConfigId { get; set; }
        public short Level { get; set; }
    }
}