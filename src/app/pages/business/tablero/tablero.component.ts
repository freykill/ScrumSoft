import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TOAST_LIFE } from 'src/app/config/app.constants';
import { ColumnaConTareasDto, GuardarTareaComando, SoltarTarea, TareaDto } from 'src/app/models';
import { ProyectoService, TareaService } from 'src/app/services';
import { calcularVecinos } from 'src/app/utilities';

/**
 * Contenedor del tablero de UN proyecto.
 *
 * Es la unica pantalla que lee tareas: la API no tiene un GET de tareas, solo
 * el tablero las devuelve, ya repartidas por columna. Por eso las tareas se
 * crean y se editan desde aqui y no desde un listado aparte.
 */
@Component({
    selector: 'app-tablero',
    templateUrl: './tablero.component.html',
    providers: [ConfirmationService]
})
export class TableroComponent implements OnInit {

    idProyecto = '';
    nombreProyecto = '';

    columnas: ColumnaConTareasDto[] = [];
    cargando = false;
    guardando = false;

    mostrarFormulario = false;
    tareaEnEdicion: TareaDto | null = null;
    /** En que columna se crea la tarea nueva. Solo se usa en el alta. */
    columnaDestino: ColumnaConTareasDto | null = null;

    constructor(
        private readonly rutaActiva: ActivatedRoute,
        private readonly router: Router,
        private readonly proyectoService: ProyectoService,
        private readonly tareaService: TareaService,
        private readonly confirmacion: ConfirmationService,
        private readonly mensajes: MessageService
    ) { }

    ngOnInit(): void {
        this.idProyecto = this.rutaActiva.snapshot.paramMap.get('idProyecto') ?? '';
        this.cargar();
    }

    // ------------------------------------------------------------------ carga

    private async cargar(): Promise<void> {
        this.cargando = true;

        try {
            const tablero = await this.proyectoService.obtenerTablero(this.idProyecto);

            this.nombreProyecto = tablero.nombreProyecto;
            this.columnas = (tablero.columnas ?? [])
                .map(columna => ({ ...columna, tareas: [...(columna.tareas ?? [])] }))
                .sort((una, otra) => una.orden - otra.orden);
        } catch (error) {
            this.columnas = [];
            this.avisarError('No se pudo cargar el tablero', error);
        } finally {
            this.cargando = false;
        }
    }

    /**
     * Sin trackBy, cada cambio del array rehace el DOM de todas las columnas y
     * el CDK pierde por medio la que se esta arrastrando.
     */
    porId(_indice: number, columna: ColumnaConTareasDto): string {
        return columna.id;
    }

    // ------------------------------------------------------------------ mover

    /**
     * Arrastrar una tarjeta.
     *
     * Se mueve en pantalla primero y se guarda despues, que es lo que hace que
     * el arrastre se sienta instantaneo. Si el servidor no acepta, la tarjeta
     * vuelve visiblemente a su sitio: aqui si se revierte a la foto previa (a
     * diferencia del orden de columnas) porque un arrastre es un gesto suelto,
     * no se pueden encadenar diez en medio segundo.
     */
    async soltar(evento: SoltarTarea): Promise<void> {
        // Soltarla donde ya estaba no es un movimiento, no hay que molestar al servidor.
        if (evento.idColumnaOrigen === evento.idColumnaDestino
            && evento.indiceOrigen === evento.indiceDestino) {
            return;
        }

        const destino = this.columnas.find(columna => columna.id === evento.idColumnaDestino);
        if (!destino) { return; }

        // Los vecinos se calculan sobre la columna destino ANTES de tocar nada.
        const vecinos = calcularVecinos(
            destino.tareas.map(tarea => tarea.id),
            evento.tarea.id,
            evento.indiceDestino
        );

        const previas = this.columnas;
        this.columnas = this.conLaTareaMovida(evento.tarea, evento.idColumnaDestino, evento.indiceDestino);

        try {
            await this.tareaService.mover({
                idProyecto: this.idProyecto,
                idTarea: evento.tarea.id,
                idColumnaDestino: evento.idColumnaDestino,
                ...vecinos
            });
        } catch (error) {
            this.columnas = previas;
            this.avisarError('No se pudo mover la tarea', error);
        }
    }

