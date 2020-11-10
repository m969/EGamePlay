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
    [CreateAssetMenu(fileName = "射弹特效配置", menuName = "技能|状态/射弹特效配置")]
    [LabelText("射弹特效配置")]
    public class ProjectileEffectObject : SerializedScriptableObject
    {
        [LabelText("射弹特效")]
        public GameObject ProjectileParticleEffect;
    }
}