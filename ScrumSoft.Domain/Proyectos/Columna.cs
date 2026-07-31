using ScrumSoft.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumSoft.Domain.Proyectos
{
    /// <summary>Etapa del flujo de trabajo. Solo el proyecto puede crearla o modificarla.</summary>
    public sealed class Columna : Entity
{       
        
        /// <summary>Nombre visible de la columna.</summary>
        public string Nombre { get; private set; } = null!;

        /// <summary>Posicion dentro del tablero.</summary>
        public int Orden { get; private set; }

        /// <summary>Proyecto al que pertenece.</summary>
        public Guid IdProyecto { get; private set; }


        private Columna() { } // Requerido por EF Core

        internal Columna(Guid idProyecto, string nombre, int orden)
        {
            IdProyecto = idProyecto;
            Nombre = Validar(nombre);
            Orden = orden;
        }



        internal void Renombrar(string nombre) => Nombre = Validar(nombre);

        internal void MoverA(int orden) => Orden = orden;

        private static string Validar(string nombre) =>
            string.IsNullOrWhiteSpace(nombre)
                ? throw new DomainException("El nombre de la columna es obligatorio.")
                : nombre.Trim();
    }
}
