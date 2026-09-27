# Sheep Isle project guidance

- Read [the project outline](doc/project-outline.md) and [the handoff](doc/sheep-isle-handoff.md) at the start of a new session. The outline is the current direction and rough checklist; the handoff records technical history. Check dated claims against the working tree before acting on them.
- Keep the outline current as work progresses: add ideas, revise or remove superseded entries, link detailed planning and validation documents, and check off items only after they are validated. Record new decisions and unresolved choices there instead of letting older notes silently override them.
- This is a Unity project being migrated from 2019.4 to Unity 6. Keep Unity changes in `Assets/`, `Packages/`, and `ProjectSettings/`, and planning notes in `doc/`; do not edit generated `Library/`, `Temp/`, `obj/`, or solution/project files.
- The Unity 6 upgrade has uncommitted changes. Inspect `git status` before editing, preserve existing work, and avoid broad cleanup or reverting files you did not change.
- The existing main scene is `Assets/Scenes/Main Scene.unity`. Keep it intact while building the new desktop companion scene. Use Unity editor tools for scene and prefab changes so references and metadata survive.
- The gameplay design is still open. Do not assume the old food and reproduction system belongs in the new game; record decisions and validation results in `doc/` as the project develops.
- The original project may be open in Unity. Use a separate project copy for batch editor runs, and verify code or scene changes in Unity when practical.
- Commit coherent, verified slices regularly. Inspect `git status`, stage only files belonging to the slice, and keep pre-existing unrelated changes out of the commit. Update the outline and linked notes in the same slice when its scope or status changes.
