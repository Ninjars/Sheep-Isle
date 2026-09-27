# Sheep Isle desktop window pass

26 September 2026

## Implemented

- `Assets/DesktopCompanion/DesktopWindowController.cs` is attached to the `Desktop Camera` in `Assets/Scenes/Desktop Companion.unity`. On Windows player launch it creates a colour-key window and applies borderless styling after the requested 480 × 480 resolution has taken effect. Empty magenta pixels are transparent. The camera uses a solid magenta clear colour, 30 FPS target, and no MSAA.
- `P` toggles always-on-top, right-button dragging moves the window, and `Esc` exits. The behavior is limited to Windows players; the Unity editor retains its normal window.
- `Assets/DesktopCompanion/Editor/DesktopCompanionBuild.cs` adds **Sheep Isle → Desktop Companion → Build Windows Player**. It builds only the new scene, sets D3D11 and the BitBlt swap chain for direct transparent launch, and writes to `Builds/Desktop Companion` in the project.

## Validation

- The isolated Unity copy compiled and built a player at `outputs/desktop-companion-player`. Launching `Sheep Isle.exe` directly, with no arguments, showed the island over two different desktop backdrops through the transparent pixels. `P` changed the pin state and `Esc` closed the player.
- The same build menu ran in the source project and produced `Builds/Desktop Companion/Sheep Isle.exe`. Launching that executable directly also showed transparent empty space around the island. The live editor Console showed 0 errors after import and build.
- The existing main scene was not edited for this pass. The player build used the new scene explicitly.
- User testing confirmed right-button dragging, `Esc`, pinning with `P`, and transparent-pixel click-through. A temporary on-screen pin notice made the pin test clearer; it was removed after confirmation. The controller still checks the applied Windows topmost style after toggling.
- On 27 September, isolated and source-project Windows builds verified that waiting one frame after `Screen.SetResolution` removes the title bar and border. An empty-pixel click was reported as targeting the Chrome window behind each player, and the user confirmed click-through in the updated project build. Unity applies `Screen.SetResolution` at the end of the frame; the original styling ran before that change and was evidently reset. See [Unity's resolution API](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Screen.SetResolution.html) and [Microsoft's frame-style guidance](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos).

## Remaining checks and next work

- Keyboard shortcuts currently require player focus, so click the island before pressing `P` or `Esc`.
- Continue with the hide/restore, saved window state, and camera controls in the [project outline](project-outline.md).
- Recheck edge quality, window movement, and longer-running performance as controls evolve. The [project outline](project-outline.md) tracks the next desktop and gameplay slices.
