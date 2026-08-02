using FluentValidation;

namespace ScrumSoft.Application.Columnas
{
    public sealed class AgregarColumnaValidator : AbstractValidator<AgregarColumnaComando>
    {
        public AgregarColumnaValidator()
        {
            RuleFor(c => c.IdProyecto).NotEmpty();

            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre de la columna es obligatorio.")
                .MaximumLength(100);
        }
    }
}
