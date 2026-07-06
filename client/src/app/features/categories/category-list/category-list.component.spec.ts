import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { CategoryListComponent } from './category-list.component';

describe('CategoryListComponent', () => {
  let component: CategoryListComponent;
  let fixture: ComponentFixture<CategoryListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoryListComponent, NoopAnimations],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show create form', () => {
    component.showCreateForm();
    expect(component.creating).toBeTrue();
    expect(component.editingCategory).toBeNull();
  });

  it('should start editing a category', () => {
    const category = { id: '1', name: 'Work', color: '#3498DB', todoCount: 3 };
    component.startEdit(category);
    expect(component.editingCategory).toEqual(category);
    expect(component.creating).toBeFalse();
  });

  it('should cancel edit', () => {
    component.editingCategory = { id: '1', name: 'Work', color: '#3498DB', todoCount: 3 };
    component.cancelEdit();
    expect(component.editingCategory).toBeNull();
  });
});
