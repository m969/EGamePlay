using ECS;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class HealthPointComponent : EcsComponent
    {
        public FloatNumeric HealthPointNumeric;
        public FloatNumeric HealthPointMaxNumeric;
        public int Value { get => (int)HealthPointNumeric.Value; }
        public int MaxValue { get => (int)HealthPointMaxNumeric.Value; }
    }
}