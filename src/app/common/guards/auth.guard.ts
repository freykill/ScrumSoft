import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { STORAGE_KEYS } from '../../config';

/**
 * Protege las rutas privadas. Cascara: por ahora solo mira si hay token en
 * localStorage. Cuando exista AuthService la validacion se mueve alla
 * (expiracion del jwt, refresh, permisos).
 */
export const authGuard: CanActivateFn = (route, state) => {
    const router = inject(Router);
    const token = localStorage.getItem(STORAGE_KEYS.TOKEN);

    if (token) {
        return true;
    }

    // Se guarda a donde queria entrar para devolverlo ahi despues del login
    return router.createUrlTree(['/auth/login'], {
        queryParams: { returnUrl: state.url }
    });
};
