import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient } from '@angular/common/http';
import { FilterBarComponent } from './filter-bar.component';

describe('FilterBarComponent', () => {
  let component: FilterBarComponent;
  let fixture: ComponentFixture<FilterBarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FilterBarComponent, NoopAnimations],
      providers: [provideHttpClient()],
    }).compileComponents();

    fixture = TestBed.createComponent(FilterBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should emit filters on apply', () => {
    const spy = spyOn(component.filtersChanged, 'emit');
    component.search = 'test';
    component.statusId = 1;
    component.priorityId = 2;
    component.categoryId = 'cat-1';
    component.dueDateFrom = '2026-01-01';
    component.dueDateTo = '2026-12-31';
    component.apply();
    expect(spy).toHaveBeenCalledWith({
      search: 'test',
      statusId: 1,
      priorityId: 2,
      categoryId: 'cat-1',
      dueDateFrom: '2026-01-01',
      dueDateTo: '2026-12-31',
    });
  });

  it('should emit empty filters on clear', () => {
    const spy = spyOn(component.filtersChanged, 'emit');
    component.clear();
    expect(component.search).toBe('');
    expect(component.statusId).toBeUndefined();
    expect(component.priorityId).toBeUndefined();
    expect(component.categoryId).toBeUndefined();
    expect(component.dueDateFrom).toBe('');
    expect(component.dueDateTo).toBe('');
    expect(spy).toHaveBeenCalledWith({});
  });

  it('should render apply and clear buttons', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    const buttons = compiled.querySelectorAll('button');
    expect(buttons.length).toBe(2);
    expect(buttons[0].getAttribute('aria-label')).toBe('Apply filters');
    expect(buttons[1].getAttribute('aria-label')).toBe('Clear filters');
  });
});
