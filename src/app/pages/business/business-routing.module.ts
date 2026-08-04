import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ColumnasComponent } from './columnas/columnas.component';
import { ProyectosComponent } from './proyectos/proyectos.component';
import { TareasComponent } from './tareas/tareas.component';
import { UsuariosComponent } from './usuarios/usuarios.component';

const routes: Routes = [
    { path: 'proyectos', component: ProyectosComponent },
    { path: 'columnas', component: ColumnasComponent },
    { path: 'tareas', component: TareasComponent },
    { path: 'usuarios', component: UsuariosComponent },
    { path: '', redirectTo: 'proyectos', pathMatch: 'full' }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class BusinessRoutingModule { }
