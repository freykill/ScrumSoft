import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { TableLazyLoadEvent } from 'primeng/table';
import { OPCIONES_PAGINA, PAGINACION } from 'src/app/config/app.constants';
import { EstadoProyecto, ETIQUETAS_ESTADO_PROYECTO } from 'src/app/enums';
import { PaginaSolicitada, ProyectoDto } from 'src/app/models';

/**
 * Presentacional. Solo pinta la tabla de proyectos.
 * Recibe la pagina por @Input y avisa por @Output, no inyecta nada.
 *
 * La tabla es lazy: `proyectos` trae solo la pagina visible y el paginador se
 * dimensiona con `totalElementos`, que viene del servidor.
 */
@Component({
    selector: 'app-proyectos-list',
    templateUrl: './proyectos-list.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProyectosListComponent {

    @Input() proyectos: ProyectoDto[] = [];
    @Input() cargando = false;
    @Input() totalElementos = 0;
    /** Indice de la primera fila de la pagina actual. */
    @Input() primeraFila = 0;
    @Input() filasPorPagina = PAGINACION.LIMIT;

    @Output() paginar = new EventEmitter<PaginaSolicitada>();
    @Output() verTablero = new EventEmitter<ProyectoDto>();
    @Output() verColumnas = new EventEmitter<ProyectoDto>();
    @Output() editar = new EventEmitter<ProyectoDto>();
    @Output() eliminar = new EventEmitter<ProyectoDto>();

    readonly opcionesPagina = OPCIONES_PAGINA;

    /**
     * En modo lazy el p-table informa `first`, el indice de la primera fila.
     * Traducirlo a numero de pagina es cosa de la tabla, no del contenedor,
     * que solo entiende de paginas porque asi las pide la API.
     */
    alPaginar(evento: TableLazyLoadEvent): void {
        const tamanoPagina = evento.rows || this.filasPorPagina;
        const pagina = Math.floor((evento.first ?? 0) / tamanoPagina) + 1;
        this.paginar.emit({ pagina, tamanoPagina });
    }

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
