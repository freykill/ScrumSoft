import { Component, DestroyRef, inject, OnDestroy, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TOAST_LIFE } from 'src/app/config/app.constants';
import { Prioridad } from 'src/app/enums';
import {
    ColumnaConTareasDto,
    ColumnaDto,
    GuardarTareaComando,
    ProyectoDto,
    SoltarTarea,
    TareaDto
} from 'src/app/models';
import { ProyectoService, TableroRealtimeService, TareaService } from 'src/app/services';
import { calcularVecinos } from 'src/app/utilities';

/**
 * Cuantos proyectos entran en el selector. El desplegable filtra en cliente,
 * asi que pasado este tope habria que buscar contra el servidor.
 */
const TOPE_DEL_SELECTOR = 100;

/**
 * Tablero de UN proyecto, con selector para saltar entre ellos.
 *
 * Es la unica pantalla que lee tareas: la API no tiene un GET de tareas, solo
 * el tablero las devuelve, ya repartidas por columna. Por eso las tareas se
 * crean y se editan desde aqui y no desde un listado aparte.
 *
 * Vive en /proyectos/:idProyecto/tablero y no en una ruta suelta aunque se
 * entre por el menu: el proyecto tiene que estar en la url para que el enlace
 * directo y el F5 abran el mismo tablero, y para que mas adelante la conexion
 * de tiempo real sepa a que tablero suscribirse. /business/tablero existe solo
 * como atajo y redirige al primer proyecto.
 */
@Component({
    selector: 'app-tablero',
    templateUrl: './tablero.component.html',
    providers: [ConfirmationService]
})
export class TableroComponent implements OnInit, OnDestroy {

    idProyecto = '';

    /** Para el selector de la cabecera. */
    proyectos: ProyectoDto[] = [];

    columnas: ColumnaConTareasDto[] = [];
    cargando = false;
    guardando = false;

    /** Hay conexion con el hub, o sea que se ven los cambios de los demas. */
    enVivo = false;

    /** Resumen de la cabecera. Se calcula al cargar, no en getters de plantilla. */
    totalTareas = 0;
    tareasPrioritarias = 0;

    mostrarFormulario = false;
    tareaEnEdicion: TareaDto | null = null;
    /** En que columna se crea la tarea nueva. Solo se usa en el alta. */
    columnaDestino: ColumnaConTareasDto | null = null;

    private readonly destroyRef = inject(DestroyRef);
    private listaPedida = false;

    constructor(
        private readonly rutaActiva: ActivatedRoute,
        private readonly router: Router,
        private readonly proyectoService: ProyectoService,
        private readonly tareaService: TareaService,
        private readonly enVivoService: TableroRealtimeService,
        private readonly confirmacion: ConfirmationService,
        private readonly mensajes: MessageService
    ) { }

    ngOnInit(): void {
        // paramMap y no snapshot: al cambiar de proyecto en el selector se
        // navega a la misma ruta con otro id, y Angular reusa el componente
        // sin volver a llamar a ngOnInit. Con snapshot el tablero se quedaria
        // pintando el proyecto anterior.
        this.rutaActiva.paramMap
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(parametros => this.alNavegar(parametros.get('idProyecto') ?? ''));

        this.escucharElHub();
    }

    ngOnDestroy(): void {
        // takeUntilDestroyed corta las suscripciones, pero el socket hay que
        // cerrarlo a mano o se queda escuchando un tablero que ya no se ve.
        this.enVivoService.desconectar();
    }

    // ------------------------------------------------------------- en vivo

    /**
     * Los eventos del hub se aplican con los mismos metodos que las respuestas
     * del REST, y eso es lo que hace que el eco sea inofensivo: quien mueve una
     * tarjeta recibe tambien su propio TareaMovida, porque el evento va a todo
     * el grupo sin excluir al autor. Como `conLaTarea` reemplaza por id y
     * reordena por `orden`, aplicarlo dos veces da el mismo resultado.
     */
    private escucharElHub(): void {
        this.enVivoService.tareaCambiada
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(tarea => {
                this.columnas = this.conLaTarea(tarea);
                this.recalcularResumen();
            });

        this.enVivoService.tareaEliminada
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(idTarea => {
                this.columnas = this.sinLaTarea(idTarea);
                this.recalcularResumen();
            });

        this.enVivoService.columnasCambiadas
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(columnas => {
                this.columnas = this.conLasColumnas(columnas);
                this.recalcularResumen();
            });

        // No hay historial de eventos: lo que paso mientras estabamos caidos
        // se perdio, asi que lo unico honesto es volver a pedir el tablero.
        this.enVivoService.reconectado
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(() => this.cargarTablero());

        this.enVivoService.estadoCambiado
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(vivo => this.enVivo = vivo);
    }

    private async conectarEnVivo(): Promise<void> {
        try {
            await this.enVivoService.conectar(this.idProyecto);
            this.enVivo = true;
        } catch (error) {
            // Sin hub la pantalla funciona igual, solo deja de enterarse de lo
            // que hacen los demas. No se molesta al usuario con un error por
            // algo que no le impide trabajar.
            this.enVivo = false;
            console.debug('Tiempo real no disponible', error);
        }
    }

    /**
     * Coloca la tarea donde dice el servidor: se quita de donde estuviera y se
     * mete en su columna, ordenando por `orden`. Vale para crear, editar,
     * mover y para los eventos del hub, y aplicarla varias veces no cambia
     * nada, que es lo que la hace segura contra el eco.
     */
    private conLaTarea(tarea: TareaDto): ColumnaConTareasDto[] {
        return this.columnas.map(columna => {
            const sinElla = columna.tareas.filter(actual => actual.id !== tarea.id);

            return columna.id !== tarea.idColumna
                ? { ...columna, tareas: sinElla }
                : { ...columna, tareas: [...sinElla, tarea].sort((una, otra) => una.orden - otra.orden) };
        });
    }

