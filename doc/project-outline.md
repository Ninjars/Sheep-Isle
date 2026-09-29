---
type: roadmap
status: active
updated: 2026-09-29
---

# Sheep Isle project outline

This is the sole current roadmap for the Windows desktop companion. Check a feature, unit, or milestone only after its acceptance behaviour has been observed. Detailed plans state intended work, reports preserve completed evidence, and references describe current technical guidance.

## Direction and current state

Sheep Isle is a calm floating island that lives on the desktop. Cuboid sheep wander, occasionally baa, and respond to attention. Interaction should be pleasant but optional, with a bounded flock and no care chores. The legacy food-placement and food-driven reproduction systems are not part of this direction.

The Windows desktop prototype is nearly complete. Its transparent window, controls, companion scene, and basic flock exist; a final integrated flock validation remains. A separate feasibility unit will determine whether the essential transparent-window experience can be reproduced in a standalone macOS player without committing the product to macOS support. The island in the companion scene is a placeholder for a larger biome-based replacement.

State persistence belongs to the feature or unit that owns the state. New documents and terminology follow the [documentation conventions](README.md) and root [product glossary](../CONTEXT.md).

## Milestone 1: Desktop prototype

Deliver a comfortable transparent desktop companion with a small living flock. The milestone remains open until the initial flock's outstanding player checks are observed and the macOS transparent-window feasibility question is resolved.

- [x] **Unit — Unity 6 migration.** Upgrade far enough to compile, open the companion scene, and build the Windows player while preserving the legacy main scene. See the [migration report](reports/unity-6-migration.md) and [dependency portability report](reports/unity-6-dependency-portability.md).
- [x] **Unit — Desktop companion scene.** Transfer the island geometry into a separate Unity scene with its own camera, lighting, and prototype navigation data. See the [scene report](reports/companion-scene.md).
- [x] **Feature — Desktop window and controls.** Provide borderless transparency, click-through empty pixels, pinning, window movement, camera orbit and zoom, saved state, and tray hide/restore/exit actions. See the [desktop window and controls report](reports/desktop-window-and-controls.md).
- [ ] **Feature — Initial flock.** Present three wandering cuboid sheep with baas, sound control and feedback, petting, and planted square feet. Core behaviour is implemented and partly observed; audio persistence, hidden silence, feedback legibility, tray restoration, and the integrated regression remain. See the [awaiting-validation plan](plans/initial-flock.md).
- [ ] **Unit — macOS transparent-window feasibility.** Research and prove whether a standalone Unity 6 macOS player can provide a borderless transparent window, background click-through, and retained island interaction comparable to the Windows shell. This unit evaluates feasibility only and does not yet commit the product to macOS support. See the [feasibility plan](plans/macos-transparent-window-feasibility.md).

### Desktop prototype decisions still needed

- Confirm that variable baa pitch is distinct, the sound-effects preference survives relaunch, and hiding the island produces silence.
- Confirm that the brief sound icon remains readable against light and dark desktop backgrounds.
- Confirm that the tray icon is discoverable and restores the current sheep player.
- Recheck sheep movement, audio, petting, click-through, pinning, camera controls, movement, hide/restore, and exit together in the final prototype player.
- Decide whether the macOS feasibility evidence justifies a production macOS window implementation and, if so, which AppKit integration boundary should own it.

## Milestone 2: Replacement island

Replace the transferred placeholder with a slightly larger island whose biome segments reveal distinct scenes as the camera rotates. Reserve a natural location for the Living world's interactive time control.

- [ ] **Feature — Biome island.** Design the biome layout, camera views, routes, scenery, and readable silhouette before replacing the placeholder geometry.
- [ ] **Unit — Authored sheep navigation.** Build walkable ground and solid trunk/rock footprints, mark ponds and streams non-walkable, bake one sheep surface, and validate body clearance and routes across biomes. Follow the [navigation authoring reference](reference/navigation-authoring.md).
- [ ] **Unit — Shared visual baseline.** Establish stylized materials, lighting, camera, and transparency settings that can support later day/night and weather states.
- [ ] **Feature — Responsive grass and flowers.** Create toon-shaded grass geometry dotted with flowers that reacts as sheep move through it.
- [ ] **Feature — Water and animated scenery.** Add ponds, streams, environmental assets, and subtle biome-appropriate animation without compromising transparent edges.
- [ ] **Feature — Island decorations.** Provide authored decoration locations that keep navigation routes and camera views clear.

