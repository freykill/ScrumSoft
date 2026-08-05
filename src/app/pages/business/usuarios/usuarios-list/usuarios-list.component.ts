import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { TableLazyLoadEvent } from 'primeng/table';
import { OPCIONES_PAGINA, PAGINACION } from 'src/app/config/app.constants';
import { RolUsuario } from 'src/app/enums';
import { PaginaSolicitada, UsuarioDto } from 'src/app/models';

/**
 * Presentacional. Solo pinta la tabla.
 *
 * No inyecta servicios, no sabe de donde salen los usuarios ni que pasa
 * cuando se pulsa un boton: recibe la pagina por @Input y avisa por @Output.
 */
@Component({
    selector: 'app-usuarios-list',
    templateUrl: './usuarios-list.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class UsuariosListComponent {

    @Input() usuarios: UsuarioDto[] = [];
    @Input() cargando = false;
    @Input() totalElementos = 0;
    /** Indice de la primera fila de la pagina actual. */
    @Input() primeraFila = 0;
    @Input() filasPorPagina = PAGINACION.LIMIT;

    @Output() paginar = new EventEmitter<PaginaSolicitada>();
    @Output() editar = new EventEmitter<UsuarioDto>();

    readonly opcionesPagina = OPCIONES_PAGINA;

    /** El p-table informa `first`, el indice de fila; la API pide pagina. */
    alPaginar(evento: TableLazyLoadEvent): void {
        const tamanoPagina = evento.rows || this.filasPorPagina;
        const pagina = Math.floor((evento.first ?? 0) / tamanoPagina) + 1;
        this.paginar.emit({ pagina, tamanoPagina });
    }

    severidadRol(rol: RolUsuario): string {
        return rol === RolUsuario.Administrador ? 'success' : 'info';
    }
}
