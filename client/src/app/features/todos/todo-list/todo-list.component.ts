import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatButton } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { TodoService } from '../../../shared/services/todo.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { TodoItem, CreateTodoRequest, UpdateTodoRequest } from '../../../shared/models/todo.model';
import { TodoCardComponent } from '../todo-card/todo-card.component';
import { TodoFormComponent } from '../todo-form/todo-form.component';
import { FilterBarComponent, TodoFilters } from '../filter-bar/filter-bar.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { SkeletonComponent } from '../../../shared/components/loading/skeleton.component';
import { ErrorStateComponent } from '../../../shared/components/loading/error-state.component';
import { withLoadingState } from '../../../shared/utils/loading.operator';

@Component({
  selector: 'app-todo-list',
  standalone: true,
  imports: [MatPaginator, MatProgressBar, MatButton, TodoCardComponent, TodoFormComponent, FilterBarComponent, SkeletonComponent, ErrorStateComponent, EmptyStateComponent],
  template: `
    <div class="todo-list-container" role="region" aria-label="Todo list" [attr.aria-busy]="loading() || refreshing()">
      <div class="header">
        <h1>Todo List</h1>
        <button mat-raised-button color="primary" (click)="showCreateForm()">
          Add Todo
        </button>
      </div>

      <app-filter-bar [disabled]="loading() || refreshing()" (filtersChanged)="onFiltersChanged($event)" />

      @if (editingTodo) {
        <app-todo-form [todo]="editingTodo" (save)="onSave($event)" (cancel)="cancelEdit()" />
      }

      @if (creating) {
        <app-todo-form (save)="onCreate($event)" (cancel)="creating = false" />
      }

      @if (loading()) {
        <app-skeleton variant="todo-list" label="Loading todos..." />
      } @else if (error()) {
        <app-error-state message="Failed to load todos." (retry)="loadTodos()" />
      } @else if (todos().length === 0) {
        <app-empty-state
          icon="checklist"
          title="No todos yet"
          message="Create your first todo to get started!"
          actionLabel="Create Todo"
          (action)="showCreateForm()"
        />
      } @else {
        @if (refreshing()) {
          <mat-progress-bar mode="indeterminate" class="refresh-bar" />
        }
        <div class="todo-grid">
          @for (todo of todos(); track todo.id) {
            <app-todo-card
              [todo]="todo"
              [deleting]="deletingId() === todo.id"
              (edit)="startEdit($event)"
              (delete)="confirmDelete($event)"
            />
          }
        </div>

        <mat-paginator
          [length]="totalCount()"
          [pageSize]="pageSize()"
          [pageIndex]="page() - 1"
          (page)="onPageChange($event)"
          [pageSizeOptions]="[10, 20, 50]"
          showFirstLastButtons
          aria-label="Todo pagination"
        />
      }
    </div>
  `,
  styles: [`
    .todo-list-container { padding: 1rem; max-width: 900px; margin: 0 auto; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
    .todo-grid { display: grid; gap: 1rem; }
    .todo-grid > * { animation: fadeSlideIn 0.3s ease-out; }
    @keyframes fadeSlideIn {
      from { opacity: 0; transform: translateY(8px); }
      to { opacity: 1; transform: translateY(0); }
    }
    .refresh-bar { margin-bottom: 1rem; }
  `]
})
export class TodoListComponent implements OnInit {
  private readonly todoService = inject(TodoService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  private readonly destroyRef = inject(DestroyRef);

  readonly todos = signal<TodoItem[]>([]);
  readonly loading = signal(false);
  readonly refreshing = signal(false);
  readonly error = signal(false);
  readonly totalCount = signal(0);
  readonly page = signal(1);
  readonly pageSize = signal(20);
  readonly deletingId = signal<string | null>(null);

  filters: TodoFilters = {};

  creating = false;
  editingTodo: TodoItem | null = null;

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.error.set(false);
    const hasData = this.todos().length > 0;
    this.todoService.getList({ page: this.page(), pageSize: this.pageSize(), ...this.filters }).pipe(
      withLoadingState(hasData ? this.refreshing : this.loading),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe({
      next: (res) => {
        this.todos.set(res.data);
        this.totalCount.set(res.totalCount);
      },
      error: () => {
        if (!hasData) {
          this.error.set(true);
        }
      }
    });
  }

  onFiltersChanged(filters: TodoFilters): void {
    this.filters = filters;
    this.page.set(1);
    this.loadTodos();
  }

  showCreateForm(): void {
    this.creating = true;
    this.editingTodo = null;
  }

  onCreate(data: { title: string; description?: string; dueDate?: string; priorityId: number; categoryId?: string; statusId?: number }): void {
    const request: CreateTodoRequest = { title: data.title, description: data.description, dueDate: data.dueDate, priorityId: data.priorityId, categoryId: data.categoryId };
    this.todoService.create(request).subscribe({
      next: () => {
        this.notification.success('Todo created successfully!');
        this.creating = false;
        this.loadTodos();
      },
      error: () => undefined
    });
  }

  startEdit(todo: TodoItem): void {
    this.editingTodo = todo;
    this.creating = false;
  }

  cancelEdit(): void {
    this.editingTodo = null;
  }

  onSave(data: { title: string; description?: string; dueDate?: string; priorityId: number; categoryId?: string; statusId?: number }): void {
    if (!this.editingTodo) return;
    const request: UpdateTodoRequest = { title: data.title, description: data.description, dueDate: data.dueDate, priorityId: data.priorityId, categoryId: data.categoryId, statusId: data.statusId ?? this.editingTodo.status.id };
    this.todoService.update(this.editingTodo.id, request).subscribe({
      next: () => {
        this.notification.success('Todo updated successfully!');
        this.editingTodo = null;
        this.loadTodos();
      },
      error: () => undefined
    });
  }

  confirmDelete(id: string): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Delete Todo', message: 'Are you sure you want to delete this todo?', confirmLabel: 'Delete' }
    });
    ref.afterClosed().subscribe(result => {
      if (result) this.deleteTodo(id);
    });
  }

  private deleteTodo(id: string): void {
    this.deletingId.set(id);
    this.todoService.delete(id).subscribe({
      next: () => {
        this.deletingId.set(null);
        this.notification.success('Todo deleted successfully!');
        this.loadTodos();
      },
      error: () => {
        this.deletingId.set(null);
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.page.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadTodos();
  }
}
