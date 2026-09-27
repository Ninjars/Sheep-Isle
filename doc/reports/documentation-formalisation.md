---
type: report
status: completed
updated: 2026-09-27
---

# Documentation formalisation

## Outcome

Sheep Isle now has one documentation entry point, one current milestone roadmap, a product glossary, one active feature plan, durable technical guidance, and focused completion reports. The structure serves both human contributors and coding agents without treating historical narration as current direction.

## Delivered scope

- Added `doc/README.md` as the documentation map and convention guide, including the milestone/feature/unit/task hierarchy, controlled statuses, authority rules, and copyable plan and report templates.
- Added root `CONTEXT.md` with the settled meanings of Sheep Isle, desktop companion, island, and flock.
- Restructured `doc/project-outline.md` into four milestones: Desktop prototype, Replacement island, Flock progression and stories, and Living world.
- Localized unresolved decisions under the milestone that owns them and moved state persistence, wool, decorations, and music out of the former miscellaneous backlog.
- Updated `AGENTS.md` so new sessions read the glossary, documentation map, roadmap, and relevant active plan. Reports supply historical evidence; references supply current technical guidance.
- Established `doc/plans/`, `doc/reports/`, and `doc/reference/`. The `doc/adr/` directory remains intentionally absent until a decision meets the documented threshold.

## Source-to-canonical mapping

- `doc/sheep-pass-plan.md` became `doc/plans/initial-flock.md`, an awaiting-validation Feature plan.
- `doc/navigation-authoring.md` became `doc/reference/navigation-authoring.md`, the current Replacement island navigation guidance.
- `doc/scene-pass.md` became `doc/reports/companion-scene.md`.
- `doc/desktop-controls-plan.md` and `doc/desktop-window-pass.md` were consolidated into `doc/reports/desktop-window-and-controls.md`.
- `doc/sheep-isle-handoff.md` was reduced to migration-specific evidence in `doc/reports/unity-6-migration.md`; current product direction lives only in the roadmap.

Git retains the superseded source documents. This mapping is explicit because the substantial rewrites of the short sheep and scene documents fell below Git's automatic rename-similarity threshold.

## Deviations and durable decisions

- The old handoff and scene notes linked to a desktop-scene preview image that is absent from the repository. The canonical reports retain the observed Windows preview result without claiming the missing artifact is accessible.
- The handoff also linked to a nonexistent `scene-pass-report.md`; canonical links now target the completed scene report.
- Deleted-source pathspecs cannot be passed directly to `git add` after the files have moved or been removed. Those slices used `git add -A -- doc` after reviewing the staged name list, keeping each commit scoped to documentation.
- No ADR was created. This organization is explained by the index and is inexpensive to revise, so it does not meet the project's ADR threshold.
- No Unity assets, packages, settings, scenes, prefabs, or generated files changed during this unit.

## Validation evidence

- A repository-local Markdown audit resolved every non-HTTP link in root and `doc/**/*.md` files and completed with no missing targets.
- A superseded-path scan found no links to the removed handoff, source reports, source plans, or unavailable preview image outside the one-time migration artifacts.
- Every YAML `status` value was checked against `draft`, `active`, `awaiting-validation`, `completed`, and `superseded`.
- `git diff --check` reported no whitespace errors throughout the migration.
- The Desktop prototype roadmap state was compared with its canonical artifacts: the migration, companion scene, and desktop window/controls are completed; the initial flock remains unchecked and `awaiting-validation` while its audio, feedback-legibility, tray, and integrated player checks remain open.

## Remaining risks or follow-up

- Documentation quality still depends on keeping the roadmap and relevant plan current in the same coherent slice as future work.
- The initial flock's pending player checks remain the next validation work; this documentation unit does not mark them complete.
- Add domain terms to `CONTEXT.md` only when their meanings settle. Add an ADR only when the decision is hard to reverse, surprising without context, and the result of a genuine trade-off.

## Relevant commits and artifacts

- `3a84182` — record the approved design, implementation plan, and initial domain glossary.
- `3af3a50` — define the documentation conventions and index.
- `dda0028` — classify the active flock plan and navigation reference.
- `0f3e715` — formalize the companion-scene report.
- `534ccb5` — consolidate desktop-window and control evidence.
- `28686df` — preserve focused Unity 6 migration evidence.
- `408627a` — organize the roadmap by product milestones.
- `39d6070` — point new sessions at canonical guidance.
- The final documentation migration commit retires the one-time documentation design and migration plan.
