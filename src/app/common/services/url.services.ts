import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

/**
 * Catalogo central de urls del backend (ScrumSoft API).
 * Aqui se declaran todos los endpoints, nada de urls sueltas en los services.
 *
 * Las rutas con parametros de ruta se exponen como funcion, no como string,
 * porque el id va en medio del path y no al final (ej: /proyectos/{id}/tareas).
 */
@Injectable({ providedIn: 'root' })
export class UrlServices {

    /** Base del api, cambia segun el ambiente compilado (local / dev / stage / prod). */
    urlApiBackend = environment.urlBackend;

    /** Contexto versionado, todos los endpoints cuelgan de aqui. */
    apiV1 = this.urlApiBackend + '/api/v1';

    // --- Auth ---
    urlLogin = this.apiV1 + '/auth/login';

    // --- Tiempo real (SignalR) ---
    // Cuelga de la raiz, no de /api/v1: se registra con MapHub en Program.cs
    // y por eso tampoco aparece en Swagger.
    urlHubTablero = this.urlApiBackend + '/hubs/tablero';

    // --- Proyectos ---
    urlProyectos = this.apiV1 + '/proyectos';
    urlProyecto = (idProyecto: string): string => `${this.urlProyectos}/${idProyecto}`;
    urlProyectoTablero = (idProyecto: string): string => `${this.urlProyecto(idProyecto)}/tablero`;

    // --- Reportes (devuelve archivo: usar METHODS.FILE) ---
    urlProyectoReporte = (idProyecto: string): string => `${this.urlProyecto(idProyecto)}/reporte`;

    // --- Columnas ---
    urlColumnas = (idProyecto: string): string => `${this.urlProyecto(idProyecto)}/columnas`;
    urlColumna = (idProyecto: string, idColumna: string): string => `${this.urlColumnas(idProyecto)}/${idColumna}`;
    urlColumnasOrden = (idProyecto: string): string => `${this.urlColumnas(idProyecto)}/orden`;

    // --- Miembros del proyecto ---
    urlMiembros = (idProyecto: string): string => `${this.urlProyecto(idProyecto)}/miembros`;
    urlMiembro = (idProyecto: string, idUsuario: string): string => `${this.urlMiembros(idProyecto)}/${idUsuario}`;

    // --- Usuarios (listar, crear y editar; no hay baja) ---
    urlUsuarios = this.apiV1 + '/usuarios';
    urlUsuario = (idUsuario: string): string => `${this.urlUsuarios}/${idUsuario}`;

    // --- Tareas ---
    urlTareas = (idProyecto: string): string => `${this.urlProyecto(idProyecto)}/tareas`;
    urlTarea = (idProyecto: string, idTarea: string): string => `${this.urlTareas(idProyecto)}/${idTarea}`;
    urlTareaMover = (idProyecto: string, idTarea: string): string => `${this.urlTarea(idProyecto, idTarea)}/mover`;
}
