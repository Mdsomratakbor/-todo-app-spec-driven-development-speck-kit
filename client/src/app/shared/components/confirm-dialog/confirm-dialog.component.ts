import { Component, inject } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface ConfirmDialogData {
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
}

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [MatButton, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose],
  template: `
    <div role="alertdialog" aria-labelledby="dialog-title" aria-describedby="dialog-message">
    <h2 mat-dialog-title id="dialog-title">{{ data.title }}</h2>
    <mat-dialog-content id="dialog-message">
      <p>{{ data.message }}</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>{{ data.cancelLabel || 'Cancel' }}</button>
      <button mat-raised-button color="warn" [mat-dialog-close]="true" aria-label="{{ data.confirmLabel || 'Confirm' }}">
        {{ data.confirmLabel || 'Confirm' }}
      </button>
    </mat-dialog-actions>
    </div>
  `
})
export class ConfirmDialogComponent {
  readonly dialogRef = inject(MatDialogRef<ConfirmDialogComponent>);
  readonly data = inject<ConfirmDialogData>(MAT_DIALOG_DATA);
}
