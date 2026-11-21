# Changelog

[简体中文](CHANGELOG.zh-CN.md)

## 0.1.2 — 2026-11-21

- Opening or closing the hidden tray icons could briefly expose the native taskbar. Shelf now responds to foreground and stacking changes, keeping the tray flyout accessible.
- Hover over a multi-window app to see its windows in a second column. The icon rail stays in place and the compact list has one row per app.
- Windows Security, Calculator and Microsoft Store use their individual app identities and icons. Shelf retries late-loading window icons, including those used by MuMu.
- The executable, Shelf window and notification area use the new neutral-gray and mist-blue icon. The SVG source and nine-size Windows icon are included.

## 0.1.1 — 2026-11-18

- Fix the native taskbar covering Window Shelf after sign-in or a background taskbar raise.
- Repair actual stacking order without requiring taskbar focus; preserve foreground focus, geometry and higher flyouts.
- Add eight native-window regression checks, for 51 passing checks in the patch validation. Local user verification confirmed hover display recovered.
- Update English and Chinese upgrade instructions and validation notes. Existing settings and delayed sign-in tasks remain compatible.

## 0.1.0 — 2026-11-18

- First public preview for a left-side vertical Windows taskbar.
- Neutral gray / mist blue theme, preserving secondary-display tint.
- Stable hover expansion, grouped windows, inline aliases, drag sorting, normal close and foreground-click minimize.
- Existing-window transfer to another display, including minimized windows.
- Editable JSON settings, bilingual documentation and optional delayed sign-in task scripts.
