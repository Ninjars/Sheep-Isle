---
type: plan
status: planned
milestone: desktop-prototype
updated: 2026-09-29
---

# macOS transparent-window feasibility unit

## Goal and classification

Resolve the **Unit**-level question of whether a standalone Unity 6 macOS player can reproduce the essential transparent desktop-companion window behaviour currently implemented with Win32 APIs. The result is evidence and a recommendation, not a production macOS implementation.

## Scope

- Research current Unity standalone-player behaviour and the relevant macOS AppKit window APIs using primary sources.
- Build the smallest disposable macOS player spike needed to test transparency and input behaviour on the current Apple silicon machine.
- Determine whether the player window can be borderless and transparent without exposing the magenta colour-key background used by Windows.
- Determine whether transparent background pixels can pass pointer input to applications behind the companion while visible island pixels retain Unity interaction.
- Identify viable equivalents for always-on-top state, window movement, saved position, hide and restore, and an exit affordance.
- Record the native-plugin boundary, Unity lifecycle hooks, architecture constraints, operating-system permissions, and known limitations that a production implementation would inherit.

## Exclusions

- Shipping or committing a production AppKit plugin as part of this unit.
- Declaring macOS a supported product platform before the spike evidence is reviewed.
- App Store distribution, sandbox entitlements, signing, notarisation, installer design, or automatic updates.
- Intel validation beyond documenting whether a universal native plugin would be required.
- Replacing or weakening the working Windows implementation.

## Probe approach

1. Establish the supported Unity-to-AppKit integration options and select the smallest reversible probe.
2. Create a development macOS player that applies transparent, borderless `NSWindow` configuration after Unity creates its native window.
3. Compare whole-window mouse ignoring with a per-frame or event-driven hit-test strategy that distinguishes island content from transparent background.
4. Exercise the player over representative light and dark desktop backgrounds at Retina scale.
5. Record observed behaviour, failures, API constraints, and the recommended production direction in a report linked from the project outline.

## Acceptance criteria

- [ ] A standalone Unity 6 macOS development player launches on the current Apple silicon machine with no visible title bar or opaque rectangular background.
- [ ] Desktop content remains visible through pixels outside the island without relying on the Windows magenta colour key.
- [ ] Pointer input reaches an application behind transparent background pixels while sheep and island interactions still work on visible content, or the report demonstrates with evidence why that combination is not attainable.
- [ ] The probe records observed focus, multi-window, Retina scaling, movement, always-on-top, hide/restore, and quit behaviour relevant to a desktop companion.
- [ ] The report names the required AppKit APIs and native integration boundary, distinguishes editor behaviour from standalone-player behaviour, and identifies Apple silicon versus universal-binary implications.
- [ ] The report recommends one of: proceed with a production macOS implementation, accept a documented reduced interaction model, or keep the product Windows-only.

## Validation evidence to retain

- Unity and macOS versions, machine architecture, build target, and exact player settings.
- Primary-source links supporting the selected AppKit and Unity integration approach.
- Screenshots or recordings over light and dark desktop backgrounds.
- A concise test matrix covering transparency, background click-through, island interaction, focus, movement, pinning equivalent, hide/restore, and exit.
- Any throwaway spike location and instructions for reproducing the result without treating the spike as production code.

## Open questions or risks

- AppKit can ignore mouse events for a whole window, but the unit must prove whether selective pass-through can coexist reliably with Unity interaction.
- Unity may recreate or reconfigure its native window during startup, resolution changes, focus changes, or display changes; the probe must identify when native settings can be applied safely.
- Transparent composition, shadows, colour space, and Retina backing scale may affect island edges differently from the Windows colour-key implementation.
- A status-bar item may be the closest macOS equivalent to the Windows tray flow, but that belongs to a later production design if transparency is feasible.
