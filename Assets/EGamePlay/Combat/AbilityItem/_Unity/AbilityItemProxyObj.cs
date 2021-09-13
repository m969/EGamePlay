using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;

namespace EGamePlay.Combat.Ability
{
    /// <summary>
    /// 能力单元体Unity前端表现代理
    /// </summary>
    public class AbilityItemProxyObj : MonoBehaviour
    {
        public AbilityItem AbilityItem { get; set; }


        private void Update()
        {
            if (AbilityItem != null)
            {
                if (AbilityItem.IsDisposed)
                {
                    GameObject.Destroy(gameObject);
                    return;
                }
                transform.position = AbilityItem.Position;
            }
        }
    }
}