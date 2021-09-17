using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay
{
    public partial class ItemData : Entity
    {
        public long UniqueId { get; set; }
        public short ConfigId { get; set; }
        public short Amount { get; set; }
    }
}