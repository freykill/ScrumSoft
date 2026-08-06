import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/** Protege las rutas privadas: exige token vigente y no expirado. */
export const authGuard: CanActivateFn = (route, state) => {
    const router = inject(Router);
    const auth = inject(AuthService);

    if (auth.estaAutenticado()) {
        return true;
    }

    // Sesion caducada o inexistente: se limpia lo que haya quedado sucio
    auth.cerrarSesion();

    // Se guarda a donde queria entrar para devolverlo ahi despues del login
    return router.createUrlTree(['/auth/login'], {
        queryParams: { returnUrl: state.url }
    });
};
