import { RolUsuario } from '../enums';

/**
 * Usuario del sistema.
 * OJO: la API que nos pasaron todavia no expone endpoints de usuarios,
 * este modelo sale de la tabla `usuarios` del modelo de datos.
 */
export interface UsuarioDto {
    id: string;
    nombre: string;
    correoElectronico: string;
    rol: RolUsuario;
    /** 'A' activo, 'E' eliminado (borrado logico) */
    estadoRegistro: string;
    fechaCreacion: string;
    fechaActualizacion?: string | null;
}
