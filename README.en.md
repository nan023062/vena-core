# Vena Core

Core runtime foundation library for the Vena framework, providing engine-agnostic data structures, algorithms, and runtime infrastructure.

> [中文版本](README.md)

## Features
- ECS-like World architecture (Actor / Component / System / Archetype)
- BehaviourTree for AI behavior
- FlowGraph and timeline orchestration
- HierarchicalFsm (hierarchical finite state machine)
- Cross-frame job balancing (JobBalancer)
- High-performance math library (vectors, matrices, quaternions, geometry)
- Object pooling and weak-reference containers
- Binary serialization and property system
- Profiling tools (GC watch, time watch)

## Installation
- Unity Package Manager: Add from disk... and select `Packages/vena.core`
- Or add a local path reference in `Packages/manifest.json`

## Documentation
- [Architecture Design](docs/architecture.md): System architecture, boundaries, and core models.
- [Internal Maintenance SOP](docs/internal-sop.md): Internal maintenance workflow and coding constraints (maintainers only).
- [External Integration & API](docs/usage-sop.md): Usage guide, API overview, and integration instructions.
- [Changelog](docs/changelog.md): Version history.
- [Dependencies](docs/dependencies.md): Module dependency notes.

## MCP Index
- Vena Core: `Packages/vena.core` / Core runtime library / Unity Package / v0.1.0 / No external dependencies
