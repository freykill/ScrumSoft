import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Lo contrario del authGuard: protege las pantallas publicas de quien ya tiene
 * sesion. Con el token vigente, volver al login no tiene sentido.
 *
 * Va como guard y no como una redireccion en el ngOnInit del componente para
 * que el formulario ni se llegue a instanciar: con ngOnInit se pinta el login
 * y desaparece de golpe, que se ve como un parpadeo raro.
 */
export const invitadoGuard: CanActivateFn = () => {
    const router = inject(Router);
    const auth = inject(AuthService);

    // La raiz redirige al layout, que ya vuelve a pasar por el authGuard.
    return auth.estaAutenticado() ? router.createUrlTree(['/']) : true;
};
