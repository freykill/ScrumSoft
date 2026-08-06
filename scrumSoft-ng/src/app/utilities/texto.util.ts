/** Ayudas de presentacion de texto. */

/**
 * Iniciales de un nombre, para los circulitos donde no cabe el nombre entero.
 * Coge como mucho dos: "Maria Lopez Garcia" -> "ML".
 */
export function iniciales(nombre: string | null | undefined): string {
    if (!nombre) { return '?'; }

    const partes = nombre.split(' ').filter(parte => parte);
    if (!partes.length) { return '?'; }

    return partes
        .slice(0, 2)
        .map(parte => parte[0].toUpperCase())
        .join('');
}
