# Architecture

## Overview

Vena Core is the foundation runtime library of the Vena framework. It provides engine-agnostic core data structures, algorithms, and runtime infrastructure. All upper-layer modules may depend on this package, but this package must not depend on any upper-layer module.

## Design Goals

- Provide high-performance, zero-GC data structures and utilities
- Unify runtime abstractions (ECS-like World, lifecycle, dependency injection)
- Offer reusable capabilities (BehaviourTree, FlowGraph, FSM, serialization, math) for upper layers
- **Non-goal**: no game business logic; no Unity-engine-specific API dependency (engine-agnostic where possible)

---

## Core Models

### World / ECS-like

| Type | Role |
|---|---|
| `World` | Entity container and system scheduler |
| `Actor` | Entity abstraction, holds a collection of Components |
| `Component` | Pure data component |
| `System` | Stateless logic processor; declares dependencies and execution order via Attributes |
| `Controller` | Stateful logic controller |
| `Archetype` | Component combination prototype for efficient queries |
| `InstanceID` | Globally unique instance identifier |

**Attributes**: `[Inject]` `[Order]` `[Require]` `[System]`  
**Lifecycle**: `ILifeCycle` `PairwiseLifeCycle`

### BehaviourTree

| Type | Role |
|---|---|
| `Node` | Base node class |
| `Action` | Leaf node, executes concrete behavior |
| `Condition` | Condition check node |
| `Composite` | Composite node (Sequence / Selector, etc.) |
| `Decorate` | Decorator node |
| `BehaviorTree` | Tree entry point and driver |
| `IBlackboard` | Shared data interface |
| `Expr` / `Value` | Expression and value abstractions |

### FlowGraph

| Type | Role |
|---|---|
| `FlowGraph` | Flow graph container |
| `TaskGraph` | Task orchestration graph |
| `TaskTrack` / `TaskClip` / `TaskEvent` | Timeline-based orchestration |
| `Context` | Execution context |

### HierarchicalFsm

Nested finite state machine supporting hierarchical state nesting.

### JobBalancer

| Type | Role |
|---|---|
| `JobBalancer` | Cross-frame task scheduler with frame-time budget |
| `SteppedJob` | Single stepped job |
| `SteppedJobChain` | Chained stepped jobs |
| `PriorityQueue` | Priority-ordered job queue |

### Math

Vector2 / Vector3 / Vector4 · Matrix / Matrix2D · Quaternion · AABB · Ray · Rectangle · Point · Hierarchy · Transformation · MathHelper

### Pool

| Type | Role |
|---|---|
| `TemplateObjectPool<T>` | Generic object pool |
| `PoolObject` / `PoolObjectReference` | Pooled object and reference |
| `WeakArray` / `WeakHashSet` / `WeakList` / `WeakMap` | Weak-reference containers |

### Serialization

| Type | Role |
|---|---|
| `ByteReader` / `ByteWriter` | Binary read/write |
| `Serializer` | Serializer entry |
| `ISerializeable` | Serializable interface |
| `Property` / `PropertySet` / `Properties` | Property system |
| `Expr` / `ExprFactory` / `ExprTree` | Expression tree |

### Profiler

`GcWatch` · `ProfilerWatch` · `TimeWatch`

### Collections

`FastList` · `ArrayBasedLinkList` · `NonAllocLinkList` · `LinkedStack` · `SafeMap` · `ListExtensions`

---

## Module Boundaries

- **Exposes**: All public types and interfaces in the namespaces above
- **Scope**: Engine-agnostic infrastructure only — no rendering, asset loading, UI, or networking
- **Interaction**: Upper modules reference `Vena.Core` assembly directly; Core never reverse-depends
- **Constraint**: No game business logic or engine-specific API in Core

---

## Design Decisions

| Decision | Rationale |
|---|---|
| Custom math library instead of `UnityEngine.Mathf` | Maintains engine-agnostic portability |
| ECS-like Archetype pattern | Balances query performance and memory layout |
| Data-driven BehaviourTree & FlowGraph | Nodes are serializable and editor-friendly |
| Weak-reference pool containers | Avoids strong-reference memory leaks |
| Dual-mode serialization (binary + expression tree) | Covers both network transmission and persistence |
