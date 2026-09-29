---
type: plan
status: draft
milestone: desktop-prototype
updated: 2026-09-29
---

# macOS Desktop Window Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ship the Desktop Companion's transparent window, selective click-through, persisted controls, and menu-bar recovery surface in an Apple-silicon macOS player without regressing Windows.

**Architecture:** Keep `DesktopWindowController` as the scene-facing coordinator and move platform operations behind `IDesktopWindowBackend`. Reuse a trimmed pinned UniWindowController runtime for the proven transparent-window path, and add a separate project-owned AppKit bundle for the status item, native visibility, focus restoration, and screen clamping.

**Tech Stack:** Unity 6000.3.25f1, C# EditMode tests/NUnit, UniWindowController 0.9.8 at commit `d9d03bdc687bc31c14cd5fed10a9a107c4c7f658`, Objective-C++/AppKit, ARM64 Mach-O bundles, Metal.

**Spec:** `doc/plans/macos-desktop-window.md`

## Global Constraints

- Preserve `Assets/Scenes/Main Scene.unity`; the production target is `Assets/Scenes/Desktop Companion.unity`.
- Keep the working Windows colour-key, tray, movement, pin, persistence, and exit behaviour unchanged.
- Target Apple silicon only; do not advertise Intel support from an untested universal binary.
- Embed only the audited UniWindowController runtime, ARM64 macOS bundle, metadata, documentation, changelog, and MIT licence; omit samples and unrelated code/binaries.
- Build from a disposable project copy while the original project is open in Unity.
- Write every production behavior test first, observe the intended failure, then implement the minimum passing code. Vendored code, plug-in metadata, and build configuration are structural exceptions verified by explicit checks rather than unit tests.
- Use `--caller plugin --skill unity-cli` on every `unity command` invocation during execution.

## Review Focus

- A missing or failed menu-bar bridge must leave the island visible and make every hide path refuse the request; Task 1 tests this through the real session policy.
- A saved position from a removed or rearranged display must be clamped and persisted again; Tasks 1, 4, and 6 test the state contract, real AppKit clamp, and player behavior.
- Option+left input must move the macOS window without petting a sheep, including modifier release before mouse release; Tasks 1 and 6 test both gesture branches.
- Multiple menu actions arriving in one poll must not lose Quit or produce an unrecoverable hidden window; Tasks 1 and 5 test combined action flags and native action transport.
- Showing a hidden, unfocused player from the menu bar must restore the same window, visibility event, menu label, focus, and audio eligibility; Tasks 1, 4, and 6 cover the coordinated transition.

---

### Task 1: Testable desktop-window session and input policy

**Files:**
- Create: `Assets/DesktopCompanion/Windowing/SheepIsle.DesktopWindowing.asmdef`
- Create: `Assets/DesktopCompanion/Windowing/DesktopWindowAction.cs`
- Create: `Assets/DesktopCompanion/Windowing/DesktopWindowDragInput.cs`
- Create: `Assets/DesktopCompanion/Windowing/IDesktopWindowBackend.cs`
- Create: `Assets/DesktopCompanion/Windowing/IDesktopWindowSettings.cs`
- Create: `Assets/DesktopCompanion/Windowing/PlayerPrefsDesktopWindowSettings.cs`
- Create: `Assets/DesktopCompanion/Windowing/DesktopWindowInputPolicy.cs`
- Create: `Assets/DesktopCompanion/Windowing/DesktopWindowSession.cs`
- Create: `Assets/DesktopCompanion/Tests/Editor/SheepIsle.DesktopWindowing.Tests.asmdef`
- Create: `Assets/DesktopCompanion/Tests/Editor/DesktopWindowSessionTests.cs`
- Create: `Assets/DesktopCompanion/Tests/Editor/DesktopWindowInputPolicyTests.cs`

