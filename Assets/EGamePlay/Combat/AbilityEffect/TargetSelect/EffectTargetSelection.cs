using EGamePlay.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGamePlay.Combat
{
    public class EffectTargetSelection : Entity
    {
        public string TargetSelect { get; set; }
        public CombatEntity OwnerBattler => GetParent<AbilityEffect>().OwnerEntity;
        public IAbilityEntity OwnerAbility => GetParent<AbilityEffect>().OwnerAbility as IAbilityEntity;


        public CombatEntity[] GetTargets()
        {
            var list = new List<CombatEntity>();

            if (TargetSelect == "MinHP")
            {
                
            }

            return list.ToArray();
        }
    }
}
