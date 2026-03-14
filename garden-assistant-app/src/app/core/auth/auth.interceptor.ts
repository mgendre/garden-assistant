import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { from, switchMap, catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const authService = inject(AuthService);
  const isAuthUrl = req.url.includes('/api/auth/');

  const token = authService.getAccessToken();
  const authorizedReq = token && !isAuthUrl
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authorizedReq).pipe(
    catchError((error) => {
      if (error instanceof HttpErrorResponse && error.status === 401 && !isAuthUrl) {
        return from(authService.refresh()).pipe(
          switchMap(() => {
            const newToken = authService.getAccessToken();
            if (!newToken) {
              return throwError(() => error);
            }
            const retryReq = req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } });
            return next(retryReq);
          }),
          catchError(() => throwError(() => error))
        );
      }
      return throwError(() => error);
    })
  );
};
