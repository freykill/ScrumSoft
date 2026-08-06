using FluentValidation;

namespace ScrumSoft.Application.Columnas
{
    public sealed class EliminarColumnaValidator : AbstractValidator<EliminarColumnaComando>
    {
        public EliminarColumnaValidator()
        {
            RuleFor(c => c.IdProyecto).NotEmpty();
            RuleFor(c => c.IdColumna).NotEmpty();
        }
    }
}
