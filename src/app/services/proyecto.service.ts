import { inject, Injectable } from '@angular/core';
import { GenericService, METHODS, UrlServices } from '../common/services';
import {
    ActualizarProyectoComando,
    CrearProyectoComando,
    ProyectoDto,
    ProyectoDtoPagedResult,
    ProyectoFiltros,
    TableroDto
} from '../models';

/**
 * Proyectos contra /api/v1/proyectos.
 *
 * Respuestas que documenta el backend:
 *   GET     200 ProyectoDtoPagedResult
 *   POST    201 ProyectoDto        400 payload invalido
 *   PUT     200 ProyectoDto        403 sin permiso   404 no existe
 *   DELETE  204 sin cuerpo         403 sin permiso   404 no existe
 *
 * GenericService normaliza cualquier fallo a HttpServiceError con el `detail`
 * del ProblemDetails como mensaje, asi que aqui no se atrapa nada: el
 * contenedor decide que se le muestra al usuario.
 */
@Injectable({ providedIn: 'root' })
export class ProyectoService {

    private readonly genericService = inject(GenericService);
    private readonly urlService = inject(UrlServices);

    /**
     * Listado paginado y filtrado en el servidor.
     *
     * Los query params van con la mayuscula que declara el contrato
     * (Nombre, Pagina, TamanoPagina). El binding de .NET no distingue
     * mayusculas, pero mandarlos igual que el swagger evita tener que
     * adivinar si el dia que falle el filtro es por el nombre del parametro.
     */
    listar(filtros: ProyectoFiltros): Promise<ProyectoDtoPagedResult> {
        const nombre = filtros.nombre?.trim();

        return this.genericService.genericCallServices<ProyectoDtoPagedResult>(
            METHODS.GET,
            this.urlService.urlProyectos,
            null,
            null,
            {
                // Buscar con el campo vacio tiene que traer todo, no filtrar
                // por cadena vacia: undefined no viaja en el query.
                Nombre: nombre || undefined,
                Pagina: filtros.pagina,
                TamanoPagina: filtros.tamanoPagina
            }
        );
    }

    /** El alta no recibe estado: el proyecto nace en Planificacion. */
    crear(comando: CrearProyectoComando): Promise<ProyectoDto> {
        return this.genericService.genericCallServices<ProyectoDto>(
            METHODS.POST, this.urlService.urlProyectos, comando
        );
    }

    /** El id viaja en la ruta y tambien en el cuerpo, asi lo pide el comando. */
    actualizar(comando: ActualizarProyectoComando): Promise<ProyectoDto> {
        return this.genericService.genericCallServices<ProyectoDto>(
            METHODS.PUT, this.urlService.urlProyecto(comando.id), comando
        );
    }

    /** Borrado logico en el backend: responde 204 y desaparece del listado. */
    eliminar(idProyecto: string): Promise<void> {
        return this.genericService.genericCallServices<void>(
            METHODS.DELETE, this.urlService.urlProyecto(idProyecto)
        );
    }

    /**
     * Tablero completo del proyecto: nombre, columnas y las tareas de cada una.
     *
     * Es el unico GET que devuelve columnas. La API no tiene
     * GET /proyectos/{id}/columnas (solo POST, PUT y DELETE), asi que la
     * pantalla de administracion de columnas tambien se sirve de aqui: coge
     * las columnas y cuenta las tareas para saber cuales se pueden borrar.
     */
    obtenerTablero(idProyecto: string): Promise<TableroDto> {
        return this.genericService.genericCallServices<TableroDto>(
            METHODS.GET, this.urlService.urlProyectoTablero(idProyecto)
        );
    }
}
