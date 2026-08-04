import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { ColumnaDto } from 'src/app/models';

/** Fila de la tabla: la columna mas cuantas tareas tiene dentro. */
interface ColumnaFila extends ColumnaDto {
    cantidadTareas: number;
}

@Component({
    selector: 'app-columnas',
    templateUrl: './columnas.component.html'
})
export class ColumnasComponent {

    readonly ruta: MenuItem[] = [{ label: 'Gestion' }, { label: 'Columnas' }];

    /** MOCK. Reemplazar por ColumnaService -> /api/v1/proyectos/{id}/columnas */
    columnas: ColumnaFila[] = [
        { id: 'c1', nombre: 'Backlog', orden: 1, cantidadTareas: 2 },
        { id: 'c2', nombre: 'Por hacer', orden: 2, cantidadTareas: 1 },
        { id: 'c3', nombre: 'En progreso', orden: 3, cantidadTareas: 1 },
        { id: 'c4', nombre: 'Hecho', orden: 4, cantidadTareas: 1 }
    ];

    /** El backend reordena con PUT /columnas/orden mandando los ids en orden. */
    subir(indice: number): void {
        if (indice === 0) {
            return;
        }
        this.intercambiar(indice, indice - 1);
    }

    bajar(indice: number): void {
        if (indice === this.columnas.length - 1) {
            return;
        }
        this.intercambiar(indice, indice + 1);
    }

    private intercambiar(a: number, b: number): void {
        const copia = [...this.columnas];
        [copia[a], copia[b]] = [copia[b], copia[a]];
        // El orden es la posicion, se recalcula tras mover
        this.columnas = copia.map((columna, i) => ({ ...columna, orden: i + 1 }));
    }
}
