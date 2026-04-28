import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import type { AppError } from '../errors/app-error';

export const errorInterceptor: HttpInterceptorFn = (request, next) =>
  next(request).pipe(
    catchError((error: unknown) => throwError(() => toAppError(error))),
  );

function toAppError(error: unknown): AppError {
  if (!(error instanceof HttpErrorResponse)) {
    return {
      type: 'unknown',
      message: 'An unexpected error occurred.',
      details: error,
    };
  }

  if (error.status === 0) {
    return {
      type: 'network',
      message: 'Unable to connect to the API.',
    };
  }

  switch (error.status) {
    case 400:
      return {
        type: 'badRequest',
        message: 'The request is invalid.',
        details: error.error,
      };
    case 401:
      return {
        type: 'unauthorized',
        message: 'Authentication is required.',
      };
    case 403:
      return {
        type: 'forbidden',
        message: 'You do not have access to this resource.',
      };
    case 404:
      return {
        type: 'notFound',
        message: 'The requested resource was not found.',
      };
    default:
      return {
        type: error.status >= 500 ? 'server' : 'unknown',
        message: 'An unexpected API error occurred.',
        status: error.status,
        details: error.error,
      };
  }
}
