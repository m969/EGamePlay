using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 状态的属性修饰组件
    /// </summary>
    public class StatusAttributeModifyComponent : Component
    {
        public override void Setup()
        {
            var status = Entity as StatusAbility;
        }
    }
}