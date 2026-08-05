import { Component, ElementRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { AuthService } from '../common/services';
import { LoginService } from '../services';
import { iniciales } from '../utilities';
import { LayoutService } from "./service/app.layout.service";

@Component({
    selector: 'app-topbar',
    templateUrl: './app.topbar.component.html'
})
export class AppTopBarComponent {

    items!: MenuItem[];

    @ViewChild('menubutton') menuButton!: ElementRef;

    @ViewChild('topbarmenubutton') topbarMenuButton!: ElementRef;

    @ViewChild('topbarmenu') menu!: ElementRef;

    /** Por ahora solo cerrar sesion; el menu deja sitio para lo que venga. */
    readonly opcionesUsuario: MenuItem[] = [
        {
            label: 'Cerrar sesion',
            icon: 'pi pi-sign-out',
            command: () => this.cerrarSesion()
        }
    ];

    constructor(
        public layoutService: LayoutService,
        private readonly auth: AuthService,
        private readonly loginService: LoginService,
        private readonly router: Router
    ) { }

    get nombre(): string {
        return this.auth.nombre ?? 'Sesion';
    }

    get rol(): string {
        return this.auth.rol ?? '';
    }

    get inicialesDelUsuario(): string {
        return iniciales(this.auth.nombre);
    }

    /**
     * El backend no tiene logout: el jwt no guarda estado, asi que basta con
     * borrar la sesion del navegador y volver al login.
     */
    private cerrarSesion(): void {
        this.loginService.logout();
        this.router.navigate(['/auth/login']);
    }
}
