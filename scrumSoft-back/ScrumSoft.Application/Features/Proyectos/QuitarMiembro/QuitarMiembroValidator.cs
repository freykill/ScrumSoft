using FluentValidation;

namespace ScrumSoft.Application.Proyectos
{
    public sealed class QuitarMiembroValidator : AbstractValidator<QuitarMiembroComando>
    {
        public QuitarMiembroValidator()
        {
            RuleFor(c => c.IdProyecto).NotEmpty();
            RuleFor(c => c.IdUsuario).NotEmpty();
        }
    }
}