**Interfaces:**
- Produce: `[Flags] DesktopWindowAction { None = 0, Show = 1, Hide = 2, TogglePin = 4, Quit = 8 }`.
- Produce: `DesktopWindowDragInput { None, MiddleMouse, OptionPrimary }`.
- Produce: `IDesktopWindowBackend` with read-only `IsReady`, `HasRecoverySurface`, `IsVisible`, `IsPinned`, `WindowPosition`, and `CursorPosition`; `IEnumerator Initialize(GameObject host, Camera camera, int windowSize)`; `DesktopWindowAction ConsumeActions()`; `bool TrySetVisible(bool requested)`; `bool TrySetPinned(bool requested)`; `bool TrySetWindowPosition(Vector2 requested, bool clampToVisibleArea)`; `void SetRecoverySurfaceState(bool visible, bool pinned)`; and `Dispose()`.
- Produce: `IDesktopWindowSettings.TryLoadPosition(out Vector2)`, `SavePosition(Vector2)`, `LoadPinned()`, and `SavePinned(bool)`.
- Produce: `DesktopWindowInputPolicy.SelectDragInput(bool isMacOS, bool middlePressed, bool primaryPressed, bool optionPressed)`, `IsDragHeld(DesktopWindowDragInput, bool middleHeld, bool primaryHeld, bool optionHeld)`, and `ReservesPrimaryClick(bool isMacOS, bool primaryHeld, bool optionHeld)`.
- Produce: `DesktopWindowSession(IDesktopWindowBackend backend, IDesktopWindowSettings settings, Action<bool> visibilityChanged)` with observable `IsVisible`, `IsPinned`, `IsMoving`; `void Restore()`; `bool ProcessActions(DesktopWindowAction actions)`; `bool SetVisible(bool requested)`; `bool TogglePin()`; `void BeginMove(DesktopWindowDragInput input)`; `void UpdateMove(bool held)`; `void SavePosition()`; and `Dispose()`. `ProcessActions` returns `true` when Quit is present; Quit dominates other flags, otherwise Show dominates Hide and TogglePin is applied once.

- [ ] **Step 1: Write failing input-policy tests**

Add `SelectDragInput_MiddlePress_ReturnsMiddleOnBothPlatforms`, `SelectDragInput_OptionPrimary_ReturnsOptionPrimaryOnlyOnMacOS`, `IsDragHeld_OptionReleased_ReturnsFalseWhilePrimaryRemainsHeld`, and `ReservesPrimaryClick_OptionPrimary_ReturnsTrueOnlyOnMacOS`, using literal booleans and enum results.

- [ ] **Step 2: Run the focused tests and verify RED**

Run:

```bash
unity command --project-path /Users/jez/conductor/workspaces/Sheep-Isle/moscow \
  --caller plugin --skill unity-cli run_tests --mode editor \
  --filter DesktopWindowInputPolicyTests --timeout 300
```

Expected: FAIL because `DesktopWindowInputPolicy` and its enum do not exist.

- [ ] **Step 3: Implement the minimum input policy**

Implement only the three specified pure functions and enum values.

- [ ] **Step 4: Run the focused tests and verify GREEN**

Run the Step 2 command. Expected: all `DesktopWindowInputPolicyTests` pass.

- [ ] **Step 5: Write failing session tests**

Use a complete in-memory backend and settings store. Assert observable session state and saved values, not mock call existence. Add:

- `Restore_ClampsSavedPosition_AndPersistsObservedPosition`;
- `Restore_FailedPin_DoesNotOverwriteSavedPin`;
- `SetVisible_HideWithoutRecoverySurface_KeepsWindowVisible`;
- `SetVisible_NativeFailure_KeepsObservedStateAndPreference`;
- `TogglePin_NativeFailure_KeepsObservedStateAndPreference`;
- `SetVisible_UnchangedObservedState_DoesNotNotify`;
- `UpdateMove_AppliesCursorDelta_AndSavesObservedEndPosition`;
- `ProcessActions_QuitCombinedWithOtherFlags_DoesNotHideOrToggle`;
- `ProcessActions_ShowAndHide_PrefersShow`.

