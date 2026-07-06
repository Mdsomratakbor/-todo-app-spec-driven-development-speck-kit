import { Component, input } from '@angular/core';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [MatProgressSpinner],
  template: `
    <div class="spinner-container">
      <mat-spinner [diameter]="diameter()" />
      @if (message()) {
        <p class="spinner-message">{{ message() }}</p>
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
  `]
})
export class LoadingSpinnerComponent {
  readonly diameter = input(40);
  readonly message = input('');
}
