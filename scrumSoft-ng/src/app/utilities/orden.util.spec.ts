import { calcularVecinos, moverElemento, renumerarOrden } from './orden.util';

/**
 * El calculo de vecinos es la pieza con mas riesgo del tablero: el backend no
 * recibe un indice sino con quien queda pegada la tarea, y equivocarse deja
 * las tarjetas en un sitio distinto del que el usuario solto.
 */
describe('calcularVecinos', () => {

    const COLUMNA = ['a', 'b', 'c'];

    describe('soltando una tarea que viene de otra columna', () => {

        it('al principio deja el hueco de arriba vacio', () => {
            expect(calcularVecinos(COLUMNA, 'nueva', 0))
                .toEqual({ idTareaAnterior: null, idTareaSiguiente: 'a' });
        });

        it('en medio deja las dos tareas que la rodean', () => {
            expect(calcularVecinos(COLUMNA, 'nueva', 2))
                .toEqual({ idTareaAnterior: 'b', idTareaSiguiente: 'c' });
        });

        it('al final deja el hueco de abajo vacio', () => {
            expect(calcularVecinos(COLUMNA, 'nueva', 3))
                .toEqual({ idTareaAnterior: 'c', idTareaSiguiente: null });
        });

        it('en una columna vacia no tiene vecinos', () => {
            expect(calcularVecinos([], 'nueva', 0))
                .toEqual({ idTareaAnterior: null, idTareaSiguiente: null });
        });
    });

    /**
     * El caso que se rompe si no se saca la tarea de la lista antes de mirar
     * los vecinos: se acabaria comparando consigo misma.
     */
    describe('moviendo una tarea dentro de su propia columna', () => {

        it('bajando, no se cuenta a si misma como vecina', () => {
            // 'a' baja al final: los vecinos salen de ['b', 'c'], no de ['a', 'b', 'c']
            expect(calcularVecinos(COLUMNA, 'a', 2))
                .toEqual({ idTareaAnterior: 'c', idTareaSiguiente: null });
        });

        it('subiendo, tampoco', () => {
            // 'c' sube al primer puesto
            expect(calcularVecinos(COLUMNA, 'c', 0))
                .toEqual({ idTareaAnterior: null, idTareaSiguiente: 'a' });
        });

        it('al centro coge las dos que quedan a los lados', () => {
            // 'a' se mete entre 'b' y 'c'
            expect(calcularVecinos(COLUMNA, 'a', 1))
                .toEqual({ idTareaAnterior: 'b', idTareaSiguiente: 'c' });
        });

        it('soltandola donde ya estaba devuelve sus mismos vecinos', () => {
            // 'b' vuelve a su sitio: sigue entre 'a' y 'c'
            expect(calcularVecinos(COLUMNA, 'b', 1))
                .toEqual({ idTareaAnterior: 'a', idTareaSiguiente: 'c' });
        });

        it('la unica tarea de la columna se queda sin vecinos', () => {
            expect(calcularVecinos(['sola'], 'sola', 0))
                .toEqual({ idTareaAnterior: null, idTareaSiguiente: null });
        });
    });

    describe('con indices fuera de rango', () => {

        it('se pega al principio si el indice es negativo', () => {
            expect(calcularVecinos(COLUMNA, 'nueva', -5))
                .toEqual({ idTareaAnterior: null, idTareaSiguiente: 'a' });
        });

        it('se pega al final si el indice se pasa', () => {
            expect(calcularVecinos(COLUMNA, 'nueva', 99))
                .toEqual({ idTareaAnterior: 'c', idTareaSiguiente: null });
        });
    });
});

describe('moverElemento', () => {

    it('mueve hacia abajo', () => {
        expect(moverElemento(['a', 'b', 'c'], 0, 2)).toEqual(['b', 'c', 'a']);
    });

    it('mueve hacia arriba', () => {
        expect(moverElemento(['a', 'b', 'c'], 2, 0)).toEqual(['c', 'a', 'b']);
    });

    it('no toca la lista original', () => {
        const original = ['a', 'b', 'c'];
        moverElemento(original, 0, 2);
        expect(original).toEqual(['a', 'b', 'c']);
    });

    it('devuelve la lista igual si el destino no existe', () => {
        expect(moverElemento(['a', 'b'], 0, 9)).toEqual(['a', 'b']);
    });
});

describe('renumerarOrden', () => {

    it('numera desde 1 segun la posicion, ignorando el orden que traia', () => {
        const lista = [{ orden: 1000 }, { orden: 300 }, { orden: 2000 }];
        expect(renumerarOrden(lista)).toEqual([{ orden: 1 }, { orden: 2 }, { orden: 3 }]);
    });

    it('devuelve objetos nuevos, que es lo que hace repintar al OnPush', () => {
        const lista = [{ orden: 5 }];
        expect(renumerarOrden(lista)[0]).not.toBe(lista[0]);
    });
});
