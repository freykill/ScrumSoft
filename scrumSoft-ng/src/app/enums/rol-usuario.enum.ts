/** RolUsuario del backend. Llega como string, no como numero. */
export enum RolUsuario {
    Miembro = 'Miembro',
    Administrador = 'Administrador'
}

/**
 * El enum ya listo para un p-dropdown.
 * Vive aqui y no en cada componente para no repetir el mismo array en el
 * filtro de la lista y en el formulario.
 */
export const OPCIONES_ROL: { label: string; value: RolUsuario }[] = [
    { label: 'Administrador', value: RolUsuario.Administrador },
    { label: 'Miembro', value: RolUsuario.Miembro }
];
