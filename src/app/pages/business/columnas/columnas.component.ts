import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TOAST_LIFE } from 'src/app/config/app.constants';
import { ColumnaFilaDto } from 'src/app/models';
import { moverElemento, renumerarOrden } from 'src/app/utilities';

/**
 * Contenedor de las columnas de UN proyecto.
 *
 * Cuelga de /business/proyectos/:idProyecto/columnas porque en la API una
 * columna no existe fuera de su proyecto: todos los endpoints son
 * /proyectos/{idProyecto}/columnas/... No hay listado global de columnas.
 */
@Component({
    selector: 'app-columnas',
    templateUrl: './columnas.component.html',
    providers: [ConfirmationService]
})
export class ColumnasComponent implements OnInit {

    idProyecto = '';
    /** MOCK: sale del GET del proyecto, aqui solo para la cabecera. */
    nombreProyecto = '';

    columnas: ColumnaFilaDto[] = [];
    cargando = false;
    guardando = false;

    mostrarFormulario = false;
    columnaEnEdicion: ColumnaFilaDto | null = null;

    constructor(
        private readonly rutaActiva: ActivatedRoute,
        private readonly confirmacion: ConfirmationService,
        private readonly mensajes: MessageService
    ) { }

    ngOnInit(): void {
        // snapshot y no observable: al cambiar de proyecto se entra por el
        // listado, nunca se reusa el componente con otro id.
        this.idProyecto = this.rutaActiva.snapshot.paramMap.get('idProyecto') ?? '';
        this.cargar();
    }

    /** MOCK. Reemplazar por ColumnaService -> GET /api/v1/proyectos/{id}/columnas */
    private cargar(): void {
        const nombres: Record<string, string> = {
            '1': 'Migracion ERP',
            '2': 'App movil de campo',
            '3': 'Portal de clientes'
        };
        this.nombreProyecto = nombres[this.idProyecto] ?? 'Proyecto';

        this.columnas = [
            { id: 'c1', nombre: 'Backlog', orden: 1, cantidadTareas: 2 },
            { id: 'c2', nombre: 'Por hacer', orden: 2, cantidadTareas: 1 },
            { id: 'c3', nombre: 'En progreso', orden: 3, cantidadTareas: 1 },
            { id: 'c4', nombre: 'Hecho', orden: 4, cantidadTareas: 0 }
        ];
    }

    // ------------------------------------------------------------- reordenar

    subir(indice: number): void {
        this.reordenar(indice, indice - 1);
    }

    bajar(indice: number): void {
        this.reordenar(indice, indice + 1);
    }

    /**
     * El orden real es la posicion en la lista; el campo `orden` se recalcula
     * despues de mover. El backend recibe solo los ids en su orden final
     * (ReordenarColumnasComando.idsEnOrden), no los numeros.
     */
    private reordenar(desde: number, hasta: number): void {
        this.columnas = renumerarOrden(moverElemento(this.columnas, desde, hasta));

        // TODO: PUT /proyectos/{id}/columnas/orden con { idsEnOrden }.
        // Al cablearlo, si el servidor falla hay que revertir a la lista previa.
        const idsEnOrden = this.columnas.map(columna => columna.id);
        console.debug('Nuevo orden de columnas', idsEnOrden);
    }

    // ------------------------------------------------------------ formulario

    nueva(): void {
        this.columnaEnEdicion = null;
        this.mostrarFormulario = true;
    }

    renombrar(columna: ColumnaFilaDto): void {
        this.columnaEnEdicion = columna;
        this.mostrarFormulario = true;
    }

    guardar(nombre: string): void {
        if (this.columnaEnEdicion) {
            const id = this.columnaEnEdicion.id;
            this.columnas = this.columnas.map(columna =>
                columna.id === id ? { ...columna, nombre } : columna
            );
            this.avisar('Columna renombrada', nombre);
        } else {
            // Una columna nueva siempre entra al final del tablero.
            this.columnas = [
                ...this.columnas,
                { id: crypto.randomUUID(), nombre, orden: this.columnas.length + 1, cantidadTareas: 0 }
            ];
            this.avisar('Columna creada', nombre);
        }

        this.mostrarFormulario = false;
    }

    // -------------------------------------------------------------- eliminar

    confirmarEliminacion(columna: ColumnaFilaDto): void {
        // La regla la aplica el backend; esto solo evita el viaje y el error feo.
        if (columna.cantidadTareas > 0) {
            this.mensajes.add({
                severity: 'warn',
                summary: 'No se puede eliminar',
                detail: `${columna.nombre} tiene ${columna.cantidadTareas} tarea(s). Muevelas antes.`,
                life: TOAST_LIFE
            });
            return;
        }

        this.confirmacion.confirm({
            header: 'Eliminar columna',
            message: `Se eliminara la columna <b>${columna.nombre}</b> del tablero.`,
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Eliminar',
            rejectLabel: 'Cancelar',
            acceptButtonStyleClass: 'p-button-danger',
            rejectButtonStyleClass: 'p-button-text',
            accept: () => this.eliminar(columna)
        });
    }

    private eliminar(columna: ColumnaFilaDto): void {
        // Al quitar una, las de abajo suben: hay que renumerar.
        this.columnas = renumerarOrden(this.columnas.filter(actual => actual.id !== columna.id));
        this.avisar('Columna eliminada', columna.nombre);
    }

    private avisar(titulo: string, detalle: string): void {
        this.mensajes.add({ severity: 'success', summary: titulo, detail: detalle, life: TOAST_LIFE });
    }
}
