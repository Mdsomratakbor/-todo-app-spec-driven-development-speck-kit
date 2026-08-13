import { Component, input, output } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-error-state',
  standalone: true,
  imports: [MatIcon, MatButton],
  template: `
    <div class="error-state" role="status">
      <div class="error-icon-wrapper">
        <mat-icon class="error-icon">error_outline</mat-icon>
      </div>
      <h3>{{ message() }}</h3>
      <button mat-raised-button color="primary" (click)="retry.emit()" [disabled]="retrying()">
        @if (retrying()) {
          <mat-icon class="btn-spinner" fontIcon="sync" />
        }
        {{ retryLabel() }}
      </button>
    </div>
  `,
  styles: [`
    .error-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 4rem 2rem;
      text-align: center;
    }
    .error-icon-wrapper {
      width: 6rem;
      height: 6rem;
      border-radius: 50%;
      background: var(--mat-sys-error-container);
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 1.5rem;
    }
    .error-icon {
      font-size: 3rem;
      width: 3rem;
      height: 3rem;
      color: var(--mat-sys-error);
    }
    h3 {
      margin: 0 0 1.5rem;
      color: var(--mat-sys-on-surface);
      font-size: 1.1rem;
      font-weight: 500;
      max-width: 320px;
    }
  `],
})
export class ErrorStateComponent {
  readonly message = input('Something went wrong.');
  readonly retryLabel = input('Retry');
  readonly retrying = input(false);
  readonly retry = output<void>();
}
