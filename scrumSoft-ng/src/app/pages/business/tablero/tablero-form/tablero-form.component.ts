import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { OPCIONES_PRIORIDAD, Prioridad } from 'src/app/enums';
import { GuardarTareaComando, MiembroDto, TareaDto } from 'src/app/models';

/** Una opcion del desplegable de responsable. */
interface OpcionResponsable {
    idUsuario: string;
    nombre: string;
}

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
    /**
     * Los del proyecto, no todos los usuarios del sistema: el backend exige
     * que el responsable pertenezca al equipo y si no responde 400.
     */
    @Input() miembros: MiembroDto[] = [];
    @Input() visible = false;
    @Input() guardando = false;

    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() guardar = new EventEmitter<GuardarTareaComando>();
    @Output() eliminar = new EventEmitter<TareaDto>();

    readonly opcionesPrioridad = OPCIONES_PRIORIDAD;

    /** Se arma al abrir, no en un getter: un getter daria un array nuevo en cada ciclo. */
    opcionesResponsable: OpcionResponsable[] = [];

    readonly form = this.fb.nonNullable.group({
        titulo: ['', [Validators.required, Validators.maxLength(150)]],
        descripcion: [''],
        prioridad: [Prioridad.Media, Validators.required],
        idResponsable: [null as string | null]
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
            prioridad: this.tarea?.prioridad ?? Prioridad.Media,
            idResponsable: this.tarea?.idResponsable ?? null
        });

        this.opcionesResponsable = this.construirOpciones();
    }

    /**
     * El backend deja guardar una tarea sin tocar su responsable aunque esa
     * persona ya no este en el equipo. Si no se ofreciera como opcion, el
     * desplegable saldria vacio y pareceria que la tarea no tiene a nadie.
     */
    private construirOpciones(): OpcionResponsable[] {
        const opciones: OpcionResponsable[] = this.miembros.map(miembro => ({
            idUsuario: miembro.idUsuario,
            nombre: miembro.nombre
        }));

        const actual = this.tarea?.idResponsable;
        if (actual && !opciones.some(opcion => opcion.idUsuario === actual)) {
            opciones.push({ idUsuario: actual, nombre: 'Ya no es miembro del proyecto' });
        }

        return opciones;
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
            prioridad: valores.prioridad,
            // Siempre viaja: si se omitiera, el PUT dejaria la tarea sin responsable.
            idResponsable: valores.idResponsable ?? null
        });
    }

    cerrar(): void {
        this.visibleChange.emit(false);
    }
}
