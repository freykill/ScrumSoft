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
