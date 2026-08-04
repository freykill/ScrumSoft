import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { ChipsModule } from 'primeng/chips';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { PasswordModule } from 'primeng/password';
import { RippleModule } from 'primeng/ripple';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';

import { BusinessRoutingModule } from './business-routing.module';
import { ColumnasComponent } from './columnas/columnas.component';
import { ColumnasFormComponent } from './columnas/columnas-form/columnas-form.component';
import { ColumnasListComponent } from './columnas/columnas-list/columnas-list.component';
import { ProyectosComponent } from './proyectos/proyectos.component';
import { ProyectosFormComponent } from './proyectos/proyectos-form/proyectos-form.component';
import { ProyectosListComponent } from './proyectos/proyectos-list/proyectos-list.component';
import { UsuariosComponent } from './usuarios/usuarios.component';
import { UsuariosFormComponent } from './usuarios/usuarios-form/usuarios-form.component';
import { UsuariosListComponent } from './usuarios/usuarios-list/usuarios-list.component';

/**
 * Unico modulo de las pantallas de negocio.
 *
 * Los modulos de PrimeNG se importan uno por uno aqui: se lee la lista y se
 * sabe de que depende el modulo. No hay un SharedModule que reexporte una
 * bolsa de cosas ni modulos anidados por pantalla.
 */
@NgModule({
    declarations: [
        ProyectosComponent,
        ProyectosListComponent,
        ProyectosFormComponent,
        ColumnasComponent,
        ColumnasListComponent,
        ColumnasFormComponent,
        UsuariosComponent,
        UsuariosListComponent,
        UsuariosFormComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        BusinessRoutingModule,
        ButtonModule,
        CalendarModule,
        ChipsModule,
        ConfirmDialogModule,
        DialogModule,
        DropdownModule,
        InputTextModule,
        InputTextareaModule,
        PasswordModule,
        RippleModule,
        TableModule,
        TagModule,
        TooltipModule
    ]
})
export class BusinessModule { }
