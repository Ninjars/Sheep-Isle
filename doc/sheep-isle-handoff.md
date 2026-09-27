# Sheep Isle: Unity 6 handoff

Updated 27 September 2026. For the current scope, order, and progress checklist, use the [project outline](project-outline.md). This handoff records the earlier concept and technical work; the new outline supersedes earlier adoption/rehoming and fixed-camera suggestions where they differ.

## Goal

Turn Sheep Isle into a Windows desktop idle companion: a floating island with cuboid sheep that wander and baa, separate music and sound-effect toggles, and a small passive or active interaction loop. Retire the existing food-driven reproduction system and avoid unbounded population growth.

## Verified project state

- Project: `E:\Jez\Documents\dev\Sheep Isle`.
- Installed and running editor: `C:\Program Files\Unity Installs\6000.3.25f1\Editor\Unity.exe`. `Unity.exe -version` returned `6000.3.25f1`.
- Git branch: `touch_input`. The last committed `ProjectVersion.txt` says `2019.4.20f1`; the working copy says `6000.3.25f1`. The upgrade has uncommitted changes in scripts, the main scene, packages, and settings. Preserve them before changing or cleaning the project.
- The editor left Safe Mode after Polybrush was removed. The live editor compiled and built the new scene with 0 Console errors on 26 September 2026. Existing package warnings remain.
- Main scene: `Assets/Scenes/Main Scene.unity`. It contains named objects including `Land`, `Underside`, `Pond`, `Tree Group`, `Rock Group`, plus ProBuilder meshes. Scene migration should use Unity editor APIs or the editor UI to preserve references, rather than editing scene YAML by hand.
- `Assets/Game/Agents/SheepAgent.cs` combines wandering, breathing, baa, food targeting, growth, and reproduction. `Assets/Game/InteractionController.cs` places food on a ground click and makes sheep baa on a sheep click. `Assets/Game/GameManager.cs` couples the existing scene to Aura, Doozy, season settings, and saves.

## Working method

- The running Unity editor can import file edits and provide visual checks. Windows computer use detected the editor and captured its Safe Mode Console, but foreground activation timed out once; do not depend on UI automation alone.
- Unity CLI can perform repeatable imports, compile checks, editor scripts, and builds using `-batchmode -quit -projectPath ... -logFile ... -executeMethod ...`. Use a separate project copy for batch runs while the current editor has the original open.
- This Codex task can read the project on `E:` but its normal writable workspace is under `C:\Users\Jez\Documents\Codex`. Writing to `E:` requires sandbox escalation or a new task/worktree configured with suitable access. Avoid launching batch Unity against the live project directory.
- No Unity-specific curated Codex skill was found. The existing computer-use skill is available. Consider a project-specific skill only after the edit, compile, and validation workflow is proven.

## Proposed order

1. **Concept pass:** direction agreed below. Next, turn it into a small playable specification and tune it after a prototype.
2. **Technical proof:** the cube proof is complete, and its transparent window method is integrated into a first island player. See [desktop window pass](desktop-window-pass.md). Edge quality, dragging, and longer-running performance still need checks.
3. **Scene pass:** completed in `Assets/Scenes/Desktop Companion.unity`. See [scene pass report](scene-pass-report.md). The original main scene remains intact.
4. **Gameplay pass:** simplify the sheep prefab and scripts, implement the chosen bounded idle loop, audio toggles, persistence, and desktop controls. Validate in editor and a Windows player build.

## Agreed concept direction

- **First platform:** Windows. The island should let clicks pass through empty space. Pinning it above other windows should be optional. Colour-key transparency and pinning are integrated in a prototype; integrated pixel hit testing still needs a direct check.
- **Core mood:** a calm desktop companion that does not demand frequent attention. Sheep wander, rest or graze, and occasionally baa. Clicking a sheep pets it and prompts a short response.
- **Idle loop:** wool accumulates automatically. A click can give a small optional bonus, but collection must not require repeated clicking. Wool buys cosmetics and can support adopting sheep.
- **Decoration:** choose from authored placement spots for the first version. This keeps the island readable and avoids blocking sheep navigation.
- **Flock:** deliberate adoption and rehoming within a small cap. No food-driven reproduction or uncontrolled population growth in the first version.
- **Audio:** separate remembered toggles for music and sound effects. Baa belongs to sound effects.

## Proposed first playable slice

