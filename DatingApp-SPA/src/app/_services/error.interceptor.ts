import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpResponse,
  HttpErrorResponse,
  HTTP_INTERCEPTORS,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  // Idea here is to intercept the request and catch any errors.
  // Anything that's not a 200-300 range request. Angular will know
  // that errors within the 400-500 range are errors, in order
  // to cathch them.
  intercept(
    // The request itself
    req: HttpRequest<any>,
    // Catch what happens next
    next: HttpHandler
  ): import('rxjs').Observable<HttpEvent<any>> {
    // Use pipe method to catch the error (rjxs operators)
    return next.handle(req).pipe(
      // 'error' below is HTTP error response
      catchError((error) => {
        // If error status is 401, return the errortext.
        if (error.status === 401) {
          return throwError(error.statusText);
        }
        // Check to see if the error is an instance of
        // Http Response,
        if (error instanceof HttpErrorResponse) {
          // Get application error from headers
          const applicationError = error.headers.get('Application-Error');
          // If it exists, throw the returned the error.
          // This takes care of 500 type errors.
          if (applicationError) {
            return throwError(applicationError);
          }

          // This const represents the 'username already exists' type error.
          // error.error is what we get back from Angular
          // in our browser, unfortunately.
          const serverError = error.error;

          // Used to store model state errors such as 'password too short'
          // or 'validation requirements not met', etc.
          let modelStateErrors = '';

          // Check if error.errors exists, an object which contains other
          // objects of array type, Then check to make sure it is
          // type of object.
          if (serverError.errors && typeof serverError.errors === 'object') {
            // Loop through and check for existence of a key
            for (const key in serverError.errors) {
              // (Object bracket notation)
              if (serverError.errors[key]) {
                // Build list of strings, for each of the model state
                // errors that we see coming back form the server,
                // separated by a new line.
                modelStateErrors += serverError.errors[key] + '\n';
              }
            }
          }

          // If Server Error is returned, we have not captured an error
          // and will need to investigate the actual type of error.
          return throwError(modelStateErrors || serverError || 'Server Error');
        }
      })
    );
  }
}

// Registering a new interceptor providor to the Angular HTTP array of providors
// that already exists.
export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true,
};
