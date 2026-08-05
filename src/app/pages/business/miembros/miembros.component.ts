import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { of, Subject } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { TOAST_LIFE } from 'src/app/config/app.constants';
import { MiembroDto, UsuarioDto } from 'src/app/models';
import { MiembroService, UsuarioService } from 'src/app/services';

/** Cuantas sugerencias trae el buscador de usuarios de una vez. */
const SUGERENCIAS = 8;

/**
 * Contenedor de los miembros de UN proyecto.
 *
 * Cuelga de /business/proyectos/:idProyecto/miembros, igual que columnas: en
 * la API un miembro no existe fuera de su proyecto.
 *
 * Agregar no es un alta de usuario: se busca a alguien que ya existe con
 * UsuarioService y se manda su id. Por eso el buscador vive aqui y no en el
 * dialogo, que es presentacional y no inyecta servicios.
 */
@Component({
    selector: 'app-miembros',
    templateUrl: './miembros.component.html',
    providers: [ConfirmationService]
})
export class MiembrosComponent implements OnInit {

    idProyecto = '';
    nombreProyecto = '';

    miembros: MiembroDto[] = [];
    cargando = false;
    guardando = false;

    mostrarFormulario = false;
    /** Resultados del buscador del dialogo. */
    sugerencias: UsuarioDto[] = [];

    private readonly destroyRef = inject(DestroyRef);
    private readonly textoBuscado = new Subject<string>();

    constructor(
        private readonly rutaActiva: ActivatedRoute,
        private readonly router: Router,
        private readonly miembroService: MiembroService,
        private readonly usuarioService: UsuarioService,
        private readonly confirmacion: ConfirmationService,
        private readonly mensajes: MessageService
    ) { }

    ngOnInit(): void {
        this.idProyecto = this.rutaActiva.snapshot.paramMap.get('idProyecto') ?? '';

        // El nombre lo manda quien navega hasta aqui: la API no tiene un GET
        // de un proyecto suelto, asi que con F5 se pierde y cae al generico.
        this.nombreProyecto = history.state?.nombreProyecto ?? '';

        // switchMap y no una promesa: al escribir rapido cada tecla lanza una
        // busqueda, y la anterior se corta en vez de llegar tarde y pisar el
        // resultado bueno. El catchError va dentro para que un fallo de red no
        // mate la suscripcion y deje el buscador muerto.
        this.textoBuscado.pipe(
            switchMap(texto => this.usuarioService
                .buscar$({ filtro: texto, pagina: 1, tamanoPagina: SUGERENCIAS })
                .pipe(catchError(() => of(null)))),
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(resultado => {
            this.sugerencias = this.sinLosQueYaEstan(resultado?.elementos ?? []);
        });

        this.cargar();
    }

    // ------------------------------------------------------------------ carga

    private async cargar(): Promise<void> {
        this.cargando = true;

        try {
            this.miembros = await this.miembroService.listar(this.idProyecto);
        } catch (error) {
            this.miembros = [];
            this.avisarError('No se pudieron cargar los miembros', error);
        } finally {
            this.cargando = false;
        }
    }

    // -------------------------------------------------------------- agregar

    abrirFormulario(): void {
        this.sugerencias = [];
        this.mostrarFormulario = true;
    }

    /** Lo dispara el autocompletar del dialogo. */
    buscarUsuarios(texto: string): void {
        this.textoBuscado.next(texto);
    }

    /** Quien ya es miembro no se ofrece: el POST responderia 400. */
    private sinLosQueYaEstan(usuarios: UsuarioDto[]): UsuarioDto[] {
        const yaEstan = new Set(this.miembros.map(miembro => miembro.idUsuario));
        return usuarios.filter(usuario => !yaEstan.has(usuario.id));
    }

    async agregar(usuario: UsuarioDto): Promise<void> {
        this.guardando = true;

        try {
            const nuevo = await this.miembroService.agregar({
                idProyecto: this.idProyecto,
                idUsuario: usuario.id
            });

            this.miembros = [...this.miembros, nuevo];
            this.avisar('Miembro agregado', nuevo.nombre);
            this.mostrarFormulario = false;
        } catch (error) {
            this.avisarError('No se pudo agregar el miembro', error);
        } finally {
            this.guardando = false;
        }
    }

    // ---------------------------------------------------------------- quitar

    confirmarBaja(miembro: MiembroDto): void {
        this.confirmacion.confirm({
            header: 'Quitar del proyecto',
            message: `<b>${miembro.nombre}</b> dejara de ser miembro de este proyecto.`
                + ' La cuenta de usuario no se toca.',
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Quitar',
            rejectLabel: 'Cancelar',
            acceptButtonStyleClass: 'p-button-danger',
            rejectButtonStyleClass: 'p-button-text',
            accept: () => this.quitar(miembro)
        });
    }

    private async quitar(miembro: MiembroDto): Promise<void> {
        const previos = this.miembros;
        this.miembros = this.miembros.filter(actual => actual.idUsuario !== miembro.idUsuario);

        try {
            await this.miembroService.quitar(this.idProyecto, miembro.idUsuario);
            this.avisar('Miembro quitado', miembro.nombre);
        } catch (error) {
            this.miembros = previos;
            this.avisarError('No se pudo quitar el miembro', error);
        }
    }

    // ------------------------------------------------------------- navegacion

    verTablero(): void {
        this.router.navigate(['/business/proyectos', this.idProyecto, 'tablero'], {
            state: { nombreProyecto: this.nombreProyecto }
        });
    }

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
