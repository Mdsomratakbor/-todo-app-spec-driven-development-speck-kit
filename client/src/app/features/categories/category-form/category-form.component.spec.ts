import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimations } from '@angular/platform-browser/animations';
import { CategoryFormComponent } from './category-form.component';

describe('CategoryFormComponent', () => {
  let component: CategoryFormComponent;
  let fixture: ComponentFixture<CategoryFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoryFormComponent, NoopAnimations],
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit save with form values on submit', () => {
    const saveSpy = spyOn(component.save, 'emit');
    component.form.patchValue({ name: 'Work', color: '#3498DB' });
    component.onSubmit();
    expect(saveSpy).toHaveBeenCalledWith({ name: 'Work', color: '#3498DB' });
  });

  it('should not emit save when form is invalid', () => {
    const saveSpy = spyOn(component.save, 'emit');
    component.form.patchValue({ name: '' });
    component.onSubmit();
    expect(saveSpy).not.toHaveBeenCalled();
  });

  it('should emit cancel', () => {
    const cancelSpy = spyOn(component.cancel, 'emit');
    component.cancel.emit();
    expect(cancelSpy).toHaveBeenCalled();
  });

  it('should patch form values when category input is provided', () => {
    const category = { id: '1', name: 'Work', color: '#3498DB', todoCount: 3 };
    fixture.componentRef.setInput('category', category);
    fixture.detectChanges();
    expect(component.form.value.name).toBe('Work');
    expect(component.form.value.color).toBe('#3498DB');
  });
});
