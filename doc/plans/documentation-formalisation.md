---
type: plan
status: draft
updated: 2026-09-27
---

# Documentation Formalisation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace Sheep Isle's overlapping planning and iteration notes with one navigable roadmap, current plans, durable references, and evidence-backed completion reports without losing validated history.

**Architecture:** `doc/README.md` will define the documentation system, while `doc/project-outline.md` remains the sole current product roadmap. Active work, completed evidence, and durable guidance will live in `doc/plans/`, `doc/reports/`, and `doc/reference/` respectively; the root `CONTEXT.md` owns product vocabulary.

**Tech Stack:** Markdown, YAML front matter, Git, shell/Ruby link validation

**Spec:** `doc/plans/documentation-formalisation-design.md`

## Global Constraints

- Preserve validated technical history, user-observed results, relevant commit references, and unresolved risks from every source document.
- Remove stale forecasts, contradicted status claims, redundant narration, the broken `scene-pass-report.md` link, and links to the unavailable `island-scene-preview/desktop-scene.png` artifact while preserving its preview result as textual evidence.
- Keep `doc/project-outline.md` as the only source of current product direction and completion state.
- Use only `draft`, `active`, `awaiting-validation`, `completed`, and `superseded` as document statuses.
- Use stable descriptive kebab-case filenames; dates belong in YAML metadata.
- Keep the original `Assets/Scenes/Main Scene.unity` intact and make no changes under `Assets/`, `Packages/`, or `ProjectSettings/` in this documentation-only unit.
- Preserve unrelated working-tree changes and stage only files named by the current task.
- A checked item means its acceptance behaviour has been observed, not merely implemented.

## Review Focus

- Relative links renamed during migration must resolve from the file containing each link; Task 8 runs a repository-local link-target audit.
- Consolidation must not discard user-observed validation evidence; Tasks 3–5 assert representative evidence in each report.
- Superseded filenames and stale status claims must not remain authoritative; Task 8 scans for both.
- The initial-flock plan must remain `awaiting-validation` with every outstanding check visible; Task 2 checks its metadata and unchecked acceptance items.
- The roadmap must agree with report and plan status rather than treating implementation as completion; Task 6 compares its checked and unchecked deliverables with the canonical artifacts.

---

### Task 1: Documentation index and conventions

**Files:**
- Create: `doc/README.md`

**Interfaces:**
- Consumes: hierarchy, lifecycle, metadata, and template decisions from the spec
- Produces: the canonical navigation and authoring rules used by every later task

- [ ] **Step 1: Record the pre-migration link failure**

Run:

```bash
ruby -e 'Dir["*.md", "doc/**/*.md"].each { |f| File.read(f).scan(/\[[^\]]*\]\(([^)]+)\)/).flatten.each { |t| next if t =~ %r{\A(?:https?:|mailto:|#)}; p = t.split("#", 2).first; next if p.empty?; r = File.expand_path(p, File.dirname(f)); warn "#{f}: missing #{t}" unless File.exist?(r) } }'
```

Expected: output includes the missing `scene-pass-report.md` link and both missing `island-scene-preview/desktop-scene.png` links, proving the existing set is not internally navigable.

- [ ] **Step 2: Create the index and conventions**

Create `doc/README.md` with `type: index`, `status: active`, and `updated: 2026-09-27`. Define milestone, feature, unit, and task; explain the authority of the roadmap, plans, reports, references, ADRs, and `CONTEXT.md`; list the controlled statuses; and state the validation-before-completion rule.

Include concise, copyable plan and report templates containing the required sections from the spec. Link the current outline, active documentation migration plan, current sheep plan, navigation reference, and existing historical documents using their paths at this point in the migration.

- [ ] **Step 3: Verify that the index encodes the agreed conventions**

Run:

```bash
rg -n 'Milestone|Feature|Unit|Task|awaiting-validation|Acceptance criteria|Validation evidence|CONTEXT.md' doc/README.md
```

