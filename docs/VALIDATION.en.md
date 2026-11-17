# Validation

This is a locally tested preview, not a claim of compatibility with every Windows application.

- Internal regression checks cover inline aliases, cancel/reset, title updates, group/window sorting, collapse state, persistence, stable icon coordinates across hover, stable scroll offsets, fixed row sizes and compact-mode close hit testing.
- Two-monitor fixtures passed ordinary, minimized and maximized relocation. The minimized fixture stayed minimized during transfer and was restored only after its destination monitor was set.
- The native tray boundary and overlay geometry were inspected on a 92-pixel-wide left vertical taskbar with two 2560×1440 displays, one above the other.
- Real Windows focus tests may be rejected by foreground-activation rules. The internal branch tests are not proof that all apps can be activated.
- Whole-application UI automation was not completed; the external automation integration did not permit access to this newly built app. Internal input-path tests and practical local use are separate evidence.
- Mixed DPI, elevated windows, unusual host processes, all system menus, auto-hide, all full-screen modes and more than two displays remain incomplete coverage.

The documentation images are generated illustrations, not test screenshots or captures of a real desktop. Run `tools/render_preview.py` with Pillow installed to regenerate them.
