import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { RolUsuario } from 'src/app/enums';
import { MiembroDto } from 'src/app/models';

/**
 * Presentacional. La tabla de miembros del proyecto.
 * Recibe la lista por @Input y avisa por @Output, no inyecta nada.
 */
@Component({
    selector: 'app-miembros-list',
    templateUrl: './miembros-list.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MiembrosListComponent {

    @Input() miembros: MiembroDto[] = [];
    @Input() cargando = false;

    @Output() quitar = new EventEmitter<MiembroDto>();

    /**
     * El backend responde 400 al quitar al ultimo: un proyecto sin nadie no le
     * aparece a nadie en su listado y quedaria inalcanzable. Se adelanta aqui
     * para no ofrecer un boton que solo puede fallar.
     */
    get esElUnicoMiembro(): boolean {
        return this.miembros.length === 1;
    }

    severidadRol(rol: RolUsuario): string {
        return rol === RolUsuario.Administrador ? 'success' : 'info';
    }
}