Expected: each hierarchy term, the controlled validation state, both template requirements, and the domain glossary appear.

- [ ] **Step 4: Commit the index**

```bash
git add doc/README.md
git commit -m "docs: define project documentation conventions"
```

### Task 2: Current plan and durable navigation reference

**Files:**
- Rename: `doc/sheep-pass-plan.md` → `doc/plans/initial-flock.md`
- Rename: `doc/navigation-authoring.md` → `doc/reference/navigation-authoring.md`
- Modify: `doc/README.md`

**Interfaces:**
- Consumes: plan/reference conventions from Task 1 and the existing unit-2 progress evidence
- Produces: the canonical active Desktop prototype feature plan and Replacement island navigation reference

- [ ] **Step 1: Move both documents while retaining Git history**

Run:

```bash
mkdir -p doc/reference
git mv doc/sheep-pass-plan.md doc/plans/initial-flock.md
git mv doc/navigation-authoring.md doc/reference/navigation-authoring.md
```

Expected: Git reports two renames and no copied source files remain.

- [ ] **Step 2: Recast the initial-flock document as an awaiting-validation feature plan**

Add YAML metadata with `type: plan`, `status: awaiting-validation`, `milestone: desktop-prototype`, and `updated: 2026-09-27`. Replace “unit 2” and “pass” language with a player-facing feature goal. Organize the preserved decisions and progress under Scope, Exclusions, Dependencies and decisions, Tasks, Acceptance criteria, Validation, and Open risks.

Mark only observed behaviours complete. Keep variable baa pitch, remembered sound preference after relaunch, silence while hidden, tray discoverability/restore, and the integrated sheep/window regression visibly unchecked. Treat scenery and water avoidance on the placeholder island as a known limitation owned by the Replacement island navigation unit rather than falsely completing it here.

- [ ] **Step 3: Recast navigation authoring as a current reference**

Add YAML metadata with `type: reference`, `status: active`, `milestone: replacement-island`, and `updated: 2026-09-27`. Preserve the current-surface diagnosis, the single-owner rule for baked NavMesh data, separate authored navigation geometry, water exclusion, blocker guidance, foot-probe separation, bake checklist, and Unity references. Update relative links for the new directory depth.

- [ ] **Step 4: Update index links and verify active-state visibility**

Update `doc/README.md` to point to the new plan and reference paths. Then run:

```bash
rg -n 'type: plan|status: awaiting-validation|variable baa|sound preference|silence while hidden|tray|integrated' doc/plans/initial-flock.md
rg -n 'type: reference|status: active|one active owner|Not Walkable|foot|Unity references' doc/reference/navigation-authoring.md
```

Expected: the plan exposes its awaiting checks and the reference retains every safety-critical authoring rule.

- [ ] **Step 5: Commit the active artifacts**

```bash
git add doc/README.md doc/plans/initial-flock.md doc/reference/navigation-authoring.md doc/sheep-pass-plan.md doc/navigation-authoring.md
git commit -m "docs: classify current flock and navigation work"
```

### Task 3: Companion-scene completion report

**Files:**
- Rename: `doc/scene-pass.md` → `doc/reports/companion-scene.md`
- Modify: `doc/README.md`

**Interfaces:**
- Consumes: scene-pass evidence and the report template
- Produces: the canonical completed companion-scene unit report

- [ ] **Step 1: Move and reshape the scene report**

Run `mkdir -p doc/reports`, then move the source with `git mv doc/scene-pass.md doc/reports/companion-scene.md`. Add `type: report`, `status: completed`, `milestone: desktop-prototype`, and `updated: 2026-09-27`. Organize it into Outcome, Delivered scope, Deviations and durable decisions, Validation evidence, Remaining risks, and Relevant artifacts.

