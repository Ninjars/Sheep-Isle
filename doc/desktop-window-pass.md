# Sheep Isle desktop window pass

26 September 2026

## Implemented

- `Assets/DesktopCompanion/DesktopWindowController.cs` is attached to the `Desktop Camera` in `Assets/Scenes/Desktop Companion.unity`. On Windows player launch it creates a 480 × 480 colour-key window and requests borderless styling; the current player still shows a title bar and border. Empty magenta pixels are transparent. The camera uses a solid magenta clear colour, 30 FPS target, and no MSAA.
- `P` toggles always-on-top, right-button dragging moves the window, and `Esc` exits. The behavior is limited to Windows players; the Unity editor retains its normal window.
- `Assets/DesktopCompanion/Editor/DesktopCompanionBuild.cs` adds **Sheep Isle → Desktop Companion → Build Windows Player**. It builds only the new scene, sets D3D11 and the BitBlt swap chain for direct transparent launch, and writes to `Builds/Desktop Companion` in the project.

## Validation

- The isolated Unity copy compiled and built a player at `outputs/desktop-companion-player`. Launching `Sheep Isle.exe` directly, with no arguments, showed the island over two different desktop backdrops through the transparent pixels. `P` changed the pin state and `Esc` closed the player.
- The same build menu ran in the source project and produced `Builds/Desktop Companion/Sheep Isle.exe`. Launching that executable directly also showed transparent empty space around the island. The live editor Console showed 0 errors after import and build.
- The existing main scene was not edited for this pass. The player build used the new scene explicitly.
- User testing confirmed right-button dragging, `Esc`, pinning with `P`, and transparent-pixel click-through. A temporary on-screen pin notice made the pin test clearer; it was removed after confirmation. The controller still checks the applied Windows topmost style after toggling. The current player shows a title bar and border despite the intended borderless mode.

## Remaining checks and next work

- Keyboard shortcuts currently require player focus, so click the island before pressing `P` or `Esc`.
- Hide the visible title bar and border while retaining transparent-pixel click-through. See the [project outline](project-outline.md) for the following desktop-control work.
- Recheck pixel hit testing, edge quality, window movement, and longer-running performance on the integrated source build. The earlier cube proof verified transparent pixel click-through with the same Windows colour-key technique.
- Add a convenient hide/restore control and remembered window position/pin state as part of desktop controls. Migrate a simplified sheep prefab next, then add wandering, baa, petting, passive wool, decorations, and capped adoption/rehoming.
