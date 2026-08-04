import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { OPCIONES_PAGINA, PAGINACION } from 'src/app/config/app.constants';
import { EstadoProyecto, ETIQUETAS_ESTADO_PROYECTO } from 'src/app/enums';
import { ProyectoDto } from 'src/app/models';

/**
 * Presentacional. Solo pinta la tabla de proyectos.
 * Recibe la lista por @Input y avisa por @Output, no inyecta nada.
 */
@Component({
    selector: 'app-proyectos-list',
    templateUrl: './proyectos-list.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProyectosListComponent {

    @Input() proyectos: ProyectoDto[] = [];
    @Input() cargando = false;

    @Output() verTablero = new EventEmitter<ProyectoDto>();
    @Output() verColumnas = new EventEmitter<ProyectoDto>();
    @Output() editar = new EventEmitter<ProyectoDto>();
    @Output() eliminar = new EventEmitter<ProyectoDto>();

    readonly filasPorPagina = PAGINACION.LIMIT;
    readonly opcionesPagina = OPCIONES_PAGINA;

    etiquetaEstado(estado: EstadoProyecto): string {
        return ETIQUETAS_ESTADO_PROYECTO[estado];
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
