---
type: report
status: completed
milestone: desktop-prototype
updated: 2026-09-29
---

# Unity 6 dependency portability

## Outcome

The repository now contains the legacy UI dependencies required to compile and reopen Sheep Isle with Unity 6000.3.25f1 on a clean machine. DOTween has one canonical location under `Assets/Plugins`, and the imported Doozy examples and installer have been removed.

## Delivered scope

- Retained Doozy UI 3.1.3 core and its `DDebug.dll` dependency.
- Retained DOTween 1.2.825 because Doozy's animation runtime depends on it.
- Consolidated the DOTween runtime, editor binaries, modules, documentation, and editor images under `Assets/Plugins/Demigiant/DOTween` while preserving asset GUIDs.
- Removed the duplicate `Assets/Demigiant` hierarchy.
- Removed `Assets/Doozy/Examples` and the post-import `Assets/DoozyInstaller` payload rather than hiding compile-affecting example code with `.gitignore` rules.

## Deviations and durable decisions

- The Desktop Companion scene does not reference Doozy or DOTween directly. They remain because the preserved Main Scene contains six Doozy `UIButton` components, two `UIView` components, a `UICanvas`, event management, and a Nody graph; `GameManager` also consumes Doozy game events.
- Replacing Doozy with project-owned UI would require a deliberate migration of the legacy Main Scene and `UIGraph.asset`. That work is not justified while the project guidance requires the scene to remain intact.
- The legacy Main Scene retains its previously documented missing Polybrush component on `Land`. Dependency cleanup did not add or remove scene components.

## Validation evidence

- The pre-change dependency-layout check failed because `Assets/Demigiant` and Doozy Examples existed and the shared DOTween modules were absent from `Assets/Plugins`.
- The post-change check confirmed one DOTween root, no Doozy Examples or installer, required runtime/UI modules, and no duplicate Unity asset GUIDs.
- Unity's compiler reported zero errors after the moves and deletions.
- The Main Scene loaded with all six Doozy buttons and both Doozy views resolved; its only missing component was the already documented Polybrush component.
- The Desktop Companion scene loaded with zero missing scripts, resolved DOTween from `Assets/Plugins/Demigiant/DOTween/DOTween.dll`, entered Play mode with three sheep, and produced zero Console errors.
- Unity Test Framework discovered no project tests, so compilation, serialized-reference audits, and live scene checks provide the validation evidence for this maintenance slice.

## Remaining risks or follow-up

- Doozy and Aura still emit obsolete-API warnings under Unity 6. They should be modernised or removed only when work enters the legacy systems that use them.
- Doozy and DOTween are licensed third-party assets. Future upgrades must preserve their asset metadata and should exclude optional examples before committing.
- Removing the legacy Main Scene would make both dependencies candidates for deletion, but that is a separate product and archival decision.

## Relevant artifacts

- `Assets/Doozy`
- `Assets/Plugins/Demigiant/DOTween`
- `Assets/Resources/DOTweenSettings.asset`
- `Packages/manifest.json`
- `Packages/packages-lock.json`
- `ProjectSettings/ProjectVersion.txt`
