import { Component, input, output, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatSelect, MatOption } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { TodoItem } from '../../../shared/models/todo.model';
import { Category } from '../../../shared/models/category.model';
import { CategoryService } from '../../../shared/services/category.service';
import { maxLengthValidator } from '../../../shared/validators/max-length.validator';
import { futureDateValidator } from '../../../shared/validators/future-date.validator';

@Component({
  selector: 'app-todo-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatButton, MatFormField, MatLabel, MatError, MatInput, MatSelect, MatOption, MatDatepickerModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="onSubmit()" class="todo-form">
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Title</mat-label>
        <input matInput formControlName="title" placeholder="Enter todo title" />
        @if (form.get('title')?.errors?.['required']) {
          <mat-error>Title is required.</mat-error>
        }
        @if (form.get('title')?.errors?.['maxLength']) {
          <mat-error>{{ form.get('title')?.errors?.['maxLength'] }}</mat-error>
        }
      </mat-form-field>

      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Description</mat-label>
        <textarea matInput formControlName="description" rows="3"></textarea>
        @if (form.get('description')?.errors?.['maxLength']) {
          <mat-error>{{ form.get('description')?.errors?.['maxLength'] }}</mat-error>
        }
      </mat-form-field>

      <div class="form-row">
        <mat-form-field appearance="fill">
          <mat-label>Priority</mat-label>
          <mat-select formControlName="priorityId">
            <mat-option [value]="1">Low</mat-option>
            <mat-option [value]="2">Medium</mat-option>
            <mat-option [value]="3">High</mat-option>
            <mat-option [value]="4">Urgent</mat-option>
          </mat-select>
          @if (form.get('priorityId')?.errors?.['required']) {
            <mat-error>Priority is required.</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="fill">
          <mat-label>Category</mat-label>
          <mat-select formControlName="categoryId">
            <mat-option [value]="undefined">None</mat-option>
            @for (cat of categories(); track cat.id) {
              <mat-option [value]="cat.id">{{ cat.name }}</mat-option>
            }
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="fill">
          <mat-label>Due Date</mat-label>
          <input matInput [matDatepicker]="picker" formControlName="dueDate" />
          <mat-datepicker-toggle matSuffix [for]="picker" />
          <mat-datepicker #picker />
          @if (form.get('dueDate')?.errors?.['futureDate']) {
            <mat-error>{{ form.get('dueDate')?.errors?.['futureDate'] }}</mat-error>
          }
        </mat-form-field>
      </div>

      <div class="form-actions">
        <button mat-button type="button" (click)="cancel.emit()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">
          {{ todo() ? 'Update' : 'Create' }}
        </button>
      </div>
    </form>
  `,
  styles: [`
    .todo-form { display: flex; flex-direction: column; gap: 1rem; padding: 1rem 0; }
    .full-width { width: 100%; }
    .form-row { display: flex; gap: 1rem; }
    .form-row mat-form-field { flex: 1; }
    .form-actions { display: flex; justify-content: flex-end; gap: 0.5rem; }
  `]
})
export class TodoFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly categoryService = inject(CategoryService);

  readonly todo = input<TodoItem | null>(null);
  readonly save = output<{ title: string; description?: string; dueDate?: string; priorityId: number; categoryId?: string; statusId?: number }>();
  readonly cancel = output<void>();

  readonly categories = signal<Category[]>([]);

  readonly form = this.fb.group({
    title: ['', [Validators.required, maxLengthValidator(200)]],
    description: ['', maxLengthValidator(2000)],
    priorityId: [2, Validators.required],
    categoryId: [''],
    dueDate: ['', futureDateValidator()],
  });

  ngOnInit(): void {
    this.categoryService.getList().subscribe(data => this.categories.set(data));

    const t = this.todo();
    if (t) {
      this.form.patchValue({
        title: t.title,
        description: t.description || '',
        priorityId: t.priority.id,
        categoryId: t.category?.id || '',
        dueDate: t.dueDate ? t.dueDate.split('T')[0] : '',
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const v = this.form.value;
    this.save.emit({
      title: v.title!,
      description: v.description || undefined,
      priorityId: v.priorityId!,
      categoryId: v.categoryId || undefined,
      dueDate: v.dueDate || undefined,
    });
  }
}
