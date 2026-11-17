# 参数说明

[返回说明](../README.zh-CN.md)

编辑数据目录中的 `settings.json`，保存后使用“⋯ → 重新加载设置”。尺寸按 DPI 缩放，毫秒不缩放。示例中的 `preferences.json` 不是参数文件，通常无需手动修改。

| 参数 | 默认值 | 有效范围 | 说明 |
|---|---|---|---|
| `ExpandedWidth` | `280` | 220–800 | 展开宽度；收起宽度始终跟随原生任务栏。 |
| `IconSize` | `32` | 16, 32, 48 | 标准逻辑图标尺寸，推荐 32；清晰度还取决于应用资源。 |
| `FontSize` | `16` | 10–28 | 文字大小，单位为逻辑像素。 |
| `FontFamily` | `Microsoft YaHei UI` | string | 已安装字体名称，也可填写 Segoe UI。 |
| `RowHeight` | `44` | max(IconSize+12, FontSize+16)–96 | 固定行高；会根据图标、字号自动提高最小值。 |
| `HeaderHeight` | `34` | 28–64 | 顶部工具栏高度。 |
| `HoverExpandDelayMs` | `180` | 0–3000 | 悬停展开延迟；由 300 毫秒界面计时器检查。 |
| `HoverCollapseDelayMs` | `450` | 100–5000 | 鼠标离开后收起延迟。 |
| `RefreshIntervalMs` | `900` | 300–5000 | 窗口、标题、屏幕归属的刷新间隔，受界面计时器粒度影响。 |
| `TopInset` | `0` | 0–500 | 顶部不覆盖高度；测试布局约 96 可保留原来的开始/搜索区域。 |
| `ChildIndent` | `12` | 0–24 | 子窗口图标、底色及标题的轻微缩进。 |
| `CloseButtonWidth` | `40` | 32–64 | 关闭按钮点击区域宽度；收起时隐藏且不响应关闭。 |
| `CenterList` | `true` | true / false | 内容放得下时居中；false 从顶部开始。 |

## 配色

`Colors` 内使用 `#RRGGBB`。无效颜色回退到该项默认颜色。

| 字段 | 默认值 |
|---|---|
| `Background` | `#F3F4F6` |
| `Header` | `#E9ECF0` |
| `Text` | `#303844` |
| `Border` | `#D8DDE4` |
| `Active` | `#E2E7ED` |
| `Hover` | `#E9EDF2` |
| `Secondary` | `#DCE5F2` |
| `SecondaryActive` | `#C5D6EC` |
| `Accent` | `#56759A` |
| `Close` | `#66717E` |
| `CloseHover` | `#A53841` |

`Background` 是主底色，`Header` 是顶栏，`Active` 是当前窗口，`Hover` 是悬停，`Secondary`/`SecondaryActive` 分别是副屏普通/当前窗口，`Accent` 是当前窗口细标记，`Close`/`CloseHover` 是关闭符号。标题文字用 `Text`，边线用 `Border`。图标本身保留原应用颜色。

## 状态与进阶修改

`preferences.json` 保存 `Pinned`、`GroupOrder`、`Collapsed`、`GroupAliases`、`WindowAliases`、`WindowOrder`。这些字段由界面管理，旧版遗留的 `ExpandedWidth`、`CompactWidth`、`TopInset` 不再覆盖 `settings.json`。不要把窗口句柄当作关闭重开后的稳定文档标识。

热键 Ctrl+Alt+F12、左侧主屏覆盖策略、300 毫秒布局计时器，以及图标提取/窗口筛选规则仍在源码中定义；修改这些需要重新编译。当前没有内置语言切换或自动深色主题切换。
