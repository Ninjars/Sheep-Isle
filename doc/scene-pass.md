# Sheep Isle desktop scene pass

26 September 2026

## Delivered scene

`Assets/Scenes/Desktop Companion.unity` is a new scene made through Unity 6 editor APIs. It clones the `Land`, `Underside`, `water pool`, `Rock Group`, and `Tree Group` roots from `Main Scene.unity` under `Island Geometry`. The original scene was not edited in this pass. The new scene adds a fixed three-quarter orthographic camera, one directional light, flat ambient light, a solid magenta background for the proven Windows colour-key path, and an `Island Navigation` surface baked from the existing terrain collider. It carries none of the old game manager, food placement, seasons, orbit camera, Aura, or UI objects.

The original project had two Unity 6 compile errors inside the deprecated Polybrush package. The `com.unity.polybrush` dependency was removed from `Packages/manifest.json` after validating the same change in an isolated project copy. The cloned land had its one missing Polybrush component removed; the original `Main Scene.unity` remains untouched.

## Validation

- Unity 6000.3.25f1 reopened the saved new scene in an isolated project copy.
- 511 mesh renderers, 0 missing meshes, 0 missing materials, 0 missing scripts in the transferred island.
- Baked NavMesh: 103 triangles.
- A Windows preview build rendered the island with transparent empty space using the previously tested BitBlt popup launch flags. [Preview image](island-scene-preview/desktop-scene.png).
- The preview build uses a temporary desktop window controller. Its small compatibility change to an old Doozy orientation script was subsequently applied to the source project. The new scene does not yet include sheep, gameplay, or the window controller.
- The source scene and NavMesh hashes match the validated isolated copy. After Safe Mode was exited, the live editor refreshed its package lock without Polybrush. `Desktop Companion` appeared in the Project browser and opened additively; its hierarchy contained the expected four top-level roots and the Inspector showed an assigned NavMesh asset. The added scene was then removed from the editor hierarchy without saving or disturbing the unsaved `Main Scene`.
- The live import exposed two broken Aura shader include paths and two calls to an incompatible lighting helper. Those two legacy shaders were corrected. A separate Doozy `ScreenOrientation.Landscape` reference, already found during the preview build, was removed from the source project. After a final asset refresh and script compile, the Unity Console showed 0 errors and 27 warnings. The warnings are mostly obsolete API uses in old packages.

## Next work

The transparent window controller and source-project Windows build were completed in the subsequent [desktop window pass](desktop-window-pass.md). Adjust framing and lighting as needed, finish interaction and longer-running window checks, then migrate a simplified sheep prefab and implement the bounded wool, petting, decoration, adoption, and audio loops.
