import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';

const errorMessages: Record<number, string> = {
  400: 'Please check your input and try again.',
  403: "You don't have permission to perform this action.",
  404: 'The requested resource was not found.',
  409: 'A conflict occurred. Please try again.',
  429: 'Too many requests. Please try again later.',
  500: 'An unexpected error occurred. Please try again later.',
};

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notification = inject(NotificationService);

  return next(req).pipe(
    catchError((error) => {
      if (error.status === 401) {
        return throwError(() => error);
      }

      const message =
        error.error?.detail ||
        errorMessages[error.status] ||
        (error.status === 0
          ? 'A network error occurred. Please check your connection.'
          : 'An unexpected error occurred. Please try again later.');

      notification.error(message);
      return throwError(() => error);
    }),
  );
};
