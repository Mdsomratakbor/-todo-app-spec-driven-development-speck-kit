import { TestBed } from '@angular/core/testing';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpClient } from '@angular/common/http';
import { NotificationService } from '../services/notification.service';
import { errorInterceptor } from './error.interceptor';

describe('errorInterceptor', () => {
  let http: HttpClient;
  let httpTesting: HttpTestingController;
  let errorSpy: ReturnType<typeof vi.fn>;

  beforeEach(() => {
    errorSpy = vi.fn();
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
        { provide: NotificationService, useValue: { error: errorSpy } },
      ],
    });

    http = TestBed.inject(HttpClient);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('maps a 500 to the unexpected error message', () => {
    http.get('/api/v1/todos').subscribe({ error: () => undefined });
    httpTesting.expectOne('/api/v1/todos').flush(null, { status: 500, statusText: 'Server Error' });

    expect(errorSpy).toHaveBeenCalledWith(
      'An unexpected error occurred. Please try again later.',
    );
  });

  it('maps a 404 to the not-found message', () => {
    http.get('/api/v1/todos/1').subscribe({ error: () => undefined });
    httpTesting.expectOne('/api/v1/todos/1').flush(null, { status: 404, statusText: 'Not Found' });

    expect(errorSpy).toHaveBeenCalledWith(
      'The requested resource was not found.',
    );
  });

  it('uses server error.error?.detail when present', () => {
    http.get('/api/v1/todos').subscribe({ error: () => undefined });
    httpTesting.expectOne('/api/v1/todos').flush(
      { detail: 'Server says no' },
      { status: 400, statusText: 'Bad Request' },
    );

    expect(errorSpy).toHaveBeenCalledWith('Server says no');
  });

  it('maps status 0 (network) to the network error message', () => {
    http.get('/api/v1/todos').subscribe({ error: () => undefined });
    httpTesting.expectOne('/api/v1/todos').flush(null, { status: 0, statusText: 'Unknown Error' });

    expect(errorSpy).toHaveBeenCalledWith(
      'A network error occurred. Please check your connection.',
    );
  });

  it('shows session-expired toast on 401 for non-auth URLs', () => {
    http.get('/api/v1/todos').subscribe({ error: () => undefined });
    httpTesting.expectOne('/api/v1/todos').flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(errorSpy).toHaveBeenCalledWith(
      'Your session has expired. Please log in again.',
    );
  });

  it('suppresses toast on /auth/ URLs (500)', () => {
    http.post('/api/v1/auth/login', {}).subscribe({ error: () => undefined });
    httpTesting.expectOne('/api/v1/auth/login').flush(null, { status: 500, statusText: 'Server Error' });

    expect(errorSpy).not.toHaveBeenCalled();
  });

  it('suppresses toast on /auth/ URLs (401 bad credentials)', () => {
    http.post('/api/v1/auth/login', {}).subscribe({ error: () => undefined });
    httpTesting.expectOne('/api/v1/auth/login').flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(errorSpy).not.toHaveBeenCalled();
  });
});
