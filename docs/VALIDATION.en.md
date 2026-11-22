# Validation

## v0.1.2.1

The self-test passed 69 checks with `--monitor-test --gesture-test`. A temporary window on the installed build also verified middle-click maximization, double-click minimization that persisted after the double-click interval, and single-click restoration.

## v0.1.2

The x64 release build passed 62 layout, stacking and dual-monitor checks. The 16-pixel notification icon and 32-pixel executable icon matched their design frames pixel for pixel. In one open-and-close cycle on the installed build, the native taskbar did not cover the sampled Shelf point in 1,255 observations. The measurements sampled window order rather than individual display frames.

## v0.1.1 regression

The installed patch binary passed 51 checks: the original 43 interaction/layout checks and eight native-window stacking checks. Small non-activating fixtures reproduce a taskbar rising while it is not foreground, then verify overlay recovery, higher-flyout preservation, unchanged focus and geometry, an unchanged already-correct order, and recovery after another taskbar raise.

The local desktop stacking order and composite screenshot confirmed the original failure. After deployment, the user confirmed hover expansion was fully visible again. These desktop captures remain local and are not included in this repository or release. The repaired binary has not yet been observed through another full sign-out/reboot cycle. The two-monitor transfer results below come from v0.1.0; that feature was unchanged in this patch.

This is a locally tested preview, not a claim of compatibility with every Windows application.

- Internal regression checks cover inline aliases, cancel/reset, title updates, group/window sorting, collapse state, persistence, stable icon coordinates across hover, stable scroll offsets, fixed row sizes and compact-mode close hit testing.
- Two-monitor fixtures passed ordinary, minimized and maximized relocation. The minimized fixture stayed minimized during transfer and was restored only after its destination monitor was set.
- The native tray boundary and overlay geometry were inspected on a 92-pixel-wide left vertical taskbar with two 2560×1440 displays, one above the other.
- Real Windows focus tests may be rejected by foreground-activation rules. The internal branch tests are not proof that all apps can be activated.
- Whole-application UI automation was not completed; the external automation integration did not permit access to this newly built app. Internal input-path tests and practical local use are separate evidence.
- Mixed DPI, elevated windows, unusual host processes, all system menus, auto-hide, all full-screen modes and more than two displays remain incomplete coverage.

The documentation images are generated illustrations, not test screenshots or captures of a real desktop. Run `tools/render_preview.py` with Pillow installed to regenerate them.
