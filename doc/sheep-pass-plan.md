# Desktop sheep: unit 2 plan

Updated 27 September 2026. This expands [unit 2 of the project outline](project-outline.md). The first sheep pass restores a small, calm flock without the old food, growth, or reproduction behavior. The original main scene and sheep prefab remain available for reference.

## Work slices

1. **Simplified sheep and navigation.** Make a new companion prefab from the old cuboid body and head, with four simple legs, a collider, and a NavMeshAgent. Attach the existing island NavMesh data explicitly and place a small authored starting flock. Keep the old SheepAgent, food targets, runtime foot spawning, and reproduction components out of the new prefab.
2. **Wandering.** Use bounded NavMesh destinations with idle pauses and a quiet breathing/trot motion. Keep sheep on the island and out of any missing NavMesh area; no food targeting. Confirm the agents move in the built scene.
3. **Baas and sound control.** Reuse the existing sheep voice clips. Space baa intervals across sheep, remember the sound-effects setting, and give the focused-window shortcut a brief on/off speaker icon below the island. Reserve a separate control and preference for music later.
4. **Petting.** Left-clicking a sheep makes it turn toward the camera and perform a short hop/head-tilt response, then resume its routine. Petting has no progression effect in this pass.
5. **Integrated check.** Build the isolated Unity 6 Windows player once after scene preparation. Check visible sheep, movement, baa/toggle feedback, petting, and coexistence with the desktop window controls. Copy validated assets and the player to the source project, update the outline, and make focused commits.

## Boundaries and tuning

- Keep the first flock small and fixed. Population, wool, accessories, lambing, and adventures belong to later passes.
- Place sheep with editor APIs and preserve scene and prefab metadata. Do not hand-edit Unity scene YAML.
- Start with the existing baked island NavMesh and adjust the walking radius and spawn points if the current geometry causes gaps. The replacement island will need a new bake.
- Favor a light 2D overlay for the sound state over a permanent interface. Test the icon against both light and dark desktop backgrounds when practical.
