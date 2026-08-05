import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { DEBOUNCE_BUSQUEDA, PAGINACION, TOAST_LIFE } from 'src/app/config/app.constants';
import { GuardarProyectoComando, PaginaSolicitada, ProyectoDto, ProyectoFiltros } from 'src/app/models';
import { ProyectoService } from 'src/app/services';

/**
 * Contenedor de la pantalla de proyectos.
 *
 * La API pagina y filtra en el servidor, asi que la tabla es lazy: aqui solo
 * vive la pagina que se esta viendo, no la coleccion entera. Cualquier cambio
 * de filtro o de pagina termina en una llamada a `cargar()`.
 */
@Component({
    selector: 'app-proyectos',
    templateUrl: './proyectos.component.html',
    providers: [ConfirmationService]
})
export class ProyectosComponent implements OnInit {

    /** Solo la pagina actual, no todos los proyectos. */
    proyectos: ProyectoDto[] = [];
    /** Total del servidor, es lo que dimensiona el paginador. */
    totalElementos = 0;

    cargando = false;
    guardando = false;

    /** Tiene la forma exacta del query del GET, se manda tal cual al servicio. */
    filtros: ProyectoFiltros = {
        nombre: '',
        pagina: 1,
        tamanoPagina: PAGINACION.LIMIT
    };

    mostrarFormulario = false;
    proyectoEnEdicion: ProyectoDto | null = null;

    private readonly destroyRef = inject(DestroyRef);
    private readonly textoBuscado = new Subject<string>();

    /**
     * Descarta respuestas viejas. Con el servidor de por medio dos busquedas
     * seguidas pueden volver en desorden y dejar en pantalla el resultado de
     * la que ya no interesa.
     */
    private peticionActual = 0;

    constructor(
        private readonly proyectoService: ProyectoService,
        private readonly router: Router,
        private readonly confirmacion: ConfirmationService,
        private readonly mensajes: MessageService
    ) { }

