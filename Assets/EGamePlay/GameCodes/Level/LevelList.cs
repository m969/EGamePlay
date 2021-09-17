using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay
{
    public partial class LevelList : Entity
    {
        public Dictionary<int, LevelData> Datas { get; set; }
    }
}