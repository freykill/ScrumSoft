/** Prioridad de una tarea. Llega como string. */
export enum Prioridad {
    Baja = 'Baja',
    Media = 'Media',
    Alta = 'Alta',
    Critica = 'Critica'
}

/**
 * Orden del enum en el backend. Es lo que manda SignalR en vez del nombre.
 * El indice importa: 0 Baja, 1 Media, 2 Alta, 3 Critica.
 */
const PRIORIDAD_POR_INDICE: Prioridad[] = [
    Prioridad.Baja,
    Prioridad.Media,
    Prioridad.Alta,
    Prioridad.Critica
];

/**
 * Normaliza la prioridad venga como venga.
 *
 * Por REST llega "Alta", porque el backend registra JsonStringEnumConverter en
 * AddJsonOptions. Pero SignalR serializa con su propio configurador, al que no
 * se le puso ese convertidor, asi que por el hub la MISMA propiedad llega
 * como 2. Sin esto, la tarjeta busca la severidad del p-tag con un numero,
 * no la encuentra y sale sin color.
 *
 * Se aceptan las dos formas a proposito: el dia que se arregle en el servidor
 * esto sigue funcionando y no hay que coordinar el despliegue de los dos.
 */
export function aPrioridad(valor: unknown): Prioridad {
    if (typeof valor === 'number') {
        return PRIORIDAD_POR_INDICE[valor] ?? Prioridad.Media;
    }

    return PRIORIDAD_POR_INDICE.includes(valor as Prioridad)
        ? valor as Prioridad
        : Prioridad.Media;
}

/** El enum listo para un p-dropdown. */
export const OPCIONES_PRIORIDAD: { label: string; value: Prioridad }[] = [
    { label: 'Baja', value: Prioridad.Baja },
    { label: 'Media', value: Prioridad.Media },
    { label: 'Alta', value: Prioridad.Alta },
    { label: 'Critica', value: Prioridad.Critica }
];
