import { Observable, OperatorFunction } from 'rxjs';
import { WritableSignal } from '@angular/core';

export interface LoadingStateOptions {
  /** Source settling sooner than this never turns the indicator on (no one-frame flash). */
  deferMs?: number;
  /** Once the indicator is on, it stays on for at least this long after the source settles. */
  minMs?: number;
}

const DEFAULT_DEFER_MS = 200;
const DEFAULT_MIN_MS = 300;

/**
 * Drives a `WritableSignal<boolean>` loading indicator around a source observable.
 *
 * - If the source settles within `deferMs` (200ms default), the indicator is never turned on
 *   (SC-005: no flicker for fast operations).
 * - Once on, the indicator stays on for at least `minMs` (300ms default) after the source settles
 *   (SC-001 minimum display time).
 * - The source value/error/complete is forwarded unchanged.
 * - On manual unsubscribe (e.g. navigation via `takeUntilDestroyed`) the indicator is cleared
 *   immediately so a pending load never leaks onto a destroyed screen.
 */
export function withLoadingState<T>(
  loading: WritableSignal<boolean>,
  opts: LoadingStateOptions = {},
): OperatorFunction<T, T> {
  const deferMs = opts.deferMs ?? DEFAULT_DEFER_MS;
  const minMs = opts.minMs ?? DEFAULT_MIN_MS;

  return (source: Observable<T>): Observable<T> =>
    new Observable<T>((subscriber) => {
      let settled = false;
      let settleTimer: ReturnType<typeof setTimeout> | undefined;

      const showTimer = setTimeout(() => {
        loading.set(true);
      }, deferMs);

      const settle = (): void => {
        if (settled) return;
        settled = true;
        clearTimeout(showTimer);
        settleTimer = setTimeout(() => loading.set(false), minMs);
      };

      const subscription = source.subscribe({
        next: (value) => {
          settle();
          subscriber.next(value);
        },
        error: (err) => {
          settle();
          subscriber.error(err);
        },
        complete: () => {
          settle();
          subscriber.complete();
        },
      });

      return () => {
        clearTimeout(showTimer);
        if (!settled) {
          if (settleTimer) clearTimeout(settleTimer);
          loading.set(false);
        }
        subscription.unsubscribe();
      };
    });
}
