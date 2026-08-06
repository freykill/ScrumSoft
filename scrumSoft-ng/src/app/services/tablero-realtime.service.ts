import { inject, Injectable, NgZone } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { AuthService, UrlServices } from '../common/services';
import { aPrioridad } from '../enums';
import { ColumnaDto, TareaDto, UsuarioConectado } from '../models';

/**
 * Canal de tiempo real del tablero (/hubs/tablero).
 *
 * No sustituye a ningun endpoint: el GET del tablero pinta la pantalla al
 * entrar y esto la mantiene fresca mientras el usuario esta dentro. Si el hub
 * no esta disponible, la pantalla sigue funcionando, solo deja de enterarse de
 * lo que hacen los demas hasta recargar.
 */
@Injectable({ providedIn: 'root' })
export class TableroRealtimeService {

    private readonly auth = inject(AuthService);
    private readonly urlService = inject(UrlServices);
    private readonly zone = inject(NgZone);

    private conexion?: HubConnection;
    /** A que tablero se esta escuchando; hace falta para resuscribirse. */
    private idProyectoActual: string | null = null;

    /** Sirve para las tres: creada, actualizada y movida. */
    readonly tareaCambiada = new Subject<TareaDto>();
    readonly tareaEliminada = new Subject<string>();
    readonly columnasCambiadas = new Subject<ColumnaDto[]>();

    /** Quien esta mirando este tablero. Solo llega cuando la lista cambia. */
    readonly usuariosConectados = new Subject<UsuarioConectado[]>();

    /**
     * Se volvio a conectar tras una caida. Hay un hueco: no existe historial
     * de eventos, asi que lo que paso mientras tanto se perdio y quien
     * escucha deberia recargar el tablero en vez de fiarse de lo que tiene.
     */
    readonly reconectado = new Subject<void>();

    /** Para el indicador de "en vivo" de la cabecera. */
    readonly estadoCambiado = new Subject<boolean>();

    /**
     * Abre la conexion si hace falta y entra al grupo del proyecto.
     * Conectarse no basta: sin suscribirse no llega ni un evento.
     */
    async conectar(idProyecto: string): Promise<void> {
        if (!this.conexion) {
            this.conexion = this.construir();
            this.registrarEventos(this.conexion);
            await this.conexion.start();
            this.emitir(this.estadoCambiado, true);
        }

        // Cambio de tablero: hay que salir del anterior o se seguirian
        // recibiendo sus eventos.
        if (this.idProyectoActual && this.idProyectoActual !== idProyecto) {
            await this.conexion.invoke('Desuscribirse', this.idProyectoActual);
        }

        this.idProyectoActual = idProyecto;
        await this.conexion.invoke('Suscribirse', idProyecto);
    }

    /** Se llama al salir de la pantalla del tablero. */
    async desconectar(): Promise<void> {
        if (!this.conexion) { return; }

        try {
            if (this.idProyectoActual) {
                await this.conexion.invoke('Desuscribirse', this.idProyectoActual);
            }
            await this.conexion.stop();
        } catch {
            // Si la conexion ya estaba caida no hay nada que cerrar; salir de
            // la pantalla no puede fallar por esto.
        }

        this.idProyectoActual = null;
        this.conexion = undefined;
        this.emitir(this.estadoCambiado, false);
    }

    private construir(): HubConnection {
        return new HubConnectionBuilder()
            .withUrl(this.urlService.urlHubTablero, {
                // Un WebSocket no puede mandar cabeceras, asi que el token va
                // en la cadena de consulta. Sale de AuthService y no de
                // localStorage a mano: la sesion puede estar en sessionStorage
                // si no se marco "recordarme", y la clave no es 'token'.
                accessTokenFactory: () => this.auth.token ?? ''
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Warning)
            .build();
    }

    private registrarEventos(conexion: HubConnection): void {
        conexion.on('TareaCreada', carga => this.emitir(this.tareaCambiada, this.aTarea(carga)));
        conexion.on('TareaActualizada', carga => this.emitir(this.tareaCambiada, this.aTarea(carga)));
        conexion.on('TareaMovida', carga => this.emitir(this.tareaCambiada, this.aTarea(carga)));
        conexion.on('TareaEliminada', (id: string) => this.emitir(this.tareaEliminada, id));
        conexion.on('ColumnasActualizadas', (columnas: ColumnaDto[]) =>
            this.emitir(this.columnasCambiadas, columnas ?? []));
        conexion.on('UsuariosConectados', (usuarios: UsuarioConectado[]) =>
            this.emitir(this.usuariosConectados, usuarios ?? []));

        conexion.onreconnecting(() => this.emitir(this.estadoCambiado, false));

        conexion.onreconnected(async () => {
            // El servidor no recuerda los grupos de una conexion caida: si no
            // se vuelve a entrar, el socket queda vivo y mudo.
            if (this.idProyectoActual) {
                await conexion.invoke('Suscribirse', this.idProyectoActual);
            }
            this.emitir(this.estadoCambiado, true);
            this.emitir(this.reconectado, undefined);
        });

        conexion.onclose(() => this.emitir(this.estadoCambiado, false));
    }

    /** La prioridad llega como numero por el hub; ver aPrioridad(). */
    private aTarea(carga: TareaDto): TareaDto {
        return { ...carga, prioridad: aPrioridad(carga?.prioridad) };
    }

    /**
     * Emitir dentro de la zona de Angular para que la vista se repinte.
     *
     * Hoy es redundante: zone.js parchea WebSocket, EventSource y XHR, que son
     * los tres transportes que puede negociar SignalR, asi que los callbacks
     * ya caen dentro de la zona. Se deja porque no cuesta nada y porque el dia
     * que no fuera cierto (otro transporte, o migrar a zoneless) el sintoma
     * seria "la vista no se actualiza" y es dificilisimo de atribuir.
     */
    private emitir<T>(sujeto: Subject<T>, valor: T): void {
        this.zone.run(() => sujeto.next(valor));
    }
}
