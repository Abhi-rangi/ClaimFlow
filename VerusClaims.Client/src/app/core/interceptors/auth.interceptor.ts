import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * Note: Auth0 automatically handles token injection via httpInterceptor configuration
 * in app.module.ts. This interceptor is kept for any custom logic if needed.
 * Auth0's built-in interceptor will add tokens to requests matching the allowedList.
 */
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Auth0 handles token injection automatically via httpInterceptor in AuthModule.forRoot()
    // This interceptor can be used for additional custom logic if needed
    return next.handle(req);
  }
}

