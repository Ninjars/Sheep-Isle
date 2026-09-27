# Sheep Isle project outline

Updated 27 September 2026. This is the living, high-level plan for the Windows desktop companion. Check an item only after its behavior has been validated; add or revise items as the design develops. Link detailed design, implementation, and test notes from the relevant section.

## Direction and current state

Sheep Isle is a calm floating island that lives on the desktop. Cuboid sheep wander, occasionally baa, and respond to attention. Interaction should be pleasant but optional, with a bounded flock and no care chores. The original food placement and food-driven reproduction systems are not part of this direction. The island currently in the companion scene is a placeholder.

- [x] Upgrade enough of the project to compile and build with Unity 6. See the [migration handoff](sheep-isle-handoff.md) and [scene pass](scene-pass.md).
- [x] Create a separate desktop companion scene containing the transferred island geometry, camera, light, and navigation surface. Keep the original main scene intact. See the [scene pass](scene-pass.md).
- [x] Prove a Windows transparent player with click-through empty pixels, pin toggle, window dragging, and exit control. The user confirmed click-through, pinning, dragging, and exit. See the [desktop window pass](desktop-window-pass.md).
- [x] Remove the visible title bar and border from the player while retaining transparent-pixel click-through. Verified in a Windows player on 27 September 2026; see the [desktop window pass](desktop-window-pass.md).

## 1. Finish the desktop window and camera

The window and camera controls are implemented. Keep the player comfortable to use alongside other Windows applications. See the [desktop controls plan](desktop-controls-plan.md) for decisions, work slices, and validation.

- [x] Provide a tray icon with Show/Hide and Exit, plus a focused-window `H` shortcut to hide the island. `H` hid the Windows player; the user confirmed tray Show and Exit.
- [x] Remember window position, pin state, camera angle, and zoom across launches, with sensible recovery if a saved position is off screen. Restart checks passed for zoom, pin, user-dragged position, and off-screen recovery.
- [x] Move the window with a **middle-button drag** instead of right-button drag. User checked the new control in a Windows player.
- [x] Rotate the camera around the island with a **right-button drag**, with a limited vertical angle. User checked the feel and requested a low-angle limit of −10 degrees.
- [x] Switch to a perspective camera and move it toward and away from the island with the **mouse wheel**. The new controller draws on the distance-related input and smoothing in [`MouseOrbiterImproved.cs`](../Assets/Game/MouseOrbiterImproved.cs); user checked the zoom feel in a Windows player.
- [x] Recheck click-through, framing, edge quality, pinning, input focus, and longer-running performance in the Windows build after these controls change. The borderless player stayed responsive, a transparent-pixel click reached the desktop, and the user confirmed camera framing at the low-angle and zoom limits.
- [ ] Recheck that the tray icon is discoverable and can restore the current sheep player. A test player kept running and baaing without an obvious tray icon on 27 September; an interactive launch check is pending.

## 2. Bring back the sheep, simply

Keep the old food and reproduction coupling out of the new scene. Petting in this pass is a visual response, without additional progression requirements. Preserve the original free-floating square feet. The replacement island needs an authored NavMesh that steers sheep around trees, rocks, and water. A more expressive petting animation can follow in a later visual pass. See the [desktop sheep plan](sheep-pass-plan.md) for work slices and validation.

- [x] Migrate a simplified cuboid sheep prefab and a three-sheep flock into the companion scene, using the original free-floating square Foot prefab. The Unity 6 player builds and shows sheep; foot placement was confirmed below.
- [x] Add bounded NavMesh wandering with idle pauses. The user observed sheep moving in the first player. Approximate runtime clearance was added, but the user confirmed sheep still cross large trees, rocks, and water on the placeholder island.
- [x] Restore occasional baas and add a focused-window `S` sound-effects toggle with brief on/off feedback. The user confirmed playback and the toggle in the first player. Individual pitch variation, the saved setting, and silence while hidden are implemented in the newer build.
- [x] Let a click pet a sheep: it turns toward the camera and performs a short hop and head tilt. The user confirmed this response is fine for the first pass; a richer animation remains later polish.
- [x] Check the revised feet in a player: they settle beneath the body corners and plant ahead of the sheep's motion instead of trailing. The user confirmed the positioning on 27 September 2026.
- [ ] Check variable baa pitch, sound preference after relaunch, and silence while the island is hidden.
- [ ] Recheck sheep movement, audio, and petting alongside the desktop window controls in the final unit 2 player.

