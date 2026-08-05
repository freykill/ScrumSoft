import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { OPCIONES_ROL, RolUsuario } from 'src/app/enums';
import { GuardarUsuarioComando, UsuarioDto } from 'src/app/models';

/**
 * Presentacional. El dialogo de alta / edicion.
 *
 * El formulario cambia segun el modo porque la API pide cosas distintas:
 *   POST  nombre, correoElectronico, contrasena, rol
 *   PUT   nombre, rol  (y nada mas)
 *
 * Asi que al editar, correo y contrasena ni se muestran: el correo es con lo
 * que se inicia sesion y no se puede cambiar, y no existe endpoint de cambio
 * de clave. Un campo ahi que no hiciera nada seria peor que no tenerlo.
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
        contrasena: ['', [Validators.required, Validators.minLength(6)]],
        rol: [RolUsuario.Miembro, Validators.required]
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
        const correo = this.form.controls.correoElectronico;
        const contrasena = this.form.controls.contrasena;

        // Los validadores se ajustan antes del reset: es el reset el que
        // recalcula la validez.
        if (this.usuario) {
            // En edicion no viajan, asi que no pueden bloquear el guardado.
            correo.clearValidators();
            contrasena.clearValidators();

            this.form.reset({
                nombre: this.usuario.nombre,
                correoElectronico: this.usuario.correoElectronico,
                contrasena: '',
                rol: this.usuario.rol
            });
        } else {
            correo.setValidators([Validators.required, Validators.email]);
            contrasena.setValidators([Validators.required, Validators.minLength(6)]);

            this.form.reset({
                nombre: '',
                correoElectronico: '',
                contrasena: '',
                rol: RolUsuario.Miembro
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
            rol: valores.rol,
            ...(this.esNuevo
                ? {
                    correoElectronico: valores.correoElectronico.trim().toLowerCase(),
                    contrasena: valores.contrasena
                }
                : {})
        });
    }

    cerrar(): void {
        this.visibleChange.emit(false);
    }
}
