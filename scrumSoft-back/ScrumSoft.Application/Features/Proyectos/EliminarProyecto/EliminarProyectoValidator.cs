using FluentValidation;

namespace ScrumSoft.Application.Proyectos
{
    public sealed class EliminarProyectoValidator : AbstractValidator<EliminarProyectoComando>
    {
        public EliminarProyectoValidator() => RuleFor(c => c.Id).NotEmpty();
    }
}
