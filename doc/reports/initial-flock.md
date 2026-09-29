---
type: report
status: completed
milestone: desktop-prototype
updated: 2026-09-29
---

# Initial flock

## Outcome

The desktop companion presents a fixed flock of three cuboid sheep that wander and idle without food targets, occasionally baa with distinct pitch variation, and respond to petting. The final Windows player passed the flock-specific and integrated desktop-companion acceptance checks, completing the Initial flock feature and the Desktop prototype milestone.

## Delivered scope

- Three simplified companion sheep reuse the original cuboid body, head, voice clips, and independent square feet without bringing the legacy food and reproduction coupling into the new product direction.
- Sheep wander within the placeholder island's NavMesh, pause naturally, breathe, trot, and place their feet beneath and ahead of their moving bodies.
- Occasional baas vary by sheep. `S`, while the player has focus, toggles a remembered sound-effects preference and briefly displays its current state below the island.
- Hiding the desktop companion stops active baa playback and prevents another baa from starting while hidden.
- Clicking a sheep turns it toward the camera and produces a brief hop and head-tilt response before wandering resumes.

## Deviations and durable decisions

- Petting is visual feedback only. It has no progression effect in this feature.
- `S` controls sound effects; music will have a separate control and remembered preference in the Living world milestone.
- The original independent square feet were retained after an earlier simplified treatment lacked the intended character. Foot relocation occurs after NavMesh movement, using corner anchors, steering look-ahead, and the foot with the greatest target error.
- The placeholder island's approximate runtime obstacles do not reliably keep sheep out of large trees, rocks, or water. This accepted prototype debt belongs to the Replacement island's Authored sheep navigation unit, not the completed flock feature.
- Keyboard shortcuts remain focus-dependent.

## Validation evidence

- Earlier player passes established three-sheep NavMesh wandering, idle behaviour, sound feedback, click-to-pet response, and the revised square-foot placement. The user confirmed the final foot placement on 27 September 2026.
- On 29 September 2026, after being given the complete outstanding checklist, the user reported that the Windows player behaviour had been validated. This closes the observed player checks for distinct baa pitch, sound preference persistence after relaunch, silence while hidden, sound-icon legibility against light and dark backgrounds, tray discoverability and restoration, and the integrated regression.
- The integrated regression covered sheep movement, audio, petting, transparent-pixel click-through, pinning, window movement, camera orbit and zoom, hide/restore, and exit in the final Windows player.

## Remaining risks or follow-up

- Final route clearance around solid scenery and authored water exclusions remains intentionally deferred to the Replacement island milestone.
- Camera framing, transparent edges, and idle performance need to be revalidated when the placeholder island is replaced.

## Relevant commits and artifacts

- `16db965` — bring the original sheep into the desktop companion scene.
- `3a0a22e` — stop sheep audio while the island is hidden.
- `c46201c` — correct sheep feet and document the navigation bake.
- `90d5855` — record validated sheep foot placement.
- `Assets/DesktopCompanion/Companion Sheep.prefab`
- `Assets/DesktopCompanion/CompanionFloatingFeet.cs`
- `Assets/DesktopCompanion/CompanionSheep.cs`
- `Assets/DesktopCompanion/CompanionSheepClick.cs`
- `Assets/DesktopCompanion/CompanionSoundSettings.cs`
- `Assets/Scenes/Desktop Companion.unity`
