import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { OPCIONES_PRIORIDAD, Prioridad } from 'src/app/enums';
import { GuardarTareaComando, TareaDto } from 'src/app/models';

/**
 * Presentacional. Alta y edicion de una tarea.
 *
 * No tiene selector de columna a proposito: ActualizarTareaComando no lleva
 * idColumna, o sea que el PUT no puede mover una tarea. Al crear la columna la
 * decide el boton `+` de esa columna, y al editar se cambia arrastrando. Un
 * desplegable aqui mentiria, o obligaria a lanzar dos peticiones por un solo
 * guardar y dejaria la tarea editada pero sin mover si la segunda falla.
 */
@Component({
    selector: 'app-tablero-form',
    templateUrl: './tablero-form.component.html'
})
export class TableroFormComponent {

    /** null = alta, con valor = edicion. */
    @Input() tarea: TareaDto | null = null;
    /** Solo para decir en el dialogo en que columna cae. */
    @Input() nombreColumna = '';
    @Input() visible = false;
    @Input() guardando = false;

    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() guardar = new EventEmitter<GuardarTareaComando>();
    @Output() eliminar = new EventEmitter<TareaDto>();

    readonly opcionesPrioridad = OPCIONES_PRIORIDAD;

    readonly form = this.fb.nonNullable.group({
        titulo: ['', [Validators.required, Validators.maxLength(150)]],
        descripcion: [''],
        prioridad: [Prioridad.Media, Validators.required]
    });

    constructor(private readonly fb: FormBuilder) { }

    get esNueva(): boolean {
        return this.tarea === null;
    }

    /** Se llama desde (onShow) del p-dialog, ver la nota en usuarios-form. */
    reiniciar(): void {
        this.form.reset({
            titulo: this.tarea?.titulo ?? '',
            descripcion: this.tarea?.descripcion ?? '',
            // Media por defecto: es la que menos afirma cuando aun no se sabe.
            prioridad: this.tarea?.prioridad ?? Prioridad.Media
        });
    }

    invalido(control: keyof typeof this.form.controls): boolean {
        const campo = this.form.controls[control];
        return campo.invalid && (campo.dirty || campo.touched);
    }

    enviar(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }
        const valores = this.form.getRawValue();

        this.guardar.emit({
            titulo: valores.titulo.trim(),
            descripcion: valores.descripcion.trim() || null,
            prioridad: valores.prioridad
        });
    }

    cerrar(): void {
        this.visibleChange.emit(false);
    }
}
