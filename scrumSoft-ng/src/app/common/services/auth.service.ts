import { Injectable } from '@angular/core';
import { STORAGE_KEYS } from '../../config';
import { RolUsuario } from '../../enums/rol-usuario.enum';
import { SesionDto } from '../../models/sesion.model';

/**
 * Manejo de la sesion en el navegador. Es el UNICO sitio que toca el storage:
 * el interceptor, el guard y los componentes leen de aqui.
 *
 * No hace peticiones http. El POST /auth/login vive en el service de negocio,
 * que al recibir el SesionDto llama a guardarSesion().
 */
@Injectable({ providedIn: 'root' })
export class AuthService {

    /**
     * Guarda la sesion.
     * @param recordar true = localStorage (sobrevive cerrar el navegador),
     *                 false = sessionStorage (muere al cerrar la pestana).
     */
    guardarSesion(sesion: SesionDto, recordar = true): void {
        this.cerrarSesion();

        const store = recordar ? localStorage : sessionStorage;
        store.setItem(STORAGE_KEYS.TOKEN, sesion.token);
        store.setItem(STORAGE_KEYS.USUARIO, JSON.stringify(sesion));
    }

    /** Borra la sesion de los dos storages, sin importar donde estuviera. */
    cerrarSesion(): void {
        [localStorage, sessionStorage].forEach(store => {
            store.removeItem(STORAGE_KEYS.TOKEN);
            store.removeItem(STORAGE_KEYS.USUARIO);
        });
    }

    get token(): string | null {
        return this.leer(STORAGE_KEYS.TOKEN);
    }

    get sesion(): SesionDto | null {
        const crudo = this.leer(STORAGE_KEYS.USUARIO);
        if (!crudo) {
            return null;
        }

        try {
            return JSON.parse(crudo) as SesionDto;
        } catch {
            // Storage corrupto o de una version anterior: se descarta
            this.cerrarSesion();
            return null;
        }
    }

    get idUsuario(): string | null {
        return this.sesion?.idUsuario ?? null;
    }

    get nombre(): string | null {
        return this.sesion?.nombre ?? null;
    }

    get rol(): RolUsuario | null {
        return this.sesion?.rol ?? null;
    }

    /** Hay token y todavia no expira. Es lo que mira el authGuard. */
    estaAutenticado(): boolean {
        return !!this.token && !this.sesionExpirada();
    }

    /** Compara expiraEn contra la hora actual. Sin sesion se considera expirada. */
    sesionExpirada(): boolean {
        const expiraEn = this.sesion?.expiraEn;
        if (!expiraEn) {
            return true;
        }

        const fecha = new Date(expiraEn).getTime();
        return isNaN(fecha) || fecha <= Date.now();
    }

    /** Minutos que le quedan a la sesion. Util para avisar antes de que caduque. */
    minutosRestantes(): number {
        const expiraEn = this.sesion?.expiraEn;
        if (!expiraEn) {
            return 0;
        }

        const restante = new Date(expiraEn).getTime() - Date.now();
        return restante > 0 ? Math.floor(restante / 60000) : 0;
    }

    tieneRol(rol: RolUsuario): boolean {
        return this.rol === rol;
    }

    esAdministrador(): boolean {
        return this.tieneRol(RolUsuario.Administrador);
    }

    /** Busca primero en localStorage y luego en sessionStorage. */
    private leer(key: string): string | null {
        return localStorage.getItem(key) ?? sessionStorage.getItem(key);
    }
}
