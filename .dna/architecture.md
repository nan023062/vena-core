# 架构设计

- **last_verified**: 2026-03-08

## 概述

Vena Core 是 Vena 框架的基础运行时库，提供引擎无关的数据结构、算法与运行时基础设施。所有上层模块可依赖本包，但本包不得反向依赖任何上层模块。

## 核心模型

| 模块 | 关键类型 | 职责 |
|------|---------|------|
| **World** | `World` `Actor` `Component` `System` `Controller` `Archetype` `InstanceID` | ECS-like 实体容器与系统调度；Attribute：`[Inject]` `[Order]` `[Require]` `[System]` |
| **BehaviourTree** | `Node` `Action` `Condition` `Composite` `Decorate` `BehaviorTree` `IBlackboard` `Expr`/`Value` | 数据驱动行为树，节点可序列化 |
| **FlowGraph** | `FlowGraph` `TaskGraph` `TaskTrack` `TaskClip` `TaskEvent` `Context` | 流程图与时间轴编排 |
| **HierarchicalFsm** | `HierarchicalFsm` | 层级状态机，支持嵌套状态 |
| **JobBalancer** | `JobBalancer` `SteppedJob` `SteppedJobChain` `PriorityQueue` | 跨帧任务均衡调度，帧时间预算控制 |
| **Math** | Vector2/3/4 · Matrix/Matrix2D · Quaternion · AABB · Ray · Rectangle · Point · Hierarchy · Transformation · MathHelper | 引擎无关高性能数学库 |
| **Pool** | `TemplateObjectPool<T>` `PoolObject` `PoolObjectReference` `WeakArray/List/Map/HashSet` | 对象池 + Weak 引用容器，避免强引用泄漏 |
| **Serialization** | `ByteReader/Writer` `Serializer` `ISerializeable` `Property/Set/s` `Expr/Factory/Tree` | 二进制序列化 + 属性系统 + 表达式树 |
| **Profiler** | `GcWatch` `ProfilerWatch` `TimeWatch` | GC 监控与性能计时 |
| **Collections** | `FastList` `ArrayBasedLinkList` `NonAllocLinkList` `LinkedStack` `SafeMap` `ListExtensions` | 高性能无分配集合 |
| **Debug** | `Debug` | 统一调试输出 |

## 程序集边界

- **对外暴露**：上述所有模块的公共类型与接口（命名空间 `Vena.Core.*`）
- **职责范围**：引擎无关的基础设施；不含游戏业务逻辑、渲染、资产加载、UI、网络
- **交互方式**：上层模块直接引用 `Vena.Core` 程序集；Core 永不反向依赖
- **边界约束**：禁止在 Core 内引用 `UnityEngine`（Math 模块除外可使用 UnityEngine.Mathf 等价自实现）；禁止游戏业务逻辑入侵

## Public API

- `World.CreateActor()` — 创建实体
- `World.Query<T>()` — 按 Component 类型查询 Actor
- `World.RegisterSystem<T>()` — 注册 System
- `World.Initialize() / Tick() / Dispose()` — 生命周期
- `Actor.Add<T>(component)` / `Actor.Get<T>()` / `Actor.Remove<T>()` — Component 操作
- `JobBalancer.Schedule(job)` / `JobBalancer.Tick()` — 跨帧任务调度
- `TemplateObjectPool<T>.Rent()` / `.Return(obj)` — 对象池借还
- `ByteReader` / `ByteWriter` — 二进制序列化读写
- `IBlackboard` — 行为树黑板接口
- `FlowGraph` / `TaskGraph` — 流程图入口

## 设计目标

- 提供高性能、零 GC 数据结构与工具（热路径零分配）
- 统一运行时抽象（ECS-like World、生命周期、依赖注入）
- 为上层提供可复用能力（行为树、流程图、FSM、序列化、数学库）
- **非目标**：不含游戏业务逻辑；不依赖 Unity 引擎特定 API

## 性能约束

- **GC 约束**：热路径零分配；Collections / Pool / Math 模块严格无分配
- **帧预算**：JobBalancer 帧内预算由调用方通过 `budgetMs` 参数声明，Core 自身不超出
- **内存预算**：无全局常驻上限，Pool 容量由上层配置

## 设计决策

| 决策 | 原因 | 权衡 |
|------|------|------|
| 自定义数学库而非 `UnityEngine.Mathf` | 保持引擎无关可移植性 | 维护成本增加，需与 Unity 类型手动互转 |
| ECS-like Archetype 模式 | 兼顾查询性能与内存布局 | 比纯 OOP 复杂，学习曲线更陡 |
| 数据驱动行为树 & 流程图 | 节点可序列化、编辑器友好 | 纯代码驱动时样板代码较多 |
| Weak 引用容器池 | 避免强引用内存泄漏 | 访问前需判空，API 略繁琐 |
| 二进制 + 表达式树双模式序列化 | 覆盖网络传输与持久化两种场景 | 两套序列化路径需同步维护 |
