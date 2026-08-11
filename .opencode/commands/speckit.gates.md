---
description: Generate the Quality Gates checklist (quality-gates.md) for the current feature from the quality-gates template.
---

## User Input

```text
$ARGUMENTS
```

You **MUST** consider the user input before proceeding (if not empty).

## Outline

This command creates `quality-gates.md` in the feature directory. It validates the **four Quality Gate definitions (Gate 1–4)** as they apply to the feature — it does NOT test implementation compliance.

### 1. Determine the feature directory

- If `$ARGUMENTS` starts with a `NNN-name` pattern (e.g. `005-add-loading-indicators`), treat it as the feature ID and set `FEATURE_DIR = <repo-root>/specs/<feature-id>`.
- Otherwise read `.specify/feature.json` in the repo root and use its `feature_directory` value as `FEATURE_DIR`.
- Fallback: run `.specify/scripts/powershell/check-prerequisites.ps1 -Json` from repo root and parse JSON for `FEATURE_DIR`.
- If no feature can be resolved, STOP and report: "No feature context found — run /speckit.specify first or pass a feature ID."

### 2. Load the quality-gates template

Read `.specify/templates/quality-gates-template.md` (resolve through the template resolution stack: `.specify/templates/overrides/`, presets, extensions, then `.specify/templates/`). If the template is unavailable, use the canonical structure: H1 title, purpose/created/feature meta lines, `##` category sections containing `- [ ] CHK### <item> [Tag]` lines.

### 3. Load the Gate definitions for cross-reference

Read `.opencode/commands/speckit.gate1.md` through `speckit.gate4.md` (the four Quality Gate definitions) so the generated checklist reflects the actual, current Gate definitions. Also load `.specify/memory/constitution.md` (if it exists) for the Traceability Rules (§VI.B) and Delivery Standards referenced by the checklist.

### 4. Write the quality-gates checklist

Create `FEATURE_DIR/quality-gates.md` from the template, replacing placeholders:

- `[FEATURE NAME]` → the feature's short name (from the directory slug, humanized, e.g. `005-add-loading-indicators` → `Add Loading Indicators`)
- `[DATE]` → today's date (YYYY-MM-DD)
- `[Link to spec.md]` → `[spec.md](spec.md)` (relative link inside the feature directory)

Keep the CHK items and categories exactly as they are in the template — do not renumber, reorder, or remove them. If a Gate definition referenced by an item has materially changed, note that in the Notes section instead of editing the items.

### 5. File handling

- If `quality-gates.md` does NOT exist: create it with the full checklist (CHK001–CHK035).
- If it already exists: preserve existing content and checkboxes. Do not delete or replace it — report its current state instead.

### 6. Report

Report to the user:

- Full path to the generated file
- Item count (CHK001–CHK035, 35 items)
- Whether the file was created or already existed
- Summary of any Gate definitions that changed since the template was last aligned

## Done When

- [ ] `quality-gates.md` written to `FEATURE_DIR` (or confirmed already present)
- [ ] Checklist items match the current Gate definitions in `.opencode/commands/speckit.gate1.md` – `speckit.gate4.md`
- [ ] Completion reported to user with file path and item count
