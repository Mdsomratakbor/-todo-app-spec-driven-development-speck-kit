import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient } from '@angular/common/http';
import { TodoFormComponent } from './todo-form.component';

describe('TodoFormComponent', () => {
  let component: TodoFormComponent;
  let fixture: ComponentFixture<TodoFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoFormComponent, NoopAnimations],
      providers: [provideHttpClient()],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit save with form values on submit', () => {
    const saveSpy = spyOn(component.save, 'emit');
    component.form.patchValue({ title: 'Buy groceries', priorityId: 2, description: 'Milk, eggs' });
    component.onSubmit();
    expect(saveSpy).toHaveBeenCalledWith({
      title: 'Buy groceries',
      description: 'Milk, eggs',
      priorityId: 2,
      categoryId: undefined,
      dueDate: undefined,
    });
  });

  it('should not emit save when form is invalid', () => {
    const saveSpy = spyOn(component.save, 'emit');
    component.form.patchValue({ title: '' });
    component.onSubmit();
    expect(saveSpy).not.toHaveBeenCalled();
  });

  it('should emit cancel', () => {
    const cancelSpy = spyOn(component.cancel, 'emit');
    component.cancel.emit();
    expect(cancelSpy).toHaveBeenCalled();
  });

  it('should patch form values when todo input is provided', () => {
    const todo = {
      id: '1',
      title: 'Existing Todo',
      description: 'Description',
      priority: { id: 3, name: 'High', color: '#F39C12' },
      status: { id: 1, name: 'Pending', color: '#95A5A6' },
      category: { id: 'cat-1', name: 'Work', color: '#3498DB', todoCount: 5 },
      createdAt: '2026-01-01T00:00:00Z',
      updatedAt: null,
    };
    fixture.componentRef.setInput('todo', todo);
    fixture.detectChanges();
    expect(component.form.value.title).toBe('Existing Todo');
    expect(component.form.value.description).toBe('Description');
    expect(component.form.value.priorityId).toBe(3);
  });
});
