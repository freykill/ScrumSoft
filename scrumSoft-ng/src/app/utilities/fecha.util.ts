/**
 * Conversion de fechas entre el p-calendar y la API.
 *
 * La API manda y espera 'yyyy-MM-dd': solo fecha, sin hora ni zona horaria.
 * El problema es que `new Date('2026-01-10')` NO se interpreta como fecha
 * local sino como medianoche UTC, asi que en cualquier huso al oeste de
 * Greenwich el calendario termina mostrando el 9. Por eso aqui se parte el
 * texto a mano en vez de dejarselo al constructor de Date.
 *
 * Lo mismo al reves: `toISOString()` pasa por UTC y puede correr el dia, asi
 * que la salida se arma con getFullYear/getMonth/getDate, que son locales.
 */

/** 'yyyy-MM-dd' -> Date en hora local. */
export function aFechaLocal(texto: string | null | undefined): Date | null {
    if (!texto) {
        return null;
    }
    const [anio, mes, dia] = texto.substring(0, 10).split('-').map(Number);
    return new Date(anio, mes - 1, dia);
}

/** Date -> 'yyyy-MM-dd'. */
export function aFechaApi(fecha: Date | null | undefined): string | null {
    if (!fecha) {
        return null;
    }
    const mes = `${fecha.getMonth() + 1}`.padStart(2, '0');
    const dia = `${fecha.getDate()}`.padStart(2, '0');
    return `${fecha.getFullYear()}-${mes}-${dia}`;
}
