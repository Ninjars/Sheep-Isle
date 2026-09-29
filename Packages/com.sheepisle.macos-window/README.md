# Sheep Isle macOS Window Bridge

Project-owned AppKit integration for the Sheep Isle desktop companion. The bundle owns
the menu-bar recovery surface, native window visibility/focus, and visible-screen
clamping. Transparent composition and hit testing remain owned by UniWindowController.

The first supported binary is Apple silicon only and has a macOS 12.0 deployment target.

## Rebuild and verify

From the repository root:

```bash
Packages/com.sheepisle.macos-window/Native~/build.sh
Packages/com.sheepisle.macos-window/Tests~/Native/verify.sh
```

The build script compiles with the active Xcode command-line tools, ad-hoc signs the
bundle, and writes it to `Runtime/Plugins/MacOS/SheepIsleMacBridge.bundle`.
