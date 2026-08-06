/** Descarga de archivos en el navegador. */

/**
 * Dispara la descarga de un blob con el nombre indicado.
 *
 * No existe una API para "guardar este archivo": el camino de siempre es crear
 * una url temporal que apunte al blob y pinchar un enlace invisible. Hay que
 * revocarla despues o el blob se queda en memoria hasta recargar la pagina, y
 * un excel de varios megas se nota.
 */
export function descargarBlob(blob: Blob, nombre: string): void {
    const url = URL.createObjectURL(blob);
    const enlace = document.createElement('a');

    enlace.href = url;
    enlace.download = nombre;

    // Firefox no dispara el click si el enlace no esta en el documento.
    document.body.appendChild(enlace);
    enlace.click();
    document.body.removeChild(enlace);

    URL.revokeObjectURL(url);
}

/**
 * Deja un texto en condiciones de ser nombre de archivo.
 * Windows no admite \ / : * ? " < > | y el resto de sistemas tampoco la barra.
 */
export function aNombreDeArchivo(texto: string): string {
    return texto
        .replace(/[\\/:*?"<>|]/g, '-')
        .replace(/\s+/g, ' ')
        .trim() || 'archivo';
}