- [ ] **Step 6: Run the session tests and verify RED**

Run the focused command with `--filter DesktopWindowSessionTests`. Expected: FAIL because the session and contracts do not exist.

- [ ] **Step 7: Implement the contracts, PlayerPrefs store, and session**

Use existing keys `DesktopCompanion.WindowPositionSaved.v1`, `WindowX.v1`, `WindowY.v1`, and `Pinned.v1`. Inject an `Action<bool>` visibility observer into the session constructor so tests and `DesktopWindowController` receive the same transition.

- [ ] **Step 8: Run the complete EditMode suite**

Run `run_tests --mode editor --timeout 300`. Expected: every discovered test passes with no compile failures.

- [ ] **Step 9: Commit**

```bash
git add Assets/DesktopCompanion/Windowing Assets/DesktopCompanion/Tests
git commit -m "test: define desktop window session behavior"
```

### Task 2: Extract and preserve the Windows backend

**Files:**
- Create: `Assets/DesktopCompanion/Windowing/WindowsDesktopWindowBackend.cs`
- Create: `Assets/DesktopCompanion/Windowing/WindowsDesktopTray.cs`
- Modify: `Assets/DesktopCompanion/DesktopWindowController.cs`
- Delete: `Assets/DesktopCompanion/DesktopTrayIcon.cs`
- Delete: `Assets/DesktopCompanion/DesktopTrayIcon.cs.meta`

**Interfaces:**
- Consume: `IDesktopWindowBackend`, `DesktopWindowSession`, and common settings from Task 1.
- Produce: `WindowsDesktopWindowBackend`, preserving the existing Win32 style, colour key, monitor clamping, topmost verification, tray requests, title text, and cleanup.
- Produce: platform-neutral `DesktopWindowController` lifecycle that selects a backend, restores the session, maps keyboard and drag input, publishes `VisibilityChanged`, and disposes once.

- [ ] **Step 1: Refactor the green code without changing behavior**

Move existing Win32 P/Invoke and tray code behind the interface. Keep the scene component class and serialized `windowSize` field in place so Unity references remain intact. Set `[DefaultExecutionOrder(-200)]` so move-gesture reservation is established before sheep click handling.

- [ ] **Step 2: Recompile the connected editor**

Run `unity command ... recompile --focus false`, then `console_status`. Expected: `compilationFailed=false` and `consoleErrors=0`.

- [ ] **Step 3: Run the complete EditMode suite**

Expected: all Task 1 tests remain green after the extraction.

- [ ] **Step 4: Build a Windows regression player from a disposable copy**

Copy only `Assets`, `Packages`, and `ProjectSettings` into `.context/macos-window-implementation-project`, then run `DesktopCompanionBuild.BuildWindowsPlayer` with `SHEEP_ISLE_BUILD_DIR` pointing under `.context/`. Expected: `BuildResult.Succeeded`. If the macOS host lacks the Windows build module, record that infrastructure limitation and at minimum compile after switching the disposable copy to `StandaloneWindows64`; do not claim a Windows build passed without its build output.

- [ ] **Step 5: Commit**

```bash
git add Assets/DesktopCompanion/DesktopWindowController.cs Assets/DesktopCompanion/Windowing \
  Assets/DesktopCompanion/DesktopTrayIcon.cs Assets/DesktopCompanion/DesktopTrayIcon.cs.meta
git commit -m "refactor: isolate desktop window backends"
```

### Task 3: Embed the audited UniWindowController runtime

