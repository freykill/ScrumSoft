# ScrumSoft — API

Backend de ScrumSoft: gestion de proyectos agiles con tablero kanban en tiempo
real. .NET 8 con arquitectura hexagonal, Entity Framework Core sobre PostgreSQL,
SignalR, y reportes en PDF y Excel.

> Esta carpeta es solo el backend. Para levantar la solucion completa (base de
> datos, API y SPA) con Docker, ver el **[README de la raiz](../README.md)**.

---

## Levantar en local

Requiere el SDK de .NET 8 (fijado en `global.json`) y un PostgreSQL accesible.

`appsettings.json` trae la cadena de conexion y la clave del JWT **vacias a
proposito**: no se versionan secretos. Hay que darlas por secretos de usuario o
por variables de entorno, o la API se niega a arrancar con un mensaje que dice
exactamente que falta.

```bash
cd ScrumSoft.Api

dotnet user-secrets set "ConnectionStrings:ScrumSoft" \
  "Host=localhost;Database=scrumsoft;Username=postgres;Password=postgres"

# 32 caracteres como minimo: HMAC-SHA256 no admite claves mas cortas
dotnet user-secrets set "Jwt:Clave" "una-clave-de-al-menos-32-caracteres-aqui"

dotnet run          # https://localhost:7086
```

Las migraciones **se aplican solas al arrancar** (`MigrateAsync` en
`Program.cs`), asi que no hay que ejecutar nada antes: basta con que la base
exista y acepte conexiones.

Swagger queda en `https://localhost:7086/swagger`, solo en desarrollo, con el
boton **Authorize** para pegar el token y probar los endpoints protegidos.

### Usuarios precargados

La migracion semilla deja dos usuarios, con la contrasena ya hasheada con BCrypt:

| Correo | Contrasena | Rol |
|---|---|---|
| admin@scrumsoft.com | Admin123* | Administrador |
| miembro@scrumsoft.com | Miembro123* | Miembro |

## Comandos

| Comando | Que hace |
|---|---|
| `dotnet run --project ScrumSoft.Api` | Levanta la API |
| `dotnet test` | Las 5 pruebas |
| `dotnet build` | Compila. Avisos tratados como errores, ver mas abajo |
| `dotnet ef migrations add <Nombre> -p ScrumSoft.Infrastructure -s ScrumSoft.Api` | Nueva migracion |

---

## Arquitectura

Hexagonal (puertos y adaptadores). Cuatro proyectos, y **la dependencia siempre
apunta hacia adentro**:

```
        ScrumSoft.Api  ──────┐          controladores, middleware, composicion
                             ▼
ScrumSoft.Infrastructure ──► ScrumSoft.Application ──► ScrumSoft.Domain
   adaptadores                 casos de uso                entidades
   (EF, SignalR, PDF,          y PUERTOS                   y reglas
    Excel, BCrypt, JWT)        (interfaces)
```

| Proyecto | Contiene | De que depende |
|---|---|---|
| `Domain` | Entidades con logica propia, `CalculadoraDeOrden`, `DomainException` | **de nada** |
| `Application` | Casos de uso en `Features/<Area>/<CasoDeUso>/`, puertos en `Ports/`, mediador propio | Domain |
| `Infrastructure` | Implementaciones de los puertos | Application, Domain |
| `Api` | Controladores, middleware de errores, composicion | todas |

El dominio no conoce EF, ni HTTP, ni SignalR. Se puede probar sin levantar nada.

### Los puertos

Son interfaces declaradas en `Application/Ports/` e implementadas en
`Infrastructure/`. Esa inversion es la que permite cambiar una tecnologia sin
tocar un caso de uso:

| Puerto | Implementacion | Tecnologia |
|---|---|---|
| `IProyectoRepository`, `ITareaRepository`, `IUsuarioRepository` | `Persistence/Repositories/` | EF Core + Npgsql |
| `INotificadorDeTablero` | `Realtime/NotificadorDeTablero` | SignalR |
| `IExportadorDeReporte` | `Reportes/ExportadorPdf`, `ExportadorExcel` | QuestPDF, ClosedXML |
| `IGeneradorDeTokens` | `Security/GeneradorDeTokensJwt` | JWT |
| `IHasheadorDeContrasenas` | `Security/HasheadorBCrypt` | BCrypt |
| `IClock` | `Time/SystemClock` | reloj del sistema |

