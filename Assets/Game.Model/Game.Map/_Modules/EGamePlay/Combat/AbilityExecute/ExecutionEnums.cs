#if !SERVER
using Sirenix.OdinInspector;
#endif

[LabelText("碰撞体形状")]
public enum CollisionShape
{
    [LabelText("圆形")]
    Sphere,
    [LabelText("矩形")]
    Box,
    [LabelText("扇形")]
    Sector,
    [LabelText("自定义")]
    Custom,
}

[LabelText("移动类型")]
public enum CollisionMoveType
{
    [LabelText("可选位置碰撞体")]
    SelectedPosition,
    [LabelText("可选朝向碰撞体")]
    SelectedDirection,
    [LabelText("目标飞行碰撞体")]
    TargetFly,
    [LabelText("朝向飞行碰撞体")]
    ForwardFly,
    [LabelText("路径飞行碰撞体")]
    PathFly,
    [LabelText("可选朝向路径飞行")]
    SelectedDirectionPathFly,
    [LabelText("固定位置碰撞体")]
    FixedPosition,
}

[LabelText("路径中轴点")]
public enum PathExecutePoint
{
    [LabelText("以执行体坐标加偏移为中轴点")]
    EntityOffset = 10,
    [LabelText("以输入坐标加偏移为中轴点")]
    InputPoint = 20
}

public enum ExecuteTriggerType
{
    [LabelText("全部主动触发")]
    AllTriggers,
    [LabelText("主动触发1")]
    Trigger1,
    [LabelText("主动触发2")]
    Trigger2,
    [LabelText("主动触发3")]
    Trigger3,

    [LabelText("其他")]
    Other = 100,
}

//[LabelText("应用效果")]
public enum EffectApplyType
{
    [LabelText("全部效果")]
    AllEffects,
    [LabelText("效果1")]
    Effect1,
    [LabelText("效果2")]
    Effect2,
    [LabelText("效果3")]
    Effect3,
    [LabelText("效果4")]
    Effect4,

    [LabelText("其他")]
    Other = 100,
}

[LabelText("应用目标")]
public enum EffectApplyTarget
{
    [LabelText("(空)")]
    None,
    Self,
    Owner,
    Parent,

    [LabelText("其他")]
    Other = 100,
}