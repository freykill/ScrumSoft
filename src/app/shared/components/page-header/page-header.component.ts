import { Component, Input } from '@angular/core';
import { MenuItem } from 'primeng/api';

/**
 * Cabecera estandar de las pantallas de negocio:
 * breadcrumb + tarjeta con titulo, descripcion y hueco para acciones.
 *
 * Las acciones se pasan por contenido proyectado:
 *   <app-page-header titulo="Usuarios" ...>
 *       <button pButton label="Nuevo"></button>
 *   </app-page-header>
 */
@Component({
    selector: 'app-page-header',
    templateUrl: './page-header.component.html'
})
export class PageHeaderComponent {

    @Input({ required: true }) titulo = '';
    @Input() descripcion = '';

    /** Migas de pan. El icono de casa se agrega solo y apunta a la raiz. */
    @Input() ruta: MenuItem[] = [];

    readonly inicio: MenuItem = { icon: 'pi pi-home', routerLink: '/' };
}
