namespace ScrumSoft.Domain.Common
{
    /// <summary>
    /// Estado de la fila en la base de datos. Lo tienen todas las tablas y se guarda
    /// en la columna estado como un solo caracter.
    /// </summary>
    public enum EstadoRegistro
    {
        /// <summary>Visible y operativo. Se guarda como 'A'.</summary>
        Activo,

        /// <summary>Existe pero esta fuera de circulacion. Se guarda como 'I'.</summary>
        Inactivo,

        /// <summary>Borrado logico. No aparece en ninguna consulta. Se guarda como 'E'.</summary>
        Eliminado
    }
}
