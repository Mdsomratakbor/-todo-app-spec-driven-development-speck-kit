import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { CategoryService } from '../../../shared/services/category.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { Category } from '../../../shared/models/category.model';
import { CategoryCardComponent } from '../category-card/category-card.component';
import { CategoryFormComponent } from '../category-form/category-form.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [MatButton, CategoryCardComponent, CategoryFormComponent, LoadingSpinnerComponent, EmptyStateComponent],
  template: `
    <div class="category-list-container" role="region" aria-label="Category list">
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
        <app-loading-spinner message="Loading categories..." />
      } @else if (categories().length === 0) {
        <app-empty-state
          icon="folder"
          title="No categories yet"
          message="Create your first category to organize your todos!"
          actionLabel="Create Category"
          (action)="showCreateForm()"
        />
      } @else {
        <div class="category-grid">
          @for (category of categories(); track category.id) {
            <app-category-card
              [category]="category"
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
  `]
})
export class CategoryListComponent implements OnInit {
  private readonly categoryService = inject(CategoryService);
  private readonly notification = inject(NotificationService);
  private readonly dialog = inject(MatDialog);

  readonly categories = signal<Category[]>([]);
  readonly loading = signal(false);

  creating = false;
  editingCategory: Category | null = null;

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.loading.set(true);
    this.categoryService.getList().subscribe({
      next: (data) => {
        this.categories.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.notification.error('Failed to load categories.');
        this.loading.set(false);
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
      error: () => this.notification.error('Failed to create category.')
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
      error: () => this.notification.error('Failed to update category.')
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
    this.categoryService.delete(id).subscribe({
      next: () => {
        this.notification.success('Category deleted successfully!');
        this.loadCategories();
      },
      error: () => this.notification.error('Failed to delete category.')
    });
  }
}
