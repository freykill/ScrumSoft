using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScrumSoft.Application.Common;
using ScrumSoft.Application.Ports;
using ScrumSoft.Domain.Common;
using ScrumSoft.Infrastructure.Persistence;
using ScrumSoft.Infrastructure.Persistence.Interceptors;
using ScrumSoft.Infrastructure.Persistence.Repositories;
using ScrumSoft.Infrastructure.Realtime;
using ScrumSoft.Infrastructure.Reportes;
using ScrumSoft.Infrastructure.Security;
using ScrumSoft.Infrastructure.Time;

namespace ScrumSoft.Infrastructure
{
    // Aqui se cierra el hexagono: cada puerto que declaro Application recibe
    // su adaptador. Application nunca menciona a estas clases.
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection servicios,
            IConfiguration configuracion)
        {
            ArgumentNullException.ThrowIfNull(servicios);
            ArgumentNullException.ThrowIfNull(configuracion);

            // Sin estado y sin dependencias: una sola instancia para toda la aplicacion.
            servicios.AddSingleton<IClock, SystemClock>();
            servicios.AddSingleton<IHasheadorDeContrasenas, HasheadorBCrypt>();

            // La seccion "Jwt" se enlaza a la clase de opciones, y se valida al
            // arrancar para no descubrir una clave vacia en el primer inicio de sesion.
            servicios
                .AddOptions<OpcionesDeJwt>()
                .Bind(configuracion.GetSection(OpcionesDeJwt.Seccion))
                .Validate(
                    o => !string.IsNullOrWhiteSpace(o.Clave) && o.Clave.Length >= 32,
                    "Jwt:Clave es obligatoria y debe tener al menos 32 caracteres. " +
                    "Configurala en los secretos de usuario o en la variable de entorno Jwt__Clave.")
                .ValidateOnStart();

            servicios.AddSingleton<IGeneradorDeTokens, GeneradorDeTokensJwt>();

            servicios.AddScoped<InterceptorDeAuditoria>();

            servicios.AddDbContext<ScrumSoftDbContext>((proveedor, opciones) =>
                opciones
                    .UseNpgsql(configuracion.GetConnectionString("ScrumSoft"))
                    // Traduce FechaInicio a fecha_inicio en lo que no se nombro a mano.
                    .UseSnakeCaseNamingConvention()
                    .AddInterceptors(proveedor.GetRequiredService<InterceptorDeAuditoria>()));

            // Scoped: una instancia por peticion HTTP. Compartir el DbContext entre
            // peticiones simultaneas corrompe datos.
            servicios.AddScoped<IUnitOfWork, UnitOfWork>();
            servicios.AddScoped<IProyectoRepository, ProyectoRepository>();
            servicios.AddScoped<ITareaRepository, TareaRepository>();
            servicios.AddScoped<IUsuarioRepository, UsuarioRepository>();

            servicios.AddScoped<INotificadorDeTablero, NotificadorDeTablero>();

            // Singleton: la presencia es estado compartido entre todas las conexiones.
            // Scoped daria una lista vacia por invocacion y nadie veria a nadie.
            servicios.AddSingleton<RegistroDePresencia>();

            // QuestPDF exige declarar la licencia antes de generar el primer documento.
            // Community es gratuita para organizaciones con ingresos bajo un millon de dolares.
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            // Los dos se registran contra la misma interfaz. GenerarReporteHandler recibe
            // IEnumerable<IExportadorDeReporte> y elige por formato: agregar un tercero
            // es escribir una clase y sumar una linea aqui, sin tocar nada existente.
            servicios.AddSingleton<IExportadorDeReporte, ExportadorPdf>();
            servicios.AddSingleton<IExportadorDeReporte, ExportadorExcel>();

            return servicios;
        }
    }
}
