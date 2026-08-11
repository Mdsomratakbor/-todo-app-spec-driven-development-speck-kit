import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { TodoItem, CreateTodoRequest, UpdateTodoRequest } from '../models/todo.model';
import { PaginatedResponse } from '../models/paginated-response.model';
import { Status, STATUS_OPTIONS } from '../models/status.model';
import { Priority, PRIORITY_OPTIONS } from '../models/priority.model';
import { Category } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class TodoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/todos';

  getList(params: {
    page?: number;
    pageSize?: number;
    statusId?: number;
    priorityId?: number;
    categoryId?: string;
    dueDateFrom?: string;
    dueDateTo?: string;
    search?: string;
  }): Observable<PaginatedResponse<TodoItem>> {
    const query = new URLSearchParams();
    if (params.page) query.set('page', String(params.page));
    if (params.pageSize) query.set('pageSize', String(params.pageSize));
    if (params.statusId) query.set('statusId', String(params.statusId));
    if (params.priorityId) query.set('priorityId', String(params.priorityId));
    if (params.categoryId) query.set('categoryId', params.categoryId);
    if (params.dueDateFrom) query.set('dueDateFrom', params.dueDateFrom);
    if (params.dueDateTo) query.set('dueDateTo', params.dueDateTo);
    if (params.search) query.set('search', params.search);
    return this.http.get<PaginatedResponse<TodoItem>>(`${this.baseUrl}?${query.toString()}`);
  }

  getById(id: string): Observable<TodoItem> {
    return this.http.get<TodoItem>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateTodoRequest): Observable<TodoItem> {
    return this.http.post<TodoItem>(this.baseUrl, request);
  }

  update(id: string, request: UpdateTodoRequest): Observable<TodoItem> {
    return this.http.put<TodoItem>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getStatuses(): Observable<Status[]> {
    return of(STATUS_OPTIONS);
  }

  getPriorities(): Observable<Priority[]> {
    return of(PRIORITY_OPTIONS);
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>('/api/v1/categories');
  }
}
