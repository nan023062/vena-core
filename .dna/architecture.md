# 架构设计

> 编写指南：总体保持 50–150 行。每段回答核心问题，不复制代码。

<!-- ═══ 元数据 ═══ -->
- **last_verified**: 2026-03-07

---

## 概述

Vena Core 是整个 Vena 框架的基础运行时库，提供与引擎无关的核心数据结构、算法与运行时基础设施。所有上层模块均可引用此包，但此包不得引用任何上层模块。

- **维护者**：<!-- 单人项目可省略 -->
- **boundary**：hard

---

## 核心模型

### World / ECS-like
- `World`：实体容器与系统调度器
- `Actor`：实体抽象，持有 Component 集合
- `Component`：纯数据组件；`System`：无状态逻辑处理器
- `Archetype`：组件组合原型；`InstanceID`：全局唯一实例标识
- Attribute：`[Inject]` `[Order]` `[Require]` `[System]`

### BehaviourTree（行为树）
- `BehaviorTree`：驱动入口；`Node` 基类派生 Action / Condition / Composite / Decorate
- `IBlackboard`：数据共享接口；`Expr`/`Value`：表达式与值抽象

### FlowGraph（流程图）
- `FlowGraph` / `TaskGraph`：任务编排容器
- `TaskTrack` / `TaskClip` / `TaskEvent`：时间轴编排；`Context`：执行上下文

### HierarchicalFsm（层级状态机）
- 支持嵌套的有限状态机，无第三方依赖

### JobBalance（任务均衡器）
- `JobBalancer`：跨帧任务调度；`SteppedJob` / `SteppedJobChain`：分步执行
- `PriorityQueue`：优先级队列

### Math（数学库）
- Vector2/3/4、Matrix/Matrix2D、Quaternion、AABB、Ray、Rectangle、Point
- Hierarchy（层级变换）、Transformation、MathHelper

### Pool（对象池）
- `TemplateObjectPool`；Weak 系列容器：WeakArray / WeakHashSet / WeakList / WeakMap

### Serialization（序列化）
- `ByteReader` / `ByteWriter`：二进制读写；`Serializer` / `ISerializeable`
- `Property` / `PropertySet` / `Properties`；`Expr` / `ExprFactory` / `ExprTree`

### Collections / Debug / Profiler
- 扩展集合；统一调试输出；GcWatch / ProfilerWatch / TimeWatch

---

## 程序集边界

- **对外暴露**：所有命名空间下的 public 类型与接口（见 Public API）
- **职责范围**：仅提供与引擎无关的基础设施；不含渲染、资源加载、UI、网络
- **交互方式**：上层模块直接引用 `Vena.Core` 程序集；Core 不反向依赖任何上层
- **边界约束**：禁止在 Core 中引入对游戏业务或特定引擎 API 的依赖

---

## Public API

- `World.CreateActor()` / `World.Tick()` — ECS 实体生命周期
- `Actor.AddComponent<T>()` / `Actor.GetComponent<T>()` — 组件管理
- `BehaviorTree.Tick(blackboard)` — 行为树驱动
- `FlowGraph.Execute(context)` — 流程图执行
- `JobBalancer.Schedule(job)` / `JobBalancer.Tick()` — 跨帧任务调度
- `TemplateObjectPool<T>.Get()` / `.Release()` — 对象池
- `ByteWriter` / `ByteReader` — 二进制序列化
- `IBlackboard` — 行为树数据共享接口

---

## 设计目标

- 提供高性能、零 GC 的基础数据结构与工具集
- 统一运行时基础抽象（ECS-like World、生命周期、依赖注入）
- 为上层模块提供行为树、流程图、状态机、序列化、数学库等能力
- 非目标：不包含任何游戏业务逻辑；不依赖 Unity 引擎特定 API

---

## 性能约束

- **GC 约束**：热路径（BehaviorTree.Tick、JobBalancer.Tick）零分配
- **帧预算**：未单独声明，由上层调用方根据场景设定
- **内存预算**：对象池上限由使用方配置

---

## 设计决策

- 自实现数学库而非依赖 UnityEngine.Mathf → 保持引擎无关性
- ECS-like 采用 Archetype 模式 → 兼顾查询性能与内存布局
- 行为树与流程图数据驱动 → 节点可序列化，支持运行时热替换
- Weak 引用系列容器 → 避免强引用导致的内存泄漏
