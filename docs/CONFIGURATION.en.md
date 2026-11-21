# Configuration

[Back to README](../README.md)

Edit `settings.json` in the data folder, save, then choose **⋯ → 重新加载设置** (Reload settings). Sizes scale with DPI; milliseconds do not. `preferences.json` is session/layout state and normally should not be edited manually.

| Parameter | Default | Range | Meaning |
|---|---|---|---|
| `ExpandedWidth` | `280` | 220–800 | Width while expanded; compact width follows the native taskbar. |
| `IconSize` | `32` | 16, 32, 48 | Standard logical icon size; 32 is recommended. Resource quality depends on the app. |
| `FontSize` | `16` | 10–28 | Text size in logical pixels. |
| `FontFamily` | `Microsoft YaHei UI` | string | Installed font family; Segoe UI is another option. |
| `RowHeight` | `44` | max(IconSize+12, FontSize+16)–96 | Fixed row height; adjusted up when necessary for the chosen icon/font. |
| `HeaderHeight` | `34` | 28–64 | Top toolbar height. |
| `HoverExpandDelayMs` | `180` | 0–3000 | Hover delay before expansion; observed on a 300 ms UI timer. |
| `HoverCollapseDelayMs` | `450` | 100–5000 | Delay before collapsing after leaving. |
| `RefreshIntervalMs` | `900` | 300–5000 | Window/title/monitor refresh cadence, rounded by the UI timer. |
| `TopInset` | `0` | 0–500 | Pixels left uncovered at the top; about 96 keeps the original Start/Search area on the tested layout. |
| `ChildIndent` | `12` | 0–24 | Inset of child-window icons from the left edge of the detail column. Titles follow the icon. |
| `CloseButtonWidth` | `40` | 32–64 | Close-button hit area; hidden and inactive in compact mode. |
| `CenterList` | `true` | true / false | Center when content fits; false starts at the top. |

## Colors

Use `#RRGGBB` inside `Colors`. Invalid colors fall back to that field's default.

| Field | Default |
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

`Background` is the main surface, `Header` the top bar, `Active` the foreground row, `Hover` the hovered row, and `Secondary`/`SecondaryActive` the secondary-display rows. `Accent` marks the foreground window. `Close`/`CloseHover` color the close glyph; `Text` and `Border` control text and edges. App icons retain their original colors.

## State and advanced changes

`preferences.json` stores `Pinned`, `GroupOrder`, `Collapsed`, `GroupAliases`, `WindowAliases`, and `WindowOrder`, managed by the UI. Legacy `ExpandedWidth`, `CompactWidth`, and `TopInset` state fields do not override `settings.json`. A window handle is not a stable document identity after reopening.

Ctrl+Alt+F12, the primary-left overlay policy, the 300 ms layout timer, and window-filter/icon-extraction rules are defined in source and require rebuilding to change. There is no built-in language switch or automatic dark-theme switch.