**Files:**
- Create: `Packages/com.kirurobo.uniwinc/package.json`
- Create: `Packages/com.kirurobo.uniwinc/README.md`
- Create: `Packages/com.kirurobo.uniwinc/CHANGELOG.md`
- Create: `Packages/com.kirurobo.uniwinc/LICENSE.md`
- Create: `Packages/com.kirurobo.uniwinc/UPSTREAM.md`
- Create: `Packages/com.kirurobo.uniwinc/Runtime/Kirurobo.UniWindowController.asmdef`
- Create: `Packages/com.kirurobo.uniwinc/Runtime/Scripts/UniWindowController.cs`
- Create: `Packages/com.kirurobo.uniwinc/Runtime/Scripts/LowLevel/UniWinCore.cs`
- Create: `Packages/com.kirurobo.uniwinc/Runtime/Plugins/MacOS/LibUniWinC.bundle`
- Modify: `Packages/manifest.json`
- Modify: `Packages/packages-lock.json`
- Modify: `Assets/DesktopCompanion/Windowing/SheepIsle.DesktopWindowing.asmdef`

**Interfaces:**
- Produce: the upstream `Kirurobo.UniWindowController` assembly and `Kirurobo.UniWindowController` component used by the macOS backend.
- Record: exact upstream URL, commit, package version, retained paths, removed paths, and local modifications in `UPSTREAM.md`.

- [ ] **Step 1: Add the embedded package as audited third-party code**

Copy only the specified runtime files and their required metadata from commit `d9d03bdc687bc31c14cd5fed10a9a107c4c7f658`. Remove the package's sample declaration because no `Samples~` payload is retained. Compile its C# assembly for Editor and macOS Standalone; enable the native importer only for ARM64 macOS Standalone. Do not retain Windows DLLs.

- [ ] **Step 2: Verify package structure**

Run a structural check that asserts the licence and upstream record exist, the two required C# runtime files and ARM64 bundle exist, and no `Samples`, `Editor`, `Prefabs`, `FilePanel`, `UniWindowMoveHandle`, or Windows plugin paths exist. Expected: one package root and zero forbidden paths.

- [ ] **Step 3: Verify native architecture and signing**

Run `file` on `LibUniWinC` and `codesign --verify --deep --strict` on its bundle. Expected: `Mach-O 64-bit bundle arm64` and exit 0.

- [ ] **Step 4: Recompile and run all EditMode tests**

Expected: the package resolves, `Kirurobo.UniWindowController` compiles, and all project tests pass.

- [ ] **Step 5: Commit**

```bash
git add Packages/com.kirurobo.uniwinc Packages/manifest.json Packages/packages-lock.json \
  Assets/DesktopCompanion/Windowing/SheepIsle.DesktopWindowing.asmdef
git commit -m "build: embed macOS window runtime"
```

### Task 4: Build the Sheep Isle AppKit bridge test-first

**Files:**
- Create: `Packages/com.sheepisle.macos-window/package.json`
- Create: `Packages/com.sheepisle.macos-window/README.md`
- Create: `Packages/com.sheepisle.macos-window/Native~/SheepIsleMacBridge.mm`
- Create: `Packages/com.sheepisle.macos-window/Native~/build.sh`
- Create: `Packages/com.sheepisle.macos-window/Tests~/Native/SheepIsleMacBridgeHost.mm`
- Create: `Packages/com.sheepisle.macos-window/Tests~/Native/verify.sh`
- Create: `Packages/com.sheepisle.macos-window/Runtime/Plugins/MacOS/SheepIsleMacBridge.bundle`
- Modify: `Packages/manifest.json`
- Modify: `Packages/packages-lock.json`

**Interfaces:**
- Produce C exports: `SheepIsleMac_Initialize() -> bool`, `SheepIsleMac_Dispose()`, `SheepIsleMac_ConsumeActions() -> int32_t`, `SheepIsleMac_SetMenuState(bool visible, bool pinned)`, `SheepIsleMac_SetWindowVisible(bool visible) -> bool`, `SheepIsleMac_IsWindowVisible() -> bool`, and `SheepIsleMac_ClampWindowToVisibleScreen() -> bool`.
- `ConsumeActions` uses Task 1 bit values. The menu exposes a `🐑` status item with dynamic Show/Hide Sheep Isle, Pin/Unpin, separator, and Quit Sheep Isle items.

