# Vena Core

[![License](https://img.shields.io/badge/license-MulanPSL--2.0-blue)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity-2020.3%2B-black)](https://unity.com)
[![Version](https://img.shields.io/badge/version-0.1.0-green)](CHANGELOG.md)
[![GitHub](https://img.shields.io/badge/GitHub-vena--core-181717?logo=github)](https://github.com/nan023062/vena-core)

Core runtime foundation library for the Vena framework — engine-agnostic data structures, algorithms, and runtime infrastructure.

> [中文版本](README.md)

---

## Modules

| Module | Description |
|---|---|
| **World (ECS-like)** | Actor / Component / System / Archetype with dependency injection and execution order |
| **BehaviourTree** | Behaviour tree with Sequence / Selector / Decorator, data-driven nodes |
| **FlowGraph** | Flow graph and timeline orchestration (TaskGraph / TaskTrack / TaskClip) |
| **HierarchicalFsm** | Hierarchical finite state machine with nested state support |
| **JobBalancer** | Cross-frame job scheduling with PriorityQueue + SteppedJob |
| **Math** | High-performance math library (Vector2/3/4, Matrix, Quaternion, AABB, Ray, etc.) |
| **Pool** | Object pool + weak-reference containers (WeakArray / WeakList / WeakMap, etc.) |
| **Serialization** | Binary serialization (ByteReader/Writer) + property system + expression tree |
| **Profiler** | GC watch, profiler sampling, time watch |
| **Collections** | High-performance collections (FastList, NonAllocLinkList, SafeMap, etc.) |
| **Debug** | Unified debug output |

---

## Installation

### Option 1: Via Git URL (Recommended)

In Unity Editor, open **Window → Package Manager → + → Add package from git URL** and enter:

```
https://github.com/nan023062/vena-core.git
```

Or add directly to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.vena.core": "https://github.com/nan023062/vena-core.git"
  }
}
```

### Option 2: Local Disk Reference

```json
{
  "dependencies": {
    "com.vena.core": "file:../path/to/vena.core"
  }
}
```

---

## Quick Start

### World / ECS-like

```csharp
// Define a component
public class PositionComponent : Component
{
    public float X, Y;
}

// Define a system
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

// Bootstrap
var world = new World();
world.RegisterSystem<MoveSystem>();
var actor = world.CreateActor();
actor.Add(new PositionComponent());
world.Initialize();
```

### JobBalancer (Cross-frame tasks)

```csharp
var balancer = new JobBalancer(budgetMs: 2f);
balancer.Schedule(new SteppedJob(() =>
{
    // Expensive work split across frames
}));
// Call every frame — executes within budget
balancer.Tick();
```

### Object Pool

```csharp
var pool = new TemplateObjectPool<MyObject>(() => new MyObject());
var obj = pool.Rent();
// Use obj...
pool.Return(obj);
```

---

## Documentation

- [Architecture](docs/architecture.md): System architecture, boundaries, and core models
- [Usage Guide](docs/usage-guide.md): API overview and integration instructions
- [Changelog](CHANGELOG.md): Version history
- [Contributing](CONTRIBUTING.md): How to contribute

---

## Design Principles

- **Engine-agnostic**: Core logic avoids Unity-specific APIs, portable to other .NET environments
- **Zero-GC hot paths**: All high-frequency call paths use allocation-free design
- **One-way dependency**: Core never references upper-layer modules; upper layers freely reference Core

---

## License

This project is licensed under [Mulan Permissive Software License v2 (MulanPSL-2.0)](LICENSE).
