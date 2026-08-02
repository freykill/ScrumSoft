using FluentValidation;

namespace ScrumSoft.Application.Columnas
{
    public sealed class RenombrarColumnaValidator : AbstractValidator<RenombrarColumnaComando>
    {
        public RenombrarColumnaValidator()
        {
            RuleFor(c => c.IdProyecto).NotEmpty();
            RuleFor(c => c.IdColumna).NotEmpty();

            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre de la columna es obligatorio.")
                .MaximumLength(100);
        }
    }
}