- [ ] **Step 1: Write the native host contract test before the bundle**

The host creates a real `NSApplication` and `NSWindow`, loads the bundle with `dlopen`, resolves every export with `dlsym`, initializes the real status item, sets both menu states, hides and shows the actual window, clamps a deliberately off-screen frame into `NSScreen.visibleFrame`, and disposes twice. Assertions use observed AppKit state and process exit status.

- [ ] **Step 2: Run the native verification and verify RED**

Run `Packages/com.sheepisle.macos-window/Tests~/Native/verify.sh`. Expected: non-zero with a missing bundle/export failure, not a host compilation error.

- [ ] **Step 3: Implement the minimum Objective-C++ bridge and build script**

Build an ARM64 bundle with `xcrun clang++`, ARC, AppKit, and a macOS 12.0 minimum. Store the Unity player `NSWindow` during initialization; queue menu actions without invoking managed callbacks. Show uses `makeKeyAndOrderFront`, `orderFrontRegardless`, and application activation; hide uses `orderOut`. Clamp the entire frame to the nearest screen's visible frame. Dispose removes the status item and clears retained objects idempotently.

- [ ] **Step 4: Run native verification and verify GREEN**

Expected: host exit 0; `file` reports ARM64; `nm -gj` lists every specified export; `codesign --verify --deep --strict` succeeds after ad-hoc signing.

- [ ] **Step 5: Configure the Unity plug-in importer and recompile**

Mark the bundle compatible only with macOS Standalone ARM64, not Any Platform or other standalone targets. Recompile the connected editor and confirm zero console errors.

- [ ] **Step 6: Commit**

```bash
git add Packages/com.sheepisle.macos-window Packages/manifest.json Packages/packages-lock.json
git commit -m "feat: add macOS menu bar bridge"
```

### Task 5: Implement the macOS backend through tested facades

**Files:**
- Create: `Assets/DesktopCompanion/Windowing/IMacOSWindowFacade.cs`
- Create: `Assets/DesktopCompanion/Windowing/IMacOSMenuBridge.cs`
- Create: `Assets/DesktopCompanion/Windowing/UniWindowFacade.cs`
- Create: `Assets/DesktopCompanion/Windowing/MacOSMenuBridge.cs`
- Create: `Assets/DesktopCompanion/Windowing/MacOSDesktopWindowBackend.cs`
- Create: `Assets/DesktopCompanion/Tests/Editor/MacOSDesktopWindowBackendTests.cs`
- Modify: `Assets/DesktopCompanion/DesktopWindowController.cs`

**Interfaces:**
- Consume: `Kirurobo.UniWindowController`, Task 1 backend contract, and Task 4 C exports.
- Produce: `IMacOSWindowFacade` with read-only `IsTransparent`, `IsPinned`, `WindowPosition`, and `CursorPosition`; `bool Configure(Camera camera, float opacityThreshold)`; `bool TrySetPinned(bool requested)`; and `bool TrySetWindowPosition(Vector2 requested)`.
- Produce: `IMacOSMenuBridge` with `bool Initialize()`, `DesktopWindowAction ConsumeActions()`, `void SetMenuState(bool visible, bool pinned)`, `bool TrySetWindowVisible(bool requested)`, `bool IsWindowVisible()`, `bool ClampWindowToVisibleScreen()`, and `Dispose()`.
- Produce: injected `IMacOSWindowFacade` and `IMacOSMenuBridge` boundaries so backend state/error policy is exercised without modifying Editor windows.
- Produce: macOS backend using alpha transparency, opacity hit testing, threshold `0.05`, automatic clear camera background, forced windowed mode, free positioning, and observed state after every operation.

- [ ] **Step 1: Write failing backend tests**