    ngOnInit(): void {
        // Escribir no puede disparar una peticion por tecla: se espera a que el
        // usuario pare, y si el texto quedo igual que el anterior no se repite.
        this.textoBuscado.pipe(
            debounceTime(DEBOUNCE_BUSQUEDA),
            distinctUntilChanged(),
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(() => this.irAPrimeraPagina());

        this.cargar();
    }

    /**
     * Indice de la primera fila de la pagina actual, que es lo que entiende el
     * paginador. Getter y no campo porque devuelve un numero: no crea
     * referencias nuevas, asi que no rompe el OnPush de la lista.
     */
    get primeraFila(): number {
        return (this.filtros.pagina! - 1) * this.filtros.tamanoPagina!;
    }

    // ---------------------------------------------------------------- carga

    private async cargar(): Promise<void> {
        const peticion = ++this.peticionActual;
        this.cargando = true;

        try {
            const resultado = await this.proyectoService.listar(this.filtros);
            if (peticion !== this.peticionActual) { return; }

            this.proyectos = resultado.elementos ?? [];
            this.totalElementos = resultado.totalElementos ?? 0;
        } catch (error) {
            if (peticion !== this.peticionActual) { return; }

            this.proyectos = [];
            this.totalElementos = 0;
            this.avisarError('No se pudieron cargar los proyectos', error);
        } finally {
            if (peticion === this.peticionActual) {
                this.cargando = false;
            }
        }
    }

    // ---------------------------------------------------------------- filtros

    /**
     * Solo se filtra por nombre, que es lo unico que la API sabe hacer. Un
     * filtro que el servidor no soporte filtraria unicamente la pagina que
     * esta cargada, y ese error no se ve hasta que hay datos de verdad.
     */
    buscar(texto: string): void {
        this.textoBuscado.next(texto);
    }

    /** Al cambiar el filtro hay que volver al principio: la pagina 4 puede ya no existir. */
    private irAPrimeraPagina(): void {
        this.filtros = { ...this.filtros, pagina: 1 };
        this.cargar();
    }

    // -------------------------------------------------------------- paginacion

    paginar(pagina: PaginaSolicitada): void {
        this.filtros = { ...this.filtros, pagina: pagina.pagina, tamanoPagina: pagina.tamanoPagina };
        this.cargar();
    }

    // ------------------------------------------------------------ formulario

    nuevo(): void {
        this.proyectoEnEdicion = null;
        this.mostrarFormulario = true;
    }

    editar(proyecto: ProyectoDto): void {
        this.proyectoEnEdicion = proyecto;
        this.mostrarFormulario = true;
    }

    async guardar(datos: GuardarProyectoComando): Promise<void> {
        this.guardando = true;

        try {
            if (this.proyectoEnEdicion) {
                await this.proyectoService.actualizar({
                    id: this.proyectoEnEdicion.id,
                    nombre: datos.nombre,
                    descripcion: datos.descripcion,
                    fechaInicio: datos.fechaInicio,
                    fechaFinPrevista: datos.fechaFinPrevista,
                    // El estado solo lo trae el formulario en edicion; si faltara
                    // se conserva el que ya tenia, nunca se manda vacio.
                    estadoProyecto: datos.estadoProyecto ?? this.proyectoEnEdicion.estadoProyecto
                });
                this.avisar('Proyecto actualizado', datos.nombre);
            } else {
                await this.proyectoService.crear({
                    nombre: datos.nombre,
                    descripcion: datos.descripcion,
                    fechaInicio: datos.fechaInicio,
                    fechaFinPrevista: datos.fechaFinPrevista,
                    columnas: datos.columnas?.length ? datos.columnas : null
                });
                this.avisar('Proyecto creado', datos.nombre);
            }

            // El dialogo se cierra solo si el backend acepto. Si falla, el
            // usuario conserva lo que escribio y puede corregir y reintentar.
            this.mostrarFormulario = false;
            await this.cargar();
        } catch (error) {
            this.avisarError('No se pudo guardar el proyecto', error);
        } finally {
            this.guardando = false;
        }
    }

    // -------------------------------------------------------------- eliminar

    confirmarEliminacion(proyecto: ProyectoDto): void {
        this.confirmacion.confirm({
            header: 'Eliminar proyecto',
            message: `Se eliminara <b>${proyecto.nombre}</b> con su tablero, sus columnas y sus tareas.`,
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Eliminar',
            rejectLabel: 'Cancelar',
            acceptButtonStyleClass: 'p-button-danger',
            rejectButtonStyleClass: 'p-button-text',
            accept: () => this.eliminar(proyecto)
        });
    }

    private async eliminar(proyecto: ProyectoDto): Promise<void> {
        try {
            await this.proyectoService.eliminar(proyecto.id);
            this.avisar('Proyecto eliminado', proyecto.nombre);

            // Era el ultimo de la pagina: hay que retroceder o la tabla se
            // queda vacia en una pagina que ya no existe.
            if (this.proyectos.length === 1 && this.filtros.pagina! > 1) {
                this.filtros = { ...this.filtros, pagina: this.filtros.pagina! - 1 };
            }

            await this.cargar();
        } catch (error) {
            this.avisarError('No se pudo eliminar el proyecto', error);
        }
    }

    // ------------------------------------------------- navegacion al proyecto

    /** Las columnas viven dentro del proyecto, no en el menu lateral. */
    verColumnas(proyecto: ProyectoDto): void {
        // El nombre viaja en el state solo para la cabecera de esa pantalla:
        // la API no tiene GET /proyectos/{id}, asi que desde alli no hay como
        // pedirlo. Si se entra por enlace directo se pierde, y por eso alla se
        // resuelve con un texto generico en vez de dejar el titulo a medias.
        this.router.navigate(['/business/proyectos', proyecto.id, 'columnas'], {
            state: { nombreProyecto: proyecto.nombre }
        });
    }

    verTablero(proyecto: ProyectoDto): void {
        // TODO: navegar a /business/proyectos/:id/tablero cuando exista la pantalla.
        this.mensajes.add({
            severity: 'info',
            summary: 'Tablero pendiente',
            detail: `El tablero de ${proyecto.nombre} todavia no esta construido.`,
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
