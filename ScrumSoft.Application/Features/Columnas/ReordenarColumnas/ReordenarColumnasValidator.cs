using FluentValidation;

namespace ScrumSoft.Application.Columnas
{
    public sealed class ReordenarColumnasValidator : AbstractValidator<ReordenarColumnasComando>
    {
        public ReordenarColumnasValidator()
        {
            RuleFor(c => c.IdProyecto).NotEmpty();

            RuleFor(c => c.IdsEnOrden)
                .NotEmpty().WithMessage("Debe indicar el orden de las columnas.");
        }
    }
}
