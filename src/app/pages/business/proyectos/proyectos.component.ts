import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TOAST_LIFE } from 'src/app/config/app.constants';
import { EstadoProyecto } from 'src/app/enums';
import { GuardarProyectoComando, ProyectoDto, ProyectoFiltros } from 'src/app/models';

/**
 * Contenedor de la pantalla de proyectos.
 *
 * OJO: hoy filtra en memoria sobre el mock. La API pagina y filtra por nombre
 * en el servidor (GET /api/v1/proyectos?nombre=&pagina=&tamanoPagina=), asi
 * que cuando entre ProyectoService este contenedor pasa a tabla lazy:
 * `aplicarFiltros()` se convierte en la llamada al servicio y la lista recibe
 * ademas el total de elementos. Los filtros ya tienen la forma del query.
 */
@Component({
    selector: 'app-proyectos',
    templateUrl: './proyectos.component.html',
    providers: [ConfirmationService]
})
export class ProyectosComponent implements OnInit {

    /** Fuente de verdad. MOCK: reemplazar por ProyectoService -> GET /api/v1/proyectos */
    private proyectos: ProyectoDto[] = [
        {
            id: '1', nombre: 'Migracion ERP', descripcion: 'Traslado del ERP a la nube',
            fechaInicio: '2026-01-10', fechaFinPrevista: '2026-06-30',
            estadoProyecto: EstadoProyecto.EnProgreso, cantidadColumnas: 4
        },
        {
            id: '2', nombre: 'App movil de campo', descripcion: 'Levantamiento de datos offline',
            fechaInicio: '2026-03-01', fechaFinPrevista: '2026-09-15',
            estadoProyecto: EstadoProyecto.Planificacion, cantidadColumnas: 3
        },
        {
            id: '3', nombre: 'Portal de clientes', descripcion: null,
            fechaInicio: '2025-09-05', fechaFinPrevista: '2026-01-20',
            estadoProyecto: EstadoProyecto.Completado, cantidadColumnas: 5
        }
    ];

    /** Lo que realmente ve la tabla. */
    proyectosVisibles: ProyectoDto[] = [];
    cargando = false;
    guardando = false;

    /** Mismo nombre que el query param de la API, para que el cambio sea directo. */
    filtros: ProyectoFiltros = { nombre: '' };

    mostrarFormulario = false;
    proyectoEnEdicion: ProyectoDto | null = null;

    constructor(
        private readonly router: Router,
        private readonly confirmacion: ConfirmationService,
        private readonly mensajes: MessageService
    ) { }

    ngOnInit(): void {
        this.aplicarFiltros();
    }

    // ---------------------------------------------------------------- filtros

    /**
     * Solo se filtra por nombre y por coincidencia parcial, que es lo unico
     * que la API sabe hacer. No se agregan filtros que el servidor no soporte:
     * con paginacion en servidor solo filtrarian la pagina actual, y eso es
     * un error que se ve en produccion, no en el mock.
     */
    aplicarFiltros(): void {
        const texto = (this.filtros.nombre ?? '').trim().toLowerCase();
        this.proyectosVisibles = this.proyectos.filter(
            proyecto => !texto || proyecto.nombre.toLowerCase().includes(texto)
        );
    }

    limpiarFiltros(): void {
        this.filtros = { nombre: '' };
        this.aplicarFiltros();
    }

    get hayFiltrosActivos(): boolean {
        return !!this.filtros.nombre;
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

    guardar(datos: GuardarProyectoComando): void {
        if (this.proyectoEnEdicion) {
            const id = this.proyectoEnEdicion.id;
            // Array y objeto nuevos: la lista es OnPush.
            this.proyectos = this.proyectos.map(proyecto =>
                proyecto.id === id
                    ? {
                        ...proyecto,
                        nombre: datos.nombre,
                        descripcion: datos.descripcion,
                        fechaInicio: datos.fechaInicio,
                        fechaFinPrevista: datos.fechaFinPrevista,
                        estadoProyecto: datos.estadoProyecto ?? proyecto.estadoProyecto
                    }
                    : proyecto
            );
            this.avisar('Proyecto actualizado', datos.nombre);
        } else {
            this.proyectos = [
                ...this.proyectos,
                {
                    id: crypto.randomUUID(),
                    nombre: datos.nombre,
                    descripcion: datos.descripcion,
                    fechaInicio: datos.fechaInicio,
                    fechaFinPrevista: datos.fechaFinPrevista,
                    // Un proyecto nuevo siempre nace en planificacion, la API no
                    // recibe el estado en el alta.
                    estadoProyecto: EstadoProyecto.Planificacion,
                    cantidadColumnas: datos.columnas?.length ?? 0
                }
            ];
            this.avisar('Proyecto creado', datos.nombre);
        }

        this.mostrarFormulario = false;
        this.aplicarFiltros();
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

    private eliminar(proyecto: ProyectoDto): void {
        this.proyectos = this.proyectos.filter(actual => actual.id !== proyecto.id);
        this.aplicarFiltros();
        this.avisar('Proyecto eliminado', proyecto.nombre);
    }

    // ------------------------------------------------- navegacion al proyecto

    /** Las columnas viven dentro del proyecto, no en el menu lateral. */
    verColumnas(proyecto: ProyectoDto): void {
        this.router.navigate(['/business/proyectos', proyecto.id, 'columnas']);
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

    private avisar(titulo: string, detalle: string): void {
        this.mensajes.add({ severity: 'success', summary: titulo, detail: detalle, life: TOAST_LIFE });
    }
}
