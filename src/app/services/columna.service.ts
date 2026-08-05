import { inject, Injectable } from '@angular/core';
import { GenericService, METHODS, UrlServices } from '../common/services';
import { AgregarColumnaComando, ColumnaDto, RenombrarColumnaComando, ReordenarColumnasComando } from '../models';

/**
 * Columnas de un proyecto, bajo /api/v1/proyectos/{idProyecto}/columnas.
 *
 * Respuestas que documenta el backend:
 *   GET         200 ColumnaDto[]    403 sin permiso        404 no existe
 *   POST        200 ColumnaDto      400 payload invalido   403 sin permiso
 *   PUT /{id}   200 ColumnaDto      404 no existe
 *   DELETE      204 sin cuerpo      400 tiene tareas       404 no existe
 *   PUT /orden  200 ColumnaDto[]    400 la lista de ids no cuadra
 *
 * El idProyecto viaja en la ruta y ademas dentro del comando, asi lo declara
 * el contrato; por eso los metodos de escritura reciben el comando entero.
 */
@Injectable({ providedIn: 'root' })
export class ColumnaService {

    private readonly genericService = inject(GenericService);
    private readonly urlService = inject(UrlServices);

    /**
     * Columnas del proyecto. Devuelve ColumnaDto, sin conteo de tareas: para
     * saber si una columna se puede borrar hay que intentarlo y leer el 400.
     */
    listar(idProyecto: string): Promise<ColumnaDto[]> {
        return this.genericService.genericCallServices<ColumnaDto[]>(
            METHODS.GET, this.urlService.urlColumnas(idProyecto)
        );
    }

    /** La columna nueva entra al final del tablero, el orden lo asigna el backend. */
    agregar(comando: AgregarColumnaComando): Promise<ColumnaDto> {
        return this.genericService.genericCallServices<ColumnaDto>(
            METHODS.POST, this.urlService.urlColumnas(comando.idProyecto), comando
        );
    }

    renombrar(comando: RenombrarColumnaComando): Promise<ColumnaDto> {
        return this.genericService.genericCallServices<ColumnaDto>(
            METHODS.PUT, this.urlService.urlColumna(comando.idProyecto, comando.idColumna), comando
        );
    }

    /** Responde 400 si la columna todavia tiene tareas dentro. */
    eliminar(idProyecto: string, idColumna: string): Promise<void> {
        return this.genericService.genericCallServices<void>(
            METHODS.DELETE, this.urlService.urlColumna(idProyecto, idColumna)
        );
    }

    /**
     * Guarda el orden mandando los ids en su posicion final, no los numeros.
     * Devuelve las columnas ya renumeradas por el servidor, que es el orden
     * bueno: el que se pinte tiene que salir de aqui, no del calculo local.
     */
    reordenar(comando: ReordenarColumnasComando): Promise<ColumnaDto[]> {
        return this.genericService.genericCallServices<ColumnaDto[]>(
            METHODS.PUT, this.urlService.urlColumnasOrden(comando.idProyecto), comando
        );
    }
}
