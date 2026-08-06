import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { EstadoProyecto, OPCIONES_ESTADO_PROYECTO } from 'src/app/enums';
import { GuardarProyectoComando, ProyectoDto } from 'src/app/models';
import { aFechaApi, aFechaLocal } from 'src/app/utilities';

/** La fecha de fin no puede ser anterior a la de inicio. */
function rangoDeFechasValido(grupo: AbstractControl): ValidationErrors | null {
    const inicio: Date | null = grupo.get('fechaInicio')?.value;
    const fin: Date | null = grupo.get('fechaFinPrevista')?.value;
    return inicio && fin && fin < inicio ? { rangoInvalido: true } : null;
}

/** Columnas con las que arranca un tablero nuevo si el usuario no las cambia. */
const COLUMNAS_POR_DEFECTO = ['Backlog', 'Por hacer', 'En progreso', 'Hecho'];

/**
 * Presentacional. El dialogo de alta / edicion de proyectos.
 *
 * El formulario cambia segun el modo, porque la API pide cosas distintas:
 * al crear se mandan las columnas iniciales del tablero (no hay estado, nace
 * en Planificacion) y al editar se manda el estado (las columnas ya se
 * administran desde su propia pantalla).
 */
@Component({
    selector: 'app-proyectos-form',
    templateUrl: './proyectos-form.component.html'
})
export class ProyectosFormComponent {

    /** null = alta, con valor = edicion. */
    @Input() proyecto: ProyectoDto | null = null;
    @Input() visible = false;
    @Input() guardando = false;

    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() guardar = new EventEmitter<GuardarProyectoComando>();

    readonly opcionesEstado = OPCIONES_ESTADO_PROYECTO;

    readonly form = this.fb.nonNullable.group({
        nombre: ['', [Validators.required, Validators.minLength(3)]],
        descripcion: [''],
        fechaInicio: [null as Date | null, Validators.required],
        fechaFinPrevista: [null as Date | null],
        estadoProyecto: [EstadoProyecto.Planificacion],
        columnas: [[] as string[]]
    }, { validators: rangoDeFechasValido });

    constructor(private readonly fb: FormBuilder) { }

    get esNuevo(): boolean {
        return this.proyecto === null;
    }

    get rangoInvalido(): boolean {
        return this.form.hasError('rangoInvalido') && this.form.controls.fechaFinPrevista.touched;
    }

    /** Se llama desde (onShow) del p-dialog, ver la nota en usuarios-form. */
    reiniciar(): void {
        // Los validadores se ajustan antes del reset: es el reset el que
        // recalcula la validez, si se hiciera al reves el formulario abriria
        // marcado con el estado del modo anterior.
        if (this.proyecto) {
            // En edicion las columnas no se tocan desde aqui, tienen su
            // propia pantalla, asi que el control deja de ser obligatorio.
            this.form.controls.columnas.clearValidators();

            this.form.reset({
                nombre: this.proyecto.nombre,
                descripcion: this.proyecto.descripcion ?? '',
                fechaInicio: aFechaLocal(this.proyecto.fechaInicio),
                fechaFinPrevista: aFechaLocal(this.proyecto.fechaFinPrevista),
                estadoProyecto: this.proyecto.estadoProyecto,
                columnas: []
            });
        } else {
            // Un tablero sin columnas no admite ni una tarea, y las columnas
            // solo se mandan en el alta: si se crea vacio hay que ir a la otra
            // pantalla antes de poder usarlo. Por eso se exige al menos una.
            this.form.controls.columnas.setValidators(Validators.required);

            this.form.reset({
                nombre: '',
                descripcion: '',
                fechaInicio: new Date(),
                fechaFinPrevista: null,
                estadoProyecto: EstadoProyecto.Planificacion,
                columnas: [...COLUMNAS_POR_DEFECTO]
            });
        }
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
            nombre: valores.nombre.trim(),
            descripcion: valores.descripcion.trim() || null,
            // El '!' es seguro: fechaInicio es required y el formulario ya paso la validacion.
            fechaInicio: aFechaApi(valores.fechaInicio)!,
            fechaFinPrevista: aFechaApi(valores.fechaFinPrevista),
            ...(this.esNuevo
                ? { columnas: valores.columnas }
                : { estadoProyecto: valores.estadoProyecto })
        });
    }

    cerrar(): void {
        this.visibleChange.emit(false);
    }
}
