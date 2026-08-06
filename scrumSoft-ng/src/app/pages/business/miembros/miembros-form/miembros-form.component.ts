import { Component, EventEmitter, Input, Output } from '@angular/core';
import { UsuarioDto } from 'src/app/models';

/**
 * Presentacional. El dialogo para agregar un miembro al proyecto.
 *
 * No es un alta de usuario: se busca a alguien que ya existe y se manda su id.
 * La busqueda la hace el contenedor, aqui solo se emite el texto y se pintan
 * las sugerencias que llegan por @Input, para no inyectar ningun servicio.
 */
@Component({
    selector: 'app-miembros-form',
    templateUrl: './miembros-form.component.html'
})
export class MiembrosFormComponent {

    @Input() visible = false;
    @Input() guardando = false;
    /** Resultados de la ultima busqueda, ya sin los que son miembros. */
    @Input() sugerencias: UsuarioDto[] = [];

    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() buscar = new EventEmitter<string>();
    @Output() agregar = new EventEmitter<UsuarioDto>();

    /** Lo que hay escrito o elegido en el autocompletar. */
    seleccionado: UsuarioDto | string | null = null;

    /** Se llama desde (onShow) del p-dialog, ver la nota en usuarios-form. */
    reiniciar(): void {
        this.seleccionado = null;
    }

    /**
     * Solo vale si se eligio una sugerencia. Mientras haya texto suelto el
     * valor es un string y todavia no hay ningun usuario que mandar.
     */
    get usuarioElegido(): UsuarioDto | null {
        return this.seleccionado && typeof this.seleccionado !== 'string'
            ? this.seleccionado
            : null;
    }

    enviar(): void {
        const usuario = this.usuarioElegido;
        if (usuario) {
            this.agregar.emit(usuario);
        }
    }

    cerrar(): void {
        this.visibleChange.emit(false);
    }
}
