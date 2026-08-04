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

/**
 * Lo que emite el formulario. El contenedor lo traduce a CrearProyectoComando
 * o a ActualizarProyectoComando segun si es alta o edicion, porque la API
 * pide campos distintos en cada caso: al crear se mandan las columnas
 * iniciales del tablero, al editar se manda el estado.
 */
export interface GuardarProyectoComando {
    nombre: string;
    descripcion?: string | null;
    fechaInicio: string;
    fechaFinPrevista?: string | null;
    /** Solo en alta. */
    columnas?: string[];
    /** Solo en edicion. */
    estadoProyecto?: EstadoProyecto;
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
