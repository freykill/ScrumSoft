import { inject, Injectable } from '@angular/core';
import { GenericService, METHODS, UrlServices } from '../common/services';
import { AgregarMiembroComando, MiembroDto } from '../models';

/**
 * Miembros de un proyecto, bajo /api/v1/proyectos/{idProyecto}/miembros.
 *
 * Es una coleccion de usuarios ya existentes, no un alta: agregar recibe el
 * id de un usuario que se busca con UsuarioService. Y la baja va por
 * idUsuario, no por un id propio de la relacion.
 *
 * Respuestas que documenta el backend:
 *   GET     200 MiembroDto[]   403 sin permiso   404 no existe el proyecto
 *   POST    200 MiembroDto     400 ya es miembro / usuario invalido   403   404
 *   DELETE  204 sin cuerpo     400   403   404
 */
@Injectable({ providedIn: 'root' })
export class MiembroService {

    private readonly genericService = inject(GenericService);
    private readonly urlService = inject(UrlServices);

    listar(idProyecto: string): Promise<MiembroDto[]> {
        return this.genericService.genericCallServices<MiembroDto[]>(
            METHODS.GET, this.urlService.urlMiembros(idProyecto)
        );
    }

    agregar(comando: AgregarMiembroComando): Promise<MiembroDto> {
        return this.genericService.genericCallServices<MiembroDto>(
            METHODS.POST, this.urlService.urlMiembros(comando.idProyecto), comando
        );
    }

    /** Quita al usuario del proyecto; el usuario en si no se toca. */
    quitar(idProyecto: string, idUsuario: string): Promise<void> {
        return this.genericService.genericCallServices<void>(
            METHODS.DELETE, this.urlService.urlMiembro(idProyecto, idUsuario)
        );
    }
}
