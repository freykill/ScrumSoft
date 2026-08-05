import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GenericService, METHODS, UrlServices } from '../common/services';
import {
    ActualizarUsuarioComando,
    CrearUsuarioComando,
    UsuarioDto,
    UsuarioDtoPagedResult,
    UsuarioFiltros
} from '../models';

/**
 * Usuarios contra /api/v1/usuarios.
 *
 * OJO: no hay baja. Se puede listar, crear y editar, pero un usuario no se
 * borra ni se desactiva, asi que la pantalla no lleva papelera.
 *
 * Respuestas que documenta el backend:
 *   GET        200 UsuarioDtoPagedResult   401 sin sesion
 *   POST       201 UsuarioDto              400 payload invalido   403 sin permiso
 *   PUT /{id}  200 UsuarioDto              400   403   404 no existe
 */
@Injectable({ providedIn: 'root' })
export class UsuarioService {

    private readonly genericService = inject(GenericService);
    private readonly urlService = inject(UrlServices);

    listar(filtros: UsuarioFiltros): Promise<UsuarioDtoPagedResult> {
        return this.genericService.genericCallServices<UsuarioDtoPagedResult>(
            METHODS.GET, this.urlService.urlUsuarios, null, null, this.aQuery(filtros)
        );
    }

    /**
     * Igual pero cancelable, para el autocompletar de miembros: al escribir
     * rapido cada tecla lanza una busqueda y con switchMap la anterior se
     * corta, asi no llegan sugerencias de lo que ya se borro.
     */
    buscar$(filtros: UsuarioFiltros): Observable<UsuarioDtoPagedResult> {
        return this.genericService.genericCallServices$<UsuarioDtoPagedResult>(
            METHODS.GET, this.urlService.urlUsuarios, null, null, this.aQuery(filtros)
        );
    }

    crear(comando: CrearUsuarioComando): Promise<UsuarioDto> {
        return this.genericService.genericCallServices<UsuarioDto>(
            METHODS.POST, this.urlService.urlUsuarios, comando
        );
    }

    /** Solo cambia nombre y rol: el correo y la contrasena no son editables. */
    actualizar(comando: ActualizarUsuarioComando): Promise<UsuarioDto> {
        return this.genericService.genericCallServices<UsuarioDto>(
            METHODS.PUT, this.urlService.urlUsuario(comando.id), comando
        );
    }

    /** Los query params van con la mayuscula que declara el contrato. */
    private aQuery(filtros: UsuarioFiltros): Record<string, unknown> {
        const filtro = filtros.filtro?.trim();

        return {
            // Buscar con el campo vacio tiene que traer todo, no filtrar por ''
            Filtro: filtro || undefined,
            Pagina: filtros.pagina,
            TamanoPagina: filtros.tamanoPagina
        };
    }
}