`IClock` existe para que las pruebas controlen el tiempo en vez de depender de
`DateTime.UtcNow`.

### Mediador propio, no MediatR

`Application/Mediador/` implementa el patron en unas pocas clases:
`IPeticion<T>`, `IManejador<TPeticion, TRespuesta>` y `MediadorEnMemoria`.

Se escribio en vez de tomar MediatR porque, para despachar en el mismo proceso,
el patron cabe en unas pocas clases y evita una dependencia externa que ademas
cambio a licencia comercial. La reflexion ocurre **una sola vez por tipo** y queda cacheada en
un `ConcurrentDictionary`; a partir de ahi todo es fuertemente tipado y no se
usa `dynamic` en ningun momento.

Los `IComportamiento<,>` son el equivalente a los pipeline behaviors:
`ComportamientoDeValidacion` ejecuta los validadores de FluentValidation antes
de que el caso de uso reciba nada, asi que **ningun handler valida entradas**.

---

## Decisiones que sostienen el diseño

### Las reglas de negocio viven en el dominio

El caso de uso trae los datos que la entidad no puede consultar y **la entidad
decide**. Se ve claro en `EliminarColumnaHandler`: el handler averigua si la
columna tiene tareas y llama a `proyecto.EliminarColumna(id, contieneTareas)`.
La regla —no se borra una columna con tareas— esta en `Proyecto`, no en el
handler ni en el controlador.

### El orden de las tarjetas va de mil en mil

`CalculadoraDeOrden` es la pieza central del tablero. Las posiciones no son
1-2-3 sino 1000-2000-3000, y **insertar entre dos es escribir una sola fila**
con el punto medio:

```
antes:   [1000]        [2000]
suelta aqui  ▲
despues: [1000] [1500] [2000]     ← una sola tarea cambia de valor
```

Con posiciones consecutivas habria que desplazar todas las de abajo en cada
arrastre. Con huecos de mil caben unas diez inserciones seguidas en el mismo
punto antes de agotarse.

Cuando ya no queda hueco (`siguiente - anterior <= 1`), `MoverTareaHandler`
renumera la columna entera con `PosicionEn(i)` y recalcula sobre las posiciones
nuevas. Es el caso raro, y es el que cubre la prueba obligatoria.

La API no recibe un indice sino **los ids de las tareas vecinas**: el cliente
dice "entre esta y esta" y el servidor decide el numero. Asi dos usuarios
moviendo tarjetas a la vez no se pisan calculando el mismo indice.

### Un solo DTO y una sola consulta para PDF y Excel

`GenerarReporteHandler` arma `ReporteProyectoDto` una vez, con **los mismos
filtros que el tablero** —por eso el archivo coincide con lo que el usuario ve
en pantalla— y se lo entrega al exportador que corresponda:

```csharp
var exportador = exportadores.FirstOrDefault(e => e.Formato == peticion.Formato)
```

Los exportadores se inyectan como `IEnumerable<IExportadorDeReporte>` y cada uno
declara que formato atiende. **Agregar un tercer formato es escribir una clase
nueva y registrarla**: ni el handler ni los exportadores existentes se tocan.
Eso es lo que pide el requisito 6.8 sobre extensibilidad.

### Acceso a proyectos centralizado

`AccesoAProyectos.ObtenerConAccesoAsync` es el unico camino para llegar a un
proyecto. Se toca un proyecto **solo si se es miembro, sin excepcion por rol**:
un administrador que necesite entrar se agrega como miembro y queda registrado.
Ningun handler consulta el repositorio de proyectos por su cuenta.

### Cerrado por defecto

```csharp
opciones.FallbackPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser().Build();
```

Todo endpoint exige token salvo que diga `[AllowAnonymous]` a proposito. Si
manana alguien agrega un controlador y olvida el `[Authorize]`, **el olvido lo
deja cerrado en vez de abierto**.

