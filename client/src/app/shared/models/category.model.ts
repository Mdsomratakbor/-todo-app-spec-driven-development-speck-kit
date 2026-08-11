export interface Category {
  id: string;
  name: string;
  color?: string;
  todoCount: number;
}

export interface CreateCategoryRequest {
  name: string;
  color?: string;
}

export interface UpdateCategoryRequest {
  name: string;
  color?: string;
}
