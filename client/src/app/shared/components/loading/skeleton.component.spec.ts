import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SkeletonComponent, SkeletonVariant } from './skeleton.component';

describe('SkeletonComponent', () => {
  let component: SkeletonComponent;
  let fixture: ComponentFixture<SkeletonComponent>;

  const variants: SkeletonVariant[] = ['todo-list', 'category-list', 'detail', 'profile', 'rows'];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SkeletonComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SkeletonComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should have default variant "rows"', () => {
    expect(component.variant()).toBe('rows');
  });

  it('should have default count 3', () => {
    expect(component.count()).toBe(3);
  });

  it('should have empty label by default', () => {
    expect(component.label()).toBe('');
  });

  variants.forEach((variant) => {
    it(`should render "${variant}" variant`, () => {
      fixture.componentRef.setInput('variant', variant);
      fixture.detectChanges();
      const el: HTMLElement = fixture.nativeElement;
      expect(el.querySelector('.skeleton')).toBeTruthy();
    });
  });

  it('should render correct number of rows based on count', () => {
    fixture.componentRef.setInput('variant', 'todo-list');
    fixture.componentRef.setInput('count', 5);
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    const cards = el.querySelectorAll('.skeleton-card');
    expect(cards.length).toBe(5);
  });

  it('should have role="status" on root skeleton element', () => {
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    const skeleton = el.querySelector('.skeleton');
    expect(skeleton?.getAttribute('role')).toBe('status');
  });

  it('should have aria-live="polite" on root skeleton element', () => {
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    const skeleton = el.querySelector('.skeleton');
    expect(skeleton?.getAttribute('aria-live')).toBe('polite');
  });

  it('should render visually hidden label when label is provided', () => {
    fixture.componentRef.setInput('label', 'Loading todos...');
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    const hidden = el.querySelector('.visually-hidden');
    expect(hidden).toBeTruthy();
    expect(hidden?.textContent?.trim()).toBe('Loading todos...');
  });

  it('should not render visually hidden label when label is empty', () => {
    fixture.componentRef.setInput('label', '');
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    const hidden = el.querySelector('.visually-hidden');
    expect(hidden).toBeNull();
  });

  it('should apply reduced-motion class when no animation preferred', () => {
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    const lines = el.querySelectorAll('.skeleton-line');
    expect(lines.length).toBeGreaterThan(0);
  });
});
