# Vena Core

[![License](https://img.shields.io/badge/license-MulanPSL--2.0-blue)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity-2020.3%2B-black)](https://unity.com)
[![Version](https://img.shields.io/badge/version-0.1.0-green)](CHANGELOG.md)
[![GitHub](https://img.shields.io/badge/GitHub-vena--core-181717?logo=github)](https://github.com/nan023062/vena-core)

Vena 框架的核心运行时基础库，提供与引擎无关的数据结构、算法与运行时基础设施。

> [English Version](README.en.md)

---

## 功能模块

| 模块 | 说明 |
|---|---|
| **World (ECS-like)** | Actor / Component / System / Archetype，支持依赖注入与执行顺序声明 |
| **BehaviourTree** | 行为树，支持 Sequence / Selector / Decorator，数据驱动节点 |
| **FlowGraph** | 流程图与时间轴编排（TaskGraph / TaskTrack / TaskClip） |
| **HierarchicalFsm** | 层级状态机，支持嵌套状态 |
| **JobBalancer** | 跨帧任务均衡调度，PriorityQueue + SteppedJob |
| **Math** | 高性能数学库（Vector2/3/4、Matrix、Quaternion、AABB、Ray 等） |
| **Pool** | 对象池 + Weak 引用容器系列（WeakArray / WeakList / WeakMap 等） |
| **Serialization** | 二进制序列化（ByteReader/Writer）+ 属性系统 + 表达式树 |
| **Profiler** | GC 监控、性能采样、计时器 |
| **Collections** | 高性能集合（FastList、NonAllocLinkList、SafeMap 等） |
| **Debug** | 统一调试输出 |

---

## 安装

### 方式一：通过 Git URL（推荐）

在 Unity 编辑器中打开 **Window → Package Manager → + → Add package from git URL**，输入：

```
https://github.com/nan023062/vena-core.git
```

或在 `Packages/manifest.json` 中直接添加：

```json
{
  "dependencies": {
    "com.vena.core": "https://github.com/nan023062/vena-core.git"
  }
}
```

### 方式二：本地磁盘引用

```json
{
  "dependencies": {
    "com.vena.core": "file:../path/to/vena.core"
  }
}
```

---

## 快速上手

### World / ECS-like

```csharp
// 声明组件
public class PositionComponent : Component
{
    public float X, Y;
}

// 声明系统
[System]
[Order(100)]
public class MoveSystem : Vena.Core.System
{
    public override void Update(World world)
    {
        foreach (var actor in world.Query<PositionComponent>())
        {
            var pos = actor.Get<PositionComponent>();
            pos.X += 1f * Time.deltaTime;
        }
    }
}

// 启动 World
var world = new World();
world.RegisterSystem<MoveSystem>();
var actor = world.CreateActor();
actor.Add(new PositionComponent());
world.Initialize();
```

### JobBalancer（跨帧任务）

```csharp
var balancer = new JobBalancer(budgetMs: 2f);
balancer.Schedule(new SteppedJob(() =>
{
    // 分步执行的耗时任务
}));
// 每帧调用，自动在预算内执行
balancer.Tick();
```

### 对象池

```csharp
var pool = new TemplateObjectPool<MyObject>(() => new MyObject());
var obj = pool.Rent();
// 使用 obj...
pool.Return(obj);
```

---

## 文档

- [架构设计](docs/architecture.md)：系统架构、边界与核心模型
- [集成指南](docs/usage-guide.md)：API 概览与集成说明
- [Changelog](CHANGELOG.md)：版本变更记录
- [贡献指南](CONTRIBUTING.md)：如何参与贡献

---

## 设计原则

- **引擎无关**：核心逻辑不依赖 UnityEngine 特定 API，可移植到其他 .NET 环境
- **零 GC 热路径**：所有高频调用路径使用无分配设计
- **单向依赖**：Core 不引用任何上层模块，上层模块可自由引用 Core

---

## License

本项目使用 [木兰宽松许可证 v2 (MulanPSL-2.0)](LICENSE)。

---

## AI 记忆索引（MCP Index）

> 供 AI Agent 快速定位上下文，无需阅读此段。

| 文件 | 用途 |
|------|------|
| `.dna/architecture.md` | 程序集定位、核心模型、Public API、设计决策 |
| `.dna/pitfalls.md` | 历史踩坑记录 |
| `.dna/changelog.md` | 版本变更记录 |
| `.dna/dependencies.md` | 依赖声明（本包无外部依赖） |
| `.dna/wip.md` | 进行中任务 |
