import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { OPCIONES_ROL, RolUsuario } from 'src/app/enums';
import { GuardarUsuarioComando, UsuarioDto } from 'src/app/models';

/**
 * Presentacional. El dialogo de alta / edicion.
 *
 * Tiene su propio FormGroup porque la validacion es asunto del formulario,
 * pero no guarda nada: emite el comando y el contenedor decide que hacer.
 */
@Component({
    selector: 'app-usuarios-form',
    templateUrl: './usuarios-form.component.html'
})
export class UsuariosFormComponent {

    /** null = alta, con valor = edicion. */
    @Input() usuario: UsuarioDto | null = null;
    @Input() visible = false;
    @Input() guardando = false;

    @Output() visibleChange = new EventEmitter<boolean>();
    @Output() guardar = new EventEmitter<GuardarUsuarioComando>();

    readonly opcionesRol = OPCIONES_ROL;

    readonly form = this.fb.nonNullable.group({
        nombre: ['', [Validators.required, Validators.minLength(3)]],
        correoElectronico: ['', [Validators.required, Validators.email]],
        rol: [RolUsuario.Miembro, Validators.required],
        clave: ['', [Validators.required, Validators.minLength(6)]]
    });

    constructor(private readonly fb: FormBuilder) { }

    get esNuevo(): boolean {
        return this.usuario === null;
    }

    /**
     * Se llama desde (onShow) del p-dialog y no desde un setter de @Input:
     * al abrir dos veces seguidas en modo "nuevo", `usuario` sigue siendo null
     * y Angular no volveria a disparar el setter, asi que el formulario
     * quedaria con lo que se escribio la vez anterior.
     */
    reiniciar(): void {
        if (this.usuario) {
            this.form.reset({
                nombre: this.usuario.nombre,
                correoElectronico: this.usuario.correoElectronico,
                rol: this.usuario.rol,
                clave: ''
            });
            // En edicion la clave es opcional: vacia significa "no la cambies".
            this.form.controls.clave.removeValidators(Validators.required);
        } else {
            this.form.reset({ nombre: '', correoElectronico: '', rol: RolUsuario.Miembro, clave: '' });
            this.form.controls.clave.addValidators(Validators.required);
        }
        this.form.controls.clave.updateValueAndValidity();
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
        const { nombre, correoElectronico, rol, clave } = this.form.getRawValue();
        this.guardar.emit({
            nombre: nombre.trim(),
            correoElectronico: correoElectronico.trim().toLowerCase(),
            rol,
            // En edicion, si no escribio clave no se manda el campo.
            clave: clave ? clave : undefined
        });
    }

    cerrar(): void {
        this.visibleChange.emit(false);
    }
}
