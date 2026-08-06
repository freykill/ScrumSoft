import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ColumnaDto } from 'src/app/models';

/**
 * Presentacional. Alta y renombrado de una columna.
 *
 * Emite solo el nombre y no un comando: tanto AgregarColumnaComando como
 * RenombrarColumnaComando llevan ademas los ids, y esos los pone el
 * contenedor, que es quien sabe en que proyecto y en que columna esta.
 */
@Component({
    selector: 'app-columnas-form',
    templateUrl: './columnas-form.component.html'
})
export class ColumnasFormComponent {

    /** null = alta, con valor = renombrado. */
    @Input() columna: ColumnaDto | null = null;
    @Input() visible = false;
    @Input() guardando = false;

    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() guardar = new EventEmitter<string>();

    readonly form = this.fb.nonNullable.group({
        nombre: ['', [Validators.required, Validators.maxLength(60)]]
    });

    constructor(private readonly fb: FormBuilder) { }

    get esNueva(): boolean {
        return this.columna === null;
    }

    /** Se llama desde (onShow) del p-dialog, ver la nota en usuarios-form. */
    reiniciar(): void {
        this.form.reset({ nombre: this.columna?.nombre ?? '' });
    }

    get invalido(): boolean {
        const campo = this.form.controls.nombre;
        return campo.invalid && (campo.dirty || campo.touched);
    }

    enviar(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }
        this.guardar.emit(this.form.getRawValue().nombre.trim());
    }

    cerrar(): void {
        this.visibleChange.emit(false);
    }
}
