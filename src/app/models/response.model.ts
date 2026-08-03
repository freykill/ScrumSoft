/**
 * Envoltorio estandar de las respuestas del backend.
 * Ojo: `Response` tambien existe como tipo global del DOM (fetch API),
 * asi que hay que importarlo siempre de forma explicita.
 */
export interface Response<T> {
    code: number;
    description: string;
    data: T;
    status?: boolean;
}
