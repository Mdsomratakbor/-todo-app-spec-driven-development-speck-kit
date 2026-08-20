# AI vs Human Code Attribution Report

**Generated**: 2026-08-19 14:45:05
**Project**: TodoApp-SpecKit
**Module**: 005-add-loading-indicators

## Tool Availability

| Tool | Status |
|------|--------|
| git-ai | Not installed - using git log fallback |
| scc | Not installed - using git numstat fallback |
| git | Available |

## Summary

| Metric | AI-Assisted | Human | Total |
|--------|-------------|-------|-------|
| Commits | 8 (40%) | 12 (60%) | 20 |
| Lines Added | 22621 | 27816 | 50437 |
| Lines Deleted | 848 | 239 | 1087 |
| Lines Changed (total) | 23469 (45.5%) | 28055 (54.5%) | 51524 |

## Commit Breakdown

```text
Author             | Lines +/-  | Classification | Subject
-------------------|------------|----------------|--------
Md Samrat Akbor    | +1277/-55  | Human          | feat(005): add loading indicators plan and implement skel...
Md Samrat Akbor    | +0/-0      | Human          | Merge pull request #1 from Mdsomratakbor/005-loading-indi...
Md Samrat Akbor    | +37/-3     | Human          | chore: add automatic reference trailer to conventional co...
Md Samrat Akbor    | +409/-0    | AI-Assisted    | [Spec Kit] Add specification
Md Samrat Akbor    | +9570/-2   | Human          | add git extension
Md Samrat Akbor    | +1351/-215 | AI-Assisted    | some new features implement and spec kit version update
Md Samrat Akbor    | +544/-19   | Human          | feat(004): implement frontend UI enhancements ΓÇö toasts,...
Md Samrat Akbor    | +2328/-123 | AI-Assisted    | feat(003): implement full auth feature (Phases 3-16) ΓÇö ...
Md Samrat Akbor    | +1122/-69  | AI-Assisted    | feat(003): implement Phase 1-2 entities, repositories, JW...
Md Samrat Akbor    | +2640/-1   | Human          | feat(003): add basic authentication spec, plan, and tasks
Md Samrat Akbor    | +2575/-76  | Human          | feat: implement lunch preference feature (002-lunch-setti...
Md Samrat Akbor    | +8711/-398 | AI-Assisted    | feat: implement Phase 5 (API layer) + pass Gate 3 validation
Md Samrat Akbor    | +303/-26   | AI-Assisted    | test(api): add unit and integration tests for Phase 2 fou...
Md Samrat Akbor    | +995/-2    | Human          | feat: implement auth, rate limiting, and domain data layer
Md Samrat Akbor    | +98/-17    | AI-Assisted    | feat: implement T004-T010 (foundational Phase 2) ΓÇö vali...
Md Samrat Akbor    | +357/-79   | Human          | some task are completed
Md Samrat Akbor    | +9774/-2   | Human          | Task 001 and 003 complete
Md Samrat Akbor    | +46/-0     | Human          | chore: add .gitignore for .NET, Angular, Node.js, and IDE...
Md Samrat Akbor    | +8299/-0   | AI-Assisted    | feat: initial project scaffolding with Speckit workflow
Md Samrat Akbor    | +1/-0      | Human          | first commit
```

## Top Changed Files

```text
Lines +/-   | File
------------|-----
+8876/-0 (8876) | client/package-lock.json
+5372/-0 (5372) | .specify/extensions/.cache/catalog-ebf165086500aab1.json
+1648/-0 (1648) | api/tests/TodoApp.UnitTests/TestResults/112c6fd8-aced-4671-ba2f-668780de7cc8/coverage.cobertura.xml
+1648/-0 (1648) | api/tests/TodoApp.UnitTests/TestResults/452635de-3fbb-4e9f-9042-77fa0cd0c2e7/coverage.cobertura.xml
+1092/-3 (1095) | specs/001-todo-management/plan.md
+1064/-0 (1064) | specs/003-basic-auth/plan.md
+370/-346 (716) | client/src/app/app.html
+634/-0 (634) | .specify/extensions/git/scripts/python/create_new_feature_branch.py
+626/-0 (626) | .specify/extensions/git/scripts/bash/create-new-feature-branch.sh
+592/-0 (592) | .specify/extensions/git/scripts/powershell/create-new-feature-branch.ps1
+558/-0 (558) | .specify/scripts/powershell/common.ps1
+513/-0 (513) | specs/002-lunch-setting/plan.md
+363/-85 (448) | specs/003-basic-auth/tasks.md
+364/-73 (437) | specs/001-todo-management/tasks.md
+379/-0 (379) | api/src/TodoApp.Infrastructure/Data/Migrations/20260715035619_AddAuthentication.Designer.cs
+376/-0 (376) | api/src/TodoApp.Infrastructure/Data/Migrations/AppDbContextModelSnapshot.cs
+372/-1 (373) | specs/003-basic-auth/spec.md
+372/-0 (372) | specs/001-todo-management/spec.md
+363/-0 (363) | api/tests/TodoApp.UnitTests/Application/LunchPreferences/CreateOrUpdateLunchPreferenceCommandValidatorTests.cs
+363/-0 (363) | .opencode/commands/speckit.checklist.md
```

## Methodology

Commit classification uses heuristic analysis of commit messages:
- **AI-Assisted**: Commits containing SpecKit markers (Phase/Task refs, 'Spec Kit', 'speckit'),
  or AI co-author trailers
- **Human**: All other commits (manual messages, descriptive without AI markers)

> Note: This is a heuristic approximation. AI-assisted commits where the human
> rewrote the commit message may be classified as Human.

