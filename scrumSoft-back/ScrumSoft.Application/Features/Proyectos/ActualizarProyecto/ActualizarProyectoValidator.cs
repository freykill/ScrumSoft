using FluentValidation;

namespace ScrumSoft.Application.Proyectos
{
    public sealed class ActualizarProyectoValidator : AbstractValidator<ActualizarProyectoComando>
    {
        public ActualizarProyectoValidator()
        {
            RuleFor(c => c.Id).NotEmpty();

            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre del proyecto es obligatorio.")
                .MaximumLength(200);

            RuleFor(c => c.Descripcion)
                .MaximumLength(2000);

            RuleFor(c => c.EstadoProyecto)
                .IsInEnum().WithMessage("El estado del proyecto no es valido.");

            RuleFor(c => c.FechaFinPrevista)
                .GreaterThanOrEqualTo(c => c.FechaInicio)
                .When(c => c.FechaFinPrevista is not null)
                .WithMessage("La fecha fin prevista no puede ser anterior a la de inicio.");
        }
    }
}
