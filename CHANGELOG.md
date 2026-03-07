# Changelog

All notable changes to Vena Core will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [0.1.0] - 2024-01-01

### Added
- **World (ECS-like)**: Actor / Component / System / Archetype 架构，支持 `[Inject]`、`[Order]`、`[Require]`、`[System]` Attribute
- **BehaviourTree**: 行为树节点体系（Action / Condition / Composite / Decorate），IBlackboard 数据共享
- **FlowGraph**: 流程图容器（FlowGraph / TaskGraph / TaskTrack / TaskClip / TaskEvent / Context）
- **HierarchicalFsm**: 层级状态机
- **JobBalancer**: 跨帧任务均衡调度（JobBalancer / SteppedJob / SteppedJobChain / PriorityQueue）
- **Math**: 高性能数学库（Vector2/3/4、Matrix/Matrix2D、Quaternion、AABB、Ray、Rectangle、Point、Hierarchy、Transformation、MathHelper）
- **Pool**: 对象池（TemplateObjectPool / PoolObject / PoolObjectReference）及 Weak 引用容器系列
- **Serialization**: 二进制序列化（ByteReader/Writer / Serializer / ISerializeable）、属性系统（Property / PropertySet / Properties）、表达式树（Expr / ExprFactory / ExprTree）
- **Profiler**: GcWatch / ProfilerWatch / TimeWatch
- **Collections**: FastList / ArrayBasedLinkList / NonAllocLinkList / LinkedStack / SafeMap / ListExtensions
- **Debug**: 统一调试输出工具