`ClockSkew = TimeSpan.Zero` porque, por defecto, un token vencido se sigue
aceptando cinco minutos.

### El token del hub viaja en la query, y solo ahi

Un WebSocket no puede enviar cabeceras, asi que el cliente de SignalR manda el
token en `access_token`. El evento `OnMessageReceived` **solo lo acepta para
rutas que empiezan por `/hubs`**: en el resto de la API sigue siendo obligatoria
la cabecera `Authorization`, para no dejar tokens escritos en los logs de acceso
de cualquier proxy.

### Errores como ProblemDetails

`MiddlewareDeErrores` traduce excepciones a codigos HTTP en un solo sitio:
ningun controlador tiene `try/catch`.

| Excepcion | HTTP |
|---|---|
| `ValidationException` (FluentValidation) | 400 con los errores agrupados por campo |
| `DomainException` | 400 |
| `RecursoNoEncontradoException` | 404 |
| `AccesoDenegadoException` | 403 |
| cualquier otra | 500 generico, con el detalle solo en el log |

El formato es **ProblemDetails (RFC 7807)**, el estandar de errores HTTP, con un
`traceId` para cruzar el error que vio el usuario con la entrada del log. No se
usa un envoltorio propio del tipo `{ code, message, data }`: eso obligaria a
responder 200 en los errores, con lo que se pierden el manejo del 401 en el
cliente, los reintentos y toda la observabilidad de la capa HTTP.

El 500 nunca devuelve el mensaje real de la excepcion: puede filtrar rutas,
consultas o credenciales.

---

## Base de datos

PostgreSQL con EF Core y migraciones incrementales. `EFCore.NamingConventions`
traduce los nombres a `snake_case`, que es la convencion de Postgres.

| Migracion | Que hace |
|---|---|
| `EsquemaInicial` | Tablas, relaciones y los datos semilla |
| `IndiceParcialDeMembresias` | Indice parcial sobre las membresias activas |

La base se puede construir desde cero ejecutando las migraciones en orden, que
es justo lo que hace la API al arrancar.

`InterceptorDeAuditoria` rellena las columnas de auditoria al guardar, sin que
ningun handler tenga que acordarse. El borrado es **logico**: `EstadoRegistro`
marca la fila como eliminada en vez de perderla.

El diagrama del modelo esta en `docs/modelo-datos.dbml`.

---

## Pruebas

xUnit con NSubstitute para los dobles de los puertos. **5 pruebas**, todas sobre
logica de dominio o de aplicacion:

| Archivo | Prueba |
|---|---|
| `Dominio/CalculadoraDeOrdenTests` | entre dos tareas devuelve el punto medio |
| | sin hueco entre vecinas, no calcula |
| `Aplicacion/MoverTareaHandlerTests` | mover entre dos tareas guarda el punto medio y avisa por tiempo real |
| | sin hueco, renumera la columna y ubica la tarea |
| `Dominio/ProyectoTests` | no se permite eliminar una columna con tareas |

Las dos primeras cubren el **calculo de la nueva posicion al reordenar**, que es
la prueba obligatoria del enunciado, y la del handler verifica ademas el camino
raro: el que renumera.

Las aserciones son las de xUnit y no FluentAssertions, que dejo de ser libre en
su version 8.

```bash
dotnet test
```

---

## Convenciones

- **Codigo, comentarios y commits en espanol, sin tildes.** Los comentarios
  explican *por que* se hizo asi, no *que* hace la linea.
- **`TreatWarningsAsErrors`** en toda la solucion. Un aviso rompe el build.
- **`CA1707` desactivada solo en `.Tests`** (`Directory.Build.props`): los
  guiones bajos del patron `Metodo_Escenario` son deliberados.
- **Gestion centralizada de paquetes** en `Directory.Packages.props`: las
  versiones se declaran una vez y ningun `.csproj` las repite.
- **Un caso de uso por carpeta**, con su comando, su handler y su validador
  juntos. Se lee todo lo de una operacion sin saltar entre proyectos.
