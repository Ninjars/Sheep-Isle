---
type: report
status: completed
milestone: desktop-prototype
updated: 2026-09-27
---

# Desktop window and controls

## Outcome

The Windows player runs as a borderless transparent desktop companion with click-through empty pixels, optional pinning, window and camera controls, saved placement and view state, and notification-area hide, restore, and exit actions. These player-facing desktop features were validated independently of the remaining initial-flock regression.

## Delivered scope

- `DesktopWindowController` creates a colour-key Windows window after requesting a 480 × 480 player resolution. Empty magenta pixels are transparent and click through to applications behind the island.
- The companion camera clears to solid magenta, targets 30 FPS, and uses no MSAA so the tested colour-key and idle-performance baseline remains reproducible.
- `P` toggles always-on-top, middle-button drag moves the desktop window, `H` hides it, and `Esc` exits. The tray menu provides Show/Hide and Exit, and re-registers its icon after Explorer restarts.
- `DesktopOrbitCamera` uses a perspective camera with right-button orbit, a bounded vertical angle including the confirmed −10° low-view limit, and wheel dolly.
- Window position, pin state, camera yaw, camera pitch, and camera distance persist between launches. An off-screen saved window position is clamped to a visible monitor work area.
- Background processing remains enabled while hidden so notification-area actions remain responsive.
- `DesktopCompanionBuild` builds only the companion scene with D3D11 and the BitBlt swap-chain model required by the colour-key transparency path.

## Deviations and durable decisions

- Unity's default D3D11 flip swap chain rendered the colour-key background as opaque. The player therefore uses the D3D11 BitBlt model.
- Applying borderless styles before `Screen.SetResolution` completed allowed Unity to restore the frame. Waiting one frame before applying the native style reliably removed the title bar and border.
- Window movement uses middle drag so right drag is reserved for camera orbit. The camera changed from the transferred scene's orthographic view to perspective so the wheel can move toward and away from the island.
- Orbit input and smoothing drew on `Assets/Game/MouseOrbiterImproved.cs`, especially its distance-related input, without importing its broad mouse-capture behaviour.
- Native window and tray behaviour stays behind `UNITY_STANDALONE_WIN && !UNITY_EDITOR`; the Unity editor retains a normal Game view.
- The camera controller remains separate from the native window controller, and the tray helper remains focused rather than expanding one large platform script.
- Keyboard shortcuts require player focus. This is current interaction behaviour, not a global shortcut contract.

## Validation evidence

- An isolated Unity 6 project copy compiled and built a player at `outputs/desktop-companion-player`. A source-project build also completed at `Builds/Desktop Companion/Sheep Isle.exe`, with 0 live-editor Console errors after import and build.
- Both executables launched directly without command-line flags and showed the island over transparent empty space.
- The user confirmed transparent-pixel click-through, pin toggling, middle-drag window movement, right-drag camera orbit, wheel dolly, tray Show, tray Exit, and the −10° low-view limit.
- Empty-pixel clicks targeted Chrome or the desktop behind the source and isolated players rather than activating the companion.
- Restart checks restored changed zoom and pin state. A user-dragged position returned within the visible work area, and an intentionally off-screen saved position recovered to a visible monitor.
- Camera framing remained comfortable across the tested orbit and wheel limits, and the source player remained responsive during a longer idle check.
- Hiding with `H` removed the island window while leaving the tray integration responsive; background processing was enabled explicitly for this behaviour.

## Remaining risks or follow-up

- Tray discoverability and restoration must still be observed with the current sheep player. That narrower integration check is `awaiting-validation` in the [initial flock plan](../plans/initial-flock.md); it does not invalidate the independently observed desktop controls above.
- Camera framing and colour-key edge quality need another visual pass when the larger replacement island replaces the placeholder.
- Focus-dependent shortcuts require the user to click the island before pressing `P`, `H`, `S`, or `Esc`.

## Relevant commits and artifacts

- `c375959` — add the borderless desktop companion player.
- `7f0faa7` — add desktop camera orbit and mouse controls.
- `afa6588` — allow the lower island camera angle.
- `91ffc2e` — remember desktop window and camera state.
- `226d289` and `7953297` — add tray hide/restore and keep hidden processing responsive.
- `49e0ba7` — record the completed control validation.
- `Assets/DesktopCompanion/DesktopWindowController.cs`
- `Assets/DesktopCompanion/DesktopOrbitCamera.cs`
- `Assets/DesktopCompanion/Editor/DesktopCompanionBuild.cs`
- `Assets/Game/MouseOrbiterImproved.cs`
- [Unity `Screen.SetResolution`](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Screen.SetResolution.html)
- [Microsoft `SetWindowPos`](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos)
