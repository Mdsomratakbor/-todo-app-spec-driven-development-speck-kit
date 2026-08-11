export interface TodoItem {
  id: string;
  title: string;
  description?: string;
  dueDate?: string;
  priority: { id: number; name: string; color: string };
  category?: { id: string; name: string; color?: string; todoCount: number };
  status: { id: number; name: string; color: string };
  createdAt: string;
  updatedAt?: string;
}

export interface CreateTodoRequest {
  title: string;
  description?: string;
  dueDate?: string;
  priorityId: number;
  categoryId?: string;
}

export interface UpdateTodoRequest {
  title: string;
  description?: string;
  dueDate?: string;
  priorityId: number;
  categoryId?: string;
  statusId: number;
}
