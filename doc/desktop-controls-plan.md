# Desktop controls: unit 1 plan

Updated 27 September 2026. This expands [unit 1 of the project outline](project-outline.md). The work is Windows-first and must keep the borderless, transparent, click-through player usable throughout. The original main scene remains intact.

## Decisions

- A notification-area (tray) icon remains available while the island is hidden. Its menu offers **Show/Hide** and **Exit**; clicking the icon can restore the island. `H` hides it while the player has focus. `Esc` still exits and `P` still toggles pinning.
- Middle-button drag moves the desktop window. Right-button drag orbits the camera horizontally and within a limited vertical range. The wheel zooms.
- Switch the companion camera from orthographic to perspective so zoom moves the camera toward or away from the island. Reuse the original [`MouseOrbiterImproved.cs`](../Assets/Game/MouseOrbiterImproved.cs) as a feel reference, especially its distance-related orbit input and smoothing, without importing its broad mouse capture behavior.
- Remember the window position, pin state, camera angle, and zoom between launches. Restore an off-screen saved position to a visible monitor work area.

## Work slices and completion checks

1. **Mouse and camera controls.** Reserve middle drag for window movement, right drag for orbit, and wheel for a bounded perspective dolly. Use an island-centered pivot, constrain pitch and distance, and avoid capturing input through transparent pixels. Build a Windows player and check visual framing plus the three controls. Commit the slice after the build and available interaction checks; record any manual checks still pending.
2. **Saved state.** Save position after a drag and on exit, save pin changes, and save camera orientation and distance after interaction. Load before applying the native window placement and camera view. Validate restart behavior, multi-monitor bounds, and an off-screen saved value. Commit the slice with its checks.
3. **Tray hide and restore.** Add the tray icon and Show/Hide/Exit menu, with safe cleanup on exit. `H` hides; tray Show restores and activates the window. Validate hide, restore, pin state, exit, and restart without leaving a dead icon. Commit the slice.
4. **Integrated regression.** Verify borderless transparency and empty-pixel click-through, framing across camera limits, middle drag, right orbit, wheel zoom, input focus, pinning, saved state, and a longer idle run. Fix any observed issues. Update the [outline](project-outline.md) and [desktop window pass](desktop-window-pass.md) with verified outcomes, then commit the final notes and fixes.

## Progress

- **Slice 1, 27 September:** the separate `DesktopOrbitCamera` changes the companion camera to perspective, orbits on right drag, and moves toward/away exponentially on wheel input. Window dragging now uses middle drag. The user checked all three controls in the Windows player and found the feel good; after a further test, they requested a low-view pitch limit of −10 degrees. The scene component was added through the Unity editor build tool, and an isolated Unity 6 player build succeeded. The source project build opened with the intended camera and remained borderless.

## Implementation boundaries

- Keep window/tray behavior behind `UNITY_STANDALONE_WIN && !UNITY_EDITOR`; the editor retains a normal Game view. Use Windows APIs only for native window and tray functions.
- Do not modify the old main scene or reintroduce its food, UI, or camera scripts into the companion scene.
- Prefer a focused controller for camera movement and a small native tray helper over expanding the existing window controller into one large script. Use Unity editor APIs for any scene component changes so references and metadata remain valid.
- Test builds in the isolated project copy if the source is open in Unity. Copy only validated source changes back to the live project, then launch the source build in the user desktop session for interaction checks.
- Keep unrelated migration changes out of each commit. Update checklist items only after the corresponding behavior has been observed.

## Tuning values to settle in the prototype

The first orbit limits, perspective field of view, zoom range, and input sensitivity are implementation starting points. Tune them against the placeholder island and expect further changes when the larger biome island replaces it. Confirm that all views stay within the colour-key window without clipping essential geometry.
