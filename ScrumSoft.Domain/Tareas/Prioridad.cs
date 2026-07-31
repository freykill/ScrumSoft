namespace ScrumSoft.Domain.Tareas
{
    /// <summary>Nivel de urgencia de una tarea.</summary>
    public enum Prioridad
    {
        /// <summary>Puede esperar.</summary>
        Baja,

        /// <summary>Ritmo normal.</summary>
        Media,

        /// <summary>Requiere atencion pronta.</summary>
        Alta,

        /// <summary>Bloquea el avance del equipo.</summary>
        Critica
    }
}