### Replacement island decisions still needed

- Choose biome themes, island scale, camera compositions, and the routes connecting the scenes.
- Decide whether sheep scatter across the island, follow the viewed biome, or combine both behaviours.
- Set the visible body clearance, path widths, shoreline treatment, and rules for bridges or separated regions.
- Choose the location reserved for the future sundial, clock, or equivalent time control.
- Decide which decoration locations belong in the base island and how movable decorations affect navigation.

### Replacement island acceptance

- Sheep route around solid scenery and never cross authored water exclusions.
- Grass, water, scenery, and decorations remain readable throughout the camera limits.
- Essential geometry does not clip the colour-key window, and frame rate remains suitable for an idle desktop companion.

## Milestone 3: Flock progression and stories

Grow the flock without care chores or unbounded population. Cosmetics and story rewards should remain easy to use once unlocked.

- [ ] **Unit — Persistent flock state.** Define and save sheep identity, age, growth, applied cosmetics, adventure state, postcards, wool, and other progression state as their owning features arrive.
- [ ] **Feature — Sheep accessories.** Add hats, shoes, wool geometry styles, and wool dye textures, with unlocked items freely reusable across multiple sheep.
- [ ] **Feature — Bounded lambing and growth.** Allow recently petted, happy sheep to produce lambs only below an adult cap, then let lambs grow through time and attention.
- [ ] **Feature — Adventures and postcards.** At or above the adult cap, let an adult depart from an island location such as a magic floating dock and later return with a persistent, reviewable postcard featuring a sheep selfie and one new accessory unlock.
- [ ] **Feature — Wool economy.** Accumulate wool automatically, allow a small optional interaction bonus, and use wool for cosmetics or decorations without requiring repeated clicking.

### Flock progression decisions still needed

- Set the adult population cap and define the meaning and duration of “recently petted” and “happy.”
- Choose lamb growth timing, how growth is shown, and any offline-growth limit.
- Decide whether deliberate adoption or rehoming still has a role alongside bounded lambing and adventures.
- Define adventure timing, including whether the rough one-hour interval varies, and whether sheep may repeat adventures.
- Define postcard composition, image storage, review UI, file size, and persistence.
- Define accessory selection, saved application, unlock rules, and reuse across sheep.
- Set wool rates, prices, optional interaction bonus, offline accumulation limit, and exact uses.

## Milestone 4: Living world

Let the stable replacement island express time, weather, seasons, and music while remaining legible against varied desktop backgrounds.

- [ ] **Unit — Environmental state model.** Define daylight, dusk, night, representative weather, seasonal states, transitions, and the lighting/material parameters they control.
- [ ] **Feature — Day/night and time control.** Drive visual time from the system clock by default and let an in-world sundial or clock set a manual visual time with a clear return to live time.
- [ ] **Feature — Weather and seasons.** Present sunny, overcast, foggy, wet, and seasonal conditions, keeping effects within the island footprint where practical.
- [ ] **Unit — Optional real-weather integration.** Evaluate a free weather source, fixed or configurable location, update frequency, caching, and a graceful offline fallback before integration.
- [ ] **Feature — Music control.** Add music with a remembered control separate from sound effects.

### Living world decisions still needed

- Define how system time maps to island time and whether a manual time setting persists between launches.
- Define how the in-world control returns to the live clock and communicates its current mode.
- Choose how seasons advance and which environmental effects vary by biome or season.
- Choose the real-weather location policy, refresh interval, cached-data lifetime, and offline presentation.
- Set visual-quality and idle-performance budgets for combined night, fog, rain, and seasonal states.

### Living world acceptance

- Sheep, scenery, and controls remain legible throughout representative time, weather, and season combinations.
- Fog, rain, and other effects respect the island's visible footprint and camera limits without damaging transparent edges or click-through behaviour.
- A Windows player remains visually stable and suitably efficient during an extended idle check.
