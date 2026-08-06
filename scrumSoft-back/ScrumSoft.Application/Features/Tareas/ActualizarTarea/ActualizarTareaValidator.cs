using FluentValidation;

namespace ScrumSoft.Application.Tareas
{
    public sealed class ActualizarTareaValidator : AbstractValidator<ActualizarTareaComando>
    {
        public ActualizarTareaValidator()
        {
            RuleFor(c => c.IdProyecto).NotEmpty();
            RuleFor(c => c.IdTarea).NotEmpty();

            RuleFor(c => c.Titulo)
                .NotEmpty().WithMessage("El titulo de la tarea es obligatorio.")
                .MaximumLength(255);

            RuleFor(c => c.Descripcion).MaximumLength(4000);

            RuleFor(c => c.Prioridad)
                .IsInEnum().WithMessage("La prioridad no es valida.");
        }
    }
}
