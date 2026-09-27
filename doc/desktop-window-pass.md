# Sheep Isle desktop window pass

Updated 27 September 2026

## Implemented

- `Assets/DesktopCompanion/DesktopWindowController.cs` is attached to the `Desktop Camera` in `Assets/Scenes/Desktop Companion.unity`. On Windows player launch it creates a colour-key window and applies borderless styling after the requested 480 × 480 resolution has taken effect. Empty magenta pixels are transparent. The camera uses a solid magenta clear colour, 30 FPS target, and no MSAA.
- `P` toggles always-on-top, middle-button dragging moves the window, `H` hides it to a tray icon, and `Esc` exits. The tray menu offers Show/Hide and Exit. The behavior is limited to Windows players; the Unity editor retains its normal window.
- `DesktopOrbitCamera.cs` uses a perspective camera with right-button orbit and wheel dolly. The player remembers camera angle, zoom, window position, and pin state. A saved off-screen window position is brought back onto a visible monitor work area.
- `Assets/DesktopCompanion/Editor/DesktopCompanionBuild.cs` adds **Sheep Isle → Desktop Companion → Build Windows Player**. It builds only the new scene, sets D3D11 and the BitBlt swap chain for direct transparent launch, and writes to `Builds/Desktop Companion` in the project.

## Validation

- The isolated Unity copy compiled and built a player at `outputs/desktop-companion-player`. Launching `Sheep Isle.exe` directly, with no arguments, showed the island over two different desktop backdrops through the transparent pixels. `P` changed the pin state and `Esc` closed the player.
- The same build menu ran in the source project and produced `Builds/Desktop Companion/Sheep Isle.exe`. Launching that executable directly also showed transparent empty space around the island. The live editor Console showed 0 errors after import and build.
- The existing main scene was not edited for this pass. The player build used the new scene explicitly.
- Early user testing confirmed the original right-button window drag, `Esc`, pinning with `P`, and transparent-pixel click-through. Window drag has since moved to the middle button. A temporary on-screen pin notice made the pin test clearer; it was removed after confirmation. The controller still checks the applied Windows topmost style after toggling.
- On 27 September, isolated and source-project Windows builds verified that waiting one frame after `Screen.SetResolution` removes the title bar and border. An empty-pixel click was reported as targeting the Chrome window behind each player, and the user confirmed click-through in the updated project build. Unity applies `Screen.SetResolution` at the end of the frame; the original styling ran before that change and was evidently reset. See [Unity's resolution API](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Screen.SetResolution.html) and [Microsoft's frame-style guidance](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos).
- On 27 September, the user confirmed middle-drag movement, right-drag orbit, wheel zoom, and tray Show/Exit. Restarting the player retained zoom and pin state. A user-dragged window position was saved and restored, with a small shift to keep the full window on screen. An intentionally off-screen saved position was also moved into the visible work area. A later empty-pixel click targeted the desktop behind the source player, confirming click-through after the control changes.
- The source player remained responsive during a longer check. The user confirmed the revised −10° low-angle limit and comfortable framing across the wheel zoom range. The control work and checks are tracked in the [desktop controls plan](desktop-controls-plan.md).

During the unit 2 sheep test on 27 September, a shell-launched player remained active and audible while the user saw no window or tray icon. That test process was terminated. Sheep now stop active baa playback and do not begin another while the window is hidden (commit `3a0a22e`); the updated Unity 6 player built successfully. Tray visibility and restore in an interactively launched player are being rechecked.

## Remaining checks and next work

- Keyboard shortcuts currently require player focus, so click the island before pressing `P` or `Esc`.
- The [project outline](project-outline.md) tracks the next desktop and gameplay slices. Camera framing will need tuning again when the larger replacement island is built.
