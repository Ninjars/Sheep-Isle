---
type: plan
status: active
milestone: desktop-prototype
updated: 2026-09-29
---

# macOS desktop window and controls

## Goal and classification

Deliver a production **Feature** that gives the Desktop Companion scene its proven transparent-window behaviour on Apple silicon macOS: alpha transparency, selective pointer pass-through, retained island interaction, pinning, movement, saved state, hide/restore, and a persistent menu-bar recovery surface.

This feature implements the successful [macOS feasibility result](../reports/macos-transparent-window-feasibility.md). It does not by itself declare macOS a released or broadly supported platform.

Execution is decomposed in the [macOS desktop window implementation plan](macos-desktop-window-implementation.md).

## Supported scope

- Unity 6000.3.25f1 standalone macOS player on Apple silicon.
- The existing `Assets/Scenes/Desktop Companion.unity` scene without changing the legacy Main Scene.
- Borderless per-pixel alpha composition using a transparent camera background.
- Selective click-through based on the rendered opacity under the pointer, with visible island and sheep pixels remaining interactive.
- Remembered pin state, window position, camera orbit, and camera zoom.
- Middle-drag window movement plus Option+left-drag for trackpads and one-button mice.
- A macOS menu-bar item with dynamic Show/Hide Sheep Isle, Pin/Unpin, and Quit commands.
- Existing `P`, `H`, and Escape keyboard controls while the companion has focus.
- An Apple-silicon macOS player build command and validation flow.

## Exclusions

- Claiming Intel support. A later universal build may be attempted speculatively, but it must not be advertised until exercised on Intel hardware.
- App Store sandboxing, notarisation, distribution packaging, automatic updates, or login-item behaviour.
- Replacing the working Windows transparency or system-tray implementation.
- Adding UniWindowController samples, prefabs, editor inspectors, file-dialog code, Windows binaries, or unrelated capabilities.
- Redesigning gameplay, the companion scene, or the legacy Main Scene.

## Architecture

`DesktopWindowController` remains the scene-facing coordinator. It owns common input, persistence, visibility events, and application lifecycle, while native window operations are delegated through a project-owned backend interface. The scene keeps its current component reference and does not gain platform-specific objects.

The existing Win32 calls and `DesktopTrayIcon` move behind a Windows backend without changing their player-facing behaviour. A macOS backend configures and observes the Unity player window through a trimmed embedded UniWindowController runtime. Platform-specific code is compiled only for its target standalone player; the Unity Editor does not modify its own windows.

The macOS native boundary has two deliberately separate owners:

- The pinned UniWindowController runtime owns transparent composition, borderless state, opacity hit testing, click-through, topmost state, pointer position, and window position.
- A small Sheep Isle AppKit bundle owns the `NSStatusItem`, menu commands, native hide/show, focus restoration, and visible-screen position clamping.

The Sheep Isle bridge queues menu actions as flags. Unity polls and consumes them from its normal `Update` loop, so gameplay state, persistence, visibility notifications, and `Application.Quit` remain controlled by C# rather than asynchronous native callbacks.

## Dependency and native-source policy

Embed a runtime-only copy of `com.kirurobo.uniwinc` 0.9.8 from audited commit `d9d03bdc687bc31c14cd5fed10a9a107c4c7f658`. Retain its package metadata, README, changelog, and MIT licence. Retain only the C# runtime needed by `UniWindowController` and `UniWinCore`, their metadata, and the ARM64 macOS plug-in. Record the upstream commit in the embedded package.

Commit the Sheep Isle AppKit bridge source beside its compiled ARM64 bundle and document the local rebuild command. The binary must be ad-hoc signed and imported only for macOS. Do not combine project-owned menu behaviour into the third-party bundle; keeping the bridge separate preserves a clear upstream boundary.

## Runtime behaviour

### Startup and transparency

The standalone player runs windowed at 480 × 480 Unity pixels, in the background, at a 30 fps target. Windows retains its existing magenta colour key. On macOS, the coordinator waits until Unity has created and sized its player window, then enables alpha transparency, a clear camera background, borderless mode, opacity hit testing with a `0.05` threshold, free positioning, and the restored pin state.

The macOS backend initializes the menu-bar bridge before allowing the window to hide. The status item uses a compact sheep label without adding an image asset. Its menu labels always reflect current visibility and pin state.

### Input and interaction

