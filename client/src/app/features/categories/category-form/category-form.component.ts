import { Component, input, output, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { Category } from '../../../shared/models/category.model';
import { maxLengthValidator } from '../../../shared/validators/max-length.validator';
import { hexColorValidator } from '../../../shared/validators/hex-color.validator';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatButton, MatIcon, MatFormField, MatLabel, MatError, MatInput],
  template: `
    <form [formGroup]="form" (ngSubmit)="onSubmit()" class="category-form">
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Name</mat-label>
        <input matInput formControlName="name" placeholder="Enter category name" />
        @if (form.get('name')?.errors?.['required']) {
          <mat-error>Name is required.</mat-error>
        }
        @if (form.get('name')?.errors?.['maxLength']) {
          <mat-error>{{ form.get('name')?.errors?.['maxLength'] }}</mat-error>
        }
      </mat-form-field>

      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Color</mat-label>
        <input matInput formControlName="color" placeholder="#3498DB" />
        @if (form.get('color')?.errors?.['hexColor']) {
          <mat-error>{{ form.get('color')?.errors?.['hexColor'] }}</mat-error>
        }
      </mat-form-field>

      <div class="form-actions">
        <button mat-button type="button" (click)="cancel.emit()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="saving() || form.invalid">
          @if (saving()) {
            <mat-icon class="btn-spinner" fontIcon="sync" />
            <span>Saving...</span>
          } @else {
            {{ category() ? 'Update' : 'Create' }}
          }
        </button>
      </div>
    </form>
  `,
  styles: [`
    .category-form { display: flex; flex-direction: column; gap: 1rem; padding: 1rem 0; }
    .full-width { width: 100%; }
    .form-actions { display: flex; justify-content: flex-end; gap: 0.5rem; }
    .btn-spinner { display: inline-flex; vertical-align: middle; margin-right: 4px; }
    @media (prefers-reduced-motion: no-preference) {
      .btn-spinner { animation: spin 1s linear infinite; }
      @keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }
    }
  `]
})
export class CategoryFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  readonly category = input<Category | null>(null);
  readonly saving = input(false);
  readonly save = output<{ name: string; color?: string }>();
  readonly cancel = output<void>();

  readonly form = this.fb.group({
    name: ['', [Validators.required, maxLengthValidator(100)]],
    color: ['', hexColorValidator()],
  });

  ngOnInit(): void {
    const c = this.category();
    if (c) {
      this.form.patchValue({
        name: c.name,
        color: c.color || '',
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const v = this.form.value;
    this.save.emit({
      name: v.name!,
      color: v.color || undefined,
    });
  }
}