Add `Initialize_AllBoundariesSucceed_SetsReadyAndRecoveryAvailable`, `Initialize_TransparencyFails_LeavesBackendNotReady`, `Initialize_MenuFails_LeavesRecoveryUnavailable`, `ConsumeActions_CombinedNativeFlags_ReturnsEveryFlag`, `TrySetVisible_ReturnsObservedNativeVisibility`, `TrySetPinned_ReturnsObservedUniWindowState`, `TrySetWindowPosition_ClampEnabled_ReturnsPostClampPosition`, and `Dispose_CalledTwice_DisposesEachBoundaryOnce`. The facades represent only the external native boundaries; assertions target backend readiness, recovery availability, and observed state.

- [ ] **Step 2: Run the focused backend tests and verify RED**

Expected: FAIL because the facades and macOS backend do not exist.

- [ ] **Step 3: Implement the C# bridge, UniWindow facade, and backend**

Use `[DllImport("SheepIsleMacBridge")]` only inside `UNITY_STANDALONE_OSX && !UNITY_EDITOR`. Initialization must leave `HasRecoverySurface=false` unless the menu bridge succeeds, and leave `IsReady=false` unless the UniWindow facade reports transparent attachment. Log one actionable error per failed boundary.

- [ ] **Step 4: Run focused and complete EditMode tests**

Expected: all macOS backend tests and the full EditMode suite pass.

- [ ] **Step 5: Wire platform selection in the coordinator**

Create `MacOSDesktopWindowBackend` only for the standalone macOS player; keep Windows selection unchanged and Editor behavior inert. Recompile and confirm zero console errors.

- [ ] **Step 6: Commit**

```bash
git add Assets/DesktopCompanion/DesktopWindowController.cs Assets/DesktopCompanion/Windowing \
  Assets/DesktopCompanion/Tests/Editor/MacOSDesktopWindowBackendTests.cs
git commit -m "feat: integrate macOS desktop window backend"
```

### Task 6: Integrate companion input, persistence, and macOS building

**Files:**
- Modify: `Assets/DesktopCompanion/CompanionSheepClick.cs`
- Modify: `Assets/DesktopCompanion/DesktopOrbitCamera.cs`
- Modify: `Assets/DesktopCompanion/Editor/DesktopCompanionBuild.cs`
- Modify: `Assets/DesktopCompanion/Tests/Editor/DesktopWindowInputPolicyTests.cs`

**Interfaces:**
- Produce and consume: `DesktopWindowInputPolicy.AllowsGameplayPrimaryClick(bool isMacOS, bool primaryPressed, bool optionPressed)` before raycast petting.
- Produce: camera persistence under `(UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX) && !UNITY_EDITOR`.
- Produce: `DesktopCompanionBuild.BuildMacOSPlayer()` menu/CLI entry configuring windowed 480 × 480, non-resizable, background execution, Retina, Metal, Mono, Apple-silicon architecture, and the Desktop Companion scene.

- [ ] **Step 1: Write failing interaction tests**

Add `AllowsGameplayPrimaryClick_OrdinaryPrimary_ReturnsTrue`, `AllowsGameplayPrimaryClick_MacOptionPrimary_ReturnsFalse`, and `AllowsGameplayPrimaryClick_WindowsOptionPrimary_ReturnsTrue`. The earlier gesture tests continue to prove modifier release ends movement.

- [ ] **Step 2: Run the focused tests and verify RED**

Expected: FAIL because `AllowsGameplayPrimaryClick` does not exist.

- [ ] **Step 3: Implement click reservation and desktop camera persistence**

Implement `AllowsGameplayPrimaryClick` as the tested policy entry point, consult it before the raycast, and extend only the persistence compilation guards. Do not change raycast, petting, right-drag orbit, zoom, or sound input.

- [ ] **Step 4: Run focused and complete EditMode tests**

Expected: all tests pass.

