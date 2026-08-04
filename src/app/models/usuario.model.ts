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

/** Lo que se manda al crear o editar. La clave viaja en claro, el hash lo hace el backend. */
export interface GuardarUsuarioComando {
    nombre: string;
    correoElectronico: string;
    rol: RolUsuario;
    /** En edicion es opcional: si no viene, la clave no se toca. */
    clave?: string;
}

/** Estado de los filtros de la lista. `null` = sin filtrar por ese campo. */
export interface UsuarioFiltros {
    busqueda: string;
    rol: RolUsuario | null;
    /** 'A' | 'E' */
    estado: string | null;
}
