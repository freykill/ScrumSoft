using FluentValidation;

namespace ScrumSoft.Application.Usuarios
{
    public sealed class CrearUsuarioValidator : AbstractValidator<CrearUsuarioComando>
    {
        public CrearUsuarioValidator()
        {
            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(150);

            RuleFor(c => c.CorreoElectronico)
                .NotEmpty().WithMessage("El correo electronico es obligatorio.")
                .EmailAddress().WithMessage("El correo electronico no es valido.")
                .MaximumLength(255);

            RuleFor(c => c.Contrasena)
                .NotEmpty().WithMessage("La contrasena es obligatoria.")
                .MinimumLength(8).WithMessage("La contrasena debe tener al menos 8 caracteres.");

            RuleFor(c => c.Rol).IsInEnum();
        }
    }
}
