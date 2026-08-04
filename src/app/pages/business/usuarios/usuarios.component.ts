import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { RolUsuario } from 'src/app/enums';
import { UsuarioDto } from 'src/app/models';

@Component({
    selector: 'app-usuarios',
    templateUrl: './usuarios.component.html'
})
export class UsuariosComponent {

    readonly ruta: MenuItem[] = [{ label: 'Administracion' }, { label: 'Usuarios' }];

    /** MOCK. Reemplazar por UsuarioService cuando la API exponga /usuarios. */
    usuarios: UsuarioDto[] = [
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

    severidadRol(rol: RolUsuario): string {
        return rol === RolUsuario.Administrador ? 'success' : 'info';
    }

    esActivo(usuario: UsuarioDto): boolean {
        return usuario.estadoRegistro === 'A';
    }
}
