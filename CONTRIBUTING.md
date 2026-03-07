# Contributing to Vena Core

感谢你对 Vena Core 的关注！以下是参与贡献的指南。

---

## 提 Issue

- **Bug 报告**：请描述复现步骤、Unity 版本、期望行为与实际行为
- **功能请求**：请说明使用场景和动机
- **使用问题**：优先查阅 [Usage Guide](docs/usage-guide.md) 和 [Architecture](docs/architecture.md)

---

## 提交 Pull Request

### 分支规范

| 分支前缀 | 用途 |
|---|---|
| `feat/` | 新功能 |
| `fix/` | Bug 修复 |
| `docs/` | 文档更新 |
| `refactor/` | 重构 |
| `perf/` | 性能优化 |

示例：`feat/behaviour-tree-parallel-node`

### Commit Message 规范

```
<type>: <简明描述>

# type 取值：feat / fix / docs / refactor / perf / test / chore
# 示例：
feat(World): add multi-world support
fix(JobBalancer): prevent negative budget overflow
docs: update usage guide with pool examples
```

### PR 检查清单

- [ ] 改动范围在 Core 职责内（无游戏业务逻辑、无 Unity 引擎强依赖）
- [ ] 热路径无 GC 分配
- [ ] 新增公共 API 已在 `docs/architecture.md` 中注册
- [ ] 版本号已按 SemVer 更新（如需要）
- [ ] `CHANGELOG.md` 已更新

---

## 模块边界约束

- **禁止**在 Core 中引入对 `UnityEngine`（除 `Mathf` 等极少数工具类）的依赖
- **禁止**在 Core 中引入游戏业务逻辑
- **禁止** Core 反向依赖任何上层模块

---

## 本地开发

```bash
git clone https://github.com/nan023062/vena-core.git
```

将目录加入任意 Unity 2020.3+ 项目的 `Packages/manifest.json` 进行本地测试：

```json
"com.vena.core": "file:../vena-core"
```

---

## License

贡献的代码将以 [MulanPSL-2.0](LICENSE) 协议发布。
