using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ScrumSoft.Api.Adapters;
using ScrumSoft.Api.Middlewares;
using ScrumSoft.Application;
using ScrumSoft.Application.Common;
using ScrumSoft.Infrastructure;
using ScrumSoft.Infrastructure.Persistence;
using ScrumSoft.Infrastructure.Realtime;

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

var claveJwt = builder.Configuration["Jwt:Clave"];

if (string.IsNullOrWhiteSpace(claveJwt) || claveJwt.Length < 32)
{
    throw new InvalidOperationException(
        "Falta 'Jwt:Clave' o tiene menos de 32 caracteres. Configurala en los secretos " +
        "de usuario o en la variable de entorno Jwt__Clave.");
}

// Cada capa registra lo suyo. Program.cs solo las junta.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Necesario para que el adaptador pueda leer el usuario de la peticion en curso.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioActual, UsuarioActual>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Emisor"],
            ValidAudience = builder.Configuration["Jwt:Audiencia"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveJwt)),
            // Sin esto, un token vencido sigue aceptandose hasta 5 minutos por defecto.
            ClockSkew = TimeSpan.Zero
        };

        opciones.Events = new JwtBearerEvents
        {
            OnMessageReceived = contexto =>
            {
                // Un WebSocket no puede enviar cabeceras: el cliente de SignalR manda
                // el token en la cadena de consulta. Solo se acepta para la ruta del hub.
                var token = contexto.Request.Query["access_token"];

                if (!string.IsNullOrEmpty(token) &&
                    contexto.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                {
                    contexto.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

// Todo endpoint exige token salvo que diga [AllowAnonymous] a proposito.
// Asi, si un controlador nuevo se queda sin [Authorize], el olvido lo deja
// cerrado en vez de abierto.
builder.Services.AddAuthorization(opciones =>
{
    opciones.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});


// Origenes permitidos para el frontend. Se configuran por appsettings o variable
// de entorno: no se codifican direcciones aqui.
var origenesPermitidos = builder.Configuration.GetSection("Cors:Origenes").Get<string[]>()
    ?? ["http://localhost:4200"];

builder.Services.AddCors(opciones =>
    opciones.AddDefaultPolicy(politica => politica
        .WithOrigins(origenesPermitidos)
        .AllowAnyHeader()
        .AllowAnyMethod()
        // Requerido por SignalR para negociar la conexion.
        .AllowCredentials()));

builder.Services
    .AddControllers()
    .AddJsonOptions(opciones =>
    {
        // Sin esto los enum salen como numeros: "prioridad": 2.
        // Con esto: "prioridad": "Alta", que es lo que el frontend puede mostrar.
        opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo { Title = "ScrumSoft API", Version = "v1" });

    // Agrega el boton "Authorize" a Swagger para pegar el token y probar
    // los endpoints protegidos sin herramientas externas.
    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega aqui el token que devuelve /api/v1/auth/login"
    });

    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Aplica las migraciones pendientes antes de atender la primera peticion, para que
// levantar el entorno sea un solo comando. EF consulta __EFMigrationsHistory y aplica
// solo lo que falta: si la base ya esta al dia, no hace nada.
//
// En un despliegue con varias replicas esto se moveria a un paso previo del pipeline,
// ejecutando el script idempotente de "dotnet ef migrations script".
using (var alcance = app.Services.CreateScope())
{
    var contexto = alcance.ServiceProvider.GetRequiredService<ScrumSoftDbContext>();
    await contexto.Database.MigrateAsync();
}

// Primero de todo: cualquier excepcion de aqui hacia adentro queda traducida.
app.UseMiddleware<MiddlewareDeErrores>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

// El orden importa: primero se averigua quien es (autenticacion),
// despues si puede (autorizacion).
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<TableroHub>("/hubs/tablero");

app.Run();