Preserve the cloned geometry roots, original-scene protection, 511-renderer inventory, zero missing meshes/materials/scripts, 103-triangle NavMesh, isolated Unity reopen, source hash match, preview result, and live-editor 0-error/27-warning result. Mark the orthographic camera and temporary preview controller as point-in-time facts superseded by later desktop-control work.

- [ ] **Step 2: Update the index and handle the unavailable preview artifact**

Point `doc/README.md` to `reports/companion-scene.md`. Remove the broken preview-image link because the image is absent from the repository, while retaining the textual evidence that a Windows preview rendered successfully.

- [ ] **Step 3: Verify the scene evidence**

Run:

```bash
rg -n '511|0 missing meshes|0 missing materials|0 missing scripts|103|hash|0 errors|27 warnings|orthographic.*superseded' doc/reports/companion-scene.md
```

Expected: every measured result and the superseded-camera clarification appear.

- [ ] **Step 4: Commit the scene report**

```bash
git add doc/README.md doc/reports/unity-6-migration.md doc/reports/companion-scene.md doc/scene-pass.md
git commit -m "docs: formalise companion scene report"
```

### Task 4: Desktop window and controls completion report

**Files:**
- Create: `doc/reports/desktop-window-and-controls.md`
- Delete: `doc/desktop-controls-plan.md`
- Delete: `doc/desktop-window-pass.md`
- Modify: `doc/README.md`

**Interfaces:**
- Consumes: the completed controls plan, desktop-window report, and report template
- Produces: one canonical report for the player-facing desktop-window and control features

- [ ] **Step 1: Consolidate implementation and decisions**

Create the report with `type: report`, `status: completed`, `milestone: desktop-prototype`, and `updated: 2026-09-27`. Preserve colour-key transparency, D3D11 BitBlt configuration, delayed borderless styling, transparent-pixel click-through, pinning, middle-drag movement, right-drag perspective orbit, wheel dolly, the −10° low-view limit, window/camera persistence, off-screen recovery, tray Show/Hide/Exit behaviour, hidden background processing, and Windows-player-only boundaries.

Record source and isolated build evidence, user-observed control checks, empty-pixel targeting, long-running responsiveness, and relevant Unity/Microsoft references. Move the still-pending tray discoverability interaction with the sheep build into Remaining risks and link it to `plans/initial-flock.md`; do not describe the entire desktop feature as incomplete.

- [ ] **Step 2: Remove both superseded sources and update links**

Delete `doc/desktop-controls-plan.md` and `doc/desktop-window-pass.md`. Point `doc/README.md` to the consolidated report.

- [ ] **Step 3: Verify implementation and user-observed evidence**

Run:

```bash
rg -n 'BitBlt|borderless|click-through|middle|right.*orbit|wheel|\u221210|off-screen|Show/Hide|longer|Windows' doc/reports/desktop-window-and-controls.md
rg -n 'awaiting-validation|initial-flock' doc/reports/desktop-window-and-controls.md
```

Expected: the completed desktop behaviour is evidenced, and the narrower remaining integrated check is delegated to the current plan.

- [ ] **Step 4: Commit the consolidated report**

```bash
git add doc/README.md doc/reports/desktop-window-and-controls.md doc/desktop-controls-plan.md doc/desktop-window-pass.md
git commit -m "docs: consolidate desktop window evidence"
```

### Task 5: Unity 6 migration report

**Files:**
- Create: `doc/reports/unity-6-migration.md`
- Delete: `doc/sheep-isle-handoff.md`
- Modify: `doc/README.md`

**Interfaces:**
- Consumes: migration-specific history from the handoff, the canonical scene and window reports, and report conventions from Task 1
- Produces: a completed migration record without superseded gameplay direction or duplicated scene/window narratives

- [ ] **Step 1: Write the focused migration report**

Create `doc/reports/unity-6-migration.md` with `type: report`, `status: completed`, `milestone: desktop-prototype`, and `updated: 2026-09-27`. Preserve the 2019.4-to-6000.3.25f1 migration context, isolated-copy workflow, Polybrush removal, Doozy orientation compatibility change, Aura shader fixes, live-editor compile result, remaining legacy warnings, and the requirement to keep `Main Scene.unity` intact.

