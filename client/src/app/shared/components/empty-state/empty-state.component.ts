import { Component, input, output } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [MatIcon, MatButton],
  template: `
    <div class="empty-state">
      <mat-icon class="empty-icon">{{ icon() }}</mat-icon>
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
      padding: 3rem 2rem;
      text-align: center;
    }
    .empty-icon {
      font-size: 4rem;
      width: 4rem;
      height: 4rem;
      color: var(--mat-sys-outline);
      margin-bottom: 1rem;
    }
    h3 {
      margin: 0 0 0.5rem;
      color: var(--mat-sys-on-surface);
    }
    p {
      margin: 0 0 1.5rem;
      color: var(--mat-sys-on-surface-variant);
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
