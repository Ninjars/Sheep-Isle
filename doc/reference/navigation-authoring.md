---
type: reference
status: active
milestone: replacement-island
updated: 2026-09-27
---

# Navigation authoring for the replacement island

This is current technical guidance for the Replacement island [milestone](../project-outline.md), based on the Unity 6 project and AI Navigation 2.0.14 package. It does not prescribe final paths or biome shapes.

## What the transferred island does now

The `Island Navigation` object in `Assets/Scenes/Desktop Companion.unity` has a `NavMeshSurface` using **All Game Objects**, **Physics Colliders**, and only layer 9 (`terrain`). Its baked data is `Assets/Scenes/Desktop Companion/NavMesh.asset`. This means the surface is built from terrain-layer colliders, not from everything visible in the scene. Layers on parent groups do not make their children part of the layer filter.

The prominent `ZLPP_Plant_A_6` tree and representative rocks are on layer 0 and have render meshes but no colliders. The `water pool` is also on layer 0; it has a MeshCollider, but the surface excludes that layer. Consequently, the baked surface does not encode the visible tree trunks, most rocks, or water. The sheep can have a valid NavMesh path through those objects. The current `DesktopSheepObstacles` script adds approximate runtime holes around some trunks and rocks, but its tree radius is capped at 1.35 units, it does not cover water, and it cannot reliably represent the larger tree's true footprint. This is a placeholder measure, not the navigation design for the new island.

The package's `NavMeshSurface` registers its own baked data when enabled. Registering the same asset again through `NavMesh.AddNavMeshData` duplicated the surface in the prototype; the companion activation script has been changed to reference the existing surface instead.

## Proposed setup for the new island

1. **Author navigation geometry separately from presentation.** Put the walkable island surface and simple solid footprints for large trunks, rocks, walls, docks, and other blocking objects under a `Navigation Geometry` hierarchy. Keep flowers, grass blades, mushrooms, tree crowns, particles, and purely decorative water meshes outside that hierarchy. A trunk proxy should match where a sheep's body must not go, rather than the full canopy bounds.
2. **Bake one sheep surface.** Add a `NavMeshSurface` to that hierarchy, set **Collect Objects** to **Current Object Hierarchy**, and **Use Geometry** to **Physics Colliders**. Put terrain and blocker proxies on included layers; keep rendering-only objects on excluded layers. Alternatively, the package offers **NavMeshModifier Component Only** for an explicitly tagged set of inputs. Avoid mixing both schemes without a reason. Keep one active owner for the baked data; do not also call `NavMesh.AddNavMeshData` for the same asset.
3. **Size the sheep at bake time.** Create or tune a sheep Agent Type with a radius that gives the cuboid body and floating feet visual clearance from trunks and ledges, and use that Agent Type on both the surface and `NavMeshAgent`. Check height, slope, and step settings against the terrain. Agent radius in the bake determines the walkable centerline clearance; changing only the runtime agent radius will not repair a surface baked with inadequate clearance.
4. **Make water explicitly non-walkable.** Place `NavMeshModifierVolume` regions marked **Not Walkable** over ponds and streams, extending through the ground beneath them. A visual water mesh or collider on an excluded layer does not remove walkability from terrain below. If there will later be bridges, keep them as authored walkable geometry above the water exclusion and inspect the resulting connectivity.
5. **Treat static and movable blockers differently.** Bake the footprints of fixed trees and rocks. Use `NavMeshModifier` for local area overrides or to keep a particular mesh out of the bake. Reserve carved `NavMeshObstacle` components for objects that genuinely appear or move during play, such as a placed decoration; carving updates after the obstacle changes rather than becoming part of the baked static geometry.
6. **Keep foot probes separate.** Floating feet should raycast against actual ground colliders, not foliage, water, or blocker proxies. Give the foot-ground probe a dedicated layer mask. The NavMesh answers where a sheep may travel; a ground raycast answers where a foot should plant.

## Bake and validation checklist

- Build the terrain and blocking proxies, then bake the `NavMeshSurface`. Inspect the blue navigation overlay from above and at sheep eye level. Check the large trunks, rock clusters, shoreline, water interiors, docks, and narrow paths rather than only the island perimeter.
- Place a sheep-sized test agent near each biome and sample destinations on both sides of major obstacles. Inspect complete paths and their corners; verify that a route skirts solid scenery and never crosses a pond. Check intentionally separated regions and any authored links.
- Recheck margins against the visible body and feet while a few sheep wander in a Windows player. A path that clears the agent center can still look like a body clipping scenery if the bake radius or proxy footprint is too small.
- Re-bake after changing terrain, proxies, modifier volumes, or agent build settings. Keep the baked `NavMeshData` committed with the scene and verify the player loads that exact asset.

## Unity references

- [NavMesh Surface: collection, layers, geometry, and bake settings](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshSurface.html)
- [NavMesh Modifier](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshModifier.html) and [Modifier Volume](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshModifierVolume.html)
- [NavMesh Obstacles and carving](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshObstacle.html)
- [Create and re-bake a NavMesh](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/CreateNavMesh.html)
