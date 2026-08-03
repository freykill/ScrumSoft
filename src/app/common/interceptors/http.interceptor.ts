import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { STORAGE_KEYS } from '../../config';

@Injectable()
export class HttpRequestInterceptor implements HttpInterceptor {

    constructor(private router: Router) { }

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        const token = localStorage.getItem(STORAGE_KEYS.TOKEN);

        // Sin token, o peticion a un json local de la plantilla: se deja pasar tal cual
        if (token && !request.url.startsWith('assets/')) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`
                }
            });
        }

        return next.handle(request).pipe(
            catchError((error: HttpErrorResponse) => {
                // Token invalido o expirado: se limpia la sesion y fuera
                if (error.status === 401) {
                    localStorage.removeItem(STORAGE_KEYS.TOKEN);
                    this.router.navigate(['/auth/login']);
                }

                // Autenticado pero sin permiso sobre el recurso
                if (error.status === 403) {
                    this.router.navigate(['/auth/access']);
                }

                return throwError(() => error);
            })
        );
    }
}