- `P` and the menu-bar Pin/Unpin command apply and persist the same pin state.
- `H` and the menu-bar Show/Hide command use the same visibility transition.
- Escape and the menu-bar Quit command save state before exiting.
- Middle-drag moves the player on Windows and macOS.
- Option+left-drag additionally moves the player on macOS.
- A modified left-drag is reserved before gameplay input runs, so its initial press cannot pet a sheep.
- Right-drag orbit, wheel zoom, ordinary left-click petting, and the `S` sound control retain their current meanings.

The window position is saved when movement ends and during clean shutdown. Position coordinates remain backend-owned because Win32 and AppKit use different origins. Restoring a macOS position clamps the complete window rectangle into the nearest available screen's visible frame. The camera view uses the existing persistence keys on either standalone desktop platform.

### Visibility, focus, and audio

Hiding saves position, cancels movement, orders the native player window out, verifies the resulting visibility state, updates the menu, and emits `DesktopWindowController.VisibilityChanged(false)`. Existing sheep audio therefore stops through its current subscription.

Showing orders the same player window forward, restores focus, verifies visibility, updates the menu, and emits `VisibilityChanged(true)`. The menu-bar item remains available while the player window is hidden.

Pinning is restored at launch and preserved across hide/show. The first implementation may retain UniWindowController's tested `NSWindow.Level.popUpMenu`; a later polish unit may reduce it to `.floating` if testing shows that covering system UI is undesirable.

## Failure behaviour

- If the AppKit menu bridge cannot initialize, log one actionable error and keep the player visible. `H` and native hide requests do nothing because no recovery surface exists; Escape remains available.
- If the player window cannot be attached or transparency cannot be applied, log one actionable error and leave an ordinary visible Unity window rather than hiding or making an unusable click-through surface.
- A failed pin request preserves the observed native state and does not overwrite the saved preference with an unverified value.
- Invalid or off-screen saved positions are clamped before being saved again.
- Disposing the coordinator removes the menu-bar item and restores native resources exactly once.

## Test strategy

Use test-driven development for common production logic. Put testable state and input policy in a small runtime assembly, separate from P/Invoke and `MonoBehaviour` lifecycle code. EditMode tests cover:

- Menu Show, Hide, Pin, Unpin, and Quit requests mapping to coordinator actions.
- Hiding being rejected until a recovery surface is ready.
- Native pin failure not corrupting persisted state.
- Middle-drag and macOS Option+left-drag selection and termination.
- Option+left input being reserved from sheep petting while ordinary left-click remains available.
- Visibility transitions emitting only when the observed native state changes.

Verify the AppKit bundle independently with architecture inspection, exported-symbol inspection, and strict code-signature verification. Verify Unity integration with editor compilation, the complete EditMode suite, a Windows standalone compile/build regression check, and a macOS player built from a disposable project copy while the original project is open.

The macOS acceptance pass must observe:

- No title bar or opaque rectangular background.
- Desktop input passing through transparent pixels while island and sheep clicks still work.
- Pin and unpin from both keyboard and menu.
- Middle-drag and Option+left-drag movement without accidental petting.
- Position and camera view restoration after relaunch.
- Menu-bar hide, restore, and quit, with audio stopping while hidden.
- Safe restoration after saving a position near the display edge.
- Correct 2× Retina alignment between rendered pixels, pointer hit testing, and window movement.

## Acceptance criteria

- [ ] A committed Apple-silicon macOS Desktop Companion player provides borderless alpha transparency and selective click-through without the Windows colour key.
- [ ] Sheep, island, camera, sound, pin, movement, and keyboard interactions retain their intended behaviour.
- [ ] Option+left-drag moves the window on a trackpad and never produces an incidental sheep pet.
- [ ] Pin state, window position, and camera view survive relaunch; restored positions remain reachable after display changes.
- [ ] The menu-bar item reliably shows, hides, pins, unpins, and quits the current player, including while its window is hidden.
- [ ] Failure of the native menu or transparent-window boundary leaves a visible, recoverable player with a useful error.
- [ ] Windows player compilation/build and its existing tray/window behaviour are not regressed.
- [ ] The embedded dependency is runtime-only, licensed, pinned to the audited commit, and contains no examples or unrelated platform binaries.
- [ ] A completed report records build settings, native architectures, direct observations, remaining platform risks, and the commits that delivered the feature.

## Remaining release decisions

- Whether to build and validate a universal native plug-in for Intel Macs.
- Whether `.floating` better matches the desired pin behaviour than the proven `.popUpMenu` level.
- Signing identity, hardened runtime, notarisation, distribution packaging, and minimum supported macOS version.
