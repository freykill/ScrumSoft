import { inject, Injectable } from '@angular/core';
import { GenericService, METHODS, UrlServices } from '../common/services';
import { AgregarColumnaComando, ColumnaDto, RenombrarColumnaComando, ReordenarColumnasComando } from '../models';

/**
 * Columnas de un proyecto, bajo /api/v1/proyectos/{idProyecto}/columnas.
 *
 * OJO: aqui no hay `listar`. La API no expone un GET de columnas, solo
 * escritura; para leerlas se usa ProyectoService.obtenerTablero(), que es el
 * unico endpoint que las devuelve.
 *
 * Respuestas que documenta el backend:
 *   POST        200 ColumnaDto      400 payload invalido   403 sin permiso
 *   PUT /{id}   200 ColumnaDto      404 no existe
 *   DELETE      204 sin cuerpo      400 tiene tareas       404 no existe
 *   PUT /orden  200 ColumnaDto[]    400 la lista de ids no cuadra
 *
 * El idProyecto viaja en la ruta y ademas dentro del comando, asi lo declara
 * el contrato; por eso los metodos reciben el comando entero.
 */
@Injectable({ providedIn: 'root' })
export class ColumnaService {

    private readonly genericService = inject(GenericService);
    private readonly urlService = inject(UrlServices);

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
