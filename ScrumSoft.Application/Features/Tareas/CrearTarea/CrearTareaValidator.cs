using FluentValidation;

namespace ScrumSoft.Application.Tareas
{
    public sealed class CrearTareaValidator : AbstractValidator<CrearTareaComando>
    {
        public CrearTareaValidator()
        {
            RuleFor(c => c.IdProyecto).NotEmpty();
            RuleFor(c => c.IdColumna).NotEmpty();

            RuleFor(c => c.Titulo)
                .NotEmpty().WithMessage("El titulo de la tarea es obligatorio.")
                .MaximumLength(255);

            RuleFor(c => c.Descripcion).MaximumLength(4000);

            RuleFor(c => c.Prioridad)
                .IsInEnum().WithMessage("La prioridad no es valida.");
        }
    }
}
