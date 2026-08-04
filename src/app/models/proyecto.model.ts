import { EstadoProyecto } from '../enums';

/** GET /api/v1/proyectos */
export interface ProyectoDto {
    id: string;
    nombre: string;
    descripcion?: string | null;
    /** date, formato yyyy-MM-dd */
    fechaInicio: string;
    fechaFinPrevista?: string | null;
    estadoProyecto: EstadoProyecto;
    cantidadColumnas: number;
}

/** Envoltorio paginado que devuelve el GET de proyectos. */
export interface ProyectoDtoPagedResult {
    elementos: ProyectoDto[];
    pagina: number;
    tamanoPagina: number;
    totalElementos: number;
    /** readOnly en el backend */
    totalPaginas: number;
    /** readOnly en el backend */
    haySiguiente: boolean;
}

/** Query params del GET /api/v1/proyectos */
export interface ProyectoFiltros {
    nombre?: string;
    pagina?: number;
    tamanoPagina?: number;
}

/** POST /api/v1/proyectos */
export interface CrearProyectoComando {
    nombre: string;
    descripcion?: string | null;
    fechaInicio: string;
    fechaFinPrevista?: string | null;
    /** Nombres de las columnas iniciales del tablero */
    columnas?: string[] | null;
}

/** PUT /api/v1/proyectos/{idProyecto} */
export interface ActualizarProyectoComando {
    id: string;
    nombre: string;
    descripcion?: string | null;
    fechaInicio: string;
    fechaFinPrevista?: string | null;
    estadoProyecto: EstadoProyecto;
}
