import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { AuthService } from 'src/app/common/services';
import { Prioridad } from 'src/app/enums';
import { ColumnaConTareasDto, SoltarTarea, TareaDto } from 'src/app/models';
import {
    MiembroService,
    ProyectoService,
    ReporteService,
    TableroRealtimeService,
    TareaService
} from 'src/app/services';
import { TableroComponent } from './tablero.component';

/**
 * El arrastre es lo que mas partes moviles junta del tablero: deducir entre
 * que dos tareas quedo la tarjeta, pintarla ahi antes de guardar y saber
 * deshacerlo si el servidor dice que no.
 *
 * Se prueba desde el componente y no contra calcularVecinos suelto porque la
 * funcion aislada siempre acierta: lo que se rompe de verdad es la
 * coordinacion, y eso solo se ve pasando por soltar().
 */
describe('TableroComponent, arrastre de tarjetas', () => {

    let fixture: ComponentFixture<TableroComponent>;
    let componente: TableroComponent;
    let tareaService: jasmine.SpyObj<TareaService>;
    let mensajes: jasmine.SpyObj<MessageService>;

    beforeEach(() => {
        tareaService = jasmine.createSpyObj<TareaService>('TareaService', ['mover']);
        mensajes = jasmine.createSpyObj<MessageService>('MessageService', ['add']);

        TestBed.configureTestingModule({
            declarations: [TableroComponent],
            providers: [
                { provide: TareaService, useValue: tareaService },
                { provide: MessageService, useValue: mensajes },
                // Los demas no se tocan en estas pruebas, pero el constructor
                // los pide. Del de tiempo real si hace falta desconectar():
                // el componente cierra el socket al destruirse y TestBed
                // destruye el fixture al terminar cada it.
                { provide: TableroRealtimeService, useValue: { desconectar: () => Promise.resolve() } },
                { provide: ActivatedRoute, useValue: {} },
                { provide: Router, useValue: {} },
                { provide: ProyectoService, useValue: {} },
                { provide: MiembroService, useValue: {} },
                { provide: ReporteService, useValue: {} },
                { provide: AuthService, useValue: {} }
            ]
        });

        // Sin plantilla: lo que se prueba es la logica del contenedor, y
        // renderizarla arrastraria PrimeNG, el CDK y los hijos sin aportar nada.
        TestBed.overrideTemplate(TableroComponent, '');

        fixture = TestBed.createComponent(TableroComponent);
        componente = fixture.componentInstance;

        // A proposito no se llama a detectChanges: ngOnInit iria a buscar el
        // tablero al servidor. Se le pone el estado a mano y se dispara el
        // gesto, que es justo el escenario que interesa.
        componente.idProyecto = 'p1';
        componente.columnas = tableroDeEjemplo();
    });

    it('manda al servidor entre que dos tareas quedo la tarjeta', async () => {
        tareaService.mover.and.resolveTo(tarea('t1', 'haciendo', 1500));

        // t1 viene de otra columna y cae entre t4 y t5
        await componente.soltar(gestoDeSoltar('t1', 'haciendo', 1));

        expect(tareaService.mover).toHaveBeenCalledWith({
            idProyecto: 'p1',
            idTarea: 't1',
            idColumnaDestino: 'haciendo',
            idTareaAnterior: 't4',
            idTareaSiguiente: 't5'
        });
    });

    it('no cuenta a la propia tarjeta como vecina al moverla dentro de su columna', async () => {
        tareaService.mover.and.resolveTo(tarea('t1', 'pendiente', 2500));

        // t1 es la primera de 'pendiente' y baja al ultimo puesto: los vecinos
        // salen de [t2, t3]. Sin sacarla antes se compararia consigo misma y
        // acabaria un puesto mas arriba del que pidio el usuario.
        await componente.soltar(gestoDeSoltar('t1', 'pendiente', 2));

        expect(tareaService.mover).toHaveBeenCalledWith(jasmine.objectContaining({
            idTareaAnterior: 't3',
            idTareaSiguiente: null
        }));
    });

    it('pinta el movimiento sin esperar la respuesta del servidor', () => {
        // Una promesa que no se resuelve nunca deja al componente congelado en
        // el instante de despues del arrastre y antes de la confirmacion.
        tareaService.mover.and.returnValue(new Promise<TareaDto>(() => { }));

        componente.soltar(gestoDeSoltar('t1', 'haciendo', 1));

        expect(idsDe('haciendo')).toEqual(['t4', 't1', 't5']);
        expect(idsDe('pendiente')).toEqual(['t2', 't3']);
    });

    it('devuelve la tarjeta a su sitio si el servidor rechaza el movimiento', async () => {
        tareaService.mover.and.rejectWith(new Error('La columna destino no existe'));
        const antesDelArrastre = componente.columnas;

        await componente.soltar(gestoDeSoltar('t1', 'haciendo', 1));

        expect(componente.columnas).toBe(antesDelArrastre);
        expect(idsDe('pendiente')).toEqual(['t1', 't2', 't3']);
        expect(idsDe('haciendo')).toEqual(['t4', 't5']);

        // La reversion tiene que ser visible: mover algo y verlo volver sin
        // explicacion se lee como que la aplicacion se equivoco sola.
        expect(mensajes.add).toHaveBeenCalledWith(jasmine.objectContaining({
            severity: 'error',
            detail: 'La columna destino no existe'
        }));
    });

    it('se queda con el orden que asigno el servidor, no con el de la pantalla', async () => {
        // El backend numera de mil en mil: entre t4 (1000) y t5 (2000) le toca
        // el punto medio. Sin aplicarlo, el primer evento del hub reordenaria
        // por `orden` y la tarjeta saltaria a su sitio anterior.
        tareaService.mover.and.resolveTo(tarea('t1', 'haciendo', 1500));

        await componente.soltar(gestoDeSoltar('t1', 'haciendo', 1));

        expect(idsDe('haciendo')).toEqual(['t4', 't1', 't5']);
        expect(tareaDe('haciendo', 't1').orden).toBe(1500);
    });

    it('no molesta al servidor si la tarjeta se suelta donde ya estaba', async () => {
        await componente.soltar(gestoDeSoltar('t2', 'pendiente', 1));

        expect(tareaService.mover).not.toHaveBeenCalled();
    });

    // ------------------------------------------------------------- auxiliares

    /** pendiente: t1 t2 t3 | haciendo: t4 t5 */
    function tableroDeEjemplo(): ColumnaConTareasDto[] {
        return [
            columna('pendiente', 1, [
                tarea('t1', 'pendiente', 1000),
                tarea('t2', 'pendiente', 2000),
                tarea('t3', 'pendiente', 3000)
            ]),
            columna('haciendo', 2, [
                tarea('t4', 'haciendo', 1000),
                tarea('t5', 'haciendo', 2000)
            ])
        ];
    }

    function columna(id: string, orden: number, tareas: TareaDto[]): ColumnaConTareasDto {
        return { id, nombre: id, orden, tareas };
    }

    function tarea(id: string, idColumna: string, orden: number): TareaDto {
        return {
            id,
            titulo: `Tarea ${id}`,
            prioridad: Prioridad.Media,
            idColumna,
            orden,
            fechaCreacion: '2026-01-01T00:00:00Z'
        };
    }

    /**
     * Reproduce el evento que emite la columna cuando el CDK suelta una
     * tarjeta, sacando origen e indice del tablero actual igual que hace
     * TableroColumnaComponent.
     */
    function gestoDeSoltar(idTarea: string, idColumnaDestino: string, indiceDestino: number): SoltarTarea {
        const origen = componente.columnas.find(actual => actual.tareas.some(t => t.id === idTarea))!;

        return {
            tarea: origen.tareas.find(t => t.id === idTarea)!,
            idColumnaOrigen: origen.id,
            idColumnaDestino,
            indiceOrigen: origen.tareas.findIndex(t => t.id === idTarea),
            indiceDestino
        };
    }

    function idsDe(idColumna: string): string[] {
        return columnaDe(idColumna).tareas.map(t => t.id);
    }

    function tareaDe(idColumna: string, idTarea: string): TareaDto {
        return columnaDe(idColumna).tareas.find(t => t.id === idTarea)!;
    }

    function columnaDe(idColumna: string): ColumnaConTareasDto {
        return componente.columnas.find(actual => actual.id === idColumna)!;
    }
});
