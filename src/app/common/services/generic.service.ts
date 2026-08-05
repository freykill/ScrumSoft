import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom, Observable, timer } from 'rxjs';

/** Metodos HTTP soportados por GenericService. */
export enum METHODS {
    GET = 'get',
    POST = 'post',
    PUT = 'put',
    PATCH = 'patch',
    DELETE = 'delete',
    FILE = 'file'   // GET que devuelve blob (descargas)
}

/** Opciones adicionales por llamada. */
export interface RequestOptions {
    /** Sobrescribe el Content-Type (ej: 'application/x-www-form-urlencoded' en un login). */
    contentType?: string;
    /** Headers extra que se fusionan sobre los base. */
    headers?: Record<string, string>;
    /** Elimina del body las claves con null o string vacio. Por defecto no. */
    cleanBody?: boolean;
    /** Reintentos ante error transitorio. Por defecto 2 en GET/FILE, 0 en el resto. */
    retries?: number;
}

/** Error normalizado que devuelve GenericService al fallar una llamada. */
export interface HttpServiceError extends Error {
    status?: number;
    originalError: unknown;
    timestamp: number;
}

/** Solo se reintenta si el server nunca proceso la peticion. */
const RETRYABLE_STATUS = [0, 502, 503, 504];

@Injectable({ providedIn: 'root' })
export class GenericService {

    constructor(private readonly _http: HttpClient) { }

    /**
     * Ejecuta una llamada HTTP y devuelve la respuesta tipada como Promise.
     *
     * @param method      Metodo HTTP (ver METHODS).
     * @param endpoint    Url completa del recurso, normalmente sale de UrlServices.
     * @param body        Cuerpo para POST / PUT / PATCH / DELETE. Acepta FormData.
     * @param pathParam   Segmento que se concatena al endpoint (ej: el id).
     * @param queryParams Parametros de query.
     * @param options     Ver RequestOptions.
     */
    genericCallServices<T>(
        method: METHODS,
        endpoint: string,
        body: unknown = null,
        pathParam: string | number | null = null,
        queryParams: Map<string, any> | Record<string, any> | null = null,
        options?: RequestOptions
    ): Promise<T> {
        const url = this.buildUrl(endpoint, pathParam);
        const retries = options?.retries ?? (this.isIdempotent(method) ? 2 : 0);
        return this.execute<T>(method, url, body, queryParams, options, retries);
    }

    /**
     * Misma llamada pero devolviendo el Observable crudo: sin reintentos, pero
     * cancelable. Usarlo con switchMap en buscadores / autocompletes, donde la
     * peticion anterior debe morir cuando el usuario sigue escribiendo.
     */
    genericCallServices$<T>(
        method: METHODS,
        endpoint: string,
        body: unknown = null,
        pathParam: string | number | null = null,
        queryParams: Map<string, any> | Record<string, any> | null = null,
        options?: RequestOptions
    ): Observable<T> {
        const url = this.buildUrl(endpoint, pathParam);
        const payload = options?.cleanBody ? this.cleanBody(body) : body;
        return this.buildRequest<T>(method, url, payload, this.buildParams(queryParams), this.buildHeaders(body, options));
    }

    /** Bucle de reintentos. Cualquier fallo sale como HttpServiceError. */
    private async execute<T>(
        method: METHODS,
        url: string,
        body: unknown,
        queryParams: Map<string, any> | Record<string, any> | null,
        options: RequestOptions | undefined,
        retries: number
    ): Promise<T> {
        const params = this.buildParams(queryParams);
        const headers = this.buildHeaders(body, options);
        const payload = options?.cleanBody ? this.cleanBody(body) : body;

        let lastError: unknown;

        for (let attempt = 0; attempt <= retries; attempt++) {
            try {
                return await firstValueFrom(this.buildRequest<T>(method, url, payload, params, headers));
            } catch (error) {
                lastError = error;
                if (attempt === retries || !this.isRetryable(error)) {
                    break;
                }
                // Espera creciente antes del siguiente intento
                await firstValueFrom(timer(300 * (attempt + 1)));
            }
        }

        throw this.normalizeError(lastError);
    }

    private buildUrl(endpoint: string, pathParam: string | number | null): string {
        return pathParam !== null && pathParam !== undefined && pathParam !== ''
            ? `${endpoint}/${pathParam}`
            : endpoint;
    }

    private buildParams(queryParams: Map<string, any> | Record<string, any> | null): HttpParams {
        let params = new HttpParams();

        if (!queryParams) {
            return params;
        }

        const entries = queryParams instanceof Map
            ? Array.from(queryParams.entries())
            : Object.entries(queryParams);

        entries.forEach(([key, value]) => {
            // null / undefined no viajan, pero 0 y false si
            if (value !== null && value !== undefined) {
                params = params.append(key, String(value));
            }
        });

        return params;
    }

