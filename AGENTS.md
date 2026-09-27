# Sheep Isle project guidance

- At the start of a new session, read the product language in [`CONTEXT.md`](CONTEXT.md), the documentation map in [`doc/README.md`](doc/README.md), the current [`project-outline.md`](doc/project-outline.md), and the relevant active plan. Check dated claims against the working tree before acting on them.
- The project outline is the only source of current product direction and completion state. Reports preserve historical implementation and validation evidence but do not override the roadmap; references describe current technical guidance.
- Keep the outline current as work progresses: add ideas, revise or remove superseded entries, link substantial plans, reports, and references, and check off items only after their acceptance behaviour has been observed. Record unresolved choices with the milestone, feature, or unit that owns them.
- This Unity project was upgraded from 2019.4 to Unity 6 and retains legacy packages and compatibility work. Keep Unity changes in `Assets/`, `Packages/`, and `ProjectSettings/`, and planning notes in `doc/`; do not edit generated `Library/`, `Temp/`, `obj/`, or solution/project files.
- Inspect `git status` before editing. Preserve existing work, avoid broad cleanup or reverting files you did not change, and verify current repository state instead of relying on dated migration notes.
- The existing main scene is `Assets/Scenes/Main Scene.unity`. Keep it intact while building the new desktop companion scene. Use Unity editor tools for scene and prefab changes so references and metadata survive.
- The gameplay design is still open. Do not assume the old food and reproduction system belongs in the new game; record decisions and validation results in `doc/` as the project develops.
- The original project may be open in Unity. Use a separate project copy for batch editor runs, and verify code or scene changes in Unity when practical.
- Commit coherent, verified slices regularly. Inspect `git status`, stage only files belonging to the slice, and keep pre-existing unrelated changes out of the commit. Update the outline and linked notes in the same slice when its scope or status changes.
