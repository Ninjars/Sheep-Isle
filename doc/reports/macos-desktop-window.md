---
type: report
status: completed
milestone: desktop-prototype
updated: 2026-09-29
---

# macOS desktop window and controls

## Outcome

The Desktop Companion now has a validated Apple-silicon macOS player with borderless alpha transparency, selective empty-pixel click-through, ordinary island and sheep interaction, pinning, persisted placement and camera view, trackpad-friendly movement, and a persistent menu-bar recovery surface. The user completed the signed-player acceptance pass and reported all requested checks passed.

This completes the production feature described by the earlier [macOS transparent-window feasibility report](macos-transparent-window-feasibility.md). It validates the Apple-silicon implementation but does not yet declare macOS a released or broadly supported product platform.

## Delivered scope

- `DesktopWindowController` remains the scene-facing coordinator and selects project-owned Windows or macOS implementations of `IDesktopWindowBackend` only in the corresponding standalone player. The Unity Editor remains inert.
- `DesktopWindowSession` owns common state, persistence, hide safety, movement, menu actions, and observed-state updates. A window cannot hide until its backend reports a working recovery surface.
- The embedded `com.kirurobo.uniwinc` 0.9.8 runtime owns macOS alpha transparency, opacity hit testing at a `0.05` threshold, borderless/free positioning, pointer and window coordinates, and topmost state.
- The project-owned `com.sheepisle.macos-window` AppKit bundle owns the 🐑 status item, dynamic Show/Hide and Pin/Unpin labels, Quit, native hide/show and focus restoration, and visible-screen clamping.
- Middle-drag remains available on both desktop platforms. Option+left-drag is additionally reserved for macOS movement before gameplay click processing, so it does not pet a sheep.
- Pin state, window position, camera yaw, camera pitch, and camera distance use the existing standalone persistence keys. Restored positions are clamped and the observed position is saved again.
- `DesktopCompanionBuild.BuildMacOSPlayer()` produces a windowed, non-resizable, background-running, Retina, Metal, Mono Apple-silicon app containing only the Desktop Companion scene. `SHEEP_ISLE_BUILD_DIR` selects deterministic verification output.

## Dependency and native provenance

The runtime-only UniWindowController package is pinned to upstream commit `d9d03bdc687bc31c14cd5fed10a9a107c4c7f658`, reports version 0.9.8, and retains the upstream README, changelog, and MIT licence. Its two retained C# runtime files and ARM64 `LibUniWinC.bundle` are unmodified. Samples, editor tooling, prefabs, `FilePanel`, `UniWindowMoveHandle`, and Windows native binaries are omitted; details are recorded in `Packages/com.kirurobo.uniwinc/UPSTREAM.md`.

The Sheep Isle AppKit bridge keeps its Objective-C++ source, rebuild script, native host, and ARM64 ad-hoc-signed bundle together under `Packages/com.sheepisle.macos-window`. It exports initialize, dispose, action consumption, menu-state update, window visibility, visibility observation, and screen-clamp functions. Keeping this bridge separate from UniWindowController preserves a clear project/upstream ownership boundary.

## Deviations and durable decisions

- Unity 6 classic standalone builds read the macOS architecture from `EditorUserBuildSettings` key `OSXUniversal/Architecture`. Setting `PlayerSettings.SetArchitecture(NamedBuildTarget.Standalone, 1)` persisted an ARM64 value in `ProjectSettings` but still produced a universal app and excluded both ARM64-only bundles. The build entry now sets the platform key to `OSArchitecture.ARM64`; the resulting app and both plug-ins are ARM64.
- Backend readiness requires UniWindowController's transparency setting and its observed native free-positioning state. This prevents its managed transparency backing field from reporting success before the player window is attached.
- Native requests never become saved truth until their resulting visibility, pin, or position state is observed. A failed pin request therefore does not corrupt the saved preference.
- Multiple native menu actions are transported as flags. Quit takes precedence over visibility or pin changes, and simultaneous Show/Hide prefers Show, preventing an unrecoverable hidden state.
- The native host keeps the Objective-C++ bundle loaded until process exit. Unloading a registered Objective-C class with `dlclose` caused a host crash during autorelease-pool teardown; Unity also keeps the plug-in loaded for the process lifetime.
- The validated pinned level remains UniWindowController's current macOS topmost implementation. Whether a lower `.floating` level better matches the companion experience remains a later polish decision.