- [ ] **Step 5: Add the macOS build entry**

Share common player settings without weakening Windows-specific graphics settings. Accept `SHEEP_ISLE_BUILD_DIR` for deterministic verification output and emit `Sheep Isle.app` for `BuildTarget.StandaloneOSX`.

- [ ] **Step 6: Verify both standalone builds from the disposable copy**

Run the Windows build check from Task 2 and the macOS build method. Expected: successful build result(s), macOS app main executable compatible with ARM64, both native bundles ARM64, no UniWindow samples/editor/Windows payload in the app, and strict code-signature verification succeeds before launch.

- [ ] **Step 7: Commit**

```bash
git add Assets/DesktopCompanion/CompanionSheepClick.cs Assets/DesktopCompanion/DesktopOrbitCamera.cs \
  Assets/DesktopCompanion/Editor/DesktopCompanionBuild.cs Assets/DesktopCompanion/Tests
git commit -m "build: add Apple silicon companion player"
```

### Task 7: Run macOS player acceptance

**Files:**
- No repository changes are planned; a reproduced defect first adds a named failing test and then amends the owning task's files.
- Retain local logs/builds under: `.context/macos-window-implementation-*`

**Interfaces:**
- Validate the complete player-facing contract from the spec; produce no new interface unless a reproduced defect forces a reviewed design change.

- [ ] **Step 1: Launch the signed macOS player with an explicit log path**

Record Unity/macOS versions, ARM64 architecture, player settings, native bundle architectures, initial position, pin state, visibility, and menu readiness.

- [ ] **Step 2: Observe transparency and interaction**

Over representative light and dark windows, verify no opaque rectangle, transparent pixels reach the application behind, island/sheep pixels remain interactive, and Retina alignment is correct.

- [ ] **Step 3: Observe controls and persistence**

Verify `P`, menu Pin/Unpin, middle-drag, Option+left-drag without petting, right-drag, zoom, `S`, edge clamping, relaunch position, pin, and camera view.

- [ ] **Step 4: Observe visibility and lifecycle**

Verify `H` and menu Hide stop audio, the status item remains, menu Show restores/focuses the same player, and menu Quit exits cleanly. Also verify the window remains visible when running a build with the menu bridge deliberately unavailable.

- [ ] **Step 5: Fix only reproduced failures through RED-GREEN**

For each defect, add a focused failing automated test when the behavior is expressible outside AppKit; otherwise make the native host reproduce the failure before changing the bridge. Re-run the complete relevant suite and player check.

### Task 8: Close the feature with durable evidence

**Files:**
- Create: `doc/reports/macos-desktop-window.md`
- Modify: `doc/project-outline.md`
- Modify: `doc/README.md`
- Delete after the report covers all durable decisions: `doc/plans/macos-desktop-window.md`
- Delete after the report covers all execution evidence: `doc/plans/macos-desktop-window-implementation.md`

**Interfaces:**
- Produce: completed report linking the feasibility report, package/native provenance, test/build results, observed acceptance matrix, remaining release risks, and implementation commits.

- [ ] **Step 1: Run final verification from a clean state**

Run native host tests, the complete Unity EditMode suite, editor console status, available Windows regression build, fresh macOS build, `codesign`, `file`, forbidden-dependency layout checks, `git diff --check`, and `git status --short`. Record exact counts and any infrastructure limitation without softening it.

- [ ] **Step 2: Write the completion report and update the roadmap**

Check the feature only for acceptance behaviors directly observed in Task 7. Move unresolved Intel, distribution, and pinned-level choices into the owning roadmap entry/report.

- [ ] **Step 3: Remove superseded plans and commit**

```bash
git add doc
git commit -m "docs: complete macOS desktop window support"
```

- [ ] **Step 4: Verify the final branch state**

Expected: all verification evidence remains green, documentation links resolve, only intended commits differ from `origin/master`, and the worktree is clean.
