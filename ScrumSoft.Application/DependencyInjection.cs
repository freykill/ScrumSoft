using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ScrumSoft.Application.Auth;
using ScrumSoft.Application.Columnas;
using ScrumSoft.Application.Common;
using ScrumSoft.Application.Mediador;
using ScrumSoft.Application.Proyectos;
using ScrumSoft.Application.Reportes;
using ScrumSoft.Application.Tablero;
using ScrumSoft.Application.Tareas;
using ScrumSoft.Application.Usuarios;

namespace ScrumSoft.Application
{
    // Registro de la capa de aplicacion. Esta lista es, de hecho, el inventario
    // completo de operaciones que expone el sistema.
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection servicios)
        {
            ArgumentNullException.ThrowIfNull(servicios);

            servicios.AddScoped<IMediador, MediadorEnMemoria>();

            // Generico abierto: el contenedor lo cierra con los tipos de cada peticion,
            // asi que aplica a todas sin nombrarlas.
            servicios.AddTransient(typeof(IComportamiento<,>), typeof(ComportamientoDeValidacion<,>));

            // Busca en este ensamblado todas las clases que hereden de AbstractValidator.
            // Al agregar un validador nuevo, no hay que tocar este archivo.
            servicios.AddValidatorsFromAssemblyContaining<CrearProyectoValidator>(ServiceLifetime.Singleton);

            servicios.AddScoped<AccesoAProyectos>();

            // Autenticacion
            servicios.AddScoped<IManejador<CredencialesComando, SesionDto>, IniciarSesionHandler>();

            // Proyectos
            servicios.AddScoped<IManejador<CrearProyectoComando, ProyectoDto>, CrearProyectoHandler>();
            servicios.AddScoped<IManejador<ActualizarProyectoComando, ProyectoDto>, ActualizarProyectoHandler>();
            servicios.AddScoped<IManejador<EliminarProyectoComando, Unidad>, EliminarProyectoHandler>();
            servicios.AddScoped<IManejador<ListarProyectosConsulta, PagedResult<ProyectoDto>>, ListarProyectosHandler>();

            // Equipo del proyecto
            servicios.AddScoped<IManejador<ListarMiembrosConsulta, IReadOnlyList<MiembroDto>>, ListarMiembrosHandler>();
            servicios.AddScoped<IManejador<AgregarMiembroComando, MiembroDto>, AgregarMiembroHandler>();
            servicios.AddScoped<IManejador<QuitarMiembroComando, Unidad>, QuitarMiembroHandler>();

            // Usuarios
            servicios.AddScoped<IManejador<ListarUsuariosConsulta, PagedResult<UsuarioDto>>, ListarUsuariosHandler>();

            // Columnas
            servicios.AddScoped<IManejador<AgregarColumnaComando, ColumnaDto>, AgregarColumnaHandler>();
            servicios.AddScoped<IManejador<RenombrarColumnaComando, ColumnaDto>, RenombrarColumnaHandler>();
            servicios.AddScoped<IManejador<ReordenarColumnasComando, IReadOnlyList<ColumnaDto>>, ReordenarColumnasHandler>();
            servicios.AddScoped<IManejador<EliminarColumnaComando, Unidad>, EliminarColumnaHandler>();
            servicios.AddScoped<IManejador<ListarColumnasConsulta, IReadOnlyList<ColumnaDto>>, ListarColumnasHandler>();

            // Tareas
            servicios.AddScoped<IManejador<CrearTareaComando, TareaDto>, CrearTareaHandler>();
            servicios.AddScoped<IManejador<ActualizarTareaComando, TareaDto>, ActualizarTareaHandler>();
            servicios.AddScoped<IManejador<EliminarTareaComando, Unidad>, EliminarTareaHandler>();
            servicios.AddScoped<IManejador<MoverTareaComando, TareaDto>, MoverTareaHandler>();

            // Tablero y reportes
            servicios.AddScoped<IManejador<ObtenerTableroConsulta, TableroDto>, ObtenerTableroHandler>();
            servicios.AddScoped<IManejador<GenerarReporteConsulta, ArchivoDeReporte>, GenerarReporteHandler>();

            return servicios;
        }
    }
}
