using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ScrumSoft.Application.Common;
using ScrumSoft.Domain.Common;

namespace ScrumSoft.Api.Middlewares
{
    // Traduce excepciones a codigos HTTP. Escrito una vez, aplica a todos los endpoints:
    // ningun controlador necesita try/catch.
    public sealed partial class MiddlewareDeErrores(
        RequestDelegate siguiente,
        ILogger<MiddlewareDeErrores> registro)
    {
        // El generador de codigo crea la implementacion: la plantilla del mensaje se
        // compila una vez y no se arma la cadena si el nivel Error esta desactivado.
        [LoggerMessage(EventId = 1000, Level = LogLevel.Error, Message = "Error no controlado en {Ruta}")]
        private static partial void RegistrarErrorNoControlado(ILogger registro, string ruta, Exception ex);

        public async Task InvokeAsync(HttpContext contexto)
        {
            ArgumentNullException.ThrowIfNull(contexto);

            try
            {
                await siguiente(contexto);
            }
            catch (ValidationException ex)
            {
                // FluentValidation: se devuelven los errores agrupados por campo,
                // para que el formulario los pinte debajo de cada input.
                var errores = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                await EscribirAsync(
                    contexto,
                    StatusCodes.Status400BadRequest,
                    "Datos invalidos",
                    "Uno o mas campos no son validos.",
                    errores).ConfigureAwait(false);
            }
            catch (DomainException ex)
            {
                await EscribirAsync(
                    contexto,
                    StatusCodes.Status400BadRequest,
                    "Regla de negocio",
                    ex.Message).ConfigureAwait(false);
            }
            catch (RecursoNoEncontradoException ex)
            {
                await EscribirAsync(
                    contexto,
                    StatusCodes.Status404NotFound,
                    "No encontrado",
                    ex.Message).ConfigureAwait(false);
            }
            catch (AccesoDenegadoException ex)
            {
                await EscribirAsync(
                    contexto,
                    StatusCodes.Status403Forbidden,
                    "Acceso denegado",
                    ex.Message).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Es el ultimo recurso: nada debe escapar sin traducirse.
            catch (Exception ex)
#pragma warning restore CA1031
            {
                // El detalle real va al log; al cliente solo un mensaje generico.
                // Un mensaje de excepcion puede filtrar rutas, consultas o credenciales.
                RegistrarErrorNoControlado(registro, contexto.Request.Path, ex);

                await EscribirAsync(
                    contexto,
                    StatusCodes.Status500InternalServerError,
                    "Error interno",
                    "Ocurrio un error al procesar la solicitud.").ConfigureAwait(false);
            }
        }

        private static Task EscribirAsync(
            HttpContext contexto,
            int codigo,
            string titulo,
            string detalle,
            IDictionary<string, string[]>? errores = null)
        {
            // ProblemDetails es el formato estandar de errores HTTP (RFC 7807):
            // cualquier cliente sabe leerlo sin que inventemos una forma propia.
            var problema = new ProblemDetails
            {
                Status = codigo,
                Title = titulo,
                Detail = detalle,
                Instance = contexto.Request.Path
            };

            // El identificador de traza permite cruzar el error del usuario con el log.
            problema.Extensions["traceId"] = contexto.TraceIdentifier;

            if (errores is not null)
                problema.Extensions["errors"] = errores;

            contexto.Response.Clear();
            contexto.Response.StatusCode = codigo;
            contexto.Response.ContentType = "application/problem+json";

            return contexto.Response.WriteAsJsonAsync(problema);
        }
    }
}
