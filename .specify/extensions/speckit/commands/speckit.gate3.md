---
description: Run implementation quality gate with auto-fix and retry
agent: build
---

# Gate 3: Implementation Quality (Auto-Fix)

Validate implementation quality. On failure: diagnose, fix, and retry automatically.

## Execution Flow

```
LOOP (max 3 attempts):
  1. Run all validation checks
  2. If ALL pass → report success, exit
  3. If ANY fail → diagnose root cause, apply fix, increment attempt, retry
END LOOP
报告 remaining failures, block progression
```

## Validation Checks

### Check 1: Build Verification
- Run `dotnet build --no-restore` from `api/` directory
- **Pass**: 0 errors
- **Fail**: Parse error messages, identify file/line, fix code

### Check 2: Test Execution
- Run `dotnet test --no-build` from `api/` directory
- **Pass**: All tests pass
- **Fail**: Parse failing test names, read test code, fix implementation

### Check 3: Task Completion
- Read `specs/{feature}/tasks.md`
- **Pass**: All tasks in current phase marked `[x]`
- **Fail**: Mark completed tasks, report truly incomplete ones

### Check 4: Spec Compliance
- Compare files created vs plan's file list
- **Pass**: All required files exist
- **Fail**: Create missing files per plan

### Check 5: Code Quality
- Grep for TODO/FIXME in new files
- **Pass**: No issues
- **Fail**: Resolve or document TODOs

## Auto-Fix Strategy

When a check fails:

1. **Diagnose**: Read error message, identify root cause
   - Build error → read file at error line, fix syntax/type
   - Test failure → read test, compare with spec, fix implementation
   - Missing file → create per plan specification
   - TODO found → implement or remove

2. **Fix**: Apply minimal fix to resolve the issue
   - Prefer editing existing files over creating new ones
   - Follow existing code conventions
   - Don't change unrelated code

3. **Verify**: Re-run the specific check that failed
   - If passes, continue to next check
   - If fails again, try different approach

4. **Retry**: Re-run full Gate 3 from start

## Output Format

```
## Gate 3 Attempt {N}/3

| Check | Status | Details |
|-------|--------|---------|
| Build | ✅/❌ | {details} |
| Tests | ✅/❌ | {details} |
| Task Completion | ✅/❌ | {details} |
| Spec Compliance | ✅/❌ | {details} |
| Code Quality | ✅/❌ | {details} |

{If fixed something}
**Auto-Fix Applied**: {description of fix}

{If all pass}
**Overall**: ✅ PASS (attempt {N})

{If still failing after 3 attempts}
**Overall**: ❌ FAIL — Manual intervention required
**Remaining Issues**: {list}
```

## On Final Success

1. Update tasks.md — mark completed tasks `[x]`
2. Update AGENTS.md — add session summary
3. Report to user: "Gate 3 passed. Ready for next phase."

## On Final Failure (3 attempts exhausted)

1. Report all remaining failures with details
2. Suggest manual remediation steps
3. Block progression — do NOT proceed to next phase
4. Ask user: "Fix manually and retry, or skip?"
