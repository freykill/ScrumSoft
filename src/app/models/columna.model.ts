import { Prioridad } from '../enums';
import { TareaDto } from './tarea.model';

export interface ColumnaDto {
    id: string;
    nombre: string;
    orden: number;
}

/** Columna con sus tareas dentro. Solo llega en el tablero. */
export interface ColumnaConTareasDto extends ColumnaDto {
    tareas: TareaDto[];
}

/** GET /api/v1/proyectos/{idProyecto}/tablero */
export interface TableroDto {
    idProyecto: string;
    nombreProyecto: string;
    columnas: ColumnaConTareasDto[];
}

/**
 * Query del GET del tablero. Filtra las TAREAS, no las columnas: estas siguen
 * llegando aunque queden vacias, porque si desaparecieran no se podria
 * arrastrar nada hacia ellas.
 *
 * El reporte acepta exactamente los mismos, para que el archivo coincida con
 * lo que se esta viendo en pantalla.
 */
export interface TableroFiltros {
    idResponsable?: string | null;
    prioridad?: Prioridad | null;
    texto?: string | null;
}

/** POST /api/v1/proyectos/{idProyecto}/columnas */
export interface AgregarColumnaComando {
    idProyecto: string;
    nombre: string;
}

/** PUT /api/v1/proyectos/{idProyecto}/columnas/{idColumna} */
export interface RenombrarColumnaComando {
    idProyecto: string;
    idColumna: string;
    nombre: string;
}

/** PUT /api/v1/proyectos/{idProyecto}/columnas/orden */
export interface ReordenarColumnasComando {
    idProyecto: string;
    /** Ids de las columnas en el orden final */
    idsEnOrden: string[];
}
