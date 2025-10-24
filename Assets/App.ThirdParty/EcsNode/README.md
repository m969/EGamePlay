# EcsNode框架

EcsNode是一个基于Unity的Entity Component System (ECS)框架，具有树形结构的设计，适用于复杂游戏逻辑的组织和管理。这个框架能够有效地分离游戏逻辑和数据，提高代码的可维护性和性能。

## 核心概念

EcsNode框架基于三个主要概念：

1. **实体(Entity)** - 游戏中的基本对象，可以拥有组件，形成树形结构
2. **组件(Component)** - 挂载在实体上的数据容器
3. **系统(System)** - 处理特定类型实体或组件的逻辑

### 特点

- **树形结构** - 实体可以有父子关系，便于管理层级关系
- **组件分离** - 组件只包含数据，不包含逻辑
- **系统驱动** - 系统处理实体和组件的逻辑，通过生命周期接口驱动
- **事件系统** - 支持异步事件处理机制
- **ID生成** - 内置高效的ID生成算法

## 快速开始

### 1. 定义组件

组件是纯数据容器，继承自`EcsComponent`：

```csharp
public class HealthComponent : EcsComponent
{
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
}

public class MovementComponent : EcsComponent
{
    public float Speed { get; set; }
    public Vector3 Direction { get; set; }
}
```

### 2. 定义实体

实体继承自`EcsEntity`，可以添加特定属性：

```csharp
public class PlayerEntity : EcsEntity
{
    public string PlayerName { get; set; }
}

public class EnemyEntity : EcsEntity
{
    public int EnemyType { get; set; }
}
```

### 3. 定义系统

系统分为实体系统和组件系统，实现相应的生命周期接口：

#### 实体系统

```csharp
// 处理玩家实体的更新
public class PlayerUpdateSystem : AEntitySystem<PlayerEntity>, IUpdate<PlayerEntity>
{
    public void Update(PlayerEntity entity)
    {
        // 每帧处理玩家逻辑
        Debug.Log($"更新玩家 {entity.PlayerName}");
    }
}
```

#### 组件系统

```csharp
// 处理健康组件的初始化
public class HealthSystem : AComponentSystem<EcsEntity, HealthComponent>, IAwake<EcsEntity, HealthComponent>
{
    public void Awake(EcsEntity entity, HealthComponent component)
    {
        // 初始化健康组件
        component.MaxHealth = 100;
        component.CurrentHealth = 100;
    }
}
```

### 4. 初始化和使用

在MonoBehaviour中初始化和使用EcsNode：

```csharp
public class GameManager : MonoBehaviour
{
    private EcsNode _ecsNode;
    
    void Start()
    {
        // 创建ECS节点（根实体）
        _ecsNode = new EcsNode(1); // 使用索引1
        
        // 注册系统接口和系统
        _ecsNode.RegisterDrives(AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsInterface && typeof(IDrive).IsAssignableFrom(t))
            .ToArray());

        _ecsNode.RegisterSystems(AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .ToArray());
            
        // 创建玩家实体
        var player = _ecsNode.AddChild<PlayerEntity>(p => {
            p.PlayerName = "Player1";
        });
        
        // 添加组件到玩家
        player.AddComponent<HealthComponent>();
        player.AddComponent<MovementComponent>();
        
        // 初始化所有实体
        player.Init();
        
        // 启用实体
        player.Enable = true;
    }
    
    void Update()
    {
        // 驱动实体更新
        _ecsNode.DriveEntityUpdate();
    }
    
    void FixedUpdate()
    {
        // 驱动实体固定更新
        _ecsNode.DriveEntityFixedUpdate();
    }
}
```

## 生命周期接口

EcsNode框架提供了多种生命周期接口来处理实体和组件：

- **IAwake** - 创建时调用
- **IInit** - 初始化时调用
- **IEnable** - 启用时调用
- **IDisable** - 禁用时调用
- **IUpdate** - 每帧更新时调用
- **IFixedUpdate** - 每个物理帧调用
- **IDestroy** - 销毁时调用
- **IOnChange** - 当组件数据变化时调用

示例：

```csharp
// 实现多个生命周期接口
public class MovementSystem : AComponentSystem<PlayerEntity, MovementComponent>, 
    IAwake<PlayerEntity, MovementComponent>, 
    IUpdate<PlayerEntity>
{
    public void Awake(PlayerEntity entity, MovementComponent component)
    {
        // 初始化逻辑
    }

    public void Update(PlayerEntity entity)
    {
        // 更新逻辑
    }
}
```

## 事件系统

EcsNode框架支持异步事件处理：

### 1. 定义事件实体

```csharp
public class DamageEvent : EcsEntity
{
    public int DamageAmount { get; set; }
    public EcsEntity Target { get; set; }
}
```

### 2. 定义事件处理器

```csharp
public class DamageEventHandler : AEventRun<DamageEvent>
{
    public override EcsNode EcsNode { get; set; }
    
    protected override async ETTask Run(DamageEvent damageEvent)
    {
        // 处理伤害事件
        var healthComponent = damageEvent.Target.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.CurrentHealth -= damageEvent.DamageAmount;
        }
        
        await ETTask.CompletedTask;
    }
}
```

### 3. 触发和处理事件

```csharp
// 初始化事件处理器
var damageHandler = new DamageEventHandler();
damageHandler.EcsNode = _ecsNode;

// 创建并触发事件
public async void TriggerDamageEvent(int amount)
{
    var damageEvent = _ecsNode.AddChild<DamageEvent>();
    damageEvent.DamageAmount = amount;
    damageEvent.Target = player;
    
    await damageHandler.Handle(damageEvent);
    _ecsNode.RemoveChild(damageEvent);
}
```

## 高级功能

### 实体树形结构

EcsNode框架支持实体之间的父子关系：

```csharp
// 添加子实体
var weapon = player.AddChild<WeaponEntity>();

// 获取子实体
var childWeapon = player.GetChild<WeaponEntity>(weaponId);
```

### 组件访问

实体可以方便地管理组件：

```csharp
// 添加组件
var health = entity.AddComponent<HealthComponent>();

// 获取组件
var movement = entity.GetComponent<MovementComponent>();

// 移除组件
entity.RemoveComponent<HealthComponent>();
```

### 实体和组件的启用/禁用

```csharp
// 启用/禁用实体
entity.Enable = true;  // 将触发IEnable接口
entity.Enable = false; // 将触发IDisable接口

// 组件也有相同的启用/禁用机制
var component = entity.GetComponent<MovementComponent>();
component.Enable = false;
```

## 最佳实践

1. **保持组件纯数据** - 组件应该只包含数据，不包含逻辑
2. **系统负责逻辑** - 所有游戏逻辑应在系统中实现
3. **适当使用事件** - 对于不同系统间的通信，使用事件系统
4. **利用树形结构** - 利用实体的树形结构组织游戏对象层次
5. **合理拆分组件** - 组件应该具有单一职责，便于重用

## 注意事项

- 在实现系统时，确保实现正确的接口以响应生命周期事件
- 处理大量实体时，可以考虑实现自定义过滤器优化性能
- 使用异步事件时，确保正确处理异常
- 在Unity编辑器中，可以创建自定义工具来可视化ECS结构

## 示例项目

可以参考`Example.cs`和`EventExample.cs`了解更多使用示例。 