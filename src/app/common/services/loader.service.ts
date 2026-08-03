import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

/**
 * Indicador de carga global.
 * Lleva un contador para que varias peticiones simultaneas no apaguen
 * el loader cuando termina la primera.
 */
@Injectable({ providedIn: 'root' })
export class LoaderService {

    private pending = 0;
    private readonly _loading = new BehaviorSubject<boolean>(false);

    /** Suscribirse desde el layout para pintar el spinner. */
    readonly loading$ = this._loading.asObservable();

    get isLoading(): boolean {
        return this._loading.value;
    }

    show(): void {
        this.pending++;
        if (this.pending === 1) {
            this._loading.next(true);
        }
    }

    hide(): void {
        if (this.pending === 0) {
            return;
        }
        this.pending--;
        if (this.pending === 0) {
            this._loading.next(false);
        }
    }

    /** Apaga el loader pase lo que pase (util al cambiar de ruta). */
    reset(): void {
        this.pending = 0;
        this._loading.next(false);
    }
}
