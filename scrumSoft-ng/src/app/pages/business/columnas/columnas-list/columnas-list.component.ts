import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { ColumnaDto } from 'src/app/models';

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

    @Input() columnas: ColumnaDto[] = [];
    @Input() cargando = false;

    @Output() subir = new EventEmitter<number>();
    @Output() bajar = new EventEmitter<number>();
    @Output() renombrar = new EventEmitter<ColumnaDto>();
    @Output() eliminar = new EventEmitter<ColumnaDto>();
}
