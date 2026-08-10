import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

export type NotificationSeverity = 'success' | 'error' | 'warning';

const SEVERITY_DURATION_MS: Record<NotificationSeverity, number> = {
  success: 4000,
  warning: 6000,
  error: 8000,
};

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly snackBar = inject(MatSnackBar);

  show(message: string, severity: NotificationSeverity = 'success'): void {
    const panelClass = [`snackbar-${severity}`];
    const duration = SEVERITY_DURATION_MS[severity];
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
