import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { BreadcrumbModule } from 'primeng/breadcrumb';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { RippleModule } from 'primeng/ripple';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';

import { PageHeaderComponent } from './components/page-header/page-header.component';

/**
 * Todo lo que se repite en las pantallas de negocio.
 * Los modulos de PrimeNG se reexportan para no repetir la misma lista
 * de imports en cada modulo de pages/.
 */
@NgModule({
    declarations: [
        PageHeaderComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        BreadcrumbModule,
        ButtonModule,
        InputTextModule,
        RippleModule,
        TableModule,
        TagModule,
        TooltipModule
    ],
    exports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        BreadcrumbModule,
        ButtonModule,
        InputTextModule,
        RippleModule,
        TableModule,
        TagModule,
        TooltipModule,
        PageHeaderComponent
    ]
})
export class SharedModule { }
