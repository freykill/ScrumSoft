import { Component, OnInit } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TOAST_LIFE } from 'src/app/config/app.constants';
import { OPCIONES_ROL, RolUsuario } from 'src/app/enums';
import { GuardarUsuarioComando, UsuarioDto, UsuarioFiltros } from 'src/app/models';

/**
 * Contenedor de la pantalla de usuarios.
 *
 * Aqui vive todo lo que es estado y decision: la lista, los filtros, que
 * dialogo esta abierto y que pasa al guardar o eliminar. Los hijos
 * (app-usuarios-list y app-usuarios-form) no saben nada de esto, solo
 * reciben datos y avisan de lo que el usuario hizo.
 */
@Component({
    selector: 'app-usuarios',
    templateUrl: './usuarios.component.html',
    // Instancia propia de la pantalla: el p-confirmDialog de esta plantilla
    // resuelve el mismo servicio y no se cruza con otras pantallas.
    providers: [ConfirmationService]
})
export class UsuariosComponent implements OnInit {

    /** Fuente de verdad. MOCK: reemplazar por UsuarioService cuando la API exponga /usuarios. */
    private usuarios: UsuarioDto[] = [
        {
            id: '1', nombre: 'Ivan Diaz', correoElectronico: 'admin@scrumsoft.com',
            rol: RolUsuario.Administrador, estadoRegistro: 'A', fechaCreacion: '2026-01-15T09:00:00Z'
        },
        {
            id: '2', nombre: 'Laura Mendez', correoElectronico: 'laura.mendez@scrumsoft.com',
            rol: RolUsuario.Miembro, estadoRegistro: 'A', fechaCreacion: '2026-02-03T11:30:00Z'
        },
        {
            id: '3', nombre: 'Carlos Rueda', correoElectronico: 'carlos.rueda@scrumsoft.com',
            rol: RolUsuario.Miembro, estadoRegistro: 'A', fechaCreacion: '2026-03-21T15:45:00Z'
        },
        {
            id: '4', nombre: 'Sofia Paredes', correoElectronico: 'sofia.paredes@scrumsoft.com',
            rol: RolUsuario.Miembro, estadoRegistro: 'E', fechaCreacion: '2026-04-08T08:20:00Z'
        }
    ];

    /** Lo que realmente ve la tabla, ya filtrado. */
    usuariosVisibles: UsuarioDto[] = [];
    cargando = false;
    guardando = false;

    filtros: UsuarioFiltros = { busqueda: '', rol: null, estado: null };

    readonly opcionesRol = OPCIONES_ROL;
    readonly opcionesEstado = [
        { label: 'Activos', value: 'A' },
        { label: 'Eliminados', value: 'E' }
    ];

    mostrarFormulario = false;
    usuarioEnEdicion: UsuarioDto | null = null;

    constructor(
        private readonly confirmacion: ConfirmationService,
        private readonly mensajes: MessageService
    ) { }

    ngOnInit(): void {
        this.aplicarFiltros();
    }

    // ---------------------------------------------------------------- filtros

    /**
     * Filtra en memoria y deja el resultado en un campo, no en un getter:
     * un getter en la plantilla se reevalua en cada ciclo de deteccion de
     * cambios y ademas devolveria un array nuevo cada vez, que es justo lo
     * que rompe el OnPush del hijo.
     *
     * Cuando esto pase a la API (como en proyectos) este metodo se cambia por
     * la llamada al servicio con los mismos filtros como query params.
     */
    aplicarFiltros(): void {
        const texto = this.filtros.busqueda.trim().toLowerCase();

        this.usuariosVisibles = this.usuarios.filter(usuario => {
            const coincideTexto = !texto
                || usuario.nombre.toLowerCase().includes(texto)
                || usuario.correoElectronico.toLowerCase().includes(texto);
            const coincideRol = !this.filtros.rol || usuario.rol === this.filtros.rol;
            const coincideEstado = !this.filtros.estado || usuario.estadoRegistro === this.filtros.estado;

            return coincideTexto && coincideRol && coincideEstado;
        });
    }

    limpiarFiltros(): void {
        this.filtros = { busqueda: '', rol: null, estado: null };
        this.aplicarFiltros();
    }

    get hayFiltrosActivos(): boolean {
        return !!this.filtros.busqueda || !!this.filtros.rol || !!this.filtros.estado;
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

    guardar(datos: GuardarUsuarioComando): void {
        if (this.usuarioEnEdicion) {
            const id = this.usuarioEnEdicion.id;
            // Array nuevo y objeto nuevo: la lista es OnPush y necesita ver el cambio.
            this.usuarios = this.usuarios.map(usuario =>
                usuario.id === id
                    ? { ...usuario, nombre: datos.nombre, correoElectronico: datos.correoElectronico, rol: datos.rol }
                    : usuario
            );
            this.avisar('Usuario actualizado', datos.nombre);
        } else {
            this.usuarios = [
                ...this.usuarios,
                {
                    id: crypto.randomUUID(),
                    nombre: datos.nombre,
                    correoElectronico: datos.correoElectronico,
                    rol: datos.rol,
                    estadoRegistro: 'A',
                    fechaCreacion: new Date().toISOString()
                }
            ];
            this.avisar('Usuario creado', datos.nombre);
        }

        this.mostrarFormulario = false;
        this.aplicarFiltros();
    }

    // -------------------------------------------------------------- eliminar

    confirmarEliminacion(usuario: UsuarioDto): void {
        this.confirmacion.confirm({
            header: 'Eliminar usuario',
            message: `Se eliminara a <b>${usuario.nombre}</b>. Podras verlo filtrando por eliminados.`,
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'Eliminar',
            rejectLabel: 'Cancelar',
            acceptButtonStyleClass: 'p-button-danger',
            rejectButtonStyleClass: 'p-button-text',
            accept: () => this.eliminar(usuario)
        });
    }

    /** Borrado logico, igual que en el backend: el registro no se pierde, se marca. */
    private eliminar(usuario: UsuarioDto): void {
        this.usuarios = this.usuarios.map(actual =>
            actual.id === usuario.id ? { ...actual, estadoRegistro: 'E' } : actual
        );
        this.aplicarFiltros();
        this.avisar('Usuario eliminado', usuario.nombre);
    }

    private avisar(titulo: string, detalle: string): void {
        this.mensajes.add({ severity: 'success', summary: titulo, detail: detalle, life: TOAST_LIFE });
    }
}
