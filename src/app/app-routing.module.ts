import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { NotfoundComponent } from './pages/notfound/notfound.component';
import { AppLayoutComponent } from "./layout/app.layout.component";
import { authGuard, invitadoGuard } from './common/guards';

@NgModule({
    imports: [
        RouterModule.forRoot([
            {
                // El guard va en el layout y no en cada pantalla: todo lo que
                // cuelga de aqui es privado, y asi no hay forma de olvidarlo al
                // anadir una ruta nueva.
                path: '', component: AppLayoutComponent, canActivate: [authGuard],
                children: [
                    // No hay dashboard: la aplicacion entra directo a proyectos,
                    // que es desde donde se llega a los tableros.
                    { path: '', redirectTo: 'business/proyectos', pathMatch: 'full' },
                    { path: 'business', loadChildren: () => import('./pages/business/business.module').then(m => m.BusinessModule) }
                ]
            },
            // Con sesion abierta el login sobra: el guard devuelve al layout.
            {
                path: 'auth', canActivate: [invitadoGuard],
                loadChildren: () => import('./pages/auth/auth.module').then(m => m.AuthModule)
            },
            { path: 'notfound', component: NotfoundComponent },
            { path: '**', redirectTo: '/notfound' },
        ], { scrollPositionRestoration: 'enabled', anchorScrolling: 'enabled', onSameUrlNavigation: 'reload' })
    ],
    exports: [RouterModule]
})
export class AppRoutingModule {
}
