/** EstadoProyecto del backend. Llega como string. */
export enum EstadoProyecto {
    Planificacion = 'Planificacion',
    EnProgreso = 'EnProgreso',
    Completado = 'Completado'
}

/** El valor del backend viene pegado ('EnProgreso'), aqui esta como se muestra. */
export const ETIQUETAS_ESTADO_PROYECTO: Record<EstadoProyecto, string> = {
    [EstadoProyecto.Planificacion]: 'Planificacion',
    [EstadoProyecto.EnProgreso]: 'En progreso',
    [EstadoProyecto.Completado]: 'Completado'
};

/** El enum listo para un p-dropdown. */
export const OPCIONES_ESTADO_PROYECTO: { label: string; value: EstadoProyecto }[] = [
    { label: ETIQUETAS_ESTADO_PROYECTO[EstadoProyecto.Planificacion], value: EstadoProyecto.Planificacion },
    { label: ETIQUETAS_ESTADO_PROYECTO[EstadoProyecto.EnProgreso], value: EstadoProyecto.EnProgreso },
    { label: ETIQUETAS_ESTADO_PROYECTO[EstadoProyecto.Completado], value: EstadoProyecto.Completado }
];
