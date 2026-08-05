using FluentValidation;

namespace ScrumSoft.Application.Usuarios
{
    public sealed class ActualizarUsuarioValidator : AbstractValidator<ActualizarUsuarioComando>
    {
        public ActualizarUsuarioValidator()
        {
            RuleFor(c => c.Id).NotEmpty();

            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(150);

            RuleFor(c => c.Rol).IsInEnum();
        }
    }
}
