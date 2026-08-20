import { WritableSignal, signal } from '@angular/core';
import { withLoadingState, LoadingStateOptions } from './loading.operator';
import { Observable, Subject } from 'rxjs';

describe('withLoadingState', () => {
  let loading: WritableSignal<boolean>;

  beforeEach(() => {
    vi.useFakeTimers();
    loading = signal(false);
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('should set loading to true after deferMs if source is slow', () => {
    const source = new Subject<string>();
    const options: LoadingStateOptions = { deferMs: 200, minMs: 300 };

    source.pipe(withLoadingState(loading, options)).subscribe();

    expect(loading()).toBe(false);

    vi.advanceTimersByTime(200);
    expect(loading()).toBe(true);

    source.next('value');
    source.complete();

    vi.advanceTimersByTime(300);
    expect(loading()).toBe(false);
  });

  it('should never set loading if source emits within deferMs', () => {
    const source = new Subject<string>();
    const options: LoadingStateOptions = { deferMs: 200, minMs: 300 };

    source.pipe(withLoadingState(loading, options)).subscribe();

    source.next('fast-value');
    source.complete();
    vi.runAllTimers();

    expect(loading()).toBe(false);
  });

  it('should hold loading for at least minMs after source settles', () => {
    const source = new Subject<string>();
    const options: LoadingStateOptions = { deferMs: 100, minMs: 300 };

    source.pipe(withLoadingState(loading, options)).subscribe();

    vi.advanceTimersByTime(150);
    expect(loading()).toBe(true);

    source.next('value');
    source.complete();

    vi.advanceTimersByTime(100);
    expect(loading()).toBe(true);

    vi.advanceTimersByTime(200);
    expect(loading()).toBe(false);
  });

  it('should forward value unchanged', () => {
    let received: string | undefined;
    const source = new Subject<string>();

    source.pipe(withLoadingState(loading, { deferMs: 0, minMs: 0 })).subscribe({
      next: (v) => { received = v; },
    });

    source.next('test-value');
    source.complete();
    vi.runAllTimers();

    expect(received).toBe('test-value');
  });

  it('should forward error unchanged', () => {
    let receivedError: unknown;
    const source = new Subject<string>();

    source.pipe(withLoadingState(loading, { deferMs: 0, minMs: 0 })).subscribe({
      error: (e) => { receivedError = e; },
    });

    source.error(new Error('fail'));
    vi.runAllTimers();

    expect(receivedError).toBeInstanceOf(Error);
    expect((receivedError as Error).message).toBe('fail');
  });

  it('should reset loading on error', () => {
    loading.set(true);
    const source = new Subject<string>();

    source.pipe(withLoadingState(loading, { deferMs: 0, minMs: 0 })).subscribe({
      error: () => {},
    });

    source.error(new Error('fail'));
    vi.runAllTimers();
    expect(loading()).toBe(false);
  });

  it('should reset loading on unsubscribe before settle', () => {
    loading.set(true);
    const source = new Observable(() => {});

    const sub = source.pipe(withLoadingState(loading, { deferMs: 200, minMs: 300 })).subscribe();
    expect(loading()).toBe(true);

    sub.unsubscribe();
    expect(loading()).toBe(false);
  });

  it('should use default deferMs=200 and minMs=300 when not specified', () => {
    const source = new Subject<string>();
    source.pipe(withLoadingState(loading)).subscribe();
    source.next('v');
    source.complete();
    vi.runAllTimers();
    expect(loading()).toBe(false);
  });
});
