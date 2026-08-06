using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumSoft.Application.Proyectos
{
    public sealed class CrearProyectoValidator : AbstractValidator<CrearProyectoComando>
    {
        public CrearProyectoValidator()
        {
            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre del proyecto es obligatorio.")
                .MaximumLength(200);

            RuleFor(c => c.Descripcion)
                .MaximumLength(2000);

            RuleFor(c => c.FechaFinPrevista)
                .GreaterThanOrEqualTo(c => c.FechaInicio)
                .When(c => c.FechaFinPrevista is not null)
                .WithMessage("La fecha fin prevista no puede ser anterior a la de inicio.");

            RuleForEach(c => c.Columnas)
                .NotEmpty().WithMessage("El nombre de la columna es obligatorio.")
                .MaximumLength(100);
        }
    }
}