1. A fixed three-quarter view of the transferred island, composed for a modest desktop footprint, with a few sheep that wander and baa.
2. A transparent Windows player with click-through empty pixels, an optional pin control, and a clear way to move, hide, and exit the island.
3. Petting feedback, passive wool accrual, and a small optional click bonus. No fail state or care chores.
4. A handful of decoration spots and a capped adopt/rehome control.
5. Save flock, wool, decorations, window position, pin state, and audio preferences between launches.

The initial flock size, cap, wool rates, prices, offline accrual limit, decorative items, and precise window controls are tuning choices. Start with simple values and adjust after a usable desktop build.

## Transparent-window proof result

- A separate Unity 6 project in `work/transparency-proof` was created and built through the CLI while the Sheep Isle editor stayed open. The sandboxed CLI could not connect to Unity licensing; running the installed CLI under the host account worked.
- The Windows build is in `outputs/transparency-proof`. Use its `RunTransparencyProof.cmd` launcher. Unity's default D3D11 flip swap chain showed an opaque magenta rectangle; `-force-d3d11-bitblt-model` made the layered colour-key window transparent. `-popupwindow` removed the frame.
- In the tested BitBlt popup mode, the checkerboard behind the cube was visible, clicking the cube changed its colour, a transparent pixel hit-tested to the checkerboard app, and pinning kept the cube above an activated backdrop. Right-drag movement and long-running performance remain unverified.
- This proves a viable Windows route for the interaction model. Colour-key edges may have quality limits; assess those with actual island geometry before adopting this rendering method for the final game.

References: [Unity editor command-line arguments](https://docs.unity3d.com/6000.0/Documentation/Manual/EditorCommandLineArguments.html); [Microsoft layered windows](https://learn.microsoft.com/en-us/windows/win32/winmsg/window-features).

## Scene pass result

- The five island geometry roots were cloned through Unity editor APIs into a new scene in an isolated project copy, with a fixed orthographic camera, simple lighting, a solid colour-key background, and a baked NavMesh. Validation found 511 renderers, no missing meshes or materials, no missing scripts in the cloned island, and 103 NavMesh triangles.
- A Windows player preview rendered the island in a transparent popup window. The separate [clean scene render](island-scene-preview/desktop-scene.png) shows the framing and surviving materials. The preview controller is temporary and is not part of the new scene.
- Polybrush was the Unity 6 compile blocker; removing its package dependency in the isolated copy allowed the scene to compile. The old main scene is preserved and may still contain a missing Polybrush component.
- A player build also surfaced a legacy Doozy `ScreenOrientation.Landscape` reference. The one-line compatibility fix was validated in the isolated preview and then applied to the source project.
- After Safe Mode was exited, the live editor refreshed the package lock without Polybrush. The new scene appeared in the Project browser and opened additively with the expected roots and assigned NavMesh. It was removed from the editor hierarchy after inspection to preserve the unsaved old scene.
- Live import revealed two broken Aura shader include paths and an incompatible lighting helper call in the same two shaders. Both were fixed. After that asset refresh and script compile, the Unity Console showed 0 errors. The source project's Windows build was completed in the following desktop window pass.

## Desktop window pass result

- `DesktopWindowController` is attached to the new scene camera. A dedicated build menu creates a D3D11 BitBlt Windows player at `Builds/Desktop Companion` in the source project. The source project build completed with 0 Console errors. An isolated build is also saved in `outputs/desktop-companion-player`.
- Launching both the isolated and source-project executables directly showed the floating island over transparent empty space without command-line flags. `P` toggled its pin state and `Esc` closed the isolated player. The right-button drag and integrated pixel hit test still need manual or controlled checks.
- User testing confirmed right-button drag, `Esc`, pinning with `P`, and transparent-pixel click-through. A temporary pin badge and unpinned notice were removed after the pin test. The controller still confirms the applied Windows topmost style after toggling. Keyboard shortcuts require player focus. On 27 September, delaying the borderless style until after Unity's resolution change removed the title bar and border in both isolated and source-project Windows players; the user rechecked click-through. See the [desktop window pass](desktop-window-pass.md).
- On 27 September, unit 1's mouse and camera slice changed window movement to middle drag and added right-drag perspective orbit and wheel dolly. The user checked the controls in a Windows player and requested a low-view pitch limit of −10 degrees. See the [desktop controls plan](desktop-controls-plan.md). Saved state and tray hide/restore are next, followed by sheep migration. Keep the old food-placement and food-driven reproduction code out of the new scene.
