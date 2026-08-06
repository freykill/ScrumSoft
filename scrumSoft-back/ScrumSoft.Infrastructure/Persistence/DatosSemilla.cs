using ScrumSoft.Domain.Common;
using ScrumSoft.Domain.Enums;

namespace ScrumSoft.Infrastructure.Persistence
{
    // Datos que las migraciones insertan al crear la base (requisito 6.2).
    //
    // Todo aqui es constante a proposito: HasData exige valores fijos. Si un Guid,
    // una fecha o un hash se generaran al vuelo, cada vez que se creara una migracion
    // EF veria valores distintos y creeria que el modelo cambio.
    //
    // Contrasenas en claro, documentadas en el README:
    //   admin@scrumsoft.com    Admin123*
    //   miembro@scrumsoft.com  Miembro123*
    //
    // Ademas de los usuarios se siembra un tablero con contenido para que la
    // aplicacion se pueda probar nada mas levantarla, sin tener que dar de alta
    // un proyecto y sus columnas antes de ver nada. Los dos usuarios comparten
    // el primer proyecto para poder validar el tiempo real con dos sesiones
    // abiertas; el segundo es solo del administrador y sirve para comprobar que
    // quien no es miembro no lo ve.
    public static class DatosSemilla
    {
        public static readonly Guid IdAdministrador = new("11111111-1111-1111-1111-111111111111");

        public static readonly Guid IdMiembro = new("22222222-2222-2222-2222-222222222222");

        public static readonly DateTimeOffset FechaDeSiembra =
            new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // --- Proyectos ---
        private static readonly Guid IdPlataforma = new("33333333-3333-3333-3333-333333333331");
        private static readonly Guid IdPortal = new("33333333-3333-3333-3333-333333333332");

        // --- Columnas de "Plataforma ScrumSoft" ---
        private static readonly Guid IdBacklog = new("44444444-4444-4444-4444-444444444001");
        private static readonly Guid IdEnProgreso = new("44444444-4444-4444-4444-444444444002");
        private static readonly Guid IdEnRevision = new("44444444-4444-4444-4444-444444444003");
        private static readonly Guid IdHecho = new("44444444-4444-4444-4444-444444444004");

        // --- Columnas de "Portal de clientes" ---
        private static readonly Guid IdPorHacer = new("44444444-4444-4444-4444-444444444011");
        private static readonly Guid IdHaciendo = new("44444444-4444-4444-4444-444444444012");
        private static readonly Guid IdListo = new("44444444-4444-4444-4444-444444444013");

        public static object[] Usuarios() =>
        [
            new
            {
                Id = IdAdministrador,
                Nombre = "Administrador",
                CorreoElectronico = "admin@scrumsoft.com",
                PasswordHash = "$2a$12$ARg7iJNOV843LqiS8ssGfOHp1tSWigIUANkA7soFy708ZExz7g1yO",
                Rol = RolUsuario.Administrador,
                Estado = EstadoRegistro.Activo,
                FechaCreacion = FechaDeSiembra,
                FechaActualizacion = (DateTimeOffset?)null
            },
            new
            {
                Id = IdMiembro,
                Nombre = "Miembro de prueba",
                CorreoElectronico = "miembro@scrumsoft.com",
                PasswordHash = "$2a$12$104Q7YvMTtAmNJxFxsTjl.N3BPTPtGnU1iVskwbfPp3gg4CQpekGa",
                Rol = RolUsuario.Miembro,
                Estado = EstadoRegistro.Activo,
                FechaCreacion = FechaDeSiembra,
                FechaActualizacion = (DateTimeOffset?)null
            }
        ];

        public static object[] Proyectos() =>
        [
            new
            {
                Id = IdPlataforma,
                Nombre = "Plataforma ScrumSoft",
                Descripcion = (string?)"Modulo inicial de gestion agil: tablero kanban, reportes y sincronizacion en tiempo real.",
                FechaInicio = new DateOnly(2026, 1, 5),
                FechaFinPrevista = (DateOnly?)new DateOnly(2026, 3, 31),
                EstadoProyecto = EstadoProyecto.EnProgreso,
                Estado = EstadoRegistro.Activo,
                FechaCreacion = FechaDeSiembra,
                FechaActualizacion = (DateTimeOffset?)null
            },
            new
            {
                Id = IdPortal,
                Nombre = "Portal de clientes",
                Descripcion = (string?)"Sitio publico para que los clientes consulten el avance de sus proyectos.",
                FechaInicio = new DateOnly(2026, 2, 1),
                FechaFinPrevista = (DateOnly?)new DateOnly(2026, 6, 30),
                EstadoProyecto = EstadoProyecto.Planificacion,
                Estado = EstadoRegistro.Activo,
                FechaCreacion = FechaDeSiembra,
                FechaActualizacion = (DateTimeOffset?)null
            }
        ];

