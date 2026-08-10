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

const NETWORK_ERROR_MESSAGE = 'A network error occurred. Please check your connection.';
const UNEXPECTED_ERROR_MESSAGE = 'An unexpected error occurred. Please try again later.';
const SESSION_EXPIRED_MESSAGE = 'Your session has expired. Please log in again.';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notification = inject(NotificationService);
  const isAuthUrl = req.url.includes('/auth/');

  return next(req).pipe(
    catchError((error) => {
      if (error.status === 401) {
        if (!isAuthUrl) {
          notification.error(SESSION_EXPIRED_MESSAGE);
        }
        return throwError(() => error);
      }

      if (!isAuthUrl) {
        const message =
          error.error?.detail ||
          errorMessages[error.status] ||
          (error.status === 0
            ? NETWORK_ERROR_MESSAGE
            : UNEXPECTED_ERROR_MESSAGE);

        notification.error(message);
      }

      return throwError(() => error);
    }),
  );
};
