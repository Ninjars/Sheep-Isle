---
type: index
status: active
updated: 2026-09-29
---

# Sheep Isle documentation

Start with the [project outline](project-outline.md) for current direction and progress. Read the relevant active plan before changing a feature or technical unit. Use reports for historical evidence and references for current technical guidance; neither overrides the roadmap.

The root [context glossary](../CONTEXT.md) defines settled Sheep Isle product language.

## Work hierarchy

- A **Milestone** is a coherent project checkpoint that leaves the game meaningfully closer to release.
- A **Feature** is a player-facing capability or experience with observable acceptance criteria.
- A **Unit** is an internal technical or production deliverable with independently verifiable acceptance criteria.
- A **Task** is the smallest atomic execution step within one feature or unit.

Features and units are sibling deliverables within a milestone. They may depend on one another but do not contain one another. A task is complete when its check passes. A feature or unit is complete only after its acceptance criteria have been observed, and a milestone is complete only when all of its required features and units are complete.

## Document authority

- `project-outline.md` is the only source of current product direction, milestone scope, and completion state.
- `plans/` contains current draft, active, or awaiting-validation plans. Plans state intended work; they are not evidence that work succeeded.
- `reports/` contains concise records of completed work and its validation evidence.
- `reference/` contains durable current technical guidance.
- `adr/` is created only when a decision is hard to reverse, surprising without context, and the result of a genuine trade-off.
- `CONTEXT.md` contains settled product vocabulary only. Documentation-process language belongs here.

Formal documents use stable descriptive kebab-case filenames and minimal YAML front matter. Allowed statuses are:

- `draft`: not yet approved for execution.
- `active`: approved and being executed, or current guidance.
- `awaiting-validation`: implemented, with acceptance evidence still outstanding.
- `completed`: a validated final record.
- `superseded`: retained but no longer authoritative.

Never mark a task, feature, unit, or milestone complete merely because implementation exists. Check it only after the named behaviour or acceptance criterion has been observed.

## Current documents

### Current direction, planning, and reference

- [Project outline](project-outline.md)
- [Initial flock plan](plans/initial-flock.md)
- [Navigation authoring reference](reference/navigation-authoring.md)

### Completed reports

- [Unity 6 migration](reports/unity-6-migration.md)
- [Unity 6 dependency portability](reports/unity-6-dependency-portability.md)
- [Desktop companion scene](reports/companion-scene.md)
- [Desktop window and controls](reports/desktop-window-and-controls.md)
- [macOS transparent-window feasibility](reports/macos-transparent-window-feasibility.md)
- [macOS desktop window and controls](reports/macos-desktop-window.md)
- [Documentation formalisation](reports/documentation-formalisation.md)

## Plan template

```markdown
---
type: plan
status: draft
milestone: milestone-slug
updated: YYYY-MM-DD
---

# Deliverable name

## Goal and classification

State the outcome and whether it is a Feature or Unit.

## Scope

## Exclusions

## Dependencies and decisions

## Tasks

## Acceptance criteria

## Validation method

## Open questions or risks
```

Omit `milestone` only for genuinely project-wide maintenance. Keep Tasks atomic and Acceptance criteria observable.

## Report template

```markdown
---
type: report
status: completed
milestone: milestone-slug
updated: YYYY-MM-DD
---

# Deliverable name

## Outcome

## Delivered scope

## Deviations and durable decisions

## Validation evidence

## Remaining risks or follow-up

## Relevant commits and artifacts
```

Validation evidence is mandatory. Omit other empty sections instead of adding boilerplate. Create a separate report only when work produced meaningful evidence, technical history, rejected approaches, or remaining risks; otherwise record completion briefly in the parent plan and roadmap.

After a completion report captures every durable decision and deviation, remove the completed plan from the working tree. Git retains the original planning artifact.
