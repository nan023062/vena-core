# Vena Core

Vena 框架的核心运行时基础库，提供与引擎无关的数据结构、算法与运行时基础设施。

> [English Version](README.en.md)

## 用途
- ECS-like World 架构（Actor / Component / System / Archetype）
- 行为树（BehaviourTree）
- 流程图（FlowGraph）与时间轴编排
- 层级状态机（HierarchicalFsm）
- 跨帧任务均衡调度（JobBalancer）
- 高性能数学库（向量、矩阵、四元数、几何体）
- 对象池与 Weak 引用容器
- 二进制序列化与属性系统
- 性能分析工具（GC 监控、计时器）

## 安装
- Unity Package Manager: Add from disk... 选择 `Packages/vena.core`
- 或在 `Packages/manifest.json` 中添加本地路径引用

## 文档索引
- [架构设计](docs/architecture.md)：系统架构、边界与核心模型说明。
- [内部维护 SOP](docs/internal-sop.md)：内部维护与改动流程规范（仅维护者使用）。
- [外部集成与 API](docs/usage-sop.md)：使用指南、API 概览与集成说明。
- [Changelog](docs/changelog.md)：版本变更记录。
- [依赖说明](docs/dependencies.md)：模块依赖与引用说明。

## MCP Index
- Vena Core：`Packages/vena.core` / 基础运行时库 / Unity Package / v0.1.0 / 无外部依赖
