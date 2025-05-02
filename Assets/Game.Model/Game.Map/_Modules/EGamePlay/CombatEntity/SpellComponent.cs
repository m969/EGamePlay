using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using GameUtils;
using System;
using ECS;

#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
using JsonIgnore = MongoDB.Bson.Serialization.Attributes.BsonIgnoreAttribute;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 技能施法组件
    /// </summary>
    public class SpellComponent : EcsComponent
    {
        public Dictionary<int, ExecutionObject> ExecutionObjects = new Dictionary<int, ExecutionObject>();
    }
}