        // El administrador esta en los dos; el miembro solo en el primero. Esa
        // diferencia es la que deja comprobar el control de acceso sin preparar nada.
        public static object[] Miembros() =>
        [
            Membresia("66666666-6666-6666-6666-666666666001", IdPlataforma, IdAdministrador),
            Membresia("66666666-6666-6666-6666-666666666002", IdPlataforma, IdMiembro),
            Membresia("66666666-6666-6666-6666-666666666003", IdPortal, IdAdministrador)
        ];

        // El orden va de mil en mil, igual que lo calcula CalculadoraDeOrden: la
        // semilla tiene que dejar la base como la dejaria la propia aplicacion.
        public static object[] Columnas() =>
        [
            Columna(IdBacklog, IdPlataforma, "Backlog", 1000),
            Columna(IdEnProgreso, IdPlataforma, "En progreso", 2000),
            Columna(IdEnRevision, IdPlataforma, "En revision", 3000),
            Columna(IdHecho, IdPlataforma, "Hecho", 4000),

            Columna(IdPorHacer, IdPortal, "Por hacer", 1000),
            Columna(IdHaciendo, IdPortal, "Haciendo", 2000),
            Columna(IdListo, IdPortal, "Listo", 3000)
        ];

        // Reparto pensado para que la pantalla se vea util desde el primer arranque:
        // las cuatro prioridades presentes, tareas de los dos usuarios, y dos sin
        // responsable para que se vea como sale eso en el tablero y en el reporte.
        public static object[] Tareas() =>
        [
            // Plataforma ScrumSoft - Backlog
            Tarea("55555555-5555-5555-5555-555555555001", IdBacklog, "Exportar el reporte a CSV",
                "Tercer formato de salida, ademas de PDF y Excel.", Prioridad.Baja, 1000, null),
            Tarea("55555555-5555-5555-5555-555555555002", IdBacklog, "Filtrar el tablero por etiqueta",
                null, Prioridad.Media, 2000, IdMiembro),
            Tarea("55555555-5555-5555-5555-555555555003", IdBacklog, "Historial de cambios de una tarea",
                "Registrar quien movio cada tarjeta y cuando.", Prioridad.Baja, 3000, null),

            // Plataforma ScrumSoft - En progreso
            Tarea("55555555-5555-5555-5555-555555555004", IdEnProgreso, "Arrastrar tarjetas entre columnas",
                "El orden se guarda por vecinos, no por indice.", Prioridad.Alta, 1000, IdAdministrador),
            Tarea("55555555-5555-5555-5555-555555555005", IdEnProgreso, "Avisar por correo al asignar una tarea",
                null, Prioridad.Media, 2000, IdMiembro),

            // Plataforma ScrumSoft - En revision
            Tarea("55555555-5555-5555-5555-555555555006", IdEnRevision, "Reporte PDF con el resumen del proyecto",
                "Encabezado con los datos del proyecto y tabla de tareas.", Prioridad.Alta, 1000, IdAdministrador),

            // Plataforma ScrumSoft - Hecho
            Tarea("55555555-5555-5555-5555-555555555007", IdHecho, "Inicio de sesion con JWT",
                "Token firmado y guard de ruta en el cliente.", Prioridad.Critica, 1000, IdAdministrador),
            Tarea("55555555-5555-5555-5555-555555555008", IdHecho, "Alta y edicion de proyectos",
                null, Prioridad.Media, 2000, IdMiembro),

            // Portal de clientes
            Tarea("55555555-5555-5555-5555-555555555011", IdPorHacer, "Definir el modelo de datos",
                "Entidades y relaciones del portal publico.", Prioridad.Alta, 1000, IdAdministrador),
            Tarea("55555555-5555-5555-5555-555555555012", IdHaciendo, "Bocetar la pantalla de avance",
                null, Prioridad.Media, 1000, IdAdministrador)
        ];

        private static object Membresia(string id, Guid idProyecto, Guid idUsuario) =>
            new
            {
                Id = new Guid(id),
                IdProyecto = idProyecto,
                IdUsuario = idUsuario,
                FechaAsignacion = FechaDeSiembra,
                Estado = EstadoRegistro.Activo,
                FechaCreacion = FechaDeSiembra,
                FechaActualizacion = (DateTimeOffset?)null
            };

        private static object Columna(Guid id, Guid idProyecto, string nombre, int orden) =>
            new
            {
                Id = id,
                IdProyecto = idProyecto,
                Nombre = nombre,
                Orden = orden,
                Estado = EstadoRegistro.Activo,
                FechaCreacion = FechaDeSiembra,
                FechaActualizacion = (DateTimeOffset?)null
            };

        private static object Tarea(
            string id,
            Guid idColumna,
            string titulo,
            string? descripcion,
            Prioridad prioridad,
            int orden,
            Guid? idResponsable) =>
            new
            {
                Id = new Guid(id),
                IdColumna = idColumna,
                Titulo = titulo,
                Descripcion = descripcion,
                Prioridad = prioridad,
                Orden = orden,
                IdResponsable = idResponsable,
                Estado = EstadoRegistro.Activo,
                FechaCreacion = FechaDeSiembra,
                FechaActualizacion = (DateTimeOffset?)null
            };
    }
}
