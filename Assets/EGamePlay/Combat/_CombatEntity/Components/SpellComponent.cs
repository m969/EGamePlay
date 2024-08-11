using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using GameUtils;
using System;

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
    public class SpellComponent : EGamePlay.Component
    {
        private CombatEntity CombatEntity => GetEntity<CombatEntity>();
        public override bool DefaultEnable { get; set; } = true;
        public Dictionary<int, ExecutionObject> ExecutionObjects = new Dictionary<int, ExecutionObject>();


        public override void Awake()
        {

        }

        public void LoadExecutionObjects()
        {
            foreach (var item in CombatEntity.GetComponent<SkillComponent>().IdSkills)
            {
                var executionObj = AssetUtils.LoadObject<ExecutionObject>($"{AbilityManagerObject.ExecutionResFolder}/Execution_{item.Key}");
                if (executionObj != null)
                {
                    ExecutionObjects.Add(item.Key, executionObj);
                }
            }
        }

        public void SpellWithTarget(Ability spellSkill, CombatEntity targetEntity)
        {
            if (CombatEntity.SpellingExecution != null)
                return;

            if (CombatEntity.SpellAbility.TryMakeAction(out var spellAction))
            {
                spellAction.SkillAbility = spellSkill;
                spellAction.InputTarget = targetEntity;
                spellAction.InputPoint = targetEntity.Position;
#if EGAMEPLAY_ET
                var rotation = Quaternion.LookRotation(targetEntity.Position - spellSkill.OwnerEntity.Position, math.up());
                spellSkill.OwnerEntity.Rotation = rotation;
                spellAction.InputDirection = math.forward(rotation).y;
#else
                spellSkill.OwnerEntity.Rotation = Quaternion.LookRotation(targetEntity.Position - spellSkill.OwnerEntity.Position);
                spellAction.InputRadian = spellSkill.OwnerEntity.Rotation.eulerAngles.y;
#endif
                spellAction.SpellSkill();
            }
        }

#if EGAMEPLAY_ET
        private float CalCos(float3 a, float3 b)
        {
            // 点积
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
        public SpellAction SpellWithPoint(Ability spellSkill, Vector3 point)
        {
            Log.Console($"SpellComponent SpellWithPoint {spellSkill.Config.Id}");
            if (CombatEntity.SpellingExecution != null)
                return null;

            if (CombatEntity.SpellAbility.TryMakeAction(out var spellAction))
            {
                spellAction.SkillAbility = spellSkill;
                var forward = Vector3.Normalize(point - spellSkill.OwnerEntity.Position);
                var rotation = Quaternion.LookRotation(forward);
                spellSkill.OwnerEntity.Rotation = rotation;
                var angle = rotation.eulerAngles.y;
                var radian = angle * MathF.PI / 180f;
                spellAction.InputDirection = forward;
                spellAction.InputRadian = radian;
                spellAction.InputPoint = point;
                spellAction.SpellSkill();
            }

            return spellAction;
        }
#endif
    }
}