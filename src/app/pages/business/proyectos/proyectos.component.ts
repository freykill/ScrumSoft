import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { EstadoProyecto } from 'src/app/enums';
import { ProyectoDto } from 'src/app/models';

@Component({
    selector: 'app-proyectos',
    templateUrl: './proyectos.component.html'
})
export class ProyectosComponent {

    readonly ruta: MenuItem[] = [{ label: 'Gestion' }, { label: 'Proyectos' }];

    /** MOCK. Reemplazar por ProyectoService -> GET /api/v1/proyectos */
    proyectos: ProyectoDto[] = [
        {
            id: '1', nombre: 'Migracion ERP', descripcion: 'Traslado del ERP a la nube',
            fechaInicio: '2026-01-10', fechaFinPrevista: '2026-06-30',
            estadoProyecto: EstadoProyecto.EnProgreso, cantidadColumnas: 4
        },
        {
            id: '2', nombre: 'App movil de campo', descripcion: 'Levantamiento de datos offline',
            fechaInicio: '2026-03-01', fechaFinPrevista: '2026-09-15',
            estadoProyecto: EstadoProyecto.Planificacion, cantidadColumnas: 3
        },
        {
            id: '3', nombre: 'Portal de clientes', descripcion: null,
            fechaInicio: '2025-09-05', fechaFinPrevista: '2026-01-20',
            estadoProyecto: EstadoProyecto.Completado, cantidadColumnas: 5
        }
    ];

    /** Etiqueta legible: el enum viene sin espacios desde el backend. */
    etiquetaEstado(estado: EstadoProyecto): string {
        const etiquetas: Record<EstadoProyecto, string> = {
            [EstadoProyecto.Planificacion]: 'Planificacion',
            [EstadoProyecto.EnProgreso]: 'En progreso',
            [EstadoProyecto.Completado]: 'Completado'
        };
        return etiquetas[estado];
    }

    severidadEstado(estado: EstadoProyecto): string {
        const severidades: Record<EstadoProyecto, string> = {
            [EstadoProyecto.Planificacion]: 'info',
            [EstadoProyecto.EnProgreso]: 'warning',
            [EstadoProyecto.Completado]: 'success'
        };
        return severidades[estado];
    }
}
