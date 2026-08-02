namespace ScrumSoft.Domain.Common
{
    public static class CalculadoraDeOrden
    {
        public const int Salto = 1000;

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

        public static int PosicionEn(int indice) => (indice + 1) * Salto;
    }
}
