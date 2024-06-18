using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;

/// <summary>
/// 
/// </summary>
public class AbilityItemShieldViewComponent : EGamePlay.Component
{
    public override void Awake()
    {
        GetEntity<AbilityItem>().Subscribe<AttributeUpdateEvent>(OnAttributeUpdate);
    }

    private void OnAttributeUpdate(AttributeUpdateEvent event_)
    {
        var abilityItem = GetEntity<AbilityItem>();
        var type = event_.Numeric.AttributeType;
        var value = event_.Numeric.Value;
        if (type == AttributeType.HealthPoint)
        {
            var shieldTrans = abilityItem.ItemProxy.AbilityItemTrans.GetChild(0).GetChild(1);
            var mat = shieldTrans.GetComponent<MeshRenderer>().material;
            if (mat != null)
            {
                var healthComp = abilityItem.GetComponent<HealthPointComponent>();
                var v = healthComp.ToPercent();
                mat.SetFloat("_Value", 1 - v);
            }
        }
    }
}
