import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ColumnasComponent } from './columnas/columnas.component';
import { ProyectosComponent } from './proyectos/proyectos.component';
import { TableroComponent } from './tablero/tablero.component';
import { UsuariosComponent } from './usuarios/usuarios.component';

/**
 * Columnas y tablero cuelgan del proyecto porque en la API no existen fuera
 * de el: todos sus endpoints son /proyectos/{idProyecto}/...
 */
const routes: Routes = [
    { path: 'proyectos', component: ProyectosComponent },
    { path: 'proyectos/:idProyecto/columnas', component: ColumnasComponent },
    { path: 'proyectos/:idProyecto/tablero', component: TableroComponent },
    { path: 'usuarios', component: UsuariosComponent },
    { path: '', redirectTo: 'proyectos', pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class BusinessRoutingModule { }
