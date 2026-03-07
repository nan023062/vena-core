# Usage Guide

## Installation

### Via Git URL (Recommended)

Add to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.vena.core": "https://github.com/nan023062/vena-core.git"
  }
}
```

To pin a specific version, append a tag:

```
https://github.com/nan023062/vena-core.git#v0.1.0
```

### Local Disk

```json
{
  "dependencies": {
    "com.vena.core": "file:../vena.core"
  }
}
```

---

## Assembly Reference

Add `Vena.Core` to your `.asmdef` references:

```json
{
  "references": ["Vena.Core"]
}
```

---

## Module Usage

### World / ECS-like

```csharp
using Vena.Core;

// 1. Define components
public class HealthComponent : Component { public int HP; }
public class TagEnemy : Component { }

// 2. Define systems
[System, Order(10)]
public class DamageSystem : Vena.Core.System
{
    public override void Update(World world)
    {
        foreach (var actor in world.Query<HealthComponent, TagEnemy>())
        {
            actor.Get<HealthComponent>().HP -= 1;
        }
    }
}

// 3. Bootstrap
var world = new World();
world.RegisterSystem<DamageSystem>();

var enemy = world.CreateActor();
enemy.Add(new HealthComponent { HP = 100 });
enemy.Add(new TagEnemy());

world.Initialize();

// 4. Game loop
void Update() => world.Update();
```

---

### JobBalancer

Distribute expensive work across multiple frames within a per-frame time budget:

```csharp
var balancer = new JobBalancer(budgetMs: 2f);

// Schedule a stepped job
balancer.Schedule(new SteppedJob(step =>
{
    ProcessChunk(step); // called once per Tick, resumes next frame if over budget
}));

// In Update()
balancer.Tick();
```

---

### Object Pool

```csharp
var pool = new TemplateObjectPool<Bullet>(() => new Bullet());

// Rent from pool
Bullet b = pool.Rent();
b.Fire(direction);

// Return when done
pool.Return(b);
```

---

### Math

```csharp
using Vena.Core.Math;

var a = new Vector3(1, 0, 0);
var b = new Vector3(0, 1, 0);
float dot = Vector3.Dot(a, b);

var box = new AABB(center, extents);
bool hit = box.Contains(point);
```

---

### Profiler

```csharp
using Vena.Core.Profiler;

using (var watch = new TimeWatch("MySystem.Update"))
{
    DoExpensiveWork();
} // logs elapsed time

var gcWatch = new GcWatch();
gcWatch.Begin();
DoAllocation();
long bytes = gcWatch.End(); // bytes allocated
```

---

## FAQ

**Q: Can I use Vena Core outside of Unity?**  
A: Yes. The core logic is engine-agnostic. Only `Vena.Core.asmdef` and Unity-specific wrappers (if any) tie it to Unity. The math, collections, pool, serialization, and job modules have no Unity dependency.

**Q: How do I update to a newer version?**  
A: If installed via git URL with a tag, update the tag in `manifest.json`. If on a branch, run **Package Manager → Update**.

**Q: Can I extend the BehaviourTree with custom nodes?**  
A: Yes. Inherit from `Action`, `Condition`, `Composite`, or `Decorate` and implement the required methods.
