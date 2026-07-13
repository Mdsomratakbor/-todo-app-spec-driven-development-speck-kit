import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { MatChip } from '@angular/material/chips';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { TodoItem } from '../../../shared/models/todo.model';

@Component({
  selector: 'app-todo-card',
  standalone: true,
  imports: [DatePipe, MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardContent, MatCardActions, MatChip, MatButton, MatIcon],
  template: `
    <mat-card class="todo-card" [class.overdue]="isOverdue()" role="article">
      <mat-card-header>
        <mat-card-title>{{ todo().title }}</mat-card-title>
        <mat-card-subtitle>
          @if (todo().dueDate) {
            <span [class.overdue-text]="isOverdue()">
              Due: {{ todo().dueDate | date:'mediumDate' }}
            </span>
          }
        </mat-card-subtitle>
      </mat-card-header>
      <mat-card-content>
        @if (todo().description) {
          <p class="description">{{ todo().description }}</p>
        }
        <div class="chips">
          <mat-chip [style.--mdc-chip-elevated-container-color]="todo().priority.color" highlighted>
            {{ todo().priority.name }}
          </mat-chip>
          @if (todo().category) {
            <mat-chip [style.--mdc-chip-elevated-container-color]="todo().category!.color">
              {{ todo().category!.name }}
            </mat-chip>
          }
          <mat-chip>{{ todo().status.name }}</mat-chip>
        </div>
      </mat-card-content>
      <mat-card-actions align="end">
        <button mat-icon-button (click)="edit.emit(todo())" attr.aria-label="Edit {{ todo().title }}">
          <mat-icon>edit</mat-icon>
        </button>
        <button mat-icon-button (click)="delete.emit(todo().id)" attr.aria-label="Delete {{ todo().title }}">
          <mat-icon>delete</mat-icon>
        </button>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [`
    .todo-card { margin-bottom: 1rem; }
    .todo-card.overdue { border-left: 4px solid #F44336; }
    .description { color: var(--mat-sys-on-surface-variant); margin: 0.5rem 0; }
    .chips { display: flex; gap: 0.5rem; flex-wrap: wrap; margin-top: 0.5rem; }
    .overdue-text { color: #F44336; font-weight: 500; }
  `]
})
export class TodoCardComponent {
  readonly todo = input.required<TodoItem>();
  readonly edit = output<TodoItem>();
  readonly delete = output<string>();

  isOverdue(): boolean {
    const dueDate = this.todo().dueDate;
    if (!dueDate) return false;
    return new Date(dueDate) < new Date() && this.todo().status.id !== 3;
  }
}