    private buildHeaders(body: unknown, options?: RequestOptions): HttpHeaders {
        let headers = new HttpHeaders().set('Accept', 'application/json');

        // Con FormData el Content-Type lo pone el navegador junto con el boundary,
        // si lo forzamos aqui el backend no sabe parsear el multipart.
        if (!(body instanceof FormData)) {
            headers = headers.set('Content-Type', options?.contentType ?? 'application/json');
        }

        if (options?.headers) {
            Object.entries(options.headers).forEach(([key, value]) => {
                headers = headers.set(key, value);
            });
        }

        return headers;
    }

    /** Quita del body las claves con null o string vacio. Solo si se pide con cleanBody. */
    private cleanBody(body: unknown): unknown {
        if (!body || typeof body !== 'object' || Array.isArray(body) || body instanceof FormData) {
            return body;
        }

        const result = { ...(body as Record<string, unknown>) };
        Object.keys(result).forEach(key => {
            const value = result[key];
            if (value === null || value === undefined || value === '') {
                delete result[key];
            }
        });

        return result;
    }

    private buildRequest<T>(method: METHODS, url: string, body: unknown, params: HttpParams, headers: HttpHeaders): Observable<T> {
        const opts = { params, headers };

        switch (method) {
            case METHODS.GET:
                return this._http.get<T>(url, opts);
            case METHODS.FILE:
                return this._http.get<T>(url, { ...opts, responseType: 'blob' as 'json' });
            case METHODS.POST:
                return this._http.post<T>(url, body, opts);
            case METHODS.PUT:
                return this._http.put<T>(url, body, opts);
            case METHODS.PATCH:
                return this._http.patch<T>(url, body, opts);
            // Algunos endpoints esperan body en el DELETE; si no hay, se manda sin cuerpo
            case METHODS.DELETE:
                return body === null || body === undefined
                    ? this._http.delete<T>(url, opts)
                    : this._http.delete<T>(url, { ...opts, body });
            default:
                throw new Error(`Metodo HTTP no soportado: ${method as string}`);
        }
    }

    /** GET y FILE no cambian estado, por eso se pueden reintentar sin duplicar nada. */
    private isIdempotent(method: METHODS): boolean {
        return method === METHODS.GET || method === METHODS.FILE;
    }

    private isRetryable(error: unknown): boolean {
        return error instanceof HttpErrorResponse && RETRYABLE_STATUS.includes(error.status);
    }

    private normalizeError(raw: unknown): HttpServiceError {
        const error = new Error() as HttpServiceError;

        if (raw instanceof HttpErrorResponse) {
            error.status = raw.status;
            error.message = raw.status === 0
                ? 'Problemas de conectividad, verifique su conexion a Internet'
                : this.extractServerMessage(raw) ?? this.mensajePorEstado(raw.status);
        } else if (raw instanceof Error) {
            error.message = raw.message;
        } else {
            error.message = 'Error desconocido';
        }

        error.originalError = raw;
        error.timestamp = Date.now();
        return error;
    }

    /**
     * Mensaje de respaldo cuando la respuesta de error no trae cuerpo.
     *
     * Pasa de verdad: el backend contesta 403 con Content-Length 0, y sin esto
     * el toast mostraba el texto crudo de Angular ("Http failure response for
     * https://...: 403 OK"), que ademas de no explicar nada acaba en "OK".
     */
    private mensajePorEstado(status: number): string {
        const mensajes: Record<number, string> = {
            400: 'Los datos enviados no son validos.',
            401: 'Tu sesion expiro, vuelve a iniciar sesion.',
            403: 'No tienes permiso para realizar esta accion.',
            404: 'No se encontro lo que buscabas.',
            409: 'Ese registro ya existe o esta en uso.',
            500: 'Error en el servidor. Intentalo de nuevo en un momento.'
        };

        return mensajes[status] ?? 'No se pudo completar la operacion.';
    }

    /** Intenta sacar el mensaje que manda el backend en vez del generico de Angular. */
    private extractServerMessage(raw: HttpErrorResponse): string | null {
        const payload = raw.error;

        if (typeof payload === 'string' && payload.trim()) {
            return payload;
        }

        if (payload && typeof payload === 'object') {
            // ProblemDetails de .NET: `detail` trae el mensaje concreto, `title` el generico
            const candidate = payload.detail ?? payload.message ?? payload.description ?? payload.title ?? payload.error;
            if (typeof candidate === 'string' && candidate.trim()) {
                return candidate;
            }
        }

        return null;
    }
}
