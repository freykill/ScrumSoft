/**
 * Reordenamiento de listas.
 *
 * Funciones puras a proposito: no tocan el array que reciben y no saben nada
 * de Angular ni de la API. Asi se prueban con dos lineas y las usan tanto la
 * pantalla de columnas como, mas adelante, el arrastre de tareas del tablero.
 *
 * Devolver una lista nueva no es un capricho: las tablas van en OnPush y solo
 * repintan si les llega otra referencia.
 */

/** Mueve el elemento de la posicion `desde` a la posicion `hasta`. */
export function moverElemento<T>(lista: T[], desde: number, hasta: number): T[] {
    const copia = [...lista];

    // Fuera de rango o sin movimiento real: se devuelve la copia tal cual.
    if (desde === hasta || desde < 0 || hasta < 0 || desde >= copia.length || hasta >= copia.length) {
        return copia;
    }

    const [elemento] = copia.splice(desde, 1);
    copia.splice(hasta, 0, elemento);
    return copia;
}

/**
 * Renumera el campo `orden` a 1..n segun la posicion en la lista.
 * El orden que manda es la posicion en el array, no el numero que traia.
 */
export function renumerarOrden<T extends { orden: number }>(lista: T[]): T[] {
    return lista.map((elemento, indice) => ({ ...elemento, orden: indice + 1 }));
}
