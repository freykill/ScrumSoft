import { RolUsuario } from '../enums';
import { ResultadoPaginado } from './paginacion.model';

/** GET /api/v1/usuarios */
export interface UsuarioDto {
    id: string;
    nombre: string;
    correoElectronico: string;
    rol: RolUsuario;
}

export type UsuarioDtoPagedResult = ResultadoPaginado<UsuarioDto>;

/**
 * Evento `UsuariosConectados` del hub: quien esta viendo un tablero ahora.
 *
 * Llega solo cuando la lista cambia, y una misma persona con varias pestañas
 * abiertas aparece una sola vez.
 */
export interface UsuarioConectado {
    idUsuario: string;
    nombre: string;
}

/** Query params del GET /api/v1/usuarios */
export interface UsuarioFiltros {
    /** Un solo campo de texto; el backend decide si busca por nombre o correo. */
    filtro?: string;
    pagina?: number;
    tamanoPagina?: number;
}

/** POST /api/v1/usuarios */
export interface CrearUsuarioComando {
    nombre: string;
    correoElectronico: string;
    /** Viaja en claro, el hash lo hace el backend. */
    contrasena: string;
    rol: RolUsuario;
}

/**
 * PUT /api/v1/usuarios/{id}
 *
 * Solo nombre y rol. El correo no se puede cambiar porque es con lo que se
 * inicia sesion, y la contrasena no tiene endpoint: no hay cambio de clave.
 */
export interface ActualizarUsuarioComando {
    id: string;
    nombre: string;
    rol: RolUsuario;
}

/**
 * Lo que emite el formulario. El contenedor lo traduce a uno u otro comando
 * segun el modo, porque la API pide campos distintos: al crear van correo y
 * contrasena, al editar no existen.
 */
export interface GuardarUsuarioComando {
    nombre: string;
    rol: RolUsuario;
    /** Solo en alta. */
    correoElectronico?: string;
    /** Solo en alta. */
    contrasena?: string;
}
