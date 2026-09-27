---
type: plan
status: awaiting-validation
milestone: desktop-prototype
updated: 2026-09-27
---

# Initial flock feature plan

## Goal and classification

Deliver a small, calm **Feature** in which cuboid sheep wander, occasionally baa, and respond to petting inside the desktop companion. The feature is implemented; the remaining work is an integrated player validation.

## Scope

- A fixed authored flock of three simplified sheep in the companion scene.
- Bounded wandering with idle pauses on the placeholder island's existing NavMesh.
- Occasional characterful baas and a remembered sound-effects preference controlled by `S` while the player has focus, with a brief speaker icon below the island for on/off feedback.
- A brief visual petting response when a sheep is clicked.
- The original free-floating square feet, positioned and stepped for the companion movement.
- Correct interaction with the desktop window's hide, restore, camera, and input behaviour.

## Exclusions

- Food targeting, food placement, growth, and food-driven reproduction from the legacy game.
- Wool, accessories, lambing, adventures, and population management.
- Final scenery and water avoidance on the placeholder island. Deliberately authored navigation belongs to the Replacement island milestone; see the [navigation authoring reference](../reference/navigation-authoring.md).
- A richer petting animation beyond the readable first response.

## Dependencies and decisions

- The companion prefab reuses the old cuboid body, head, voice clips, and independent square `Foot` prefab without using `SheepAgent` or its food and reproduction coupling.
- `NavMeshSurface` owns and registers its baked data. Companion activation must not register the same `NavMeshData` a second time.
- Petting is visual feedback only and has no progression effect in this feature.
- `S` controls sound effects; music will have a separate control and preference later.
- Sound feedback remains a light 2D overlay and must be legible against light and dark desktop backgrounds.
- Hiding the desktop companion must stop active baa playback and prevent another baa from starting while hidden.
- Scene and prefab changes use Unity editor APIs so serialized references and metadata remain valid.

## Tasks

- [x] Create the simplified companion sheep prefab and place the fixed three-sheep flock.
- [x] Add bounded NavMesh wandering, idle pauses, breathing, and trot movement.
- [x] Restore occasional baas and focused-window `S` feedback.
- [x] Add the click-to-pet turn, hop, and head-tilt response.
- [x] Restore and tune the independent square feet so they plant beneath and ahead of the moving body.
- [ ] Validate variable baa pitch, remembered sound preference after relaunch, and silence while hidden.
- [ ] Confirm the brief speaker icon remains legible against light and dark desktop backgrounds.
- [ ] Validate tray discoverability and restoration with the current sheep player.
- [ ] Run the integrated sheep, audio, petting, camera, window-control, and transparency regression.

## Acceptance criteria

- [x] Three recognisable cuboid sheep appear and move on the island.
- [x] Sheep alternate between wandering and idle behaviour without food targets.
- [x] A sheep occasionally baas, and `S` produces clear sound on/off feedback.
- [x] Clicking a sheep makes it face the camera and perform a brief petting response before resuming.
- [x] The square feet rest near the body corners and plant ahead of motion rather than trailing.
- [ ] Distinct baa pitch is audible, the sound preference survives relaunch, and silence while hidden is observed in a player.
- [ ] The sound icon is readable against representative light and dark desktop backgrounds.
- [ ] The tray icon is discoverable and restores the current sheep player.
- [ ] The final player preserves sheep behaviour alongside click-through, pinning, movement, camera controls, hide/restore, and exit.

## Validation

- The first isolated player showed the flock, NavMesh wandering, sound feedback, and petting. The user confirmed sound and petting, then requested the original independent feet, stronger pitch variation, and better obstacle handling.
- A later player restored the original feet and per-sheep pitch variation, but the feet trailed behind the body and did not settle at its corners. Sheep also continued to cross trees, rocks, and water because the placeholder island lacks deliberately authored navigation.
- The current build relocates feet only after NavMesh movement, uses corner anchors and steering look-ahead, and steps the foot with the greatest target error. The user confirmed the revised foot placement on 27 September 2026.
- Commit `3a0a22e` stops active baa playback and prevents a new baa while the window is hidden. The updated Unity 6 player built successfully, but the behaviour still needs player observation.

## Open questions or risks

- The placeholder island's approximate runtime obstacles do not reliably keep sheep out of large trees, rocks, or water. This is accepted prototype debt and must not be mistaken for completion of the Replacement island navigation unit.
- A shell-launched test player previously remained active and audible without an obvious tray icon. The process was stopped; discoverability and restoration need an interactive launch check.
- Keyboard shortcuts require player focus, so the integrated check must distinguish focus behaviour from a broken toggle.