Under Remaining risks, distinguish historical machine paths and branch names from current repository truth. Link to `companion-scene.md` and `desktop-window-and-controls.md` as the owners of those results instead of repeating their detailed narratives. Do not retain the handoff's broken scene-report or unavailable preview-image links.

- [ ] **Step 2: Remove the stale handoff and update the index**

Delete `doc/sheep-isle-handoff.md`. Replace its entry in `doc/README.md` with the migration report and remove language that presents the handoff as current guidance.

- [ ] **Step 3: Verify preservation and removal boundaries**

Run:

```bash
rg -n '2019\.4|6000\.3\.25f1|Polybrush|Doozy|Aura|0 errors|27 warnings|Main Scene\.unity|companion-scene|desktop-window-and-controls' doc/reports/unity-6-migration.md
rg -n 'adopt|rehome|fixed three-quarter|next.*sheep migration|scene-pass-report|island-scene-preview' doc/reports/unity-6-migration.md
```

Expected: the first command finds the migration evidence and canonical report links; the second returns no matches for superseded product direction, next-step forecasts, or broken artifacts.

- [ ] **Step 4: Commit the migration report**

```bash
git add doc/README.md doc/reports/unity-6-migration.md doc/sheep-isle-handoff.md
git commit -m "docs: preserve Unity 6 migration evidence"
```

### Task 6: Milestone roadmap

**Files:**
- Modify: `doc/project-outline.md`

**Interfaces:**
- Consumes: canonical status from Tasks 2–5 and the four approved milestones
- Produces: the sole current source of product direction, deliverable ownership, and validation status

- [ ] **Step 1: Rebuild the outline around four milestones**

Add `type: roadmap`, `status: active`, and `updated: 2026-09-27`. Retain the concise product direction and the rule that checkmarks require observed validation. Structure the work as Desktop prototype, Replacement island, Flock progression and stories, and Living world.

Within each milestone, label every child as **Feature** or **Unit** and link substantial completed or active deliverables to their canonical report, plan, or reference. Keep the Desktop prototype open while the initial-flock acceptance checks remain. Remove the generic Supporting systems section by moving wool/economy, decorations, music, and state persistence to their owning milestones or deliverables.

- [ ] **Step 2: Localize unresolved decisions**

Place tray/audio regression choices with Desktop prototype; biome scale, camera-follow behaviour, and navigation choices with Replacement island; population, lambing, accessories, adventures, postcards, adoption/rehoming, wool rates, and offline progress with Flock progression and stories; and manual/system time, seasons, real-weather location, refresh, and fallback with Living world.

- [ ] **Step 3: Compare roadmap state with canonical artifacts**

Run:

```bash
rg -n '^## Milestone|\*\*(Feature|Unit)\*\*|reports/|plans/initial-flock|reference/navigation-authoring' doc/project-outline.md
rg -n 'Supporting idle systems|## Open design choices|sheep-isle-handoff|scene-pass|desktop-window-pass|desktop-controls-plan|unit [123]|pass' doc/project-outline.md
```

Expected: the first command shows all four milestones, classified deliverables, and canonical links. The second returns no obsolete catch-all headings, source paths, or old work-granularity language.

- [ ] **Step 4: Commit the roadmap**

```bash
git add doc/project-outline.md
git commit -m "docs: organise roadmap by product milestones"
```

### Task 7: Session-start guidance

**Files:**
- Modify: `AGENTS.md`

**Interfaces:**
- Consumes: canonical entry points and lifecycle from Tasks 1–6
- Produces: session instructions that cannot silently re-authorize a stale handoff

- [ ] **Step 1: Update the required reading order**

Replace the outline-and-handoff instruction with `CONTEXT.md`, `doc/README.md`, `doc/project-outline.md`, and the relevant active plan. State that reports provide historical evidence but do not override the roadmap, and references describe current technical guidance.

