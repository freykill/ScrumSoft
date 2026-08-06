using FluentValidation;

namespace ScrumSoft.Application.Proyectos
{
    public sealed class AgregarMiembroValidator : AbstractValidator<AgregarMiembroComando>
    {
        public AgregarMiembroValidator()
        {
            RuleFor(c => c.IdProyecto).NotEmpty();
            RuleFor(c => c.IdUsuario).NotEmpty();
        }
    }
}
