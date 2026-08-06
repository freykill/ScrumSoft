import { RolUsuario } from '../enums/rol-usuario.enum';

/** Body de POST /api/v1/auth/login */
export interface CredencialesComando {
    correoElectronico: string;
    contrasena: string;
}

/** Respuesta de POST /api/v1/auth/login */
export interface SesionDto {
    token: string;
    /** ISO date-time, ej: 2026-08-03T18:30:00Z */
    expiraEn: string;
    idUsuario: string;
    nombre: string;
    rol: RolUsuario;
}