Update the Unity migration warning so it reflects the current working tree rather than asserting that the original upgrade changes are still uncommitted. Preserve the rules about inspecting `git status`, keeping the main scene intact, using Unity editor tools for scenes/prefabs, using an isolated project copy for batch runs, and committing coherent validated slices.

- [ ] **Step 2: Verify no stale handoff dependency remains**

Run:

```bash
rg -n 'CONTEXT.md|doc/README.md|project-outline.md|relevant active plan|reports|references' AGENTS.md
rg -n 'sheep-isle-handoff|upgrade has uncommitted changes' AGENTS.md
```

Expected: the first command finds every canonical source; the second returns no stale session dependency or dated working-tree claim.

- [ ] **Step 3: Commit the session guidance**

```bash
git add AGENTS.md
git commit -m "docs: point sessions at canonical project guidance"
```

### Task 8: Repository-wide validation and migration report

**Files:**
- Create: `doc/reports/documentation-formalisation.md`
- Modify: `doc/README.md`
- Delete after all checks pass: `doc/plans/documentation-formalisation-design.md`
- Delete after all checks pass: `doc/plans/documentation-formalisation.md`

**Interfaces:**
- Consumes: every canonical artifact created by Tasks 1–7
- Produces: a validated, self-describing documentation set and the durable completion record for this project-wide unit

- [ ] **Step 1: Run the local-link audit**

Run:

```bash
ruby -e 'bad = []; Dir["*.md", "doc/**/*.md"].each { |f| File.read(f).scan(/\[[^\]]*\]\(([^)]+)\)/).flatten.each { |t| next if t =~ %r{\A(?:https?:|mailto:|#)}; p = t.split("#", 2).first; next if p.empty?; r = File.expand_path(p, File.dirname(f)); bad << "#{f}: missing #{t}" unless File.exist?(r) } }; abort bad.join("\n") unless bad.empty?'
```

Expected: exit 0 with no output.

- [ ] **Step 2: Scan for superseded paths and ambiguous work language**

Run:

```bash
rg -n '\((?:doc/)?(?:sheep-isle-handoff|scene-pass|desktop-window-pass|desktop-controls-plan|navigation-authoring|sheep-pass-plan|scene-pass-report)\.md|island-scene-preview/desktop-scene\.png' AGENTS.md CONTEXT.md doc --glob '!doc/plans/documentation-formalisation*.md'
```

Expected: no output; no deleted source path, broken report path, or unavailable preview-image link remains.

- [ ] **Step 3: Review status and authority consistency**

Run:

```bash
rg -n '^status:' CONTEXT.md doc
git diff --check
git status --short
```

Expected: controlled statuses only; no whitespace errors; changes limited to this documentation migration.

Manually compare the Desktop prototype checkboxes in `doc/project-outline.md` with `doc/plans/initial-flock.md` and the three completion reports. Confirm that no pending acceptance check is marked complete in the roadmap.

- [ ] **Step 4: Write the completion report and finalize the index**

Create `doc/reports/documentation-formalisation.md` with `type: report`, `status: completed`, and `updated` set to the execution date. Record the new structure, the source-to-canonical mapping, validation commands and results, any deviations, and the commits created by this plan. Update `doc/README.md` so its document index points only to canonical artifacts and includes the new report.

- [ ] **Step 5: Retire the migration planning artifacts and rerun validation**

After the report contains every durable design decision and deviation, delete `doc/plans/documentation-formalisation-design.md` and this implementation plan. Rerun the local-link audit, superseded-path scan, `git diff --check`, and `git status --short`.

Expected: all checks pass and `doc/plans/` contains only genuinely current product work.

- [ ] **Step 6: Commit the completed migration**

```bash
git add AGENTS.md CONTEXT.md doc
git commit -m "docs: complete documentation formalisation"
```
