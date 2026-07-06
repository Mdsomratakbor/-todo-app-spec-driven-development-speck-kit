import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

export type NotificationSeverity = 'success' | 'error' | 'warning';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly snackBar = inject(MatSnackBar);
  private readonly duration = 4000;
  private readonly warningDuration = 6000;

  show(message: string, severity: NotificationSeverity = 'success'): void {
    const panelClass = [`snackbar-${severity}`];
    const duration = severity === 'warning' ? this.warningDuration : this.duration;
    this.snackBar.open(message, 'Close', { duration, panelClass });
  }

  success(message: string): void {
    this.show(message, 'success');
  }

  error(message: string): void {
    this.show(message, 'error');
  }

  warning(message: string): void {
    this.show(message, 'warning');
  }
}