    /**
     * Saca la tarea de donde estuviera y la mete en la columna destino, en su
     * posicion. Se quita de todas las columnas sin preguntar de cual venia:
     * asi el caso de mover entre columnas y el de mover dentro de la misma son
     * el mismo codigo, igual que en calcularVecinos.
     */
    private conLaTareaMovida(tarea: TareaDto, idDestino: string, indice: number): ColumnaConTareasDto[] {
        return this.columnas.map(columna => {
            const tareas = columna.tareas.filter(actual => actual.id !== tarea.id);

            if (columna.id !== idDestino) {
                return { ...columna, tareas };
            }

            return {
                ...columna,
                tareas: [
                    ...tareas.slice(0, indice),
                    { ...tarea, idColumna: idDestino },
                    ...tareas.slice(indice)
                ]
            };
        });
    }

    // ------------------------------------------------------------ formulario

    nueva(columna: ColumnaConTareasDto): void {
        this.tareaEnEdicion = null;
        this.columnaDestino = columna;
        this.mostrarFormulario = true;
    }

    editar(tarea: TareaDto): void {
        this.tareaEnEdicion = tarea;
        this.columnaDestino = this.columnas.find(columna => columna.id === tarea.idColumna) ?? null;
        this.mostrarFormulario = true;
    }

    async guardar(datos: GuardarTareaComando): Promise<void> {
        this.guardando = true;

        try {
            if (this.tareaEnEdicion) {
                const actualizada = await this.tareaService.actualizar({
                    idProyecto: this.idProyecto,
                    idTarea: this.tareaEnEdicion.id,
                    ...datos
                });
                this.columnas = this.conLaTareaReemplazada(actualizada);
                this.avisar('Tarea actualizada', actualizada.titulo);
            } else {
                const creada = await this.tareaService.crear({
                    idProyecto: this.idProyecto,
                    idColumna: this.columnaDestino!.id,
                    ...datos
                });
                this.columnas = this.conLaTareaAnadida(creada);
                this.avisar('Tarea creada', creada.titulo);
            }

            // Solo se cierra si el backend acepto: si falla, lo escrito sigue ahi.
            this.mostrarFormulario = false;
        } catch (error) {
            this.avisarError('No se pudo guardar la tarea', error);
        } finally {
            this.guardando = false;
        }
    }

    private conLaTareaReemplazada(tarea: TareaDto): ColumnaConTareasDto[] {
        return this.columnas.map(columna => ({
            ...columna,
            tareas: columna.tareas.map(actual => actual.id === tarea.id ? tarea : actual)
        }));
    }

    /** Se coloca por el `orden` que le dio el backend, no siempre al final. */
    private conLaTareaAnadida(tarea: TareaDto): ColumnaConTareasDto[] {
        return this.columnas.map(columna => columna.id !== tarea.idColumna
            ? columna
            : { ...columna, tareas: [...columna.tareas, tarea].sort((una, otra) => una.orden - otra.orden) }
        );
    }

    // -------------------------------------------------------------- eliminar

    confirmarEliminacion(tarea: TareaDto): void {
        // Se cierra el formulario antes de preguntar: dos dialogos modales
        // encima del otro se leen mal y el de abajo ya no aporta nada.
        this.mostrarFormulario = false;

        this.confirmacion.confirm({
            header: 'Eliminar tarea',
            message: `Se eliminara <b>${tarea.titulo}</b> del tablero.`,
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Eliminar',
            rejectLabel: 'Cancelar',
            acceptButtonStyleClass: 'p-button-danger',
            rejectButtonStyleClass: 'p-button-text',
            accept: () => this.eliminar(tarea)
        });
    }

    private async eliminar(tarea: TareaDto): Promise<void> {
        const previas = this.columnas;

        this.columnas = this.columnas.map(columna => ({
            ...columna,
            tareas: columna.tareas.filter(actual => actual.id !== tarea.id)
        }));

        try {
            await this.tareaService.eliminar(this.idProyecto, tarea.id);
            this.avisar('Tarea eliminada', tarea.titulo);
        } catch (error) {
            this.columnas = previas;
            this.avisarError('No se pudo eliminar la tarea', error);
        }
    }

    // ------------------------------------------------------------- navegacion

    verColumnas(): void {
        this.router.navigate(['/business/proyectos', this.idProyecto, 'columnas'], {
            state: { nombreProyecto: this.nombreProyecto }
        });
    }

    // ----------------------------------------------------------------- avisos

    private avisar(titulo: string, detalle: string): void {
        this.mensajes.add({ severity: 'success', summary: titulo, detail: detalle, life: TOAST_LIFE });
    }

    /** GenericService ya trae el `detail` del ProblemDetails dentro del Error. */
    private avisarError(titulo: string, error: unknown): void {
        const detalle = error instanceof Error && error.message
            ? error.message
            : 'Ocurrio un error inesperado.';
        this.mensajes.add({ severity: 'error', summary: titulo, detail: detalle, life: TOAST_LIFE });
    }
}
