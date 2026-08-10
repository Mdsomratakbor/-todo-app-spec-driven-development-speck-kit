---
description: "Auto-commit changes after a Spec Kit command completes"
---

# Auto-Commit Changes

Automatically stage and commit all changes after a Spec Kit command completes.

## Behavior

This command is invoked as a hook after (or before) core commands. It:

1. Determines the event name from the hook context (e.g., if invoked as an `after_specify` hook, the event is `after_specify`; if `before_plan`, the event is `before_plan`)
2. Checks `.specify/extensions/git/git-config.yml` for the `auto_commit` section
3. Looks up the specific event key to see if auto-commit is enabled
4. Falls back to `auto_commit.default` if no event-specific key exists
5. Determines the commit message based on `commit_style` (see below)
6. If enabled and there are uncommitted changes, runs `git add .` + `git commit`

## Commit Message Styles

Controlled by the `commit_style` key in `.specify/extensions/git/git-config.yml`:

- **`fixed`** (default): use the per-command `message` if configured, otherwise a generic `[Spec Kit] Auto-commit <phase> <command>` message.
- **`conventional`**: inspect the actual changes (`git diff` / `git status`) since the last commit and generate a [Conventional Commit](https://www.conventionalcommits.org/) message (`type(scope): subject`, e.g. `feat: add OAuth specification` or `docs: update implementation plan`) that accurately summarizes the change, then append the reference trailer described below. Write this message to a temporary file and pass the file's path to the script (see Execution below). The configured `message` values are ignored in this mode.

### Reference Trailer (auto-tagging)

When `commit_style: conventional`, tag every commit with its GitHub issue / user story / task IDs so it can be traced back to the spec, per the constitution's traceability requirement. Format is controlled by `commit_ref_trailer` in `.specify/extensions/git/git-config.yml` (default: `Refs: {issues}, {us}, {tasks}`).

To build the trailer, resolve each token against the **current feature** context:

1. **Determine the feature directory**: read `.specify/feature.json` (fallback: the single dir under `specs/` matching the current branch prefix, e.g. branch `005-loading-indicators` → `specs/005-*`). If none can be determined, skip the trailer entirely.
2. **`{tasks}`** → task IDs from `<feature>/tasks.md` covered by this diff (T followed by three digits, e.g. `T001`, `T034`). If `tasks.md` does not exist yet (e.g. commit right after `/speckit.specify`), leave this segment empty.
3. **`{us}`** → user story IDs from `<feature>/spec.md` related to the change (pattern `US-<n>` or `US<n>`, e.g. `US-1`). If `spec.md` does not exist or nothing maps, leave this segment empty.
4. **`{issues}`** → GitHub issue numbers. GitHub issues are created by `/speckit.taskstoissues` with titles of the form `T###: <description>`. If the GitHub MCP tools (`list_issues`) are available, look up the task IDs from step 2 by matching `\bT\d{3}\b` in issue titles and collect their issue numbers. Do **not** invent or guess issue numbers; if they cannot be looked up, leave this segment empty.

Then compose the trailer:
- Replace each token in `commit_ref_trailer` with the resolved, comma-separated list (or drop the whole token segment when empty, leaving no dangling separators/commas/spaces).
- Append the composed trailer as a footer **only if at least one segment resolved to a real value**; otherwise omit it. Keep the subject on the first line; the trailer goes after a blank line (Conventional Commits footer convention).

Example (`feat(web): add skeleton loaders to todo list` where tasks T012–T015 map to issues #42/#43, US-1):

```
feat(web): add skeleton loaders to todo list

Refs: #42, #43, US-1, T012, T013, T014, T015
```

## Execution

Determine the event name from the hook that triggered this command, then run the script:

- **Bash**: `.specify/extensions/git/scripts/bash/auto-commit.sh <event_name> [--message-file <path>]`
- **PowerShell**: `.specify/extensions/git/scripts/powershell/auto-commit.ps1 <event_name> [-MessageFile <path>]`

Replace `<event_name>` with the actual hook event (e.g., `after_specify`, `before_plan`, `after_implement`). Only pass a generated message when `commit_style: conventional` is configured — first check `.specify/extensions/git/git-config.yml` for the value of `commit_style`:

- If `conventional`: inspect the diff and generate a Conventional Commit message, then append the reference trailer (see Reference Trailer above) as a footer. **Do not interpolate the generated message directly into a shell command string** — its content is derived from repository changes and may contain characters (quotes, `$(...)`, backticks) that a shell would execute or that would break command quoting. Instead, write the message (subject + trailer) to a temporary file using your file-editing tool (not a shell `echo`/`printf`), then pass that file's path via `--message-file <path>` (Bash) or `-MessageFile <path>` (PowerShell).
- If `fixed` or absent: run the script with just `<event_name>`; it uses the configured/static message.

## Configuration

In `.specify/extensions/git/git-config.yml`:

```yaml
# "fixed" (default) uses the messages below; "conventional" asks the agent
# to generate a Conventional Commit message from the diff instead.
commit_style: fixed

auto_commit:
  default: false          # Global toggle — set true to enable for all commands
  after_specify:
    enabled: true          # Override per-command
    message: "[Spec Kit] Add specification"
  after_plan:
    enabled: false
    message: "[Spec Kit] Add implementation plan"
```

## Graceful Degradation

- If Git is not available or the current directory is not a repository: skips with a warning
- If no config file exists: skips (disabled by default)
- If no changes to commit: skips with a message
- If `commit_style: conventional` is set and no generated message was supplied: fails with a clear error instead of silently falling back to the fixed message format
