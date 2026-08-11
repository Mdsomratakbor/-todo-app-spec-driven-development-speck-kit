import { Routes } from '@angular/router';
import { authGuard } from './shared/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/todos', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(c => c.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register.component').then(c => c.RegisterComponent),
  },
  {
    path: 'profile',
    loadComponent: () => import('./features/auth/profile/profile.component').then(c => c.ProfileComponent),
    canActivate: [authGuard],
  },
  {
    path: 'todos',
    loadComponent: () => import('./features/todos/todo-list/todo-list.component').then(c => c.TodoListComponent),
    canActivate: [authGuard],
  },
  {
    path: 'todos/:id',
    loadComponent: () => import('./features/todos/todo-detail/todo-detail.component').then(c => c.TodoDetailComponent),
    canActivate: [authGuard],
  },
  {
    path: 'categories',
    loadComponent: () => import('./features/categories/category-list/category-list.component').then(c => c.CategoryListComponent),
    canActivate: [authGuard],
  },
];
