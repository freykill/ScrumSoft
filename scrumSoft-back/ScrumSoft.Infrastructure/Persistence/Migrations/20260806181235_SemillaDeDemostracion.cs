using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ScrumSoft.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SemillaDeDemostracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "proyectos",
                columns: new[] { "id_proyecto", "descripcion", "estado", "estado_proyecto", "fecha_actualizacion", "fecha_creacion", "fecha_fin_prevista", "fecha_inicio", "nombre" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333331"), "Modulo inicial de gestion agil: tablero kanban, reportes y sincronizacion en tiempo real.", "A", "EnProgreso", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateOnly(2026, 3, 31), new DateOnly(2026, 1, 5), "Plataforma ScrumSoft" },
                    { new Guid("33333333-3333-3333-3333-333333333332"), "Sitio publico para que los clientes consulten el avance de sus proyectos.", "A", "Planificacion", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateOnly(2026, 6, 30), new DateOnly(2026, 2, 1), "Portal de clientes" }
                });

            migrationBuilder.InsertData(
                table: "columnas",
                columns: new[] { "id_columna", "estado", "fecha_actualizacion", "fecha_creacion", "id_proyecto", "nombre", "orden" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444001"), "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333331"), "Backlog", 1000 },
                    { new Guid("44444444-4444-4444-4444-444444444002"), "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333331"), "En progreso", 2000 },
                    { new Guid("44444444-4444-4444-4444-444444444003"), "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333331"), "En revision", 3000 },
                    { new Guid("44444444-4444-4444-4444-444444444004"), "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333331"), "Hecho", 4000 },
                    { new Guid("44444444-4444-4444-4444-444444444011"), "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333332"), "Por hacer", 1000 },
                    { new Guid("44444444-4444-4444-4444-444444444012"), "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333332"), "Haciendo", 2000 },
                    { new Guid("44444444-4444-4444-4444-444444444013"), "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333332"), "Listo", 3000 }
                });

            migrationBuilder.InsertData(
                table: "proyecto_usuarios",
                columns: new[] { "id_proyecto_usuario", "estado", "fecha_actualizacion", "fecha_asignacion", "fecha_creacion", "id_proyecto", "id_usuario" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666001"), "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333331"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("66666666-6666-6666-6666-666666666002"), "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333331"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("66666666-6666-6666-6666-666666666003"), "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("33333333-3333-3333-3333-333333333332"), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "tareas",
                columns: new[] { "id_tarea", "descripcion", "estado", "fecha_actualizacion", "fecha_creacion", "id_columna", "id_responsable", "orden", "prioridad", "titulo" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555001"), "Tercer formato de salida, ademas de PDF y Excel.", "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("44444444-4444-4444-4444-444444444001"), null, 1000, "Baja", "Exportar el reporte a CSV" },
                    { new Guid("55555555-5555-5555-5555-555555555002"), null, "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("44444444-4444-4444-4444-444444444001"), new Guid("22222222-2222-2222-2222-222222222222"), 2000, "Media", "Filtrar el tablero por etiqueta" },
                    { new Guid("55555555-5555-5555-5555-555555555003"), "Registrar quien movio cada tarjeta y cuando.", "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("44444444-4444-4444-4444-444444444001"), null, 3000, "Baja", "Historial de cambios de una tarea" },
                    { new Guid("55555555-5555-5555-5555-555555555004"), "El orden se guarda por vecinos, no por indice.", "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("44444444-4444-4444-4444-444444444002"), new Guid("11111111-1111-1111-1111-111111111111"), 1000, "Alta", "Arrastrar tarjetas entre columnas" },
                    { new Guid("55555555-5555-5555-5555-555555555005"), null, "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("44444444-4444-4444-4444-444444444002"), new Guid("22222222-2222-2222-2222-222222222222"), 2000, "Media", "Avisar por correo al asignar una tarea" },
                    { new Guid("55555555-5555-5555-5555-555555555006"), "Encabezado con los datos del proyecto y tabla de tareas.", "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("44444444-4444-4444-4444-444444444003"), new Guid("11111111-1111-1111-1111-111111111111"), 1000, "Alta", "Reporte PDF con el resumen del proyecto" },
                    { new Guid("55555555-5555-5555-5555-555555555007"), "Token firmado y guard de ruta en el cliente.", "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("44444444-4444-4444-4444-444444444004"), new Guid("11111111-1111-1111-1111-111111111111"), 1000, "Critica", "Inicio de sesion con JWT" },
                    { new Guid("55555555-5555-5555-5555-555555555008"), null, "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("44444444-4444-4444-4444-444444444004"), new Guid("22222222-2222-2222-2222-222222222222"), 2000, "Media", "Alta y edicion de proyectos" },
                    { new Guid("55555555-5555-5555-5555-555555555011"), "Entidades y relaciones del portal publico.", "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("44444444-4444-4444-4444-444444444011"), new Guid("11111111-1111-1111-1111-111111111111"), 1000, "Alta", "Definir el modelo de datos" },
                    { new Guid("55555555-5555-5555-5555-555555555012"), null, "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("44444444-4444-4444-4444-444444444012"), new Guid("11111111-1111-1111-1111-111111111111"), 1000, "Media", "Bocetar la pantalla de avance" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "columnas",
                keyColumn: "id_columna",
                keyValue: new Guid("44444444-4444-4444-4444-444444444013"));

            migrationBuilder.DeleteData(
                table: "proyecto_usuarios",
                keyColumn: "id_proyecto_usuario",
                keyValue: new Guid("66666666-6666-6666-6666-666666666001"));

            migrationBuilder.DeleteData(
                table: "proyecto_usuarios",
                keyColumn: "id_proyecto_usuario",
                keyValue: new Guid("66666666-6666-6666-6666-666666666002"));

            migrationBuilder.DeleteData(
                table: "proyecto_usuarios",
                keyColumn: "id_proyecto_usuario",
                keyValue: new Guid("66666666-6666-6666-6666-666666666003"));

            migrationBuilder.DeleteData(
                table: "tareas",
                keyColumn: "id_tarea",
                keyValue: new Guid("55555555-5555-5555-5555-555555555001"));

            migrationBuilder.DeleteData(
                table: "tareas",
                keyColumn: "id_tarea",
                keyValue: new Guid("55555555-5555-5555-5555-555555555002"));

            migrationBuilder.DeleteData(
                table: "tareas",
                keyColumn: "id_tarea",
                keyValue: new Guid("55555555-5555-5555-5555-555555555003"));

            migrationBuilder.DeleteData(
                table: "tareas",
                keyColumn: "id_tarea",
                keyValue: new Guid("55555555-5555-5555-5555-555555555004"));

            migrationBuilder.DeleteData(
                table: "tareas",
                keyColumn: "id_tarea",
                keyValue: new Guid("55555555-5555-5555-5555-555555555005"));

            migrationBuilder.DeleteData(
                table: "tareas",
                keyColumn: "id_tarea",
                keyValue: new Guid("55555555-5555-5555-5555-555555555006"));

            migrationBuilder.DeleteData(
                table: "tareas",
                keyColumn: "id_tarea",
                keyValue: new Guid("55555555-5555-5555-5555-555555555007"));

            migrationBuilder.DeleteData(
                table: "tareas",
                keyColumn: "id_tarea",
                keyValue: new Guid("55555555-5555-5555-5555-555555555008"));

            migrationBuilder.DeleteData(
                table: "tareas",
                keyColumn: "id_tarea",
                keyValue: new Guid("55555555-5555-5555-5555-555555555011"));

            migrationBuilder.DeleteData(
                table: "tareas",
                keyColumn: "id_tarea",
                keyValue: new Guid("55555555-5555-5555-5555-555555555012"));

            migrationBuilder.DeleteData(
                table: "columnas",
                keyColumn: "id_columna",
                keyValue: new Guid("44444444-4444-4444-4444-444444444001"));

            migrationBuilder.DeleteData(
                table: "columnas",
                keyColumn: "id_columna",
                keyValue: new Guid("44444444-4444-4444-4444-444444444002"));

            migrationBuilder.DeleteData(
                table: "columnas",
                keyColumn: "id_columna",
                keyValue: new Guid("44444444-4444-4444-4444-444444444003"));

            migrationBuilder.DeleteData(
                table: "columnas",
                keyColumn: "id_columna",
                keyValue: new Guid("44444444-4444-4444-4444-444444444004"));

            migrationBuilder.DeleteData(
                table: "columnas",
                keyColumn: "id_columna",
                keyValue: new Guid("44444444-4444-4444-4444-444444444011"));

            migrationBuilder.DeleteData(
                table: "columnas",
                keyColumn: "id_columna",
                keyValue: new Guid("44444444-4444-4444-4444-444444444012"));

            migrationBuilder.DeleteData(
                table: "proyectos",
                keyColumn: "id_proyecto",
                keyValue: new Guid("33333333-3333-3333-3333-333333333331"));

            migrationBuilder.DeleteData(
                table: "proyectos",
                keyColumn: "id_proyecto",
                keyValue: new Guid("33333333-3333-3333-3333-333333333332"));
        }
    }
}
