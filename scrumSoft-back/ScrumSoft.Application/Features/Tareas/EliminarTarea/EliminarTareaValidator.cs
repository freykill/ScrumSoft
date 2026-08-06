using FluentValidation;

namespace ScrumSoft.Application.Tareas
{
    public sealed class EliminarTareaValidator : AbstractValidator<EliminarTareaComando>
    {
        public EliminarTareaValidator()
        {
            RuleFor(c => c.IdProyecto).NotEmpty();
            RuleFor(c => c.IdTarea).NotEmpty();
        }
    }
}
