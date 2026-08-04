import { OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { LayoutService } from './service/app.layout.service';

@Component({
    selector: 'app-menu',
    templateUrl: './app.menu.component.html'
})
export class AppMenuComponent implements OnInit {

    model: any[] = [];

    constructor(public layoutService: LayoutService) { }

    ngOnInit() {
        // Solo las pantallas que la aplicacion usa de verdad. Las secciones de
        // demo que trae Sakai (UI Components, Prime Blocks, Hierarchy, Get
        // Started) se quitaron: no son parte del producto y solo ensucian la
        // navegacion. La configuracion del tema no vive aqui, esta en el
        // engranaje del topbar (AppConfigComponent).
        this.model = [
            {
                label: 'Gestion',
                items: [
                    // Columnas y tablero no van aqui: cuelgan de un proyecto
                    // (/business/proyectos/:id/...) y se entra desde su fila.
                    { label: 'Proyectos', icon: 'pi pi-fw pi-briefcase', routerLink: ['/business/proyectos'] },
                    { label: 'Usuarios', icon: 'pi pi-fw pi-users', routerLink: ['/business/usuarios'] }
                ]
            }
        ];
    }
}
