import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

/**
 * Catalogo central de urls del backend.
 * Aqui se declaran todos los endpoints, nada de urls sueltas en los services.
 */
@Injectable({ providedIn: 'root' })
export class UrlServices {

    /** Base del api, cambia segun el ambiente compilado (local / dev / stage / prod). */
    urlApiBackend = environment.urlBackend;

    // --- Contextos ---
    // securityContext = this.urlApiBackend + '/secv1';

    // --- Endpoints ---
    // urlUsuario = this.securityContext + '/usuario';
    // urlUsuarioDelete = this.urlUsuario + '/delete';
}
