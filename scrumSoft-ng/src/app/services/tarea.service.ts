import { inject, Injectable } from '@angular/core';
import { GenericService, METHODS, UrlServices } from '../common/services';
import { ActualizarTareaComando, CrearTareaComando, MoverTareaComando, TareaDto } from '../models';

/**
 * Tareas de un proyecto, bajo /api/v1/proyectos/{idProyecto}/tareas.
 *
 * Aqui tampoco hay `listar`: las tareas solo se leen desde el tablero
 * (ProyectoService.obtenerTablero), que las devuelve ya repartidas por columna.
 *
 * Respuestas que documenta el backend:
 *   POST         200 TareaDto      400 payload invalido   403 sin permiso
 *   PUT /{id}    200 TareaDto      404 no existe
 *   DELETE       204 sin cuerpo    404 no existe
 *   PUT /mover   200 TareaDto      400 destino invalido   404 no existe
 *
 * Cambiar contenido y cambiar de sitio son dos operaciones distintas a
 * proposito: ActualizarTareaComando no lleva idColumna, mover tiene su
 * propio endpoint. Un solo "guardar" que hiciera las dos podria dejar la
 * tarea editada pero sin mover si la segunda llamada falla.
 */
@Injectable({ providedIn: 'root' })
export class TareaService {

    private readonly genericService = inject(GenericService);
    private readonly urlService = inject(UrlServices);

    /** La columna es obligatoria: una tarea no existe fuera de una columna. */
    crear(comando: CrearTareaComando): Promise<TareaDto> {
        return this.genericService.genericCallServices<TareaDto>(
            METHODS.POST, this.urlService.urlTareas(comando.idProyecto), comando
        );
    }

    /** Solo contenido: titulo, descripcion, prioridad y responsable. */
    actualizar(comando: ActualizarTareaComando): Promise<TareaDto> {
        return this.genericService.genericCallServices<TareaDto>(
            METHODS.PUT, this.urlService.urlTarea(comando.idProyecto, comando.idTarea), comando
        );
    }

    eliminar(idProyecto: string, idTarea: string): Promise<void> {
        return this.genericService.genericCallServices<void>(
            METHODS.DELETE, this.urlService.urlTarea(idProyecto, idTarea)
        );
    }

    /**
     * Posiciona la tarea entre otras dos, no en un indice: el backend guarda
     * el orden con huecos y con los vecinos sabe que numero ponerle sin tocar
     * el resto de la columna. Los vecinos salen de calcularVecinos().
     */
    mover(comando: MoverTareaComando): Promise<TareaDto> {
        return this.genericService.genericCallServices<TareaDto>(
            METHODS.PUT, this.urlService.urlTareaMover(comando.idProyecto, comando.idTarea), comando
        );
    }
}
