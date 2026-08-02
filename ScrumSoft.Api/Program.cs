using System.Text.Json.Serialization;
using ScrumSoft.Api.Adapters;
using ScrumSoft.Api.Middlewares;
using ScrumSoft.Application;
using ScrumSoft.Application.Common;
using ScrumSoft.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Falla al arrancar y con un mensaje util, en vez de con un error de red
// incomprensible en la primera peticion.
if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("ScrumSoft")))
{
    throw new InvalidOperationException(
        "Falta la cadena de conexion 'ScrumSoft'. Configurala en los secretos de usuario " +
        "(clic derecho en ScrumSoft.Api > Administrar secretos de usuario) o en la " +
        "variable de entorno ConnectionStrings__ScrumSoft.");
}

// Cada capa registra lo suyo. Program.cs solo las junta.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Necesario para que el adaptador pueda leer el usuario de la peticion en curso.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioActual, UsuarioActual>();

builder.Services
    .AddControllers()
    .AddJsonOptions(opciones =>
    {
        // Sin esto los enum salen como numeros: "prioridad": 2.
        // Con esto: "prioridad": "Alta", que es lo que el frontend puede mostrar.
        opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Primero de todo: cualquier excepcion de aqui hacia adentro queda traducida.
app.UseMiddleware<MiddlewareDeErrores>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
