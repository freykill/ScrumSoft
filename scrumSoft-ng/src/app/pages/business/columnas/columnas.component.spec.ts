import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ColumnaDto } from 'src/app/models';
import { ColumnaService } from 'src/app/services';
import { ColumnasComponent } from './columnas.component';

/**
 * Reordenar columnas es el otro sitio donde se pinta antes de guardar. Cambia
 * la forma de recuperarse respecto al tablero: aqui no se vuelve a una foto
 * previa sino que se recarga, porque las flechas se pueden pulsar varias veces
 * seguidas y esa foto ya no seria el estado real.
 */
describe('ColumnasComponent, reordenar el flujo de trabajo', () => {

    let fixture: ComponentFixture<ColumnasComponent>;
    let componente: ColumnasComponent;
    let columnaService: jasmine.SpyObj<ColumnaService>;
    let mensajes: jasmine.SpyObj<MessageService>;

    beforeEach(() => {
        columnaService = jasmine.createSpyObj<ColumnaService>('ColumnaService', ['listar', 'reordenar']);
        mensajes = jasmine.createSpyObj<MessageService>('MessageService', ['add']);

        TestBed.configureTestingModule({
            declarations: [ColumnasComponent],
            providers: [
                { provide: ColumnaService, useValue: columnaService },
                { provide: MessageService, useValue: mensajes },
                { provide: ActivatedRoute, useValue: {} },
                { provide: Router, useValue: {} }
            ]
        });

        TestBed.overrideTemplate(ColumnasComponent, '');

        fixture = TestBed.createComponent(ColumnasComponent);
        componente = fixture.componentInstance;

        // Sin detectChanges: ngOnInit leeria la ruta y cargaria del servidor.
        componente.idProyecto = 'p1';
        componente.columnas = columnasDeEjemplo();
    });

    // subir() y bajar() no devuelven la promesa (los llama un (click) de la
    // plantilla), asi que esperarlas con await no sirve: hace falta fakeAsync
    // para vaciar lo que quede pendiente del servidor falso.
    it('manda los ids en el orden en que quedaron, no los numeros', fakeAsync(() => {
        columnaService.reordenar.and.resolveTo(columnasDeEjemplo());

        // 'Hecho' sube un puesto y se pone delante de 'Haciendo'
        componente.subir(2);
        tick();

        expect(columnaService.reordenar).toHaveBeenCalledWith({
            idProyecto: 'p1',
            idsEnOrden: ['a', 'c', 'b']
        });
    }));

    it('bajar mueve la columna hacia el final', fakeAsync(() => {
        columnaService.reordenar.and.resolveTo(columnasDeEjemplo());

        componente.bajar(0);
        tick();

        expect(columnaService.reordenar).toHaveBeenCalledWith(jasmine.objectContaining({
            idsEnOrden: ['b', 'a', 'c']
        }));
    }));

    it('renumera el orden por la posicion antes de que conteste el servidor', () => {
        // Promesa que no se resuelve: el componente queda en el instante en que
        // la flecha ya respondio pero el PUT sigue en vuelo.
        columnaService.reordenar.and.returnValue(new Promise<ColumnaDto[]>(() => { }));

        componente.subir(2);

        // El orden real es la posicion en la lista; el campo `orden` se
        // recalcula 1..n y no conserva el 1-2-3 que traia cada una.
        expect(componente.columnas.map(c => c.id)).toEqual(['a', 'c', 'b']);
        expect(componente.columnas.map(c => c.orden)).toEqual([1, 2, 3]);
    });

    it('pinta el orden que confirma el servidor y no el calculado en pantalla', fakeAsync(() => {
        // Llegan desordenadas y con otra numeracion a proposito: lo que manda
        // es el campo `orden` del servidor, no como venga el array.
        columnaService.reordenar.and.resolveTo([
            { id: 'c', nombre: 'Hecho', orden: 20 },
            { id: 'b', nombre: 'Haciendo', orden: 30 },
            { id: 'a', nombre: 'Pendiente', orden: 10 }
        ]);

        componente.subir(2);
        tick();

        expect(componente.columnas.map(c => c.id)).toEqual(['a', 'c', 'b']);
        expect(componente.columnas.map(c => c.orden)).toEqual([10, 20, 30]);
    }));

    it('recarga desde el servidor si no se pudo guardar el orden', fakeAsync(() => {
        columnaService.reordenar.and.rejectWith(new Error('No tienes permiso para realizar esta accion.'));
        columnaService.listar.and.resolveTo(columnasDeEjemplo());

        componente.subir(2);
        tick();

        // Ni se deja el orden nuevo pintado ni se restaura una foto local: se
        // vuelve a preguntar, que es lo unico cierto tras un fallo.
        expect(columnaService.listar).toHaveBeenCalledWith('p1');
        expect(componente.columnas.map(c => c.id)).toEqual(['a', 'b', 'c']);
        expect(mensajes.add).toHaveBeenCalledWith(jasmine.objectContaining({
            severity: 'error',
            detail: 'No tienes permiso para realizar esta accion.'
        }));
    }));

    // ------------------------------------------------------------- auxiliares

    function columnasDeEjemplo(): ColumnaDto[] {
        return [
            { id: 'a', nombre: 'Pendiente', orden: 1 },
            { id: 'b', nombre: 'Haciendo', orden: 2 },
            { id: 'c', nombre: 'Hecho', orden: 3 }
        ];
    }
});
