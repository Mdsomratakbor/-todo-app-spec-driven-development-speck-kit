import { Component, input, output } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [MatIcon, MatButton],
  template: `
    <div class="empty-state" role="status" aria-live="polite">
      <div class="empty-icon-wrapper">
        <mat-icon class="empty-icon">{{ icon() }}</mat-icon>
      </div>
      <h3>{{ title() }}</h3>
      @if (message()) {
        <p>{{ message() }}</p>
      }
      @if (actionLabel()) {
        <button mat-raised-button color="primary" (click)="action.emit()">
          {{ actionLabel() }}
        </button>
      }
    </div>
  `,
  styles: [`
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 4rem 2rem;
      text-align: center;
      animation: fadeSlideIn 0.3s ease-out;
    }
    .empty-icon-wrapper {
      width: 6rem;
      height: 6rem;
      border-radius: 50%;
      background: var(--mat-sys-surface-variant);
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 1.5rem;
    }
    .empty-icon {
      font-size: 3rem;
      width: 3rem;
      height: 3rem;
      color: var(--mat-sys-outline);
    }
    h3 {
      margin: 0 0 0.5rem;
      color: var(--mat-sys-on-surface);
      font-size: 1.25rem;
    }
    p {
      margin: 0 0 1.5rem;
      color: var(--mat-sys-on-surface-variant);
      max-width: 320px;
    }
    @keyframes fadeSlideIn {
      from { opacity: 0; transform: translateY(8px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class EmptyStateComponent {
  readonly icon = input('inbox');
  readonly title = input('No data');
  readonly message = input('');
  readonly actionLabel = input('');
  readonly action = output<void>();
}
