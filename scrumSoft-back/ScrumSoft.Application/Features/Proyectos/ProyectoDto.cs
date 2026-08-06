using ScrumSoft.Domain.Entities;
using ScrumSoft.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumSoft.Application.Proyectos
{
    public sealed record ProyectoDto
    {
        public required Guid Id { get; init; }

        public required string Nombre { get; init; }

        public string? Descripcion { get; init; }

        public required DateOnly FechaInicio { get; init; }

        public DateOnly? FechaFinPrevista { get; init; }

        public required EstadoProyecto EstadoProyecto { get; init; }

        public required int CantidadColumnas { get; init; }

        public static ProyectoDto Desde(Proyecto proyecto)
        {
            ArgumentNullException.ThrowIfNull(proyecto);

            return new ProyectoDto
            {
                Id = proyecto.Id,
                Nombre = proyecto.Nombre,
                Descripcion = proyecto.Descripcion,
                FechaInicio = proyecto.FechaInicio,
                FechaFinPrevista = proyecto.FechaFinPrevista,
                EstadoProyecto = proyecto.EstadoProyecto,
                CantidadColumnas = proyecto.Columnas.Count
            };
        }
    }
}
