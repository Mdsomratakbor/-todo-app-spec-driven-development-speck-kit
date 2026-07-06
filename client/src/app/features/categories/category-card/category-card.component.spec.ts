import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimations } from '@angular/platform-browser/animations';
import { CategoryCardComponent } from './category-card.component';

describe('CategoryCardComponent', () => {
  let component: CategoryCardComponent;
  let fixture: ComponentFixture<CategoryCardComponent>;

  const testCategory = { id: '1', name: 'Work', color: '#3498DB', todoCount: 5 };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoryCardComponent, NoopAnimations],
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryCardComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('category', testCategory);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display category name', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Work');
  });

  it('should emit edit on edit click', () => {
    const editSpy = spyOn(component.edit, 'emit');
    component.edit.emit(testCategory);
    expect(editSpy).toHaveBeenCalledWith(testCategory);
  });

  it('should emit delete on delete click', () => {
    const deleteSpy = spyOn(component.delete, 'emit');
    component.delete.emit('1');
    expect(deleteSpy).toHaveBeenCalledWith('1');
  });
});
