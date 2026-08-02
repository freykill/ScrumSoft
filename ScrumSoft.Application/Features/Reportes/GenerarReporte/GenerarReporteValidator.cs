using FluentValidation;

namespace ScrumSoft.Application.Reportes
{
    public sealed class GenerarReporteValidator : AbstractValidator<GenerarReporteConsulta>
    {
        public GenerarReporteValidator()
        {
            RuleFor(c => c.IdProyecto).NotEmpty();

            RuleFor(c => c.Formato)
                .IsInEnum().WithMessage("El formato solicitado no existe.");
        }
    }
}
