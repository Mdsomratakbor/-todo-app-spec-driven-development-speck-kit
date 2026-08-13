# Feature Specification: Loading Indicators for All Operations

**Feature Branch**: `005-loading-indicators`

**Created**: 2026-08-10

**Status**: Draft

**Input**: User description: "i want to add loader in every operation, and want to add loader like facebook data loading"

## Clarifications

### Session 2026-08-11

- Q: What is the canonical fast-operation threshold that gates flicker suppression (FR-007)? → A: 200 ms defer — no loader shown for operations completing within 200 ms; indicators shown within 200 ms for slower operations, with a 300 ms minimum display time once visible
- Q: Which screens are in scope for skeleton loading (US1 mentions "lunch settings, dashboard" which do not exist in the client)? → A: The 4 existing data screens — todo list, categories, todo detail, profile; lunch settings and dashboard are out of scope (no such screens in the app yet), and the shared skeleton component will cover any future screens
- Q: Are third-party loader/skeleton libraries permitted? → A: No — zero new dependencies; skeletons are built in-house using existing Angular Material primitives and Material CSS variables
- Q: Where is "retry where appropriate" (FR-005) offered? → A: Retry button is shown only when a first load fails with no data on screen; if a background refresh fails while data is already visible, the existing content is kept and an error toast is shown (no retry button)
- Q: How should SC-003's "imperceptible layout shift" be measured? → A: Cumulative Layout Shift (CLS) < 0.1 on skeleton-to-content resolution, verified via Chrome DevTools performance trace or screen capture

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Facebook-Style Skeleton Loading for Data Screens (Priority: P1)

When a user opens any screen that loads data (todo list, categories, todo detail, profile), they see lightweight skeleton placeholders shaped like the real content instead of a blank page or a single spinner. As data arrives, the skeletons smoothly transition into the actual content.

**Why this priority**: This is the most visible part of the request. Skeleton loading directly addresses the "like Facebook data loading" requirement and is the primary way users experience loading feedback on every data-driven screen.

**Independent Test**: Can be fully tested by loading each data screen on a throttled/slow connection and confirming skeletons appear in the shape of the final content, then resolve into real data without layout jump.

**Acceptance Scenarios**:

1. **Given** a user opens a data screen with pending network requests, **When** the screen begins loading, **Then** skeleton placeholders matching the real layout are displayed instead of an empty or static state
2. **Given** skeleton placeholders are showing, **When** the data request completes successfully, **Then** the skeletons are replaced by the real content in the same positions (no significant layout shift)
3. **Given** a data screen with cached or already-loaded content, **When** the user navigates to it, **Then** no skeleton is shown (content displays immediately)

---

### User Story 2 - Loading Indicator for Every User-Triggered Operation (Priority: P1)

Every operation the user initiates that involves waiting — submitting a form, saving a todo, deleting a category, logging in, logging out — visibly indicates progress at the point of action. The triggering control (e.g., button) shows a busy state, is disabled against duplicate submission, and returns to normal when the operation finishes.

**Why this priority**: This covers the "loader in every operation" part of the request. It directly prevents the common failure where a user clicks, sees nothing happen, and clicks again, causing duplicates.

**Independent Test**: Can be fully tested by initiating each operation, confirming the action control immediately shows a busy indicator and cannot be triggered twice, and returns to its normal state after completion or failure.

**Acceptance Scenarios**:

1. **Given** a user initiates an operation with a network wait, **When** the operation starts, **Then** the triggering control immediately shows a visible loading state and is disabled against repeated activation
2. **Given** an operation is in progress, **When** the operation completes, **Then** the loading state is removed and the control returns to its normal, enabled state
3. **Given** an operation fails, **When** the failure response arrives, **Then** the loading state is removed, the control is re-enabled, and an error notification is shown

---

### User Story 3 - Loading Feedback for Background Data Operations (Priority: P2)

Operations that refresh or sync data in the background (e.g., pull-to-refresh, periodic refresh, list updates after an edit) show a lightweight, non-blocking progress indication so the user knows new data is on the way without being blocked from interacting.

**Why this priority**: Important for perceived responsiveness, but secondary to the initial-load and submit flows. It can be delivered as a follow-up slice after the core loader behavior ships.

**Independent Test**: Can be fully tested by triggering a background refresh and confirming a non-blocking indicator appears, the UI remains interactive, and the indicator disappears when the refresh finishes.

**Acceptance Scenarios**:

1. **Given** a background refresh is triggered, **When** the refresh starts, **Then** a non-blocking progress indicator is shown and the user can continue interacting
2. **Given** a background refresh is in progress, **When** it completes or fails, **Then** the indicator disappears and the outcome is surfaced (data updated or error shown)

---

### User Story 4 - Loading States Are Accessible and Flicker-Free (Priority: P3)

Loading indicators never cause visual flashing for very fast operations, respect the user's reduced-motion preference, and are conveyed to assistive technologies rather than purely decorative.

