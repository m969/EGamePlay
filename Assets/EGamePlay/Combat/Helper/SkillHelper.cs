namespace EGamePlay.Combat
{
    using System;
    using Sirenix.OdinInspector;

    public class SkillHelper
    {

    }

    [LabelText("技能释放方式")]
    public enum SkillSpellType
    {
        [LabelText("主动")]
        Initiative,
        [LabelText("被动")]
        Passive,
    }

    //[LabelText("技能类型")]
    //public enum SkillType
    //{
    //    [LabelText("指定目标类技能")]
    //    Targeting,
    //    [LabelText("非指向性技能")]
    //    Untoward,
    //}

    //[LabelText("技能效果触发类型")]
    //public enum SkillEffectTriggerType
    //{
    //    [LabelText("指定目标类技能")]
    //    Targeting,
    //    [LabelText("非指向性技能")]
    //    Untoward,
    //}

    [LabelText("技能目标检测方式")]
    public enum SkillTargetSelectType
    {
        [LabelText("自动")]
        Auto,
        [LabelText("手动指定")]
        PlayerSelect,
        [LabelText("弹体碰撞检测")]
        BodyCollideSelect,
        [LabelText("固定区域场检测")]
        AreaSelect,
        [LabelText("条件指定")]
        ConditionSelect,
        [LabelText("其他")]
        Other,
    }

    [LabelText("区域场类型")]
    public enum SkillAffectAreaType
    {
        [LabelText("圆形")]
        Circle = 0,
        [LabelText("矩形")]
        Rect = 1,
        [LabelText("组合")]
        Compose = 2,
    }

    [LabelText("技能作用对象")]
    public enum SkillAffectTargetType
    {
        [LabelText("自身")]
        Self = 0,
        [LabelText("己方")]
        SelfTeam = 1,
        [LabelText("敌方")]
        EnemyTeam = 2,
    }

    [LabelText("施加对象")]
    public enum AddSkillEffetTargetType
    {
        [LabelText("技能目标")]
        SkillTarget = 0,
        [LabelText("自身")]
        Self = 1,
    }

    [LabelText("目标类型")]
    public enum SkillTargetType
    {
        [LabelText("单体检测")]
        Single = 0,
        [LabelText("多人检测")]
        Multiple = 1,
    }

    [LabelText("伤害类型")]
    public enum DamageType
    {
        [LabelText("物理伤害")]
        Physic = 0,
        [LabelText("魔法伤害")]
        Magic = 1,
        [LabelText("真实伤害")]
        Real = 2,
    }

    [LabelText("效果类型")]
    public enum SkillEffectType
    {
        [LabelText("(选择效果)")]
        None = 0,
        [LabelText("造成伤害")]
        CauseDamage = 1,
        [LabelText("治疗英雄")]
        CureHero = 2,
        [LabelText("施加Buff")]
        AddBuff = 3,
        [LabelText("移除Buff")]
        RemoveBuff = 4,
        [LabelText("改变状态")]
        ChangeState = 5,
        [LabelText("改变数值")]
        ChangeNumeric = 6,
        [LabelText("添加护盾")]
        AddShield = 7,
        //[LabelText("标记叠加")]
        //AddTag = 8,
    }

    [Flags]
    [LabelText("状态类型")]
    public enum StateType
    {
        [LabelText("（空）")]
        None = 0,
        [LabelText("移动禁止")]
        MoveForbid = 1 << 1,
        [LabelText("技能禁止")]
        SkillForbid = 1 << 2,

        [LabelText("禁锢")]
        Immobility = MoveForbid,
        [LabelText("眩晕")]
        Vertigo = MoveForbid | SkillForbid,
        [LabelText("沉默")]
        Silent = SkillForbid,
        [LabelText("中毒")]
        Poison = 1 << 21,
        [LabelText("灼烧")]
        Burn = 1 << 22,
    }

    [LabelText("数值类型")]
    public enum NumericType
    {
        [LabelText("物理攻击")]
        PhysicAttack = 1001,
        [LabelText("物理护甲")]
        Defense = 1002,
        [LabelText("法术强度")]
        SpellPower = 1003,
        [LabelText("魔法抗性")]
        MagicDefense = 1004,

        [LabelText("暴击概率")]
        CriticalProb = 2001,
    }

    [LabelText("整形数值")]
    public enum IntNumericType
    {
        [LabelText("物理攻击")]
        PhysicAttack = 1001,
        [LabelText("物理护甲")]
        Defense = 1002,
        [LabelText("法术强度")]
        SpellPower = 1003,
        [LabelText("魔法抗性")]
        MagicDefense = 1004,
    }

    [LabelText("浮点型形数值")]
    public enum FloatNumericType
    {
        [LabelText("暴击概率")]
        CriticalProb = 2001,
    }

    //[Flags]
    //[LabelText("伤害类型")]
    //public enum DamageType
    //{
    //    [LabelText("(空)")]
    //    None = 0,
    //    [HideLabel]
    //    [LabelText("物理伤害")]
    //    Physic = 1,//2-0
    //    [LabelText("魔法伤害")]
    //    Magic = 1 << 1,//2-10
    //    [LabelText("真实伤害")]
    //    Real = 1 << 2,//2-100
    //    [LabelText("暴击伤害")]
    //    Critical = 1 << 3,//2-1000
    //    [LabelText("物理暴击伤害")]
    //    PhysicCritical = Physic | Critical,
    //} 
}