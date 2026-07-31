using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumSoft.Domain.Proyectos
{
    /// <summary>Estado del ciclo de vida de un proyecto.</summary>
    public enum EstadoProyecto
    {
        /// <summary>Creado, todavia no arranca.</summary>
        Planificacion,

        /// <summary>En ejecucion.</summary>
        EnProgreso,

        /// <summary>Cerrado.</summary>
        Completado
    }
}
