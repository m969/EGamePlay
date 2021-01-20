# EGamePlay

<img src="EGamePaly_logo.png" width="50%">

## 请用 Unity 2019.4.2 以上版本打开
---
## 知乎文章：

- [0、如何实现一个灵活、通用的战斗（技能）系统——序章](https://zhuanlan.zhihu.com/p/272216809)
- [1、如何实现一个灵活、通用的战斗（技能）系统——数值系统](https://zhuanlan.zhihu.com/p/269901872)
- [2、如何实现一个灵活、通用的战斗（技能）系统——数值系统（升级版）](https://zhuanlan.zhihu.com/p/274795206)
- [3、如何实现一个灵活、通用的战斗（技能）系统——战斗行动机制](https://zhuanlan.zhihu.com/p/272865602)
- [4、如何实现一个灵活、通用的战斗（技能）系统——战斗实体](https://zhuanlan.zhihu.com/p/284192989)
- [5、如何实现一个灵活、通用的战斗（技能）系统——能力Ability](https://zhuanlan.zhihu.com/p/292590253)
- [6、如何实现一个灵活、通用的战斗（技能）系统——Status状态效果](https://zhuanlan.zhihu.com/p/334825494)
- [7、如何实现一个灵活、通用的战斗（技能）系统——Skill技能](https://zhuanlan.zhihu.com/p/340447052)
- [8、如何实现一个灵活、通用的战斗（技能）系统——技能效果](https://zhuanlan.zhihu.com/p/341431038)

---

<img src="https://pic4.zhimg.com/v2-3e8543f56f4f9e6d678e1286409e20bb_b.webp" width="50%">

<img src="https://pic1.zhimg.com/v2-17d463886042dae07e684a5d03442dee_1440w.gif?source=172ae18b" width="50%">

<img src="https://pic4.zhimg.com/v2-6f56270edd1bb2fdda7cc02c8ad410a3_b.webp" width="50%">

***

<details>
<summary>
Numeric
</summary>
NumericFloat
</details>

<details>
<summary>
CombatEntity
</summary>
CombatEntity
</details>

<details>
<summary>
Ability
</summary>
Ability
</details>

<details>
<summary>
Status
</summary>
StatusAbility
</details>

<details>
<summary>
Skill
</summary>
SkillAbility
</details>

<details>
<summary>
Effect
</summary>
SkillEffect
</details>

<details>
<summary>
ExpressionParser
</summary>
ExpressionParser
</details>

<details>
<summary>
EffectConfigToJson
</summary>
EffectConfigToJson
</details>

---
## 基于Odin和ScriptableObject实现的灵活的技能、buff配置工具

![SkillConfigImage.png](ConfigImage.png)

---
### 该项目使用了以下收费插件：
- [Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041) （编辑器扩展、工作流改善）
- [Animancer Pro](https://assetstore.unity.com/packages/tools/animation/animancer-pro-116514) （基于Playable的简单强大的动画解决方案）

---
### 该项目的ETTask、TimerComponent及部分代码来自ET框架 [https://github.com/egametang/ET](https://github.com/egametang/ET)

---
GamePlay战斗框架技术讨论交流qq群：763696367
===
对战斗感兴趣的同学可以进群一起探讨更合适、高效的战斗框架实现

---
对EGamePlay有任何疑问或建议可以进群反馈，或是提在[Discussions](https://github.com/m969/EGamePlay/discussions)

---
## 其他类似项目
- https://github.com/KrazyL/SkillSystem-3 (Dota2 alike Skill System Implementation for KnightPhone)
- https://github.com/weichx/AbilitySystem
- https://github.com/dongweiPeng/SkillSystem (丰富的接口可便于使用扩展 完整的技能效果流程【如流程图】 配套的技能管理器 自定义的技能数据表)
- https://github.com/sjai013/UnityGameplayAbilitySystem (The approach for this is taken from that used by Unreal's Gameplay Ability System, but implemented in Unity using the Data-Oriented Technology Stack (DOTS) where possible.)
- https://github.com/dx50075/SkillSystem (skill system for unity ， 思路 http://blog.csdn.net/qq18052887/article/details/50358463
技能描述文件如下 skill(1000) //技能1 { FaceToTarget(0) PlayAnimation(1,Skill_1) Bullet(1.3,Bullet,7) PlayEffect(0,Explode8,3) })
