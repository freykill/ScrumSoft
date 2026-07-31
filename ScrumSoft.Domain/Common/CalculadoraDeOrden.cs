namespace ScrumSoft.Domain.Common
{
    /// <summary>Calcula posiciones dentro de una lista ordenada por enteros con huecos.</summary>
    public static class CalculadoraDeOrden
    {
        /// <summary>Separacion entre posiciones consecutivas.</summary>
        public const int Salto = 1000;

        /// <summary>
        /// Calcula la posicion entre dos vecinos. Devuelve false cuando ya no queda hueco
        /// entre ellos y hay que renormalizar la lista completa.
        /// </summary>
        /// <param name="ordenAnterior">Orden del vecino de arriba, o null si se suelta al inicio.</param>
        /// <param name="ordenSiguiente">Orden del vecino de abajo, o null si se suelta al final.</param>
        /// <param name="orden">Posicion calculada. Solo tiene sentido si el metodo devuelve true.</param>
        /// <returns>True si se pudo calcular una posicion intermedia.</returns>
        public static bool TryCalcular(int? ordenAnterior, int? ordenSiguiente, out int orden)
        {
            // Cae entre dos: punto medio.
            if (ordenAnterior is int anterior && ordenSiguiente is int siguiente)
            {
                if (siguiente - anterior <= 1)
                {
                    orden = 0;
                    return false;
                }

                orden = anterior + ((siguiente - anterior) / 2);
                return true;
            }

            // Se suelta al final, despues del ultimo.
            if (ordenAnterior is int ultimo)
            {
                orden = ultimo + Salto;
                return true;
            }

            // Se suelta al inicio, antes del primero.
            if (ordenSiguiente is int primero)
            {
                if (primero <= 1)
                {
                    orden = 0;
                    return false;
                }

                orden = primero / 2;
                return true;
            }

            // La lista estaba vacia.
            orden = Salto;
            return true;
        }

        /// <summary>Posicion equiespaciada para el elemento en el indice indicado. Se usa al renormalizar.</summary>
        /// <param name="indice">Posicion del elemento en la lista, empezando en cero.</param>
        /// <returns>El orden que le corresponde: 1000, 2000, 3000...</returns>
        public static int PosicionEn(int indice) => (indice + 1) * Salto;
    }
}
