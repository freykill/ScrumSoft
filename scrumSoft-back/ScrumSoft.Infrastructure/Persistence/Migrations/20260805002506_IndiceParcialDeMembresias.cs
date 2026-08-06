using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrumSoft.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IndiceParcialDeMembresias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_proyecto_usuarios",
                table: "proyecto_usuarios");

            migrationBuilder.CreateIndex(
                name: "ux_proyecto_usuarios",
                table: "proyecto_usuarios",
                columns: new[] { "id_proyecto", "id_usuario" },
                unique: true,
                filter: "estado = 'A'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_proyecto_usuarios",
                table: "proyecto_usuarios");

            migrationBuilder.CreateIndex(
                name: "ux_proyecto_usuarios",
                table: "proyecto_usuarios",
                columns: new[] { "id_proyecto", "id_usuario" },
                unique: true);
        }
    }
}
