# ***EGamePlay***

<img src="Readme/EGamePaly_logo.png" width="60%">

---

# 3.0版本
1、3.0版本技能配置数据结构没有变动，主要是运行时的业务代码大重构

2、引入了EcsNode框架，引入System概念，基于EcsNode的 实体-组件-系统 模式重构了运行时代码，将逻辑和数据分离，将业务逻辑和视图逻辑分离

3、去掉了回合制demo，仅保留Rpg demo示例

# 2.0版本在2.0分支

# 1.0版本在1.0分支

---
[![Unity Version: 2022.3.53f1](https://img.shields.io/badge/Unity-2022.3.53f1-333333.svg?logo=unity)](https://unity3d.com/get-unity/download/archive) [![Status: Work-in-progress](https://img.shields.io/badge/status-work--in--progress-orange)](https://github.com/m969/EGamePlay/projects/1)

## Stargazers over time

[![Stargazers over time](https://starchart.cc/m969/EGamePlay.svg)](https://starchart.cc/m969/EGamePlay)

---
# 文档：
- [EGamePlay文档(wiki)](https://github.com/m969/EGamePlay/wiki)

# 商业项目
- [暗黑之地](https://www.taptap.cn/app/227372) https://www.taptap.cn/app/227372 一个人开发，用EGP重写了整个战斗后重新上线

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
- [9、如何实现一个灵活、通用的战斗（技能）系统——Entity-Component模式](https://zhuanlan.zhihu.com/p/343624199)
---

## demo运行
- rpg demo，运行RpgExample Scene场景
- 技能调试编辑，运行ExecutionLinkScene场景

---
## 如何制作一个简单的技能
- 首先在AbilityConfig.xlsx表里添加一个技能，给定技能id，配置参数
- 在Project面板右键选择 ```能力/能力配置``` 创建对应id的技能配置，配置效果
- 在Project面板右键选择 ```能力/Execution``` 创建对应id的技能执行体，配置片段表现
- 最后就是运行时将技能挂载到CombatEntity上，再通过施法组件SpellComponent释放技能即可

---

## 该项目使用了以下收费插件：
- [DOTween Pro](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416) （简单易用强大的动画插件）
- [Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041) （编辑器扩展、工作流改善）
- [Animancer Pro](https://assetstore.unity.com/packages/tools/animation/animancer-pro-116514) （基于Playable的简单强大的Animation解决方案）
---

## 如何将EGamePlay移植到别的Unity工程
如果要将EGamePlay移植到自己的Unity工程里，首先要确保Odin Inspector插件已经在工程中，并加上UNITY条件编译，最后将以下目录复制过去即可：
- /Assets/Gizmos
- /Assets/Game.Model
- /Assets/Game.System
- /Assets/Game.ThirdParty
- /Assets/Unity.EditorScripts/Editor
- /Assets/Unity.Scripts/UnityMono/EGamePlay
- /Assets/Plugins/Editor/npoi
- /Excel
- 目前还不是完美接入，接入过程中会有冲突或者缺失，需要按需处理

---
## 如何将EGamePlay移植到ET框架里(以ET8.1版本为例)
- 把 /Assets/Unity.EditorScripts/Editor 移到ET.Editor程序集下
- 把 /Assets/Unity.Scripts/UnityMono/EGamePlay 移到ET.Loader程序集下
- 把 /Assets/Game.Model 里的业务代码移到Model程序集下
- 把 /Assets/Game.Model 里的视图代码移到ModelView程序集下
- 把 /Assets/Game.System 里的业务代码移到Hotfix程序集下
- 把 /Assets/Game.System 里的视图代码移到HotfixView程序集下
- 把 /Assets/Game.ThirdParty 移到ThirdParty程序集下
- 最后要添加条件编译EGAMEPLAY_ET
- ETHelper文件夹里是老版本的ET代码，会和ET框架的代码有冲突，可以整个删掉，改成用原ET框架的代码，配置表流程也需要改成原ET框架的流程
- 目前还不是完美接入，接入过程中会有冲突或者缺失，需要按需处理

## EGamePlay demo示意图
---

- rpg demo，运行RpgExample Scene场景

<img src="Readme/EGamePlay.gif" width="60%">

- 回合制demo，运行TurnBaseExample Scene场景（在2.0和1.0分支）

<img src="Readme/EGamePlayTurn.gif" width="60%">

- 技能调试编辑，运行ExecutionLinkScene场景

<img src="Readme/EGamePlayExecutionLink2.gif" width="90%">

***

---
## 基于Odin和ScriptableObject实现的灵活的技能、buff配置工具

- 可使用菜单栏：```Assets/Create/能力/能力配置``` 创建能力效果配置
<img src="Readme/AbilityObjectSkill2.png" width="50%">

- 可使用菜单栏：```Tools/EGamePlay/SkillEditorWindow``` 打开SkillEditorWindow界面
<img src="Readme/SkillEditorWindow.png" width="100%">

### 技能Excel配置
- 可使用菜单栏：```Tools/导出配置``` 生成json和配置类
![AbilityConfigExcel.png](Readme/AbilityConfigExcel.png)

---
## EGamePlay战斗框架技术讨论交流qq群：763696367
对战斗感兴趣的同学可以进群一起探讨更合适、高效的战斗框架实现
<br>对EGamePlay有任何疑问或建议可以进群反馈，或是提在[Discussions](https://github.com/m969/EGamePlay/discussions)

---
## 其他类似项目
- https://github.com/KrazyL/SkillSystem-3 (Dota2 alike Skill System Implementation for KnightPhone)
- https://github.com/weichx/AbilitySystem
- https://github.com/dongweiPeng/SkillSystem (丰富的接口可便于使用扩展 完整的技能效果流程【如流程图】 配套的技能管理器 自定义的技能数据表)
- https://github.com/sjai013/UnityGameplayAbilitySystem (The approach for this is taken from that used by Unreal's Gameplay Ability System, but implemented in Unity using the Data-Oriented Technology Stack (DOTS) where possible.)
- https://github.com/dx50075/SkillSystem (skill system for unity ， 思路 http://blog.csdn.net/qq18052887/article/details/50358463
技能描述文件如下 skill(1000) //技能1 { FaceToTarget(0) PlayAnimation(1,Skill_1) Bullet(1.3,Bullet,7) PlayEffect(0,Explode8,3) })

---
## 包含
- https://github.com/m969/EcsNode

---
## 参考
- https://github.com/egametang/ET
