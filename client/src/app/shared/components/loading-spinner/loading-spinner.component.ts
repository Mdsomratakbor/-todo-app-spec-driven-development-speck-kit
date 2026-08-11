import { Component, input } from '@angular/core';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [MatProgressSpinner],
  template: `
    <div role="status" aria-live="polite">
    @if (skeleton()) {
      <div class="skeleton-list">
        @for (_ of [1,2,3]; track $index) {
          <div class="skeleton-card">
            <div class="skeleton-line skeleton-title"></div>
            <div class="skeleton-line skeleton-text"></div>
            <div class="skeleton-line skeleton-text short"></div>
          </div>
        }
      </div>
    } @else {
      <div class="spinner-container">
        <mat-spinner [diameter]="diameter()" />
        @if (message()) {
          <p class="spinner-message">{{ message() }}</p>
        }
      </div>
    }
    </div>
  `,
  styles: [`
    .spinner-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 2rem;
    }
    .spinner-message {
      margin-top: 1rem;
      color: var(--mat-sys-on-surface-variant);
    }
    .skeleton-list {
      display: flex;
      flex-direction: column;
      gap: 1rem;
      padding: 1rem 0;
    }
    .skeleton-card {
      border-radius: 8px;
      padding: 1rem;
      background: var(--mat-sys-surface);
      border: 1px solid var(--mat-sys-outline-variant);
    }
    .skeleton-line {
      height: 14px;
      border-radius: 4px;
      background: linear-gradient(90deg, var(--mat-sys-surface-variant) 25%, var(--mat-sys-outline-variant) 50%, var(--mat-sys-surface-variant) 75%);
      background-size: 200% 100%;
      animation: shimmer 1.5s infinite;
      margin-bottom: 0.5rem;
    }
    .skeleton-title {
      width: 60%;
      height: 18px;
    }
    .skeleton-text.short { width: 40%; }
    @keyframes shimmer {
      0% { background-position: 200% 0; }
      100% { background-position: -200% 0; }
    }
  `]
})
export class LoadingSpinnerComponent {
  readonly diameter = input(40);
  readonly message = input('');
  readonly skeleton = input(false);
}
