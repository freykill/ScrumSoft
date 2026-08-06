namespace ScrumSoft.Application.Mediador
{
    // Respuesta vacia. C# no permite usar void como parametro de tipo generico,
    // asi que las peticiones que no devuelven nada devuelven esto.
    public readonly struct Unidad : IEquatable<Unidad>
    {
        public static readonly Unidad Valor;

        public bool Equals(Unidad other) => true;

        public override bool Equals(object? obj) => obj is Unidad;

        public override int GetHashCode() => 0;

        public override string ToString() => "()";

        public static bool operator ==(Unidad izquierda, Unidad derecha) => true;

        public static bool operator !=(Unidad izquierda, Unidad derecha) => false;
    }
}
