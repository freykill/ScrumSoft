import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { LayoutService } from 'src/app/layout/service/app.layout.service';
import { LoginService } from 'src/app/services';
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
    private readonly loginService = inject(LoginService);
    private readonly messageService = inject(MessageService);
    readonly layoutService = inject(LayoutService);

    readonly form = this.fb.nonNullable.group({
        correo: ['', [Validators.required, Validators.email]],
        clave: ['', [Validators.required, Validators.minLength(4)]],
        recordarme: [false]
    });

    /** Loader puntual: solo bloquea el boton, no la pantalla. */
    cargando = false;
    error: string | null = null;

    /** Usuarios sembrados por el backend para probar. QUITAR ANTES DE PRODUCCION. */
    readonly cuentasDemo = [
        { correo: 'admin@scrumsoft.com', clave: 'Admin123*', rol: RolUsuario.Administrador },
        { correo: 'miembro@scrumsoft.com', clave: 'Miembro123*', rol: RolUsuario.Miembro }
    ];

    get correo() {
        return this.form.controls.correo;
    }

    get clave() {
        return this.form.controls.clave;
    }

    /** Rellena el formulario con la cuenta elegida y entra de una vez. */
    entrarConDemo(cuenta: { correo: string; clave: string }): Promise<void> {
        this.form.patchValue({ correo: cuenta.correo, clave: cuenta.clave });
        this.error = null;
        return this.iniciarSesion();
    }

    async iniciarSesion(): Promise<void> {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.cargando = true;
        this.error = null;

        try {
            const { correo, clave, recordarme } = this.form.getRawValue();

            // El service hace el POST y deja la sesion guardada
            const resp = await this.loginService.login({
                correoElectronico: correo,
                contrasena: clave
            }, recordarme);

            if(!resp.token){
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo iniciar sesion, intenta de nuevo' });
                return;
            }

            // El authGuard manda aqui el returnUrl para devolver al usuario a donde iba
            const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
            await this.router.navigateByUrl(returnUrl);
        } catch (e: any) {
            this.error = e?.status === 403 || e?.status === 400
                ? 'Correo o contrasena incorrectos'
                : e?.message ?? 'No se pudo iniciar sesion, intenta de nuevo';
        } finally {
            this.cargando = false;
        }
    }
}
