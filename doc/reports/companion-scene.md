---
type: report
status: completed
milestone: desktop-prototype
updated: 2026-09-27
---

# Desktop companion scene

## Outcome

Unity 6 has a separate `Assets/Scenes/Desktop Companion.unity` scene containing the transferred island geometry, a camera, lighting, and baked navigation data. The original `Assets/Scenes/Main Scene.unity` was not edited during this unit.

## Delivered scope

- Cloned the `Land`, `Underside`, `water pool`, `Rock Group`, and `Tree Group` roots under `Island Geometry` through Unity editor APIs.
- Added a desktop camera, one directional light, flat ambient lighting, the solid magenta background used by the Windows colour-key path, and an `Island Navigation` surface.
- Baked `Assets/Scenes/Desktop Companion/NavMesh.asset` from the existing terrain collider.
- Kept the old game manager, food placement, seasons, orbit camera, Aura components, and UI objects out of the companion scene.
- Removed the cloned land's missing Polybrush component after the obsolete `com.unity.polybrush` dependency was removed. The legacy main scene remained intact.

## Deviations and durable decisions

- Scene transfer and cleanup used Unity editor APIs rather than hand-edited scene YAML so serialized references and metadata survived.
- The delivered camera was a fixed three-quarter orthographic camera, and the Windows preview used a temporary window controller. Both are point-in-time facts: the orthographic camera and temporary controller were superseded by the later desktop window and camera-control work.
- The transferred island and its 103-triangle NavMesh are prototype assets. The replacement island requires deliberately authored walkable ground and blocker footprints rather than treating this bake as final navigation.

## Validation evidence

- Unity 6000.3.25f1 reopened the saved scene in an isolated project copy.
- Inventory: 511 mesh renderers, 0 missing meshes, 0 missing materials, and 0 missing scripts in the transferred island.
- Baked NavMesh: 103 triangles.
- The source scene and NavMesh hashes matched the validated isolated copy.
- A Windows preview build rendered the island with transparent empty space. The previously referenced preview image is not present in this repository, so this report preserves the observed result without retaining a broken artifact link.
- The live editor imported the scene, displayed the expected hierarchy and assigned NavMesh, and compiled with 0 errors and 27 warnings. The warnings were primarily obsolete API usage in legacy packages.
- The companion scene was removed from the live editor hierarchy after inspection without saving or disturbing the unsaved legacy scene.

## Remaining risks or follow-up

- The transferred island is a placeholder. Camera framing, lighting, materials, and navigation are intentionally revisited by the Replacement island milestone.
- The original main scene may still contain a missing Polybrush component; preserving that scene took priority over cleanup during this unit.

## Relevant artifacts

- `Assets/Scenes/Desktop Companion.unity`
- `Assets/Scenes/Desktop Companion/NavMesh.asset`
- `Assets/Scenes/Main Scene.unity`
