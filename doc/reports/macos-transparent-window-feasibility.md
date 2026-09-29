---
type: report
status: completed
milestone: desktop-prototype
updated: 2026-09-29
---

# macOS transparent-window feasibility

## Outcome

A Unity 6000.3.25f1 standalone player can reproduce Sheep Isle's essential transparent desktop-companion behaviour on Apple silicon. The probe displayed only the rendered island, passed pointer input through transparent pixels, retained sheep and island interaction over visible pixels, stayed above ordinary windows, moved with a trackpad-friendly control, restored its saved position, hid and restored, and quit cleanly.

Proceed with a production macOS implementation when macOS support is scheduled. Use a project-owned desktop-window interface backed initially by a pinned, runtime-only [UniWindowController](https://github.com/kirurobo/UniWindowController) integration. Retain its MIT licence and omit its samples. This is preferable to immediately rewriting the native layer: the API surface Sheep Isle needs is small, but the package already contains Retina-coordinate corrections, Unity 6 focus handling, free-positioning support, and macOS window lifecycle fixes that are easy to rediscover incorrectly.

This result proves feasibility; it does not add macOS as a supported product platform.

## Probe and native boundary

The disposable probe copied the project into `.context/macos-window-spike-project`, added UniWindowController 0.9.8 at commit `d9d03bdc687bc31c14cd5fed10a9a107c4c7f658`, and built `.context/macos-window-spike-build/Sheep Isle Spike.app`. Nothing from the probe or package was added to the production Unity project.

The selected commit is newer than the 0.9.8 release while still reporting that package version. It includes a 2026-03-06 Unity 6 focus/crash correction, so a production integration should pin an audited commit rather than assuming the 0.9.8 tag has the same behaviour.

The probe applied its window settings only in `UNITY_STANDALONE_OSX` after Unity had created the player window. Unity C# called a Swift/AppKit `.bundle`; the relevant native operations were:

- `NSWindow.backgroundColor = .clear`, `NSWindow.isOpaque = false`, and a clear, non-opaque content-view layer for alpha composition.
- A borderless `NSWindow.StyleMask` and disabled shadow for the undecorated shape.
- `NSWindow.ignoresMouseEvents`, switched from sampled framebuffer opacity, for selective input pass-through.
- `NSWindow.Level.popUpMenu` for the tested pinned state.
- `NSWindow` frame positioning for movement and saved-position restoration.

These operations correspond to Apple's current [`NSWindow`](https://developer.apple.com/documentation/appkit/nswindow), [`isOpaque`](https://developer.apple.com/documentation/appkit/nswindow/isopaque), [`ignoresMouseEvents`](https://developer.apple.com/documentation/appkit/nswindow/ignoresmouseevents), and [`StyleMask`](https://developer.apple.com/documentation/appkit/nswindow/stylemask-swift.struct) APIs. Unity documents macOS native plug-ins as bundles loaded through the usual native plug-in interface in its [desktop native plug-in guidance](https://docs.unity3d.com/6000.0/Documentation/Manual/plug-ins-for-desktop.html).

The production shell should expose window operations through project-owned code so the current Win32 implementation and a macOS adapter do not leak platform APIs into companion behaviour. A later native macOS shell should use [`NSStatusItem`](https://developer.apple.com/documentation/appkit/nsstatusitem) for discoverable hide, restore, and quit actions.

## Validation evidence

Environment and build:

- macOS 26.2 on an Apple M1 Max (`arm64`), with one 1728 × 1117-point Retina display at 2× backing scale.
- Unity 6000.3.25f1, Xcode 26.6, Metal, Mono scripting backend, development player, windowed 480 × 480 Unity pixels, Retina enabled, and a macOS 12.0 deployment target.
- The 2026-09-29 build completed successfully at 351,398,589 bytes and passed `codesign --verify --deep --strict` after provenance was written beside, rather than inside, the app bundle.
- The Unity player executable was universal, but `LibUniWinC.bundle` was `arm64` only. The tested application is therefore Apple-silicon-only despite the universal player executable.

Observed behaviour:

| Check | Result and evidence |
| --- | --- |
| Borderless alpha transparency | Pass. The user observed no opaque rectangular background; the probe used alpha composition rather than the Windows magenta colour key. |
| Transparent-pixel click-through | Pass. The user confirmed transparent pixels were transparent to interaction. Runtime telemetry repeatedly switched `isClickThrough` on at sampled alpha `0.000`. |
| Island and sheep interaction | Pass. The user confirmed clicks on the island worked. Runtime telemetry switched click-through off at sampled alpha `1.000`. |
| Focus | Pass for the required interaction model. The companion could remain inactive over transparent pixels and became interactive over rendered content; no Accessibility permission was required. |
| Retina scale | Pass. Unity rendered at 480 × 480 pixels while WindowServer reported a 240 × 240-point window on the 2× display, with aligned input and visible content. |
| Movement | Pass. Middle-drag remained available, and Option+left-drag was added for trackpads and mice without a middle button. Four observed Option-drag moves changed the saved window position. |
| Position restoration | Pass. After relaunch, telemetry reported the final saved position `(1407, 757)`. |
| Always on top | Pass. WindowServer reported layer 101 for the pinned window. The production implementation should review whether `.floating` is sufficient before retaining the probe's stronger `.popUpMenu` level. |
| Hide and restore | Pass. Hiding removed the window from the on-screen WindowServer list; activation restored it at layer 101 and at the same position. A production status item remains necessary for discoverable restoration. |
| Quit | Pass. Escape closed the player cleanly after saving position. |
| Multiple displays/windows | Not exercised. The test machine exposed one display and the companion uses one player window. Production must clamp a restored position when display topology changes. |

The final player log is retained at `.context/macos-window-spike-player-relaunch.log`; the Option-drag session is retained at `.context/macos-window-spike-player-option-drag.log`; the successful build log is retained at `.context/macos-window-spike-project/Logs/build-StandaloneOSX-1790685544531.log`.

Automated screenshots and pointer-event injection were unavailable because this host had neither Screen Recording nor Accessibility event-posting permission. Visual transparency and input routing were therefore observed directly by the user and corroborated by player/window telemetry.

## Deviations and durable decisions

- macOS relocation must not require a middle mouse button. Use Option+left-drag as the platform-friendly companion-window gesture; keep middle-drag as an optional secondary binding. Production input routing must suppress an incidental sheep click when the modified drag begins.
- Alpha-based transparency is the macOS path. The Windows colour key remains Windows-specific.
- Selective click-through is achieved by changing whole-window `ignoresMouseEvents` state based on the pixel under the pointer. It worked at the probe's 30 fps, but production validation should include edge jitter, fast pointer movement, and frame-rate degradation.
- The spike depended on UniWindowController only in a disposable copy. A production dependency should vendor or otherwise pin the audited runtime, native source/binary, metadata, and MIT licence without samples or unrelated editor tooling.
- The existing Doozy `OrientationDetector` referenced the Unity 6-obsolete `ScreenOrientation.Landscape` alias in standalone builds. Removing that alias, while preserving `LandscapeLeft` and `LandscapeRight`, was required for the macOS player to compile and is retained as a compatibility fix.

## Remaining risks or follow-up

- Decide whether the first supported macOS build is Apple-silicon-only. Universal distribution requires an `x86_64` or universal native bundle and Intel validation.
- Validate transparency edges over representative light and dark desktops, multiple displays, display reconnection, Spaces, Mission Control, full-screen applications, and sleep/wake before claiming platform support.
- Add a macOS status item, safe position clamping, a recovery/reset-position action, and production signing/notarisation work in the eventual implementation.
- Re-evaluate the pinned window level. `.popUpMenu` proves feasibility but may cover system UI more aggressively than the desired companion behaviour.
- Verify that Option-drag cannot trigger petting or other primary-click actions once the platform adapter is integrated with production input handling.

## Reproduction

The throwaway source and built player remain under `.context/` for local inspection. To reproduce from a clean checkout, copy `Assets`, `Packages`, and `ProjectSettings` to a disposable project, add the pinned UniWindowController package, add a standalone-only bootstrap that enables alpha transparency and opacity hit testing after the first rendered frame, then build and run the Desktop Companion scene as a macOS development player. Do not batch-build from the original project while it is open in Unity.
