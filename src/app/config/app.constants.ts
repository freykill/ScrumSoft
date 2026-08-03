/**
 * Constantes globales del proyecto: valores fijos que NO dependen del ambiente.
 * Lo que cambia entre local / dev / stage / prod va en environments, no aqui.
 */

/** Claves con las que se guarda info en localStorage / sessionStorage. */
export const STORAGE_KEYS = {
    TOKEN: 'access_token',
    REFRESH_TOKEN: 'refresh_token',
    USUARIO: 'usuario'
} as const;

/** Paginacion por defecto de las tablas. */
export const PAGINACION = {
    LIMIT: 10,
    OFFSET: 0
} as const;

/** Opciones del selector de filas por pagina. */
export const OPCIONES_PAGINA: number[] = [10, 25, 50, 100];

/** Formatos de fecha. FECHA y FECHA_HORA para mostrar, API para enviar al backend. */
export const FORMATOS = {
    FECHA: 'dd/MM/yyyy',
    FECHA_HORA: 'dd/MM/yyyy HH:mm',
    API: 'yyyy-MM-dd'
} as const;

/** Duracion por defecto de los toast (ms). */
export const TOAST_LIFE = 4000;

/** Milisegundos de espera antes de disparar la busqueda mientras se escribe. */
export const DEBOUNCE_BUSQUEDA = 400;
