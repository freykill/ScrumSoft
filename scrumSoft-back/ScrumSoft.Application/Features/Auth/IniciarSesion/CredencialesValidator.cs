using FluentValidation;

namespace ScrumSoft.Application.Auth
{
    public sealed class CredencialesValidator : AbstractValidator<CredencialesComando>
    {
        public CredencialesValidator()
        {
            RuleFor(c => c.CorreoElectronico)
                .NotEmpty().WithMessage("El correo electronico es obligatorio.")
                .EmailAddress().WithMessage("El correo electronico no es valido.")
                .MaximumLength(255);

            RuleFor(c => c.Contrasena)
                .NotEmpty().WithMessage("La contrasena es obligatoria.");
        }
    }
}
