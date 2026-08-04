import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { OPCIONES_PAGINA, PAGINACION } from 'src/app/config/app.constants';
import { RolUsuario } from 'src/app/enums';
import { UsuarioDto } from 'src/app/models';

/**
 * Presentacional. Solo pinta la tabla.
 *
 * No inyecta servicios, no sabe de donde salen los usuarios ni que pasa
 * cuando se pulsa un boton: recibe la lista por @Input y avisa por @Output.
 * Por eso puede ir en OnPush, el contenedor siempre le pasa un array nuevo.
 */
@Component({
    selector: 'app-usuarios-list',
    templateUrl: './usuarios-list.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class UsuariosListComponent {

    @Input() usuarios: UsuarioDto[] = [];
    @Input() cargando = false;

    @Output() editar = new EventEmitter<UsuarioDto>();
    @Output() eliminar = new EventEmitter<UsuarioDto>();

    readonly filasPorPagina = PAGINACION.LIMIT;
    readonly opcionesPagina = OPCIONES_PAGINA;

    severidadRol(rol: RolUsuario): string {
        return rol === RolUsuario.Administrador ? 'success' : 'info';
    }

    esActivo(usuario: UsuarioDto): boolean {
        return usuario.estadoRegistro === 'A';
    }
}
