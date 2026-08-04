import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { ColumnaFilaDto } from 'src/app/models';

/**
 * Presentacional. La tabla de columnas con las flechas de reordenar.
 * Emite el indice porque lo que importa al reordenar es la posicion, no el id.
 */
@Component({
    selector: 'app-columnas-list',
    templateUrl: './columnas-list.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ColumnasListComponent {

    @Input() columnas: ColumnaFilaDto[] = [];
    @Input() cargando = false;

    @Output() subir = new EventEmitter<number>();
    @Output() bajar = new EventEmitter<number>();
    @Output() renombrar = new EventEmitter<ColumnaFilaDto>();
    @Output() eliminar = new EventEmitter<ColumnaFilaDto>();

    /** El backend no deja borrar una columna con tareas, aqui se adelanta el aviso. */
    tieneTareas(columna: ColumnaFilaDto): boolean {
        return columna.cantidadTareas > 0;
    }
}
