# Sheep Isle project outline

Updated 27 September 2026. This is the living, high-level plan for the Windows desktop companion. Check an item only after its behavior has been validated; add or revise items as the design develops. Link detailed design, implementation, and test notes from the relevant section.

## Direction and current state

Sheep Isle is a calm floating island that lives on the desktop. Cuboid sheep wander, occasionally baa, and respond to attention. Interaction should be pleasant but optional, with a bounded flock and no care chores. The original food placement and food-driven reproduction systems are not part of this direction. The island currently in the companion scene is a placeholder.

- [x] Upgrade enough of the project to compile and build with Unity 6. See the [migration handoff](sheep-isle-handoff.md) and [scene pass](scene-pass.md).
- [x] Create a separate desktop companion scene containing the transferred island geometry, camera, light, and navigation surface. Keep the original main scene intact. See the [scene pass](scene-pass.md).
- [x] Prove a Windows transparent player with click-through empty pixels, pin toggle, window dragging, and exit control. The user confirmed click-through, pinning, dragging, and exit. See the [desktop window pass](desktop-window-pass.md).
- [ ] Remove the visible title bar and border from the current player. The transparent click-through behavior must survive this change.

## 1. Finish the desktop window and camera

This is the next implementation pass. Keep the player comfortable to use alongside other Windows applications.

- [ ] Provide a convenient hide and restore control; decide how to restore when the island is hidden.
- [ ] Remember window position and pin state across launches, with sensible recovery if a saved position is off screen.
- [ ] Move the window with a **middle-button drag** instead of the current right-button drag.
- [ ] Rotate the camera around the island with a **right-button drag**. Define sensible angle limits and input behavior over transparent pixels.
- [ ] Move the camera toward and away from the island with the **mouse wheel**. Review [`MouseOrbiterImproved.cs`](../Assets/Game/MouseOrbiterImproved.cs) from the original scene for its distance-scaled orbit input and smoothed positioning; adapt the useful feel to the new camera and window.
- [ ] Recheck click-through, framing, edge quality, pinning, input focus, and longer-running performance in the Windows build after these controls change.

## 2. Bring back the sheep, simply

Keep the old food and reproduction coupling out of the new scene. Petting in this pass is a visual response, without additional progression requirements.

- [ ] Migrate a simplified cuboid sheep prefab into the companion scene.
- [ ] Migrate wandering behavior suitable for the island navigation surface.
- [ ] Restore occasional baas. Add a focused-window keyboard toggle for sound effects, with a brief sound-on or sound-off icon below the island after each press. Remember the setting.
- [ ] Let a click pet a sheep: it should look toward the camera and perform a cute animated response.
- [ ] Validate sheep movement, audio, and petting in a Windows player while the desktop window controls remain usable.

## 3. Design and build the replacement island

The transferred island is a prototype. Build a slightly larger island with distinct biome segments that camera rotation can reveal as different scenes.

- [ ] Sketch the biome layout, camera views, routes, and sheep navigation before replacing the placeholder geometry.
- [ ] Decide whether sheep scatter across the island, follow the camera between biomes, or combine the two behaviors.
- [ ] Create toon-shaded grass geometry dotted with flowers that reacts as sheep pass through it.
- [ ] Add more environmental assets and subtle animations appropriate to each biome.
- [ ] Add animated water for ponds, streams, and other water features.
- [ ] Check readability, frame rate, and transparent window edges across the new views.

## 4. Grow the flock and its stories

These systems are later design passes. Keep population bounded and accessories easy to use once unlocked.

- [ ] Add sheep accessories: hats, shoes, wool geometry styles, and wool dye textures. Decide how selection and persistence work.
- [ ] Define the adult population cap and a controlled lambing rule: recently petted, happy sheep may produce lambs only below the cap.
- [ ] Let lambs grow into adults with time and attention; decide the timing and how growth is shown.
- [ ] Add an island departure point, such as a magic floating dock for flying boats. At or above the adult cap, allow an adult sheep to go on an adventure.
- [ ] After a real-time interval (roughly an hour, possibly randomized), deliver a persistent, reviewable postcard image with a sheep selfie and one new accessory unlock.
- [ ] Let any unlocked accessory be applied freely to multiple sheep.

## Supporting idle systems and presentation

Place these into playable slices as the core interactions settle. Earlier adoption and rehoming ideas need review against the newer lambing and adventure loop before implementation.

- [ ] Add passive wool accumulation. Consider a small optional click bonus without making repeated clicking necessary; define rates and any offline limit.
- [ ] Decide what wool buys or unlocks once the accessory and decoration loops are clearer.
- [ ] Add island decorations at authored placement spots, keeping navigation and views clear.
- [ ] Add music with its own remembered toggle, separate from the sound-effects toggle.
- [ ] Persist flock, lamb growth, accessories, postcards, wool, decorations, window position, pin state, and audio preferences as the relevant systems arrive.
- [ ] Decide whether deliberate adoption or rehoming still has a role alongside bounded lambing and adventures.

## Open design choices

- Exact window size, hide/restore mechanism, camera limits, and whether camera orientation and distance persist.
- Biome themes, island scale, and sheep behavior when the camera changes scenes.
- Flock cap, the meaning and duration of “recently petted,” lamb growth timing, and adventure timing.
- How postcard images are composed, stored, and reviewed, including save size and repeat adventures.
- Wool rates, prices, optional click bonus, offline progress, and where the idle loop fits in the rollout.
