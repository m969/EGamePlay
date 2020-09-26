using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.IO;
using Sirenix.Utilities.Editor;

namespace EGamePlay.Combat
{
    [CreateAssetMenu(fileName = "技能特效配置", menuName = "技能|状态/技能特效配置")]
    [LabelText("技能特效配置")]
    public class SkillEffectObject : SerializedScriptableObject
    {
        [LabelText("技能特效")]
        public GameObject SkillParticleEffect;

        [Space(20)]
        [LabelText("射弹")]
        public ProjectileEffectObject Projectile;
    }
}