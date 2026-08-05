import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TOAST_LIFE } from 'src/app/config/app.constants';
import { ColumnaDto } from 'src/app/models';
import { ColumnaService } from 'src/app/services';
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
    nombreProyecto = '';

    columnas: ColumnaDto[] = [];
    cargando = false;
    guardando = false;

    mostrarFormulario = false;
    columnaEnEdicion: ColumnaDto | null = null;

    /** Descarta respuestas de reordenaciones que ya quedaron atras. */
    private ordenEnVuelo = 0;

    constructor(
        private readonly rutaActiva: ActivatedRoute,
        private readonly columnaService: ColumnaService,
        private readonly confirmacion: ConfirmationService,
        private readonly mensajes: MessageService
    ) { }

    ngOnInit(): void {
        // snapshot y no observable: al cambiar de proyecto se entra por el
        // listado, nunca se reusa el componente con otro id.
        this.idProyecto = this.rutaActiva.snapshot.paramMap.get('idProyecto') ?? '';

        // El nombre lo manda el listado al navegar. La API no tiene un GET de
        // un proyecto suelto (/proyectos/{id} solo acepta PUT y DELETE), asi
        // que entrando por enlace directo o tras F5 no hay de donde sacarlo y
        // la cabecera se queda con el texto generico.
        this.nombreProyecto = history.state?.nombreProyecto ?? '';

        this.cargar();
    }

    // ------------------------------------------------------------------ carga

    private async cargar(): Promise<void> {
        this.cargando = true;

        try {
            this.columnas = this.ordenadas(await this.columnaService.listar(this.idProyecto));
        } catch (error) {
            this.columnas = [];
            this.avisarError('No se pudieron cargar las columnas', error);
        } finally {
            this.cargando = false;
        }
    }

    /**
     * Se ordena por `orden` aunque el backend ya lo mande ordenado: la pantalla
     * promete que lo que se ve es el orden real, y eso no puede depender de en
     * que secuencia venga el json.
     */
    private ordenadas(columnas: ColumnaDto[]): ColumnaDto[] {
        return [...columnas].sort((una, otra) => una.orden - otra.orden);
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
            this.columnas = this.ordenadas(confirmadas);
        } catch (error) {
            if (peticion !== this.ordenEnVuelo) { return; }

            this.avisarError('No se pudo guardar el orden', error);
            await this.cargar();
        }
    }

    // ------------------------------------------------------------ formulario

    nueva(): void {
        this.columnaEnEdicion = null;
        this.mostrarFormulario = true;
    }

    renombrar(columna: ColumnaDto): void {
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

                // Renombrar no mueve nada: se parchea la fila y no se recarga.
                this.columnas = this.columnas.map(columna =>
                    columna.id === actualizada.id ? { ...columna, nombre: actualizada.nombre } : columna
                );
                this.avisar('Columna renombrada', nombre);
            } else {
                const creada = await this.columnaService.agregar({ idProyecto: this.idProyecto, nombre });

                // Entra al final del tablero, con el orden que le puso el backend.
                this.columnas = [...this.columnas, creada];
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

    confirmarEliminacion(columna: ColumnaDto): void {
        this.confirmacion.confirm({
            header: 'Eliminar columna',
            message: `Se eliminara la columna <b>${columna.nombre}</b> del tablero.`
                + ' Si tiene tareas dentro, el servidor no dejara.',
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Eliminar',
            rejectLabel: 'Cancelar',
            acceptButtonStyleClass: 'p-button-danger',
            rejectButtonStyleClass: 'p-button-text',
            accept: () => this.eliminar(columna)
        });
    }

    /**
     * No se adelanta ninguna comprobacion: el GET de columnas no dice cuantas
     * tareas tiene cada una, asi que la regla la contesta el backend con un
     * 400 y su mensaje, que es de donde sale el aviso.
     */
    private async eliminar(columna: ColumnaDto): Promise<void> {
        try {
            await this.columnaService.eliminar(this.idProyecto, columna.id);
            this.avisar('Columna eliminada', columna.nombre);
        } catch (error) {
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
            detail: 'El tablero de este proyecto todavia no esta construido.',
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
