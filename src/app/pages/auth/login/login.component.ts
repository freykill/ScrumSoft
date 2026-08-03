import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { AuthService } from 'src/app/common/services/auth.service';
import { RolUsuario } from 'src/app/enums';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrl: './login.component.scss'
})
export class LoginComponent {

    private readonly fb = inject(FormBuilder);
    private readonly router = inject(Router);
    private readonly route = inject(ActivatedRoute);
    private readonly auth = inject(AuthService);
    readonly layoutService = inject(LayoutService);

    readonly form = this.fb.nonNullable.group({
        correo: ['', [Validators.required, Validators.email]],
        clave: ['', [Validators.required, Validators.minLength(4)]],
        recordarme: [false]
    });

    /** Loader puntual: solo bloquea el boton, no la pantalla. */
    cargando = false;
    error: string | null = null;

    get correo() {
        return this.form.controls.correo;
    }

    get clave() {
        return this.form.controls.clave;
    }

    async iniciarSesion(): Promise<void> {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.cargando = true;
        this.error = null;

        try {
            const { correo, recordarme } = this.form.getRawValue();

            // TODO: reemplazar por el POST /api/v1/auth/login real. Por ahora se
            // fabrica una sesion falsa para poder probar el guard y el interceptor.
            // QUITAR ANTES DE DESPLEGAR.
            this.auth.guardarSesion({
                token: 'token-de-prueba',
                expiraEn: new Date(Date.now() + 60 * 60 * 1000).toISOString(),
                idUsuario: '00000000-0000-0000-0000-000000000000',
                nombre: correo,
                rol: RolUsuario.Administrador
            }, recordarme);

            // El authGuard manda aqui el returnUrl para devolver al usuario a donde iba
            const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
            await this.router.navigateByUrl(returnUrl);
        } catch (e: any) {
            this.error = e?.message ?? 'No se pudo iniciar sesion, intenta de nuevo';
        } finally {
            this.cargando = false;
        }
    }
}
