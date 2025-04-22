import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  console.log('Auth Interceptor executing for:', req.url);
  const authToken = localStorage.getItem('authToken');
  let clonedRequest = req;
  if (authToken) {
    clonedRequest = req.clone({
      setHeaders: { Authorization: `Bearer ${authToken}` }
    });
    console.log('Authorization header added.');
  } else {
    console.log('No auth token found, sending request without Authorization header.');
  }
  const startTime = Date.now();
  return next(clonedRequest).pipe(
    finalize(() => {
      const endTime = Date.now();
      const duration = endTime - startTime;
      console.log(`HTTP Request to ${req.url} took ${duration}ms`);
    })
  );
};