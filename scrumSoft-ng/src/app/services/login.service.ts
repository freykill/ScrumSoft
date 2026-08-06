import { inject, Injectable } from '@angular/core';
import { AuthService, GenericService, METHODS, UrlServices } from '../common/services';
import { CredencialesComando, SesionDto } from '../models';

/**
 * Autenticacion contra /api/v1/auth/login.
 *
 * Respuestas del backend:
 *   200 SesionDto        { token, expiraEn, idUsuario, nombre, rol }
 *   400 ProblemDetails   payload invalido
 *   403 ProblemDetails   credenciales incorrectas
 *
 * El GenericService normaliza 400/403 a HttpServiceError, con el `detail`
 * del ProblemDetails como mensaje.
 */
@Injectable({ providedIn: 'root' })
export class LoginService {

    private readonly genericService = inject(GenericService);
    private readonly urlService = inject(UrlServices);
    private readonly authService = inject(AuthService);

    /**
     * Hace el POST y, si sale bien, deja la sesion guardada.
     * Asi ningun caller se puede olvidar de persistirla.
     *
     * @param recordar true = localStorage, false = sessionStorage.
     */
    async login(credenciales: CredencialesComando, recordar = true): Promise<SesionDto> {
        const sesion = await this.genericService.genericCallServices<SesionDto>(
            METHODS.POST, this.urlService.urlLogin, credenciales
        );

        // Solo se persiste si la respuesta trae token. Una sesion sin token
        // dejaria al usuario "logueado" pero sin poder llamar a ningun endpoint.
        if (sesion?.token) {
            this.authService.guardarSesion(sesion, recordar);
        }

        return sesion;
    }

    /** Cierra la sesion local. El backend no tiene endpoint de logout (jwt sin estado). */
    logout(): void {
        this.authService.cerrarSesion();
    }
}
