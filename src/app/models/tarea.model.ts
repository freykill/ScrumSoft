import { Prioridad } from '../enums';

export interface TareaDto {
    id: string;
    titulo: string;
    descripcion?: string | null;
    prioridad: Prioridad;
    idResponsable?: string | null;
    idColumna: string;
    orden: number;
    /** date-time ISO */
    fechaCreacion: string;
}

/** POST /api/v1/proyectos/{idProyecto}/tareas */
export interface CrearTareaComando {
    idProyecto: string;
    idColumna: string;
    titulo: string;
    descripcion?: string | null;
    prioridad: Prioridad;
    idResponsable?: string | null;
}

/** PUT /api/v1/proyectos/{idProyecto}/tareas/{idTarea} */
export interface ActualizarTareaComando {
    idProyecto: string;
    idTarea: string;
    titulo: string;
    descripcion?: string | null;
    prioridad: Prioridad;
    idResponsable?: string | null;
}

/** PUT /api/v1/proyectos/{idProyecto}/tareas/{idTarea}/mover */
export interface MoverTareaComando {
    idProyecto: string;
    idTarea: string;
    idColumnaDestino: string;
    /** Para posicionar entre dos tareas al soltar en el tablero */
    idTareaAnterior?: string | null;
    idTareaSiguiente?: string | null;
}

/**
 * Lo que emite el formulario de tarea. El contenedor le pone los ids segun
 * si es alta o edicion: al crear hace falta la columna, al editar no, porque
 * ActualizarTareaComando no puede mover una tarea de sitio.
 */
export interface GuardarTareaComando {
    titulo: string;
    descripcion?: string | null;
    prioridad: Prioridad;
    /**
     * null desasigna. Viaja SIEMPRE, tambien al editar: el PUT reemplaza la
     * tarea entera, asi que omitir el campo deja la tarea sin responsable.
     */
    idResponsable: string | null;
}

/**
 * Lo que reporta una columna del tablero cuando sueltan una tarjeta encima.
 * El indice lo traduce la columna, que es quien habla con el CDK; el
 * contenedor solo entiende de tareas y columnas.
 */
export interface SoltarTarea {
    tarea: TareaDto;
    idColumnaOrigen: string;
    idColumnaDestino: string;
    indiceOrigen: number;
    indiceDestino: number;
}
