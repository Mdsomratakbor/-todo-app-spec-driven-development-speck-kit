import { Component, input } from '@angular/core';

export type SkeletonVariant = 'todo-list' | 'category-list' | 'detail' | 'profile' | 'rows';

@Component({
  selector: 'app-skeleton',
  standalone: true,
  imports: [],
  template: `
    <div class="skeleton" role="status" aria-live="polite">
      @if (label()) {
        <span class="visually-hidden">{{ label() }}</span>
      }
      @switch (variant()) {
        @case ('todo-list') {
          @for (_ of range(); track $index) {
            <div class="skeleton-card">
              <div class="skeleton-line skeleton-title"></div>
              <div class="skeleton-line skeleton-subtitle"></div>
              <div class="skeleton-line skeleton-text"></div>
              <div class="skeleton-chips">
                <span class="skeleton-chip"></span>
                <span class="skeleton-chip"></span>
                <span class="skeleton-chip"></span>
              </div>
            </div>
          }
        }
        @case ('category-list') {
          @for (_ of range(); track $index) {
            <div class="skeleton-row">
              <span class="skeleton-dot"></span>
              <div class="skeleton-line skeleton-row-title"></div>
              <div class="skeleton-line skeleton-row-subtitle"></div>
            </div>
          }
        }
        @case ('detail') {
          <div class="skeleton-card skeleton-detail">
            <div class="skeleton-line skeleton-detail-title"></div>
            <div class="skeleton-line skeleton-detail-subtitle"></div>
            <div class="skeleton-line skeleton-text"></div>
            <div class="skeleton-line skeleton-text"></div>
            <div class="skeleton-line skeleton-text short"></div>
            <div class="skeleton-chips">
              <span class="skeleton-chip"></span>
              <span class="skeleton-chip"></span>
              <span class="skeleton-chip"></span>
            </div>
            <div class="skeleton-line skeleton-text"></div>
          </div>
        }
        @case ('profile') {
          <div class="skeleton-card skeleton-profile">
            <div class="skeleton-line skeleton-profile-title"></div>
            <div class="skeleton-line skeleton-field"></div>
            <div class="skeleton-line skeleton-field"></div>
            <div class="skeleton-line skeleton-field"></div>
          </div>
        }
        @default {
          @for (_ of range(); track $index) {
            <div class="skeleton-row">
              <div class="skeleton-line skeleton-row-title"></div>
            </div>
          }
        }
      }
    </div>
  `,
  styles: [`
    .visually-hidden {
      position: absolute;
      width: 1px;
      height: 1px;
      margin: -1px;
      padding: 0;
      overflow: hidden;
      clip: rect(0 0 0 0);
      white-space: nowrap;
      border: 0;
    }
    .skeleton {
      padding: 1rem 0;
    }
    .skeleton-card {
      border-radius: 8px;
      padding: 1rem;
      margin-bottom: 1rem;
      background: var(--mat-sys-surface);
      border: 1px solid var(--mat-sys-outline-variant);
    }
    .skeleton-row {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      border-radius: 8px;
      padding: 1rem;
      margin-bottom: 0.5rem;
      background: var(--mat-sys-surface);
      border: 1px solid var(--mat-sys-outline-variant);
    }
    .skeleton-line {
      height: 14px;
      border-radius: 4px;
      background: linear-gradient(90deg, var(--mat-sys-surface-variant) 25%, var(--mat-sys-outline-variant) 50%, var(--mat-sys-surface-variant) 75%);
      background-size: 200% 100%;
      margin-bottom: 0.5rem;
    }
    .skeleton-title { width: 60%; height: 18px; }
    .skeleton-subtitle { width: 35%; height: 12px; }
    .skeleton-text { width: 100%; }
    .skeleton-text.short { width: 40%; }
    .skeleton-chips { display: flex; gap: 0.5rem; margin-top: 0.75rem; }
    .skeleton-chip { width: 64px; height: 24px; border-radius: 16px; background: var(--mat-sys-surface-variant); }
    .skeleton-dot { width: 12px; height: 12px; border-radius: 50%; background: var(--mat-sys-surface-variant); flex-shrink: 0; }
    .skeleton-row-title { flex: 1; margin-bottom: 0; }
    .skeleton-row-subtitle { width: 20%; margin-bottom: 0; }
    .skeleton-detail-title { width: 70%; height: 20px; }
    .skeleton-detail-subtitle { width: 45%; height: 12px; }
    .skeleton-profile { max-width: 400px; margin: 0 auto; }
    .skeleton-profile-title { width: 50%; height: 18px; }
    .skeleton-field { width: 100%; height: 16px; }
    @media (prefers-reduced-motion: no-preference) {
      .skeleton-line {
        animation: shimmer 1.5s infinite;
      }
    }
    @keyframes shimmer {
      0% { background-position: 200% 0; }
      100% { background-position: -200% 0; }
    }
  `],
})
export class SkeletonComponent {
  readonly variant = input<SkeletonVariant>('rows');
  readonly count = input(3);
  readonly label = input('');

  range(): number[] {
    return Array.from({ length: this.count() }, (_, i) => i);
  }
}
