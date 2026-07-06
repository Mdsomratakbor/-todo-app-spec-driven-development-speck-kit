import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/todos', pathMatch: 'full' },
  {
    path: 'todos',
    loadComponent: () => import('./features/todos/todo-list/todo-list.component').then(c => c.TodoListComponent),
  },
  {
    path: 'todos/:id',
    loadComponent: () => import('./features/todos/todo-detail/todo-detail.component').then(c => c.TodoDetailComponent),
  },
  {
    path: 'categories',
    loadComponent: () => import('./features/categories/category-list/category-list.component').then(c => c.CategoryListComponent),
  },
];
