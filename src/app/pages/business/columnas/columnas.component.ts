import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TOAST_LIFE } from 'src/app/config/app.constants';
import { ColumnaDto, ColumnaFilaDto } from 'src/app/models';
import { ColumnaService, ProyectoService } from 'src/app/services';
import { moverElemento, renumerarOrden } from 'src/app/utilities';

/**
 * Contenedor de las columnas de UN proyecto.
 *
 * Cuelga de /business/proyectos/:idProyecto/columnas porque en la API una
 * columna no existe fuera de su proyecto: todos los endpoints son
 * /proyectos/{idProyecto}/columnas/... No hay listado global de columnas.
 *
 * Para leerlas se llama al tablero y no a un GET de columnas, que no existe:
 * la API solo expone escritura sobre /columnas. De paso el tablero trae el
 * nombre del proyecto para la cabecera y las tareas de cada columna, que es
 * como se sabe cual se puede borrar.
 */
@Component({
    selector: 'app-columnas',
    templateUrl: './columnas.component.html',
    providers: [ConfirmationService]
})
export class ColumnasComponent implements OnInit {

    idProyecto = '';
    nombreProyecto = '';

    columnas: ColumnaFilaDto[] = [];
    cargando = false;
    guardando = false;

    mostrarFormulario = false;
    columnaEnEdicion: ColumnaFilaDto | null = null;

    /** Descarta respuestas de reordenaciones que ya quedaron atras. */
    private ordenEnVuelo = 0;

    constructor(
        private readonly rutaActiva: ActivatedRoute,
        private readonly proyectoService: ProyectoService,
        private readonly columnaService: ColumnaService,
        private readonly confirmacion: ConfirmationService,
        private readonly mensajes: MessageService
    ) { }

    ngOnInit(): void {
        // snapshot y no observable: al cambiar de proyecto se entra por el
        // listado, nunca se reusa el componente con otro id.
        this.idProyecto = this.rutaActiva.snapshot.paramMap.get('idProyecto') ?? '';
        this.cargar();
    }

    // ------------------------------------------------------------------ carga

    private async cargar(): Promise<void> {
        this.cargando = true;

        try {
            const tablero = await this.proyectoService.obtenerTablero(this.idProyecto);

            this.nombreProyecto = tablero.nombreProyecto;
            this.columnas = this.aFilas(tablero.columnas ?? []);
        } catch (error) {
            this.columnas = [];
            this.avisarError('No se pudieron cargar las columnas', error);
        } finally {
            this.cargando = false;
        }
    }

    /**
     * El tablero trae las tareas enteras y aqui solo interesa cuantas hay.
     * Se ordena por `orden` aunque el backend ya lo mande ordenado: la pantalla
     * promete que lo que se ve es el orden real, y eso no puede depender de en
     * que secuencia venga el json.
     */
    private aFilas(columnas: { id: string; nombre: string; orden: number; tareas?: unknown[] }[]): ColumnaFilaDto[] {
        return [...columnas]
            .sort((una, otra) => una.orden - otra.orden)
            .map(columna => ({
                id: columna.id,
                nombre: columna.nombre,
                orden: columna.orden,
                cantidadTareas: columna.tareas?.length ?? 0
            }));
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
     *
     * Se pinta antes de guardar para que la flecha responda al instante. Si el
     * PUT falla no se vuelve a una foto previa sino que se recarga: con varios
     * clics seguidos esa foto ya no seria el estado real. Y como en cada PUT
     * viajan todos los ids, cada peticion se basta a si misma y vale la ultima.
     */
    private async reordenar(desde: number, hasta: number): Promise<void> {
        this.columnas = renumerarOrden(moverElemento(this.columnas, desde, hasta));

        const peticion = ++this.ordenEnVuelo;

        try {
            const confirmadas = await this.columnaService.reordenar({
                idProyecto: this.idProyecto,
                idsEnOrden: this.columnas.map(columna => columna.id)
            });
            if (peticion !== this.ordenEnVuelo) { return; }

            // Manda el orden que quedo grabado, no el que se calculo aqui.
            this.columnas = this.conCantidadDeTareas(confirmadas);
        } catch (error) {
            if (peticion !== this.ordenEnVuelo) { return; }

            this.avisarError('No se pudo guardar el orden', error);
            await this.cargar();
        }
    }

    /** El backend devuelve ColumnaDto, sin tareas: se conserva el conteo que ya habia. */
    private conCantidadDeTareas(columnas: ColumnaDto[]): ColumnaFilaDto[] {
        const conteos = new Map(this.columnas.map(columna => [columna.id, columna.cantidadTareas]));

        return [...columnas]
            .sort((una, otra) => una.orden - otra.orden)
            .map(columna => ({ ...columna, cantidadTareas: conteos.get(columna.id) ?? 0 }));
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

    async guardar(nombre: string): Promise<void> {
        this.guardando = true;

        try {
            if (this.columnaEnEdicion) {
                const actualizada = await this.columnaService.renombrar({
                    idProyecto: this.idProyecto,
                    idColumna: this.columnaEnEdicion.id,
                    nombre
                });

                // Renombrar no mueve nada, asi que no hace falta recargar el
                // tablero entero (que se traeria todas las tareas) por un nombre.
                this.columnas = this.columnas.map(columna =>
                    columna.id === actualizada.id ? { ...columna, nombre: actualizada.nombre } : columna
                );
                this.avisar('Columna renombrada', nombre);
            } else {
                const creada = await this.columnaService.agregar({ idProyecto: this.idProyecto, nombre });

                // Entra al final y recien creada no puede tener tareas.
                this.columnas = [...this.columnas, { ...creada, cantidadTareas: 0 }];
                this.avisar('Columna creada', nombre);
            }

            // Solo se cierra si el backend acepto: si falla, el nombre escrito
            // sigue ahi para corregirlo.
            this.mostrarFormulario = false;
        } catch (error) {
            this.avisarError('No se pudo guardar la columna', error);
        } finally {
            this.guardando = false;
        }
    }

    // -------------------------------------------------------------- eliminar

    confirmarEliminacion(columna: ColumnaFilaDto): void {
        // La regla la aplica el backend (responde 400); esto solo evita el viaje.
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

    private async eliminar(columna: ColumnaFilaDto): Promise<void> {
        try {
            await this.columnaService.eliminar(this.idProyecto, columna.id);
            this.avisar('Columna eliminada', columna.nombre);
        } catch (error) {
            // Puede llegar un 400 si alguien le metio una tarea mientras tanto:
            // el conteo que tenemos seria viejo.
            this.avisarError('No se pudo eliminar la columna', error);
        }

        // En los dos casos se recarga: al quitar una columna las de abajo suben
        // y el `orden` de todas lo renumera el servidor, no nosotros.
        await this.cargar();
    }

    // ------------------------------------------------------------- navegacion

    verTablero(): void {
        // TODO: navegar a /business/proyectos/:id/tablero cuando exista la pantalla.
        this.mensajes.add({
            severity: 'info',
            summary: 'Tablero pendiente',
            detail: `El tablero de ${this.nombreProyecto} todavia no esta construido.`,
            life: TOAST_LIFE
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
