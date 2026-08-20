import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardContent } from '@angular/material/card';
import { MatChip } from '@angular/material/chips';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { TodoService } from '../../../shared/services/todo.service';
import { TodoItem } from '../../../shared/models/todo.model';
import { SkeletonComponent } from '../../../shared/components/loading/skeleton.component';
import { ErrorStateComponent } from '../../../shared/components/loading/error-state.component';
import { withLoadingState } from '../../../shared/utils/loading.operator';

@Component({
  selector: 'app-todo-detail',
  standalone: true,
  imports: [DatePipe, MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardContent, MatChip, MatButton, MatIcon, SkeletonComponent, ErrorStateComponent],
  template: `
    <div class="detail-container" [attr.aria-busy]="loading()">
      <button mat-icon-button (click)="goBack()" aria-label="Back">
        <mat-icon>arrow_back</mat-icon>
      </button>

      @if (loading()) {
        <app-skeleton variant="detail" label="Loading todo..." />
      } @else if (error()) {
        <app-error-state message="Failed to load todo." (retry)="loadTodo()" />
      } @else if (todo()) {
        <mat-card>
          <mat-card-header>
            <mat-card-title>{{ todo()!.title }}</mat-card-title>
            <mat-card-subtitle>
              Created: {{ todo()!.createdAt | date:'medium' }}
            </mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            @if (todo()!.description) {
              <p>{{ todo()!.description }}</p>
            }
            <div class="chips">
              <mat-chip [style.--mdc-chip-elevated-container-color]="todo()!.priority.color" highlighted>
                {{ todo()!.priority.name }}
              </mat-chip>
              @if (todo()!.category) {
                <mat-chip [style.--mdc-chip-elevated-container-color]="todo()!.category!.color">
                  {{ todo()!.category!.name }}
                </mat-chip>
              }
              <mat-chip>{{ todo()!.status.name }}</mat-chip>
            </div>
            @if (todo()!.dueDate) {
              <p class="due-date">Due: {{ todo()!.dueDate | date:'mediumDate' }}</p>
            }
          </mat-card-content>
        </mat-card>
      } @else {
        <p>Todo not found.</p>
      }
    </div>
  `,
  styles: [`
    .detail-container { padding: 1rem; max-width: 600px; margin: 0 auto; }
    .chips { display: flex; gap: 0.5rem; flex-wrap: wrap; margin: 1rem 0; }
    .due-date { color: var(--mat-sys-on-surface-variant); margin-top: 1rem; }
  `]
})
export class TodoDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly todoService = inject(TodoService);
  private readonly destroyRef = inject(DestroyRef);

  readonly todo = signal<TodoItem | undefined>(undefined);
  readonly loading = signal(true);
  readonly error = signal(false);

  ngOnInit(): void {
    this.loadTodo();
  }

  loadTodo(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.goBack();
      return;
    }
    this.error.set(false);
    this.todoService.getById(id).pipe(
      withLoadingState(this.loading),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe({
      next: (item) => {
        this.todo.set(item);
      },
      error: () => {
        this.error.set(true);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/todos']);
  }
}
