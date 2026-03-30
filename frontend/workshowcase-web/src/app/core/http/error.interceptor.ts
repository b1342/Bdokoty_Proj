import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { ApiError } from '../models/api-error.model';

/**
 * Normalizes all backend error responses into ApiError.
 * Handles 401 by clearing the session (backend: ErrorHandlingMiddleware).
 * Backend error shape: { message: string, errors?: Record<string, string[]> }
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401) {
        auth.clearSession();
      }

      const apiError: ApiError = {
        status: err.status,
        message: err.error?.message ?? 'An unexpected error occurred.',
        errors: err.error?.errors ?? null,
      };

      return throwError(() => apiError);
    }),
  );
};
