---
type: report
status: completed
milestone: desktop-prototype
updated: 2026-09-27
---

# Unity 6 migration

## Outcome

Sheep Isle was upgraded far enough from Unity 2019.4.20f1 to compile, open the new companion scene, and build the Windows desktop companion with Unity 6000.3.25f1. Legacy package warnings remain, but they no longer block the Desktop prototype milestone.

Detailed scene-transfer evidence lives in the [desktop companion scene report](companion-scene.md). Transparent-player and interaction evidence lives in the [desktop window and controls report](desktop-window-and-controls.md).

## Delivered scope

- Removed the deprecated `com.unity.polybrush` package dependency after its Unity 6 compile errors were reproduced and the removal was validated in an isolated project copy.
- Removed the cloned land's missing Polybrush component while keeping `Assets/Scenes/Main Scene.unity` intact.
- Removed an obsolete Doozy `ScreenOrientation.Landscape` reference surfaced by the Unity 6 Windows build.
- Corrected two Aura shader include paths and incompatible lighting-helper calls exposed by the live Unity 6 import.
- Refreshed the live editor out of Safe Mode, imported the companion assets, and produced a Windows player build.

## Deviations and durable decisions

- Batch editor operations use a separate project copy when the original project may be open. This avoids competing imports, lock files, or accidental edits to an unsaved live scene.
- Scene and prefab changes use Unity editor APIs rather than hand-edited YAML so serialized references and metadata survive.
- The existing main scene remains a legacy reference. Migration work must not use broad cleanup to rewrite it or silently remove components from it.
- Historical notes referred to `E:\Jez\Documents\dev\Sheep Isle`, the `touch_input` branch, and a Windows-installed editor path. Those identify the environment in which the migration was validated; they are not instructions about the current worktree or branch.
- A previous sandboxed Windows CLI process could not connect to Unity licensing, while the installed CLI under the host account succeeded. Future automation must verify its actual licensing context rather than assuming either result still applies.

## Validation evidence

- `Unity.exe -version` returned `6000.3.25f1` in the migration environment.
- Unity 6000.3.25f1 left Safe Mode after Polybrush was removed.
- The companion scene reopened in an isolated copy and later imported into the live editor with its expected hierarchy and navigation data.
- After the Doozy and Aura compatibility corrections, the live Unity Console showed 0 errors and 27 warnings. The warnings were primarily obsolete API use in legacy packages.
- The source project completed the dedicated Windows companion build. The companion-scene and desktop-window reports own the detailed asset inventories and player interaction results.

## Remaining risks or follow-up

- The legacy main scene may still contain a missing Polybrush component. It remains intentionally untouched unless a future feature requires it.
- Existing package warnings should be evaluated when work enters the affected legacy systems; this migration did not perform unrelated package modernization.
- Machine paths, installed editor locations, active branches, dirty files, and whether an editor is running are volatile facts. Check the current environment and `git status` before acting instead of treating this report as live state.

## Relevant artifacts

- `Packages/manifest.json`
- `Packages/packages-lock.json`
- `ProjectSettings/ProjectVersion.txt`
- `Assets/Scenes/Main Scene.unity`
- [Desktop companion scene report](companion-scene.md)
- [Desktop window and controls report](desktop-window-and-controls.md)
