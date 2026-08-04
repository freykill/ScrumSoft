import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Prioridad } from 'src/app/enums';
import { TareaDto } from 'src/app/models';

/** Fila de la tabla: la tarea mas los nombres que en la API vienen por id. */
interface TareaFila extends TareaDto {
    nombreColumna: string;
    nombreResponsable: string;
}

@Component({
    selector: 'app-tareas',
    templateUrl: './tareas.component.html'
})
export class TareasComponent {

    readonly ruta: MenuItem[] = [{ label: 'Gestion' }, { label: 'Tareas' }];

    /** MOCK. Reemplazar por TareaService -> /api/v1/proyectos/{id}/tareas */
    tareas: TareaFila[] = [
        {
            id: '1', titulo: 'Definir esquema de base de datos',
            descripcion: 'Tablas de proyectos, columnas y tareas',
            prioridad: Prioridad.Critica, idResponsable: '1', idColumna: 'c3', orden: 1,
            fechaCreacion: '2026-07-20T10:00:00Z',
            nombreColumna: 'En progreso', nombreResponsable: 'Ivan Diaz'
        },
        {
            id: '2', titulo: 'Endpoint de autenticacion',
            descripcion: 'JWT con expiracion y rol',
            prioridad: Prioridad.Alta, idResponsable: '2', idColumna: 'c4', orden: 1,
            fechaCreacion: '2026-07-21T14:30:00Z',
            nombreColumna: 'Hecho', nombreResponsable: 'Laura Mendez'
        },
        {
            id: '3', titulo: 'Pantalla de tablero kanban',
            descripcion: 'Drag and drop entre columnas',
            prioridad: Prioridad.Alta, idResponsable: '3', idColumna: 'c2', orden: 1,
            fechaCreacion: '2026-07-25T09:15:00Z',
            nombreColumna: 'Por hacer', nombreResponsable: 'Carlos Rueda'
        },
        {
            id: '4', titulo: 'Exportar reporte a PDF',
            descripcion: null,
            prioridad: Prioridad.Media, idResponsable: null, idColumna: 'c1', orden: 2,
            fechaCreacion: '2026-07-28T16:00:00Z',
            nombreColumna: 'Backlog', nombreResponsable: 'Sin asignar'
        },
        {
            id: '5', titulo: 'Revisar textos de la interfaz',
            descripcion: 'Acentos y mayusculas',
            prioridad: Prioridad.Baja, idResponsable: null, idColumna: 'c1', orden: 3,
            fechaCreacion: '2026-08-01T08:45:00Z',
            nombreColumna: 'Backlog', nombreResponsable: 'Sin asignar'
        }
    ];

    severidadPrioridad(prioridad: Prioridad): string {
        const severidades: Record<Prioridad, string> = {
            [Prioridad.Baja]: 'info',
            [Prioridad.Media]: 'success',
            [Prioridad.Alta]: 'warning',
            [Prioridad.Critica]: 'danger'
        };
        return severidades[prioridad];
    }
}