## Validation evidence

### Automated and structural checks

- Unity 6000.3.25f1 EditMode suite: 27 of 27 tests passed. The tests cover session restore and hide safety, failed native state transitions, combined action flags, movement policy, gameplay click reservation, macOS backend readiness and recovery, observed visibility and pin state, clamped positions, and idempotent disposal.
- The settled live Editor reported zero compilation errors, zero Console errors, and zero Console warnings after verification.
- `Packages/com.sheepisle.macos-window/Tests~/Native/verify.sh` passed the real AppKit host contract, confirmed all seven exports, identified the bridge as an ARM64 Mach-O bundle, and passed strict code-signature verification.
- A clean `git archive HEAD` built successfully with Unity 6000.3.25f1 at `.context/macos-window-final-build.PrDRGA/Sheep Isle.app`. Its allocated size was 94,593,024 bytes.
- The app executable, `LibUniWinC`, and `SheepIsleMacBridge` were each ARM64 Mach-O files. Both bundles passed strict signature verification, and the complete app passed `codesign --verify --deep --strict`.
- The packaged app contained neither UniWindowController samples/editor tooling/FilePanel nor unrelated Windows native payload.
- Windows target script compilation succeeded from an isolated copy earlier in the implementation. A Windows player could not be packaged on this Mac because Unity 6000.3.25f1 has no Windows playback module installed; the fresh regression attempt reached `BuildPipeline.BuildPlayer` and reported `build target was unsupported`. This infrastructure limitation is preserved in `.context/windows-regression-task6.log` and is not reported as a successful Windows build.

### Player acceptance

The signed ARM64 player ran on macOS 26.2 with an Apple M1 Max, Metal, Mono, Unity 6000.3.25f1, and an explicit log at `.context/macos-window-implementation-player.log`. The log contained no runtime errors. The user reported all requested checks passed:

| Check | Result |
| --- | --- |
| Borderless alpha transparency | Pass; no opaque rectangular background was observed. |
| Selective click-through | Pass; transparent pixels passed interaction to the application behind. |
| Island and sheep interaction | Pass; visible rendered pixels remained interactive. |
| Option+left-drag reservation | Pass; the window moved without incidental sheep petting. |
| Pin/unpin | Pass in the integrated signed player. |
| Menu-bar recovery | Pass; the 🐑 item hid and restored the current player. |
| Persistence | Pass for the requested position/state restoration check. |
| Quit | Pass; menu Quit closed the current player cleanly. |

A separate disposable copy deliberately omitted `SheepIsleMacBridge.bundle`. It logged the actionable recovery error, retained an on-screen layer-0 player window, and was then terminated. This confirms the failure path leaves the companion visible rather than stranded without a Show action.

## Remaining risks and release follow-up

- Intel is unsupported. A universal distribution requires an `x86_64` or universal UniWindowController bundle, a matching Sheep Isle bridge, and validation on Intel hardware.
- Developer ID signing, hardened runtime, notarisation, distribution packaging, updates, and login-item behaviour remain release work. The validated local build is ad-hoc signed.
- Multiple displays, display removal/reconnection, Spaces, Mission Control, full-screen applications, and sleep/wake were not exhaustively exercised. The native clamp is host-tested, but those operating-system transitions need release-platform validation.
- Revisit whether the current topmost level is too aggressive around system UI.
- The Windows runtime behaviour remains covered by its earlier completed report and target compilation, but this Mac cannot produce the fresh Windows player package needed for a post-refactor runtime regression.

## Relevant commits and artifacts

- `acbd472` — define the desktop-window session and input behaviour with tests.
- `8bded4a` — isolate the existing Windows implementation behind the backend boundary.
- `3be1dc8` — embed the pinned runtime-only UniWindowController package.
- `aa2e87e` — add the project-owned AppKit menu-bar bridge and native host.
- `8301666` — integrate and test the macOS desktop-window backend.
- `4eaf172` — add macOS input integration, persistence, and Apple-silicon building.
- `Packages/com.kirurobo.uniwinc/UPSTREAM.md`
- `Packages/com.sheepisle.macos-window/README.md`
- `.context/macos-window-final-build.log`
- `.context/macos-window-implementation-player.log`
- `.context/windows-regression-task6.log`
