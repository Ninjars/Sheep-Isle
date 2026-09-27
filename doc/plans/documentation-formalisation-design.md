---
type: design
status: active
updated: 2026-09-27
---

# Documentation formalisation design

## Purpose

Make Sheep Isle's planning and iteration documents easy for a new human contributor or coding agent to navigate. Preserve meaningful implementation and validation evidence while establishing a lightweight structure that remains useful as the project grows.

## Work hierarchy

- A **milestone** is a coherent project checkpoint that leaves the game meaningfully closer to release.
- A **feature** is a player-facing capability or experience with observable acceptance criteria.
- A **unit** is an internal technical or production deliverable with independently verifiable acceptance criteria.
- A **task** is the smallest atomic execution step within one feature or unit.

Features and units are sibling deliverables within a milestone. They may depend on one another but do not contain one another. A task is complete when its check passes; a feature or unit is complete only when its acceptance criteria have been observed. A milestone is complete when all of its required features and units are complete.

## Canonical structure

```text
doc/
├── README.md
├── project-outline.md
├── plans/
├── reports/
├── reference/
└── adr/

CONTEXT.md
```

- `doc/README.md` is the entry point and defines the documentation conventions.
- `doc/project-outline.md` is the only source of current product direction and milestone status.
- `doc/plans/` contains current draft, active, or awaiting-validation plans.
- `doc/reports/` contains concise, evidence-backed records of completed work.
- `doc/reference/` contains durable current technical guidance.
- `doc/adr/` is created lazily when a decision is hard to reverse, surprising without context, and the result of a real trade-off.
- `CONTEXT.md` contains settled product language only. Process terminology belongs in `doc/README.md`.

Formal documents use stable descriptive kebab-case names. Minimal YAML front matter records `type`, `status`, `milestone` where applicable, and `updated`. Allowed statuses are `draft`, `active`, `awaiting-validation`, `completed`, and `superseded`.

## Roadmap milestones

1. **Desktop prototype**: Unity 6 migration, companion scene, desktop window and controls, and initial flock.
2. **Replacement island**: biome layout, authored navigation, visual baseline, grass, water, scenery, and decoration foundations.
3. **Flock progression and stories**: persistent flock state, accessories, lambing and growth, adventures and postcards, and wool economy.
4. **Living world**: day/night, interactive time control, weather, seasons, music, and environmental presentation.

Unresolved choices live with the milestone, feature, or unit that owns them. Only genuinely project-wide decisions belong in a global section. Add a release or polish milestone only after its scope is known.

## Plans and reports

A substantial feature or unit receives a plan with:

- goal and classification;
- scope and explicit exclusions;
- dependencies and settled decisions;
- atomic tasks;
- acceptance criteria;
- validation method; and
- open questions or risks.

A separate completion report is warranted when work produces meaningful validation evidence, technical history, rejected approaches, or remaining risks. A report records:

- outcome;
- delivered scope;
- deviations and durable decisions;
- validation evidence;
- remaining risks or follow-up; and
- relevant commits and artifacts.

Never omit validation evidence from a completed report. Omit other empty sections rather than adding boilerplate. Small work may be completed directly in its parent plan. Once a completion report preserves every durable decision and deviation, remove the corresponding completed plan from the working tree; Git retains its original form.

## Existing-document migration

- Consolidate `desktop-controls-plan.md` and `desktop-window-pass.md` into `reports/desktop-window-and-controls.md`.
- Convert `scene-pass.md` into `reports/companion-scene.md`.
- Convert `sheep-isle-handoff.md` into `reports/unity-6-migration.md`, replacing duplicated scene and window results with links to their reports.
- Move and revise `sheep-pass-plan.md` as `plans/initial-flock.md` until its remaining acceptance checks are complete.
- Move and revise `navigation-authoring.md` as `reference/navigation-authoring.md`.
- Restructure `project-outline.md` around the four milestones and point its deliverables to the canonical plan, report, and reference documents.
- Update `AGENTS.md` so a new session starts with the documentation index, roadmap, and relevant active plan instead of a stale global handoff.

Preserve material technical history and validation evidence. Remove stale forecasts, contradicted status claims, redundant narration, the broken `scene-pass-report.md` link, and links to the unavailable `island-scene-preview/desktop-scene.png` artifact. Preserve the preview result as textual evidence without claiming the missing image remains accessible. Git history, rather than an archive directory, preserves the superseded source documents.

## Initial domain language

The root `CONTEXT.md` initially defines Sheep Isle, desktop companion, island, and flock. Add lamb, adventure, postcard, accessory, wool, and other product terms only after their meanings become precise.

## Decision-record policy

This documentation reorganisation does not need an ADR: its rationale is visible in the documentation index, and the structure is inexpensive to revise. Apply the ADR threshold to future architectural or product decisions rather than creating decision records by default.
