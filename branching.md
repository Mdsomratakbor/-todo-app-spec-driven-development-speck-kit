# Branching Convention

```
main ───────────────────────────────────── (stable, release-ready)
  │
  └─── develop ──────────────────────────── (integration, all features merged)
       │
       ├── change/001-todo-management    speckit feature
       ├── change/002-user-auth          speckit feature
       └── change/003-billing            speckit feature
```

## Branch Lifecycle

| Branch | Purpose | Base Branch | Merges To |
|---|---|---|---|
| `main` | Production-ready. Only merge reviewed, fully gated changes. | — | — |
| `develop` | Integration branch. All completed features merge here. | `main` | `main` |
| `change/<feature-id>` | Feature work. One per speckit feature under `specs/<feature-id>/`. | `develop` | `develop` |

---

## Per-Feature Workflow

```bash
# ─── Start ───────────────────────────────────────────────
git checkout develop
git checkout -b change/001-todo-management

# ─── Phase 1-2: Spec & Design ────────────────────────────
/speckit.specify        # create spec.md
/speckit.clarify        # resolve ambiguities
/speckit.constitution   # constitutional compliance
/speckit.checklist      # generate requirements quality checklist
/speckit.plan           # create plan.md
/speckit.tasks          # generate tasks.md

# ─── Phase 3: Validate before code ───────────────────────
/speckit.gate1          # spec quality — must PASS
/speckit.gate2          # plan quality — must PASS

# ─── Phase 4: Implementation ─────────────────────────────
/speckit.implement      # execute tasks phase by phase
/speckit.gate3          # implementation quality — run after each phase

# ─── Phase 5: Convergence & Closure ──────────────────────
/speckit.converge       # find remaining gaps
/speckit.gate4          # deployment readiness

# ─── Close feature branch ────────────────────────────────
git checkout develop
git merge --squash change/001-todo-management
git branch -d change/001-todo-management
```

---

## Gate Triggers

| Gate | Command | When | Who |
|---|---|---|---|
| Gate 1 | `/speckit.gate1` | Before any code | Feature author |
| Gate 2 | `/speckit.gate2` | After plan/tasks generated | Feature author |
| Gate 3 | `/speckit.gate3` | After each implementation phase | Feature author |
| Gate 4 | `/speckit.gate4` | Before merging to develop | Author + reviewer |

---

## Rules

1. **Branch per spec** — `change/<feature-id>` maps 1:1 to `specs/<feature-id>/`
2. **Gate before merge** — Gate 4 must PASS before merging to `develop`
3. **Squash merge to develop** — keeps history clean; one commit per feature
4. **Release to main** — merge `develop` into `main` (no squash) for version tag
5. **Spec isolation** — each feature's specs live under `specs/<feature-id>/`, so no branch conflicts on spec files
6. **Source conflicts** — resolve on `develop` when merging feature branches that touch the same files

---

## Remote Setup

```bash
git push -u origin develop
git push -u origin main
```