    private sinLaTarea(idTarea: string): ColumnaConTareasDto[] {
        return this.columnas.map(columna => ({
            ...columna,
            tareas: columna.tareas.filter(tarea => tarea.id !== idTarea)
        }));
    }

    /**
     * ColumnasActualizadas trae la estructura SIN las tareas, asi que hay que
     * conservar las que ya estan en pantalla. Una columna nueva entra vacia y
     * una que desaparecio se lleva sus tareas, que es lo correcto: el backend
     * no deja borrar una columna con tareas dentro.
     */
    private conLasColumnas(columnas: ColumnaDto[]): ColumnaConTareasDto[] {
        const tareasPorColumna = new Map(this.columnas.map(columna => [columna.id, columna.tareas]));

        return [...columnas]
            .sort((una, otra) => una.orden - otra.orden)
            .map(columna => ({ ...columna, tareas: tareasPorColumna.get(columna.id) ?? [] }));
    }

    // ------------------------------------------------------------------ carga

    private async alNavegar(idProyecto: string): Promise<void> {
        await this.cargarProyectos();

        if (!idProyecto) {
            this.abrirPrimerProyecto();
            return;
        }

        this.idProyecto = idProyecto;
        await this.cargarTablero();

        // Despues de pintar: primero se ve el tablero, luego se engancha en vivo.
        await this.conectarEnVivo();
    }

    /** El selector se pide una sola vez, no en cada salto de proyecto. */
    private async cargarProyectos(): Promise<void> {
        if (this.listaPedida) { return; }
        this.listaPedida = true;

        try {
            const pagina = await this.proyectoService.listar({ pagina: 1, tamanoPagina: TOPE_DEL_SELECTOR });
            this.proyectos = pagina.elementos ?? [];
        } catch (error) {
            this.avisarError('No se pudo cargar la lista de proyectos', error);
        }
    }

    /** Se entro por /business/tablero, sin proyecto: se abre el primero que haya. */
    private abrirPrimerProyecto(): void {
        const primero = this.proyectos[0];
        if (!primero) { return; }

        // replaceUrl para que el boton de atras del navegador no rebote entre
        // el atajo y el tablero al que acaba de mandar.
        this.router.navigate(['/business/proyectos', primero.id, 'tablero'], { replaceUrl: true });
    }

    private async cargarTablero(): Promise<void> {
        this.cargando = true;

        try {
            const tablero = await this.proyectoService.obtenerTablero(this.idProyecto);

            this.columnas = (tablero.columnas ?? [])
                .map(columna => ({ ...columna, tareas: [...(columna.tareas ?? [])] }))
                .sort((una, otra) => una.orden - otra.orden);
        } catch (error) {
            this.columnas = [];
            this.avisarError('No se pudo cargar el tablero', error);
        } finally {
            this.cargando = false;
            this.recalcularResumen();
        }
    }

    private recalcularResumen(): void {
        const tareas = this.columnas.flatMap(columna => columna.tareas);

        this.totalTareas = tareas.length;
        this.tareasPrioritarias = tareas.filter(
            tarea => tarea.prioridad === Prioridad.Alta || tarea.prioridad === Prioridad.Critica
        ).length;
    }

    /**
     * Sin trackBy, cada cambio del array rehace el DOM de todas las columnas y
     * el CDK pierde por medio la que se esta arrastrando.
     */
    porId(_indice: number, columna: ColumnaConTareasDto): string {
        return columna.id;
    }

    // ------------------------------------------------------------- proyectos

    cambiarProyecto(idProyecto: string): void {
        if (idProyecto === this.idProyecto) { return; }

        this.router.navigate(['/business/proyectos', idProyecto, 'tablero']);
    }

    get nombreProyecto(): string {
        return this.proyectos.find(proyecto => proyecto.id === this.idProyecto)?.nombre ?? '';
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
            const movida = await this.tareaService.mover({
                idProyecto: this.idProyecto,
                idTarea: evento.tarea.id,
                idColumnaDestino: evento.idColumnaDestino,
                ...vecinos
            });

            // Se aplica el `orden` que asigno el servidor. Sin esto la tarea se
            // queda con el viejo, y en cuanto llegue cualquier evento del hub
            // (que reordena por `orden`) la tarjeta saltaria a su sitio anterior.
            this.columnas = this.conLaTarea(movida);
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
                this.columnas = this.conLaTarea(actualizada);
                this.avisar('Tarea actualizada', actualizada.titulo);
            } else {
                const creada = await this.tareaService.crear({
                    idProyecto: this.idProyecto,
                    idColumna: this.columnaDestino!.id,
                    ...datos
                });
                this.columnas = this.conLaTarea(creada);
                this.avisar('Tarea creada', creada.titulo);
            }

            // Solo se cierra si el backend acepto: si falla, lo escrito sigue ahi.
            this.mostrarFormulario = false;
            this.recalcularResumen();
        } catch (error) {
            this.avisarError('No se pudo guardar la tarea', error);
        } finally {
            this.guardando = false;
        }
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

        this.columnas = this.sinLaTarea(tarea.id);
        this.recalcularResumen();

        try {
            await this.tareaService.eliminar(this.idProyecto, tarea.id);
            this.avisar('Tarea eliminada', tarea.titulo);
        } catch (error) {
            this.columnas = previas;
            this.recalcularResumen();
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
