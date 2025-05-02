using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;

namespace EGamePlay
{
    public class SpellSystem : AComponentSystem<CombatEntity, SpellComponent>,
        IAwake<CombatEntity, SpellComponent>
    {
        public void Awake(CombatEntity entity, SpellComponent component)
        {

        }

        public static void LoadExecutionObjects(CombatEntity entity)
        {
            foreach (var item in entity.GetComponent<SkillComponent>().IdSkills)
            {
                var executionObj = AssetUtils.LoadObject<ExecutionObject>($"{AbilityManagerObject.ExecutionResFolder}/Execution_{item.Key}");
                if (executionObj != null)
                {
                    entity.GetComponent<SpellComponent>().ExecutionObjects.Add(item.Key, executionObj);
                }
            }
        }

        public static void SpellWithTarget(CombatEntity entity, Ability spellSkill, CombatEntity targetEntity)
        {
            if (entity.SpellingExecution != null)
                return;

            if (entity.SpellAbility.TryMakeAction(out var spellAction))
            {
                spellAction.SkillAbility = spellSkill;
                spellAction.InputTarget = targetEntity;
                spellAction.InputPoint = targetEntity.Position;
#if EGAMEPLAY_ET
                var rotation = Quaternion.LookRotation(targetEntity.Position - spellSkill.OwnerEntity.Position, math.up());
                spellSkill.OwnerEntity.Rotation = rotation;
                spellAction.InputDirection = math.forward(rotation).y;
#else
                if (targetEntity.Position != spellSkill.OwnerEntity.Position)
                {
                    spellSkill.OwnerEntity.Rotation = Quaternion.LookRotation(targetEntity.Position - spellSkill.OwnerEntity.Position);
                    spellAction.InputRadian = spellSkill.OwnerEntity.Rotation.eulerAngles.y;
                }
#endif
                SpellActionSystem.Execute(spellAction);
            }
        }

#if EGAMEPLAY_ET
        private float CalCos(float3 a, float3 b)
        {
            // µã»ý
            var dotProduct = a[0] * b[0] + a[1] * b[1];
            var d = MathF.Sqrt(a[0] * a[0] + a[1] * a[1]) * MathF.Sqrt(b[0] * b[0] + b[1] * b[1]);
            return dotProduct / d;
        }

        public SpellAction SpellWithPoint(SkillAbility spellSkill, float3 point)
        {
            //if (CombatEntity.SpellingExecution != null)
            //    return null;

            if (CombatEntity.SpellAbility.TryMakeAction(out var spellAction))
            {
                spellAction.SkillAbility = spellSkill;
                spellAction.InputPoint = point;
                var forward = math.normalizesafe(point - spellSkill.OwnerEntity.Position, math.right());
                //var forward = new float3(rawForward.x, -rawForward.y, 1);
                var rotate = quaternion.LookRotation(forward, math.up());
                spellSkill.OwnerEntity.Rotation = rotate;//.GetQuaternionEulerAngles() / MathF.PI * 180;
                var cos = CalCos(math.right(), forward);
                var radian = MathF.Acos(cos);
                if (forward.y < 0) radian = -radian;
                spellAction.InputDirection = forward;
                spellAction.InputRadian = radian;
                spellAction.InputPoint = point;
                if (spellSkill.SkillConfig.Id == 2003) CombatEntity.AttackAbility.InputDirection = forward;
                spellAction.SpellSkill();
                return spellAction;
            }
            return null;
        }
#else
        public static SpellAction SpellWithPoint(CombatEntity entity, Ability spellSkill, Vector3 point)
        {
            if (entity.SpellingExecution != null)
                return null;

            if (entity.SpellAbility.TryMakeAction(out var spellAction))
            {
                spellAction.SkillAbility = spellSkill;
                var forward = Vector3.Normalize(point - spellSkill.OwnerEntity.Position);
                var rotation = Quaternion.LookRotation(forward);
                spellSkill.OwnerEntity.Rotation = rotation;
                var angle = -(rotation.eulerAngles.y - 90);
                var radian = angle * MathF.PI / 180f;
                spellAction.InputDirection = forward;
                spellAction.InputRadian = radian;
                spellAction.InputPoint = point;
                //Log.Console($"SpellComponent SpellWithPoint {spellSkill.Config.Id} {angle} {radian}");
                SpellActionSystem.Execute(spellAction);
            }

            return spellAction;
        }
#endif
    }
}