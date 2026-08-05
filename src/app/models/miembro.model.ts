import { RolUsuario } from '../enums';

/**
 * GET /api/v1/proyectos/{idProyecto}/miembros
 *
 * OJO con `rol`: es el rol global del usuario en el sistema, no un rol dentro
 * del proyecto. La API no tiene el concepto de "rol en este proyecto", asi que
 * en la pantalla es informativo y no se puede cambiar desde ahi.
 *
 * `idUsuario` y no `id`: la clave del miembro es el usuario, y es lo que pide
 * el DELETE (/miembros/{idUsuario}).
 */
export interface MiembroDto {
    idUsuario: string;
    nombre: string;
    correoElectronico: string;
    rol: RolUsuario;
    /** date-time ISO */
    fechaAsignacion: string;
}

/** POST /api/v1/proyectos/{idProyecto}/miembros */
export interface AgregarMiembroComando {
    idProyecto: string;
    idUsuario: string;
}
