import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';

import { BusinessRoutingModule } from './business-routing.module';
import { ColumnasComponent } from './columnas/columnas.component';
import { ProyectosComponent } from './proyectos/proyectos.component';
import { TareasComponent } from './tareas/tareas.component';
import { UsuariosComponent } from './usuarios/usuarios.component';

@NgModule({
    declarations: [
        ProyectosComponent,
        ColumnasComponent,
        TareasComponent,
        UsuariosComponent
    ],
    imports: [
        SharedModule,
        BusinessRoutingModule
    ]
})
export class BusinessModule { }
