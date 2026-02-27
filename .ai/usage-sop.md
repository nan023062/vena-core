# API 使用 SOP (External Integration)

## 模块简介
- 一句话说明：Vena Core 提供与引擎无关的核心数据结构、算法与运行时基础设施（ECS-like World、行为树、流程图、状态机、数学库、对象池、序列化等）。
- 适用场景：所有 Vena 上层模块均需引用此包作为基础运行时依赖。

## 前置条件
- Unity 2020.3 或更高版本
- 通过 Unity Package Manager 引用 `com.vena.core`
- 无其他外部依赖

## Quick Start
```csharp
// 1. 创建 World
var world = new World();

// 2. 创建 Actor 并添加 Component
var actor = world.CreateActor();
actor.Add<MyComponent>();

// 3. 注册并运行 System
world.AddSystem<MySystem>();
world.Update();
```

## 常见使用模式

### 模式 A：ECS-like World 驱动
- 场景描述：使用 World / Actor / Component / System 组织游戏逻辑
- 代码示例：
```csharp
// 定义 Component
public class HealthComponent : Component {
    public int Hp;
}

// 定义 System
[Order(100)]
public class DamageSystem : Vena.Core.System {
    public override void OnUpdate() {
        // 遍历并处理
    }
}
```

### 模式 B：行为树
- 场景描述：使用 BehaviourTree 驱动 AI 行为
- 代码示例：
```csharp
var tree = new BehaviorTree();
// 构建节点树
tree.Tick(blackboard);
```

### 模式 C：JobBalancer 跨帧任务
- 场景描述：将耗时任务拆分到多帧执行，避免卡顿
- 代码示例：
```csharp
var balancer = new JobBalancer();
balancer.Add(new SteppedJob(...));
balancer.Update(timeBudgetMs);
```

## API 概览
| API | 说明 | 副作用 |
|-----|------|--------|
| `World.CreateActor()` | 创建实体 | 分配 InstanceID |
| `Actor.Add<T>()` | 添加组件 | 修改 Archetype |
| `World.AddSystem<T>()` | 注册系统 | 加入调度队列 |
| `World.Update()` | 驱动所有系统执行 | 触发系统 OnUpdate |
| `BehaviorTree.Tick()` | 行为树单次 Tick | 执行节点逻辑 |
| `JobBalancer.Update()` | 按时间预算执行分步任务 | 推进任务进度 |
| `ByteWriter.Write<T>()` | 二进制序列化 | 写入缓冲区 |
| `ByteReader.Read<T>()` | 二进制反序列化 | 推进读取位置 |

## 错误处理
- World / Actor 操作在参数非法时抛出 `ArgumentException`
- 序列化读写越界时抛出 `IndexOutOfRangeException`
- 推荐在调用方捕获异常并记录上下文日志

## 注意事项
- 线程安全性：World 及其子系统默认非线程安全，需在主线程调用
- 性能提示：Archetype 查询在首次构建后有缓存，大批量 Actor 创建建议批量操作
- 版本兼容：Core 包 API 变更遵循语义化版本，Breaking Change 会在 changelog 中标注