## 3. Design and build the replacement island

The transferred island is a prototype. Build a slightly larger island with distinct biome segments that camera rotation can reveal as different scenes.

The prototype NavMesh omits much of the visible scenery and water. Use the [navigation authoring reference](navigation-authoring.md) when designing the replacement island; the current runtime obstacle shapes are a temporary approximation.

- [ ] Sketch the biome layout, camera views, routes, and sheep navigation before replacing the placeholder geometry.
- [ ] Reserve an island location for an in-world time control, such as a sundial or clock, as part of the biome layout.
- [ ] Decide whether sheep scatter across the island, follow the camera between biomes, or combine the two behaviors.
- [ ] Author and bake sheep navigation from deliberate walkable ground and solid trunk/rock footprints, with ponds and streams marked non-walkable. Validate routes and body clearance across biomes. See the [navigation authoring reference](navigation-authoring.md).
- [ ] Establish a shared stylized material and lighting baseline for the new biomes, including camera and transparency settings that can support the later day/night and weather pass.
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

## 5. Give the island time, weather, and seasons

Build this visual pass once the replacement island's geometry and materials are stable. It can proceed before or alongside the later flock systems. Keep effects readable against different desktop backgrounds and within the transparent window.

- [ ] Define the visual treatment for daylight, dusk, night, and representative weather across the biome camera views.
- [ ] Add a stylized day/night cycle driven by the system clock by default. Tune environmental lighting, camera settings, and shaders so the island and sheep remain legible throughout the cycle.
- [ ] Make the planned sundial or clock interactive so it can set the visual time of day, with a clear way to return to the live clock.
- [ ] Create weather and seasonal visual states, including sunny, overcast, foggy, and wet conditions. Decide which effects vary by biome and season before adding them.
- [ ] Keep fog, rain, and other effects within the island's visible footprint where practical; check transparent edges, click-through, and clipping at camera and zoom limits.
- [ ] Investigate free weather data sources for an optional real-weather default. A fixed location is acceptable initially; decide update frequency and a graceful offline fallback before integrating a source.
- [ ] Validate the day/night and weather combinations in a Windows player for readability, visual quality, and idle performance.

## Supporting idle systems and presentation

Place these into playable slices as the core interactions settle. Earlier adoption and rehoming ideas need review against the newer lambing and adventure loop before implementation.

- [ ] Add passive wool accumulation. Consider a small optional click bonus without making repeated clicking necessary; define rates and any offline limit.
- [ ] Decide what wool buys or unlocks once the accessory and decoration loops are clearer.
- [ ] Add island decorations at authored placement spots, keeping navigation and views clear.
- [ ] Add music with its own remembered toggle, separate from the sound-effects toggle.
- [ ] Persist flock, lamb growth, accessories, postcards, wool, decorations, window position, pin state, and audio preferences as the relevant systems arrive.
- [ ] Decide whether deliberate adoption or rehoming still has a role alongside bounded lambing and adventures.

## Open design choices

- Tray icon discoverability in the current sheep player. Hide/restore and camera persistence decisions are in the [desktop controls plan](desktop-controls-plan.md).
- Biome themes, island scale, and sheep behavior when the camera changes scenes.
- Flock cap, the meaning and duration of “recently petted,” lamb growth timing, and adventure timing.
- How postcard images are composed, stored, and reviewed, including save size and repeat adventures.
- Wool rates, prices, optional click bonus, offline progress, and where the idle loop fits in the rollout.
- How the clock's manual time setting returns to system time, whether it persists, and how system time maps to the island's day/night cycle.
- How seasons advance, which location any optional real-weather lookup uses, and what the island shows when weather data is unavailable.
