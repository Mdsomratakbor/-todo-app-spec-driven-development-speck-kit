import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatButton } from '@angular/material/button';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatDialog } from '@angular/material/dialog';
import { CategoryService } from '../../../shared/services/category.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { Category } from '../../../shared/models/category.model';
import { CategoryCardComponent } from '../category-card/category-card.component';
import { CategoryFormComponent } from '../category-form/category-form.component';
import { SkeletonComponent } from '../../../shared/components/loading/skeleton.component';
import { ErrorStateComponent } from '../../../shared/components/loading/error-state.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { withLoadingState } from '../../../shared/utils/loading.operator';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [MatButton, MatProgressBar, CategoryCardComponent, CategoryFormComponent, SkeletonComponent, ErrorStateComponent, EmptyStateComponent],
  template: `
    <div class="category-list-container" role="region" aria-label="Category list" [attr.aria-busy]="loading() || refreshing()">
      <div class="header">
        <h1>Categories</h1>
        <button mat-raised-button color="primary" (click)="showCreateForm()">
          Add Category
        </button>
      </div>

      @if (editingCategory) {
        <app-category-form [category]="editingCategory" (save)="onSave($event)" (cancel)="cancelEdit()" />
      }
      @if (creating) {
        <app-category-form (save)="onCreate($event)" (cancel)="creating = false" />
      }

      @if (loading()) {
        <app-skeleton variant="category-list" label="Loading categories..." />
      } @else if (error()) {
        <app-error-state message="Failed to load categories." (retry)="loadCategories()" />
      } @else if (categories().length === 0) {
        <app-empty-state
          icon="folder"
          title="No categories yet"
          message="Create your first category to organize your todos!"
          actionLabel="Create Category"
          (action)="showCreateForm()"
        />
      } @else {
        @if (refreshing()) {
          <mat-progress-bar mode="indeterminate" class="refresh-bar" />
        }
        <div class="category-grid">
          @for (category of categories(); track category.id) {
            <app-category-card
              [category]="category"
              [deleting]="deletingId() === category.id"
              (edit)="startEdit($event)"
              (delete)="confirmDelete($event)"
            />
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .category-list-container { padding: 1rem; max-width: 600px; margin: 0 auto; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
    .category-grid { display: grid; gap: 0.5rem; }
    .refresh-bar { margin-bottom: 1rem; }
  `]
})
export class CategoryListComponent implements OnInit {
  private readonly categoryService = inject(CategoryService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);
  private readonly destroyRef = inject(DestroyRef);

  readonly categories = signal<Category[]>([]);
  readonly loading = signal(false);
  readonly refreshing = signal(false);
  readonly error = signal(false);
  readonly deletingId = signal<string | null>(null);

  creating = false;
  editingCategory: Category | null = null;

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.error.set(false);
    const hasData = this.categories().length > 0;
    this.categoryService.getList().pipe(
      withLoadingState(hasData ? this.refreshing : this.loading),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe({
      next: (data) => {
        this.categories.set(data);
      },
      error: () => {
        if (!hasData) {
          this.error.set(true);
        }
      }
    });
  }

  showCreateForm(): void {
    this.creating = true;
    this.editingCategory = null;
  }

  onCreate(data: { name: string; color?: string }): void {
    this.categoryService.create({ name: data.name, color: data.color }).subscribe({
      next: () => {
        this.notification.success('Category created successfully!');
        this.creating = false;
        this.loadCategories();
      },
      error: () => undefined
    });
  }

  startEdit(category: Category): void {
    this.editingCategory = category;
    this.creating = false;
  }

  cancelEdit(): void {
    this.editingCategory = null;
  }

  onSave(data: { name: string; color?: string }): void {
    if (!this.editingCategory) return;
    this.categoryService.update(this.editingCategory.id, { name: data.name, color: data.color }).subscribe({
      next: () => {
        this.notification.success('Category updated successfully!');
        this.editingCategory = null;
        this.loadCategories();
      },
      error: () => undefined
    });
  }

  confirmDelete(id: string): void {
    const category = this.categories().find(c => c.id === id);
    const todoCount = category?.todoCount ?? 0;
    const message = todoCount > 0
      ? `Delete "${category?.name}"? ${todoCount} todo(s) will be moved to Uncategorized.`
      : `Are you sure you want to delete "${category?.name}"?`;

    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Category',
        message,
        confirmLabel: 'Delete',
      }
    });
    ref.afterClosed().subscribe(result => {
      if (result) this.deleteCategory(id);
    });
  }

  private deleteCategory(id: string): void {
    this.deletingId.set(id);
    this.categoryService.delete(id).subscribe({
      next: () => {
        this.deletingId.set(null);
        this.notification.success('Category deleted successfully!');
        this.loadCategories();
      },
      error: () => {
        this.deletingId.set(null);
      }
    });
  }
}
