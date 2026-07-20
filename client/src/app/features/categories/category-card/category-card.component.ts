import { Component, input, output } from '@angular/core';
import { MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardActions } from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { Category } from '../../../shared/models/category.model';

@Component({
  selector: 'app-category-card',
  standalone: true,
  imports: [MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardActions, MatIcon],
  template: `
    <mat-card class="category-card">
      <mat-card-header>
        <mat-card-title>
          <span class="color-dot" [style.background]="category().color || '#95A5A6'"></span>
          {{ category().name }}
        </mat-card-title>
        <mat-card-subtitle>{{ category().todoCount }} todos</mat-card-subtitle>
      </mat-card-header>
      <mat-card-actions align="end">
        <button mat-icon-button (click)="edit.emit(category())" attr.aria-label="Edit {{ category().name }}">
          <mat-icon>edit</mat-icon>
        </button>
        <button mat-icon-button (click)="delete.emit(category().id)" attr.aria-label="Delete {{ category().name }}">
          <mat-icon>delete</mat-icon>
        </button>
      </mat-card-actions>
    </mat-card>
  `,
  styles: [`
    .category-card { margin-bottom: 0.5rem; transition: transform 0.2s ease-out, box-shadow 0.2s ease-out; }
    @media (prefers-reduced-motion: no-preference) {
      .category-card:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0,0,0,0.15); }
    }
    .color-dot { display: inline-block; width: 12px; height: 12px; border-radius: 50%; margin-right: 0.5rem; vertical-align: middle; }
  `]
})
export class CategoryCardComponent {
  readonly category = input.required<Category>();
  readonly edit = output<Category>();
  readonly delete = output<string>();
}
