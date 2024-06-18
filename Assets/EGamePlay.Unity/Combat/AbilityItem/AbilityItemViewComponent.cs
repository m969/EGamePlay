using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力单元体Unity前端表现代理
    /// </summary>
    public class AbilityItemViewComponent : Component
    {
        public AbilityItem AbilityItem { get; set; }
        public Transform AbilityItemTrans { get; set; }
        public override bool DefaultEnable => true;


        public override void Update()
        {
            if (AbilityItem != null && AbilityItemTrans != null)
            {
                AbilityItemTrans.position = AbilityItem.Position;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (AbilityItemTrans != null)
            {
                GameObject.Destroy(AbilityItemTrans.gameObject);
            }
        }
    }
}