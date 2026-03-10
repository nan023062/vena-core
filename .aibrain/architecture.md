# 架构设计

## 概述
Vena Core 是整个 Vena 框架的基础运行时库，提供与引擎无关的核心数据结构、算法与运行时基础设施。所有上层模块均可引用此包，但此包不得引用任何上层模块。

## 设计目标
- 提供高性能、零 GC 的基础数据结构与工具集
- 统一运行时基础抽象（ECS-like World、生命周期、依赖注入）
- 为上层模块提供可复用的行为树、流程图、状态机、序列化、数学库等能力
- 非目标：不包含任何游戏业务逻辑；不依赖 Unity 引擎特定 API（尽量保持引擎无关）

## 核心模型

### World / ECS-like 架构
- `World`：实体容器与系统调度器
- `Actor`：实体抽象，持有 Component 集合
- `Component`：纯数据组件
- `System`：无状态逻辑处理器，通过 Attribute 声明依赖与执行顺序
- `Controller`：有状态逻辑控制器
- `Archetype`：组件组合原型，用于高效查询
- `InstanceID`：全局唯一实例标识
- Attribute：`[Inject]`、`[Order]`、`[Require]`、`[System]`
- LifeCycle：`ILifeCycle`、`PairwiseLifeCycle`

### BehaviourTree（行为树）
- `Node`：节点基类
- `Action`：叶节点，执行具体行为
- `Condition`：条件判断节点
- `Composite`：组合节点（Sequence / Selector 等）
- `Decorate`：装饰器节点
- `BehaviorTree`：树的入口与驱动
- `IBlackboard`：数据共享接口
- `Expr` / `Value`：表达式与值抽象

### FlowGraph（流程图）
- `FlowGraph`：流程图容器
- `TaskGraph`：任务编排图
- `TaskTrack` / `TaskClip` / `TaskEvent`：时间轴编排
- `Context`：执行上下文

### HierarchicalFsm（层级状态机）
- 支持嵌套的有限状态机

### JobBalance（任务均衡器）
- `JobBalancer`：跨帧任务调度
- `SteppedJob` / `SteppedJobChain`：分步执行任务
- `PriorityQueue`：优先级队列

### Math（数学库）
- 向量（Vector2 / Vector3 / Vector4）、矩阵（Matrix / Matrix2D）、四元数（Quaternion）
- AABB、Ray、Rectangle、Point
- Hierarchy（层级变换）、Transformation、MathHelper

### Pool（对象池）
- `TemplateObjectPool`：模板对象池
- `PoolObject` / `PoolObjectReference`：池化对象与引用
- Weak 系列容器：`WeakArray`、`WeakHashSet`、`WeakList`、`WeakMap`

### Profiler（性能分析）
- `GcWatch`：GC 分配监控
- `ProfilerWatch`：性能采样
- `TimeWatch`：计时器

### Serialization（序列化）
- `ByteReader` / `ByteWriter`：二进制读写
- `Serializer`：序列化器
- `ISerializeable`：可序列化接口
- `Property` / `PropertySet` / `Properties`：属性系统
- `Expr` / `ExprFactory` / `ExprTree`：表达式树
- `NetworkSequenceReader`：网络序列读取
- `SimpleList`、`ConvertHelper`、`Utils`、`Value`

### Collections（集合）
- 扩展集合类型

### Debug（调试）
- 统一调试输出工具

## 模块边界
- **对外暴露**：上述所有命名空间的公共类型与接口
- **职责范围**：仅提供与引擎无关的基础设施；不包含渲染、资源加载、UI、网络等上层功能
- **交互方式**：上层模块通过直接引用 Vena.Core 程序集使用；Core 不反向依赖任何上层模块
- **边界约束**：禁止在 Core 中引入对游戏业务或特定引擎 API 的依赖

## 性能约束
<!-- 声明本程序集的性能预算。未声明的项视为无特殊约束。
     AI 实现代码时必须在此预算内工作，超标须在设计决策中说明原因。 -->
- **帧预算**：关键方法的每帧耗时上限（如 `UpdateSpot ≤ 2ms`）
- **内存预算**：常驻内存上限 / 对象池容量上限（如 `Chunk 池 ≤ 256`）
- **GC 约束**：热路径零分配 | 允许每帧 N 字节 | 无特殊要求
- **批处理**：单帧处理上限（如 `每帧最多加载 2 个 Chunk`）

## 设计决策
- 选择自实现数学库而非依赖 UnityEngine.Mathf，以保持引擎无关性
- ECS-like 架构采用 Archetype 模式，兼顾查询性能与内存布局
- 行为树与流程图采用数据驱动设计，节点可序列化
- 对象池使用 Weak 引用系列容器，避免强引用导致的内存泄漏
- 序列化层支持二进制与表达式树两种模式，适配网络传输与持久化场景
