import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ErrorStateComponent } from './error-state.component';

describe('ErrorStateComponent', () => {
  let component: ErrorStateComponent;
  let fixture: ComponentFixture<ErrorStateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ErrorStateComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ErrorStateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render default message', () => {
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('h3')?.textContent?.trim()).toBe('Something went wrong.');
  });

  it('should render custom message', () => {
    fixture.componentRef.setInput('message', 'Custom error');
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('h3')?.textContent?.trim()).toBe('Custom error');
  });

  it('should render default retry button label', () => {
    const el: HTMLElement = fixture.nativeElement;
    const button = el.querySelector('button');
    expect(button?.textContent?.trim()).toContain('Retry');
  });

  it('should render custom retry button label', () => {
    fixture.componentRef.setInput('retryLabel', 'Try Again');
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    const button = el.querySelector('button');
    expect(button?.textContent?.trim()).toContain('Try Again');
  });

  it('should emit retry when button is clicked', () => {
    const spy = vi.spyOn(component.retry, 'emit');
    const el: HTMLElement = fixture.nativeElement;
    const button = el.querySelector('button')!;
    button.click();
    expect(spy).toHaveBeenCalledOnce();
  });

  it('should disable button when retrying', () => {
    fixture.componentRef.setInput('retrying', true);
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    const button = el.querySelector('button')!;
    expect(button.disabled).toBe(true);
  });

  it('should enable button when not retrying', () => {
    fixture.componentRef.setInput('retrying', false);
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    const button = el.querySelector('button')!;
    expect(button.disabled).toBe(false);
  });

  it('should show spinner icon when retrying', () => {
    fixture.componentRef.setInput('retrying', true);
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    const spinner = el.querySelector('.btn-spinner');
    expect(spinner).toBeTruthy();
  });

  it('should have role="status" on root element', () => {
    const el: HTMLElement = fixture.nativeElement;
    const root = el.querySelector('.error-state');
    expect(root?.getAttribute('role')).toBe('status');
  });
});