**Why this priority**: This is a polish layer over the previous stories. Accessibility and flicker avoidance matter for quality but depend on the core loaders already existing.

**Independent Test**: Can be fully tested by running operations that complete in milliseconds (no flashing loader), enabling reduced-motion (static non-animated indicators), and using a screen reader (loading state announced).

**Acceptance Scenarios**:

1. **Given** an operation completes quickly (within the 200 ms fast-operation threshold), **When** it starts and finishes, **Then** no flickering loader is shown
2. **Given** a user has reduced-motion enabled, **When** a loader is displayed, **Then** it uses a static/non-animated presentation
3. **Given** assistive technology is active, **When** a screen enters a loading state, **Then** the loading status is announced rather than silent

---

### Edge Cases

- What happens when a request completes so fast the loader would only flash for a frame? (deferred/minimum-display-time handling)
- How does the system handle a loading state when the user navigates away mid-request? (loading state must be cancelled, never applied to the wrong screen)
- How does the system handle parallel requests on the same screen (e.g., categories + todos loading together)? (screen stays in loading state until all required data arrives)
- What happens on retry after a failed load? (a retry button is offered when a first load fails with no data; the loader reappears and resolves or fails again)
- How is a failed background refresh handled when data is already visible? (content is kept, an error toast is shown, and no retry button is displayed — the user can re-trigger via existing controls)
- How does the system handle slow-but-eventual responses, e.g., long-running imports? (indicator persists with clear messaging; no false timeout)

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST display a loading indicator for every user-facing operation that waits for data or processing to complete (initial data loads, submit/save actions, refreshes, authentication)
- **FR-002**: Initial data loading on data-driven screens MUST use skeleton placeholders shaped like the final content layout (Facebook-style) rather than a generic spinner
- **FR-003**: Skeleton placeholders MUST mirror the position, size, and shape of the real content so replacing them causes no significant layout shift
- **FR-004**: Controls that trigger an operation with a wait MUST show a busy state (e.g., inline spinner) and MUST be disabled against duplicate activation while the operation is pending
- **FR-005**: When an operation fails, the loading indicator MUST be removed, the control MUST be re-enabled, and an error notification MUST be shown with retry where appropriate (retry is offered when a first load fails with no data on screen; refresh failures keep existing content and show an error toast)
- **FR-006**: Background/refresh operations MUST show a non-blocking progress indicator and MUST NOT block the user from interacting with the rest of the screen
- **FR-007**: Loading indicators MUST be suppressed (no flicker) for operations that complete within the defined fast-operation threshold (200 ms defer; once shown, a 300 ms minimum display time)
- **FR-008**: Loading states MUST respect reduced-motion preferences by using static, non-animated presentation
- **FR-009**: Loading states MUST be announced to assistive technology (e.g., screen readers) so they are not purely visual
- **FR-010**: Loading indicators MUST be consistently styled and behave identically across all screens and operations

### Key Entities *(include if feature involves data)*

- **Loading State**: The transient UI state representing a pending operation — tracks which operation is in flight, the type of indicator to show (skeleton, inline spinner, non-blocking progress), and the target element/screen
- **Skeleton Layout**: The static placeholder presentation for a data screen — describes the shapes rendered in place of real content and how they map to the final content once loaded

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of user-triggered operations that wait on a network response display a loading indicator within 200 ms of initiation
- **SC-002**: Every data-driven screen shows a Facebook-style skeleton during initial load instead of a blank or static state
- **SC-003**: Replacing skeletons with real content causes a Cumulative Layout Shift (CLS) below 0.1 (content appears in the skeleton's position; verified via browser performance trace or screen capture)
- **SC-004**: No duplicate submissions occur from double-clicking a busy control (0 duplicate-created records attributable to loader absence)
- **SC-005**: Operations completing within 200 ms display no visible loading flash (verified by screen capture or manual review)
- **SC-006**: All loading indicators respect reduced-motion and are announced to assistive technologies (validated by accessibility review)

## Assumptions

- This feature is frontend-only: no backend or API contract changes are required; loading behavior reflects existing request latency
- No new third-party dependencies: skeletons and loading indicators are built in-house using the UI primitives and design tokens already available in the app
- Skeleton loading applies to the existing data screens in the Angular client: todo list, categories, todo detail, and profile; "lunch settings" and "dashboard" are out of scope (no such screens exist in the app yet — the shared skeleton component will cover any future screens)
- The app already has button-loading spinners on some forms (login, register, todo form, category form); this feature standardizes and extends that behavior to every operation
- Skeleton placeholders are used for initial data screens (lists, categories, settings); inline spinners are used for point-of-action feedback; both are valid "loaders" per this spec
- A "fast-operation threshold" of 200 ms is the canonical value: operations completing faster than 200 ms never display a loader; slower operations show one within 200 ms with a 300 ms minimum display time (see SC-001, SC-005, FR-007)
- Animation (shimmer/pulse) is optional and must respect reduced-motion preferences
- Existing routing transition animations remain unchanged
