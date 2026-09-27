# Desktop sheep: unit 2 plan

Updated 27 September 2026. This expands [unit 2 of the project outline](project-outline.md). The first sheep pass restores a small, calm flock without the old food, growth, or reproduction behavior. The original main scene and sheep prefab remain available for reference.

## Work slices

1. **Simplified sheep and navigation.** Make a new companion prefab from the old cuboid body and head, retaining the original free-floating square Foot prefab and its stepping animation. Keep a collider and NavMeshAgent. Attach the existing island NavMesh data explicitly and place a small authored starting flock. Keep the old SheepAgent, food targets, and reproduction components out of the new prefab.
2. **Wandering.** Use bounded NavMesh destinations with idle pauses and a quiet breathing/trot motion. Keep sheep on the island and out of any missing NavMesh area; no food targeting. Carve clearance around tree trunks and sizeable rocks on this mostly collider-free placeholder island. Confirm the agents move around scenery in the built scene.
3. **Baas and sound control.** Reuse the existing sheep voice clips and their variable-pitch character. Space baa intervals across sheep, remember the sound-effects setting, and give the focused-window `S` shortcut a brief on/off speaker icon below the island. Reserve a separate control and preference for music later.
4. **Petting.** Left-clicking a sheep makes it turn toward the camera and perform a short hop/head-tilt response, then resume its routine. Petting has no progression effect in this pass.
5. **Integrated check.** Build the isolated Unity 6 Windows player once after scene preparation. Check visible sheep, movement, baa/toggle feedback, petting, and coexistence with the desktop window controls. Copy validated assets and the player to the source project, update the outline, and make focused commits.

## Boundaries and tuning

- Keep the first flock small and fixed. Population, wool, accessories, lambing, and adventures belong to later passes.
- Place sheep with editor APIs and preserve scene and prefab metadata. Do not hand-edit Unity scene YAML.
- Start with the existing baked island NavMesh and adjust the walking radius and spawn points if the current geometry causes gaps. The replacement island will need a new bake.
- This pass gives petting a simple readable response. A more expressive sheep animation pass is a later visual refinement.
- Favor a light 2D overlay for the sound state over a permanent interface. Test the icon against both light and dark desktop backgrounds when practical.

## Progress

- The first isolated player showed the cuboid sheep, NavMesh wandering, sound icon, and petting response. The user confirmed sound and petting, requested the original independent square feet and more distinct baa pitches, and observed sheep passing through scenery.
- The corrected companion prefab spawns the original Foot prefab and uses its hop animation. Each sheep gets its own pitch range. Runtime NavMesh obstacles carve clearance around tree trunks and sizeable rocks. The updated Unity 6 player builds cleanly; the feet and route check in that player is pending.
- A later test player remained active while its window was unseen, and the user reported no obvious tray icon. That process was stopped. Hiding the island now stops current baas and suppresses new ones until it is shown; the updated Unity 6 build was copied to the source project and committed. The latest build still needs a user check of tray discoverability, independent foot motion, obstacle routes, pitch variation, saved sound preference, and hidden audio behavior. No test player is currently running.
- The user then checked the player and found that feet trail far behind and do not rest at the body corners. They also confirmed that the placeholder NavMesh still routes through trees, rocks, and water. Foot placement now waits until NavMesh relocation, uses corner anchors and look-ahead from the agent's steering, and takes the foot with the greatest target error. The transferred island's NavMeshSurface already loads its baked data, so the extra registration in the companion script has been removed. The remaining scenery problem belongs to the replacement island's authored bake; see the [navigation authoring reference](navigation-authoring.md). The revised foot behavior needs a new player check.
