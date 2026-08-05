import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MessageService } from 'primeng/api';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { DEBOUNCE_BUSQUEDA, PAGINACION, TOAST_LIFE } from 'src/app/config/app.constants';
import { AuthService } from 'src/app/common/services';
import { GuardarUsuarioComando, PaginaSolicitada, UsuarioDto, UsuarioFiltros } from 'src/app/models';
import { UsuarioService } from 'src/app/services';

/**
 * Contenedor de la pantalla de usuarios.
 *
 * La API pagina y filtra en el servidor, asi que la tabla es lazy: aqui solo
 * vive la pagina que se esta viendo.
 *
 * No hay eliminar: /usuarios acepta GET, POST y PUT, pero no DELETE. Tampoco
 * hay estado de registro en UsuarioDto, asi que no existe el borrado logico
 * que si tienen las otras entidades.
 */
@Component({
    selector: 'app-usuarios',
    templateUrl: './usuarios.component.html'
})
export class UsuariosComponent implements OnInit {

    usuarios: UsuarioDto[] = [];
    totalElementos = 0;

    cargando = false;
    guardando = false;

    /** Tiene la forma exacta del query del GET, se manda tal cual al servicio. */
    filtros: UsuarioFiltros = {
        filtro: '',
        pagina: 1,
        tamanoPagina: PAGINACION.LIMIT
    };

    mostrarFormulario = false;
    usuarioEnEdicion: UsuarioDto | null = null;

    /**
     * Crear y editar usuarios son las UNICAS dos acciones que dependen del rol;
     * todo lo demas en la aplicacion se decide por pertenecer al proyecto.
     * Esconder los botones no es la seguridad -el backend responde 403 igual-,
     * es no ofrecer algo que va a fallar.
     */
    readonly esAdministrador = this.auth.esAdministrador();
    private readonly idUsuarioEnSesion = this.auth.idUsuario;

    private readonly destroyRef = inject(DestroyRef);
    private readonly textoBuscado = new Subject<string>();

    /** Descarta respuestas de busquedas que ya quedaron atras. */
    private peticionActual = 0;

    constructor(
        private readonly usuarioService: UsuarioService,
        private readonly auth: AuthService,
        private readonly mensajes: MessageService
    ) { }

    /**
     * El backend rechaza que alguien cambie su propio rol: si el ultimo
     * administrador se degradara, nadie podria volver a crear usuarios.
     */
    get editandoseASiMismo(): boolean {
        return !!this.usuarioEnEdicion && this.usuarioEnEdicion.id === this.idUsuarioEnSesion;
    }

    ngOnInit(): void {
        this.textoBuscado.pipe(
            debounceTime(DEBOUNCE_BUSQUEDA),
            distinctUntilChanged(),
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(() => this.irAPrimeraPagina());

        this.cargar();
    }

    /**
     * Indice de la primera fila de la pagina actual. Getter y no campo porque
     * devuelve un numero: no crea referencias nuevas, no rompe el OnPush.
     */
    get primeraFila(): number {
        return (this.filtros.pagina! - 1) * this.filtros.tamanoPagina!;
    }

    // ---------------------------------------------------------------- carga

    private async cargar(): Promise<void> {
        const peticion = ++this.peticionActual;
        this.cargando = true;

        try {
            const resultado = await this.usuarioService.listar(this.filtros);
            if (peticion !== this.peticionActual) { return; }

            this.usuarios = resultado.elementos ?? [];
            this.totalElementos = resultado.totalElementos ?? 0;
        } catch (error) {
            if (peticion !== this.peticionActual) { return; }

            this.usuarios = [];
            this.totalElementos = 0;
            this.avisarError('No se pudieron cargar los usuarios', error);
        } finally {
            if (peticion === this.peticionActual) {
                this.cargando = false;
            }
        }
    }

    // ---------------------------------------------------------------- filtros

    /**
     * Un solo campo de busqueda, que es lo unico que acepta la API. No se
     * anaden filtros de rol o estado: con paginacion en servidor solo
     * filtrarian la pagina cargada, y ese error no se ve hasta produccion.
     */
    buscar(texto: string): void {
        this.textoBuscado.next(texto);
    }

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
        this.usuarioEnEdicion = null;
        this.mostrarFormulario = true;
    }

    editar(usuario: UsuarioDto): void {
        this.usuarioEnEdicion = usuario;
        this.mostrarFormulario = true;
    }

    async guardar(datos: GuardarUsuarioComando): Promise<void> {
        this.guardando = true;

        try {
            if (this.usuarioEnEdicion) {
                const actualizado = await this.usuarioService.actualizar({
                    id: this.usuarioEnEdicion.id,
                    nombre: datos.nombre,
                    rol: datos.rol
                });

                // Editar no cambia el orden ni el total: se parchea la fila.
                this.usuarios = this.usuarios.map(usuario =>
                    usuario.id === actualizado.id ? actualizado : usuario
                );
                this.avisar('Usuario actualizado', actualizado.nombre);

                this.mostrarFormulario = false;
            } else {
                const creado = await this.usuarioService.crear({
                    nombre: datos.nombre,
                    // El '!' es seguro: en alta el formulario los exige.
                    correoElectronico: datos.correoElectronico!,
                    contrasena: datos.contrasena!,
                    rol: datos.rol
                });
                this.avisar('Usuario creado', creado.nombre);

                // Se recarga porque el nuevo puede caer en otra pagina y ademas
                // cambia el total que dimensiona el paginador.
                this.mostrarFormulario = false;
                await this.cargar();
            }
        } catch (error) {
            // El dialogo sigue abierto: lo escrito se conserva para corregir.
            this.avisarError('No se pudo guardar el usuario', error);
        } finally {
            this.guardando = false;
        }
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
