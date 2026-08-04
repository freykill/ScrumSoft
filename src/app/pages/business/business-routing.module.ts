import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ColumnasComponent } from './columnas/columnas.component';
import { ProyectosComponent } from './proyectos/proyectos.component';
import { UsuariosComponent } from './usuarios/usuarios.component';

/**
 * Columnas cuelga del proyecto porque en la API no existe fuera de el:
 * todos sus endpoints son /proyectos/{idProyecto}/columnas/...
 * El tablero seguira la misma forma: /proyectos/:idProyecto/tablero.
 */
const routes: Routes = [
    { path: 'proyectos', component: ProyectosComponent },
    { path: 'proyectos/:idProyecto/columnas', component: ColumnasComponent },
    { path: 'usuarios', component: UsuariosComponent },
    { path: '', redirectTo: 'proyectos', pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class BusinessRoutingModule { }
