import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { Prioridad } from 'src/app/enums';
import { ColumnaConTareasDto, SoltarTarea, TareaDto } from 'src/app/models';
import { iniciales } from 'src/app/utilities';

/**
 * Presentacional. Una columna del tablero con sus tarjetas.
 *
 * Es el unico sitio que sabe del CDK: traduce el evento de arrastre a algo que
 * el contenedor entienda (que tarea, de que columna a cual y en que posicion)
 * para que arriba no haya que razonar con indices de listas de arrastre.
 */
@Component({
    selector: 'app-tablero-columna',
    templateUrl: './tablero-columna.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class TableroColumnaComponent {

    @Input({ required: true }) columna!: ColumnaConTareasDto;
    /** id -> nombre. Un Map y no la lista, para resolver en O(1) por tarjeta. */
    @Input() nombresDeMiembros = new Map<string, string>();

    @Output() nuevaTarea = new EventEmitter<ColumnaConTareasDto>();
    @Output() editarTarea = new EventEmitter<TareaDto>();
    @Output() soltarTarea = new EventEmitter<SoltarTarea>();

    alSoltar(evento: CdkDragDrop<ColumnaConTareasDto>): void {
        this.soltarTarea.emit({
            tarea: evento.item.data as TareaDto,
            idColumnaOrigen: evento.previousContainer.data.id,
            idColumnaDestino: evento.container.data.id,
            indiceOrigen: evento.previousIndex,
            indiceDestino: evento.currentIndex
        });
    }

    porId(_indice: number, tarea: TareaDto): string {
        return tarea.id;
    }

    /**
     * Puede no estar en el mapa: la tarea conserva a su responsable aunque a
     * esa persona la hayan sacado del equipo. Se avisa en vez de dejarlo vacio.
     */
    nombreResponsable(idResponsable: string): string {
        return this.nombresDeMiembros.get(idResponsable) ?? 'Ya no es miembro del proyecto';
    }

    /** Las tarjetas miden 18rem: cabe un circulito con iniciales, no un nombre. */
    inicialesResponsable(idResponsable: string): string {
        return iniciales(this.nombresDeMiembros.get(idResponsable));
    }

    severidadPrioridad(prioridad: Prioridad): string {
        const severidades: Record<Prioridad, string> = {
            [Prioridad.Baja]: 'success',
            [Prioridad.Media]: 'info',
            [Prioridad.Alta]: 'warning',
            [Prioridad.Critica]: 'danger'
        };
        return severidades[prioridad];
    }
}
