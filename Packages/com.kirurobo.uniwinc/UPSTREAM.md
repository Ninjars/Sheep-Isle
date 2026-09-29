# Upstream provenance

- Project: [kirurobo/UniWindowController](https://github.com/kirurobo/UniWindowController)
- Upstream commit: `d9d03bdc687bc31c14cd5fed10a9a107c4c7f658`
- Package version: `0.9.8`
- Licence: MIT; retained verbatim in `LICENSE.md`

## Retained upstream content

- `README.md`, `CHANGELOG.md`, and `LICENSE.md`
- `Runtime/Scripts/UniWindowController.cs`
- `Runtime/Scripts/LowLevel/UniWinCore.cs`
- `Runtime/Plugins/MacOS/LibUniWinC.bundle`
- the runtime assembly definition and required Unity metadata

## Omitted upstream content

Samples, editor tooling, prefabs, `FilePanel`, `UniWindowMoveHandle`, and all Windows
native plug-ins are intentionally omitted. Sheep Isle supplies its own coordinator,
movement policy, menu-bar integration, and Windows implementation.

## Local modifications

- Removed the package sample declaration because no sample payload is retained.
- Limited the runtime assembly to the Unity Editor and macOS Standalone.
- Limited the native plug-in importer to ARM64 macOS Standalone players.
- Changed only package metadata and assembly/import platform configuration; the two
  retained C# source files and native bundle are unmodified from the pinned commit.
