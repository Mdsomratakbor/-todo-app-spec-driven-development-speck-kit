import { TestBed } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NotificationService } from './notification.service';

describe('NotificationService', () => {
  let service: NotificationService;
  let openSpy: ReturnType<typeof vi.fn>;

  beforeEach(() => {
    openSpy = vi.fn();
    TestBed.configureTestingModule({
      providers: [
        NotificationService,
        { provide: MatSnackBar, useValue: { open: openSpy } },
      ],
    });
    service = TestBed.inject(NotificationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('shows success toast with snackbar-success panel class and 4s duration', () => {
    service.success('Done');
    expect(openSpy).toHaveBeenCalledWith('Done', 'Close', {
      duration: 4000,
      panelClass: ['snackbar-success'],
    });
  });

  it('shows warning toast with snackbar-warning panel class and 6s duration', () => {
    service.warning('Careful');
    expect(openSpy).toHaveBeenCalledWith('Careful', 'Close', {
      duration: 6000,
      panelClass: ['snackbar-warning'],
    });
  });

  it('shows error toast with snackbar-error panel class and 8s duration', () => {
    service.error('Failed');
    expect(openSpy).toHaveBeenCalledWith('Failed', 'Close', {
      duration: 8000,
      panelClass: ['snackbar-error'],
    });
  });

  it('defaults to success severity when show() has no severity', () => {
    service.show('Hi');
    expect(openSpy).toHaveBeenCalledWith('Hi', 'Close', {
      duration: 4000,
      panelClass: ['snackbar-success'],
    });
  });
});
