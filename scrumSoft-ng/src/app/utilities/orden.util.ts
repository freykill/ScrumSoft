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

/** Entre que dos tareas queda la que se acaba de soltar. */
export interface VecinosDeTarea {
    idTareaAnterior: string | null;
    idTareaSiguiente: string | null;
}

/**
 * Calcula los vecinos con los que el backend posiciona una tarea al soltarla.
 *
 * MoverTareaComando no lleva un indice sino los ids de la tarea de arriba y la
 * de abajo, asi que el frontend tiene que deducirlos de la columna destino.
 *
 * @param idsDestino  Tareas de la columna destino ANTES de soltar, en orden.
 * @param idTarea     La que se esta moviendo.
 * @param indice      Posicion final, tal como la reporta el CDK.
 *
 * El `filter` es la clave y es donde se rompe un tablero mal hecho: si la
 * tarea ya estaba en esa columna hay que sacarla antes de mirar quien le
 * queda al lado. Si no, se compara consigo misma y los vecinos salen corridos
 * una posicion, con lo que arrastrar una tarjeta dentro de su propia columna
 * la deja donde estaba o la manda un puesto mas arriba de lo que pediste.
 *
 * Sacarla primero tambien hace que el caso de mover entre columnas distintas
 * y el de mover dentro de la misma sean el mismo calculo, sin ramas.
 */
export function calcularVecinos(idsDestino: string[], idTarea: string, indice: number): VecinosDeTarea {
    const sinLaTarea = idsDestino.filter(id => id !== idTarea);
    const posicion = Math.max(0, Math.min(indice, sinLaTarea.length));

    return {
        // Al principio no hay anterior y al final no hay siguiente: van null,
        // que es como el backend entiende "pegala al borde de la columna".
        idTareaAnterior: sinLaTarea[posicion - 1] ?? null,
        idTareaSiguiente: sinLaTarea[posicion] ?? null
    };
}
