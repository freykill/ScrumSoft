using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ScrumSoft.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EsquemaInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "proyectos",
                columns: table => new
                {
                    id_proyecto = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin_prevista = table.Column<DateOnly>(type: "date", nullable: true),
                    estado_proyecto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estado = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    fecha_creacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_actualizacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_proyectos", x => x.id_proyecto);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    correo_electronico = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    rol = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estado = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    fecha_creacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_actualizacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "columnas",
                columns: table => new
                {
                    id_columna = table.Column<Guid>(type: "uuid", nullable: false),
                    id_proyecto = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    fecha_creacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_actualizacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_columnas", x => x.id_columna);
                    table.ForeignKey(
                        name: "fk_columnas_proyectos_id_proyecto",
                        column: x => x.id_proyecto,
                        principalTable: "proyectos",
                        principalColumn: "id_proyecto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "proyecto_usuarios",
                columns: table => new
                {
                    id_proyecto_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    id_proyecto = table.Column<Guid>(type: "uuid", nullable: false),
                    id_usuario = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha_asignacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    estado = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    fecha_creacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_actualizacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_proyecto_usuarios", x => x.id_proyecto_usuario);
                    table.ForeignKey(
                        name: "fk_proyecto_usuarios_proyectos_id_proyecto",
                        column: x => x.id_proyecto,
                        principalTable: "proyectos",
                        principalColumn: "id_proyecto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_proyecto_usuarios_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tareas",
                columns: table => new
                {
                    id_tarea = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    prioridad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    id_responsable = table.Column<Guid>(type: "uuid", nullable: true),
                    id_columna = table.Column<Guid>(type: "uuid", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    fecha_creacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_actualizacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tareas", x => x.id_tarea);
                    table.ForeignKey(
                        name: "fk_tareas_columnas_id_columna",
                        column: x => x.id_columna,
                        principalTable: "columnas",
                        principalColumn: "id_columna",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tareas_usuarios_id_responsable",
                        column: x => x.id_responsable,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "usuarios",
                columns: new[] { "id_usuario", "correo_electronico", "estado", "fecha_actualizacion", "fecha_creacion", "nombre", "password_hash", "rol" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "admin@scrumsoft.com", "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Administrador", "$2a$12$ARg7iJNOV843LqiS8ssGfOHp1tSWigIUANkA7soFy708ZExz7g1yO", "Administrador" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "miembro@scrumsoft.com", "A", null, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Miembro de prueba", "$2a$12$104Q7YvMTtAmNJxFxsTjl.N3BPTPtGnU1iVskwbfPp3gg4CQpekGa", "Miembro" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_columnas_proyecto_orden",
                table: "columnas",
                columns: new[] { "id_proyecto", "orden" });

            migrationBuilder.CreateIndex(
                name: "ix_proyecto_usuarios_usuario",
                table: "proyecto_usuarios",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "ux_proyecto_usuarios",
                table: "proyecto_usuarios",
                columns: new[] { "id_proyecto", "id_usuario" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_proyectos_nombre",
                table: "proyectos",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "ix_tareas_columna_orden",
                table: "tareas",
                columns: new[] { "id_columna", "orden" });

            migrationBuilder.CreateIndex(
                name: "ix_tareas_responsable",
                table: "tareas",
                column: "id_responsable");

            migrationBuilder.CreateIndex(
                name: "ux_usuarios_correo",
                table: "usuarios",
                column: "correo_electronico",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "proyecto_usuarios");

            migrationBuilder.DropTable(
                name: "tareas");

            migrationBuilder.DropTable(
                name: "columnas");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "proyectos");
        }
    }
}
