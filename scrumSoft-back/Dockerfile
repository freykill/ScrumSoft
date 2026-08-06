# ---------------------------------------------------------------
# Compilacion
# ---------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Primero solo los manifiestos. Mientras no cambien las dependencias, Docker
# reutiliza la capa del restore y no vuelve a bajar los paquetes en cada build.
COPY global.json Directory.Build.props Directory.Packages.props ./
COPY ScrumSoft.Domain/ScrumSoft.Domain.csproj ScrumSoft.Domain/
COPY ScrumSoft.Application/ScrumSoft.Application.csproj ScrumSoft.Application/
COPY ScrumSoft.Infrastructure/ScrumSoft.Infrastructure.csproj ScrumSoft.Infrastructure/
COPY ScrumSoft.Api/ScrumSoft.Api.csproj ScrumSoft.Api/
RUN dotnet restore ScrumSoft.Api/ScrumSoft.Api.csproj

COPY . .
RUN dotnet publish ScrumSoft.Api/ScrumSoft.Api.csproj -c Release -o /app --no-restore

# ---------------------------------------------------------------
# Ejecucion
# ---------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Dentro del contenedor se habla HTTP plano: el TLS lo termina el proxy de
# enfrente. Publicar certificados aqui solo moveria el problema de sitio.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app .

# Usuario sin privilegios que ya trae la imagen: si alguien logra ejecutar algo
# dentro del contenedor, no lo hace como root.
USER $APP_UID

ENTRYPOINT ["dotnet", "ScrumSoft.Api.dll"]
