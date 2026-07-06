import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TodoListComponent } from './todo-list.component';
import { of } from 'rxjs';

describe('TodoListComponent', () => {
  let component: TodoListComponent;
  let fixture: ComponentFixture<TodoListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoListComponent, NoopAnimations],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show create form', () => {
    component.showCreateForm();
    expect(component.creating).toBeTrue();
    expect(component.editingTodo).toBeNull();
  });

  it('should start editing a todo', () => {
    const todo = {
      id: '1',
      title: 'Test Todo',
      description: 'Description',
      priority: { id: 2, name: 'Medium', color: '#3498DB' },
      status: { id: 1, name: 'Pending', color: '#95A5A6' },
      category: null,
      createdAt: '2026-01-01T00:00:00Z',
      updatedAt: null,
    };
    component.startEdit(todo);
    expect(component.editingTodo).toEqual(todo);
    expect(component.creating).toBeFalse();
  });

  it('should cancel edit', () => {
    component.editingTodo = { id: '1', title: 'Test', priority: { id: 2, name: 'Medium', color: '#3498DB' }, status: { id: 1, name: 'Pending', color: '#95A5A6' }, createdAt: '2026-01-01T00:00:00Z', updatedAt: null, description: null, category: null };
    component.cancelEdit();
    expect(component.editingTodo).toBeNull();
  });
});
