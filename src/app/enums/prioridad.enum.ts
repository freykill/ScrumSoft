/** Prioridad de una tarea. Llega como string. */
export enum Prioridad {
    Baja = 'Baja',
    Media = 'Media',
    Alta = 'Alta',
    Critica = 'Critica'
}

/** El enum listo para un p-dropdown. */
export const OPCIONES_PRIORIDAD: { label: string; value: Prioridad }[] = [
    { label: 'Baja', value: Prioridad.Baja },
    { label: 'Media', value: Prioridad.Media },
    { label: 'Alta', value: Prioridad.Alta },
    { label: 'Critica', value: Prioridad.Critica }
];
