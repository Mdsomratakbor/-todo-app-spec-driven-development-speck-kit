import { Component, output, signal, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatSelect, MatOption } from '@angular/material/select';
import { MatInput } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { TodoService } from '../../../shared/services/todo.service';
import { Category } from '../../../shared/models/category.model';
import { Priority } from '../../../shared/models/priority.model';
import { Status } from '../../../shared/models/status.model';

export interface TodoFilters {
  search?: string;
  statusId?: number;
  priorityId?: number;
  categoryId?: string;
  dueDateFrom?: string;
  dueDateTo?: string;
}

@Component({
  selector: 'app-filter-bar',
  standalone: true,
  imports: [FormsModule, MatFormField, MatLabel, MatSelect, MatOption, MatInput, MatDatepickerModule, MatButton, MatIcon],
  template: `
    <div class="filter-bar">
      <mat-form-field appearance="fill" class="search-field">
        <mat-label>Search</mat-label>
        <input matInput [(ngModel)]="search" placeholder="Search todos..." />
      </mat-form-field>

      <mat-form-field appearance="fill">
        <mat-label>Status</mat-label>
        <mat-select [(ngModel)]="statusId">
          <mat-option [value]="undefined">All</mat-option>
          @for (s of statuses(); track s.id) {
            <mat-option [value]="s.id">{{ s.name }}</mat-option>
          }
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="fill">
        <mat-label>Priority</mat-label>
        <mat-select [(ngModel)]="priorityId">
          <mat-option [value]="undefined">All</mat-option>
          @for (p of priorities(); track p.id) {
            <mat-option [value]="p.id">{{ p.name }}</mat-option>
          }
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="fill">
        <mat-label>Category</mat-label>
        <mat-select [(ngModel)]="categoryId">
          <mat-option [value]="undefined">All</mat-option>
          @for (c of categories(); track c.id) {
            <mat-option [value]="c.id">{{ c.name }}</mat-option>
          }
        </mat-select>
      </mat-form-field>

      <mat-form-field appearance="fill">
        <mat-label>Due From</mat-label>
        <input matInput [matDatepicker]="fromPicker" [(ngModel)]="dueDateFrom" />
        <mat-datepicker-toggle matSuffix [for]="fromPicker" />
        <mat-datepicker #fromPicker />
      </mat-form-field>

      <mat-form-field appearance="fill">
        <mat-label>Due To</mat-label>
        <input matInput [matDatepicker]="toPicker" [(ngModel)]="dueDateTo" />
        <mat-datepicker-toggle matSuffix [for]="toPicker" />
        <mat-datepicker #toPicker />
      </mat-form-field>

      <button mat-raised-button color="primary" (click)="apply()" aria-label="Apply filters">
        <mat-icon>search</mat-icon>
        Apply
      </button>

      <button mat-button (click)="clear()" aria-label="Clear filters">
        <mat-icon>clear</mat-icon>
        Clear
      </button>
    </div>
  `,
  styles: [`
    .filter-bar { display: flex; gap: 0.75rem; align-items: center; flex-wrap: wrap; padding: 0.5rem 0; }
    .search-field { flex: 1; min-width: 200px; }
  `]
})
export class FilterBarComponent implements OnInit {
  private readonly todoService = inject(TodoService);

  readonly filtersChanged = output<TodoFilters>();

  readonly statuses = signal<Status[]>([]);
  readonly priorities = signal<Priority[]>([]);
  readonly categories = signal<Category[]>([]);

  search = '';
  statusId: number | undefined;
  priorityId: number | undefined;
  categoryId: string | undefined;
  dueDateFrom = '';
  dueDateTo = '';

  ngOnInit(): void {
    this.todoService.getStatuses().subscribe(s => this.statuses.set(s));
    this.todoService.getPriorities().subscribe(p => this.priorities.set(p));
    this.todoService.getCategories().subscribe(c => this.categories.set(c));
  }

  apply(): void {
    this.filtersChanged.emit({
      search: this.search || undefined,
      statusId: this.statusId,
      priorityId: this.priorityId,
      categoryId: this.categoryId,
      dueDateFrom: this.dueDateFrom || undefined,
      dueDateTo: this.dueDateTo || undefined,
    });
  }

  clear(): void {
    this.search = '';
    this.statusId = undefined;
    this.priorityId = undefined;
    this.categoryId = undefined;
    this.dueDateFrom = '';
    this.dueDateTo = '';
    this.filtersChanged.emit({});
  }
}
