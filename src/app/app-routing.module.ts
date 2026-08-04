import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { NotfoundComponent } from './pages/notfound/notfound.component';
import { AppLayoutComponent } from "./layout/app.layout.component";

@NgModule({
    imports: [
        RouterModule.forRoot([
            {
                // TODO: falta reponer `canActivate: [authGuard]` (import en
                // ./common/guards). Sin el, se entra al layout sin sesion.
                // El requisito 6.2 pide guardia de ruta, no se puede entregar asi.
                path: '', component: AppLayoutComponent,
                children: [
                    // No hay dashboard: la aplicacion entra directo a proyectos,
                    // que es desde donde se llega a los tableros.
                    { path: '', redirectTo: 'business/proyectos', pathMatch: 'full' },
                    { path: 'business', loadChildren: () => import('./pages/business/business.module').then(m => m.BusinessModule) }
                ]
            },
            { path: 'auth', loadChildren: () => import('./pages/auth/auth.module').then(m => m.AuthModule) },
            { path: 'notfound', component: NotfoundComponent },
            { path: '**', redirectTo: '/notfound' },
        ], { scrollPositionRestoration: 'enabled', anchorScrolling: 'enabled', onSameUrlNavigation: 'reload' })
    ],
    exports: [RouterModule]
})
export class AppRoutingModule {
}
