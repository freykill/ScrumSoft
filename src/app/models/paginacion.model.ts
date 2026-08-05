/**
 * Pagina que pide una tabla al contenedor.
 *
 * La tabla trabaja con `first` (indice de la primera fila) porque asi razona
 * el paginador, pero la API pide numero de pagina. La traduccion la hace el
 * componente de lista, que es quien conoce la tabla, y hacia arriba solo sube
 * esto, que ya tiene la forma del query.
 */
export interface PaginaSolicitada {
    /** Empieza en 1, igual que el query param `Pagina` del backend. */
    pagina: number;
    tamanoPagina: number;
}

/**
 * Envoltorio de los listados paginados del backend. Es el mismo para todos
 * (proyectos, usuarios), por eso va generico y no repetido en cada modelo.
 */
export interface ResultadoPaginado<T> {
    elementos: T[];
    pagina: number;
    tamanoPagina: number;
    totalElementos: number;
    /** readOnly en el backend */
    totalPaginas: number;
    /** readOnly en el backend */
    haySiguiente: boolean;
}
