using FluentValidation;

namespace ScrumSoft.Application.Tareas
{
    public sealed class MoverTareaValidator : AbstractValidator<MoverTareaComando>
    {
        public MoverTareaValidator()
        {
            RuleFor(c => c.IdProyecto).NotEmpty();
            RuleFor(c => c.IdTarea).NotEmpty();
            RuleFor(c => c.IdColumnaDestino).NotEmpty();

            RuleFor(c => c.IdTareaAnterior)
                .NotEqual(c => c.IdTarea)
                .When(c => c.IdTareaAnterior is not null)
                .WithMessage("Una tarea no puede ser su propio vecino.");

            RuleFor(c => c.IdTareaSiguiente)
                .NotEqual(c => c.IdTarea)
                .When(c => c.IdTareaSiguiente is not null)
                .WithMessage("Una tarea no puede ser su propio vecino.");
        }
    }
}